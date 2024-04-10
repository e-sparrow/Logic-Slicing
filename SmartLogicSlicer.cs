using System;
using System.Collections.Generic;
using System.Linq;
using Birdhouse.Common.Extensions;
using Birdhouse.Common.Mathematics.Average;
using Birdhouse.Common.Mathematics.Average.Interfaces;
using Birdhouse.Features.LogicSlicing.Interfaces;
using Birdhouse.Features.Registries;
using Birdhouse.Features.Registries.Interfaces;
using Birdhouse.Tools.Tense;
using Birdhouse.Tools.Tense.Providers.Interfaces;

namespace Birdhouse.Features.LogicSlicing
{
    public class SmartLogicSlicer<TKey> 
        : ISmartLogicSlicer<TKey>
    {
        public SmartLogicSlicer(int bucketsCount, ITenseProvider<float> tenseProvider = null)
        {
            tenseProvider ??= TenseHelper
                .RealtimeTenseProvider
                .Value;
            
            _bucketsCount = bucketsCount;
            _tenseProvider = tenseProvider;
        }

        private readonly int _bucketsCount;
        private readonly ITenseProvider<float> _tenseProvider;

        private readonly List<List<LogicGroupWrapper>> _buckets = new List<List<LogicGroupWrapper>>();
        private readonly IDictionary<TKey, LogicGroupWrapper> _groups = new Dictionary<TKey, LogicGroupWrapper>();

        private int _index = -1;

        public bool IsMeasuring 
        { 
            get; 
            set;
        } = false;

        public ILogicGroup GetOrRegisterGroup(TKey key)
        {
            var hasValue = _groups.TryGetValue(key, out var value);
            if (hasValue)
            {
                return value;
            }

            var group = new LogicGroup(new RegistryEnumerable<Action<float>>());

            List<LogicGroupWrapper> bucket;
            if (_buckets.Count < _bucketsCount)
            {
                bucket = new List<LogicGroupWrapper>();
                _buckets.Add(bucket);
                while (_buckets.Count < _bucketsCount)
                {
                    _buckets.Add(new List<LogicGroupWrapper>());
                }
            }
            else
            {
                bucket = _buckets
                    .WithMin(item => item
                        .Sum(wrapper => wrapper.Calculator.Current));
            }

            value = new LogicGroupWrapper(group, _tenseProvider);
            var bucketToken = bucket.AddAsDisposable(value);
            
            value.SetBucketToken(bucketToken);
            
            var groupToken = _groups.AddAsDisposable(new KeyValuePair<TKey, LogicGroupWrapper>(key, value));
            value.SetGroupToken(groupToken);

            const bool IsCalibrating = true;
            if (IsCalibrating)
            {
                const int CalibrationIterations = 10;
                for (var i = 0; i < CalibrationIterations; i++)
                {
                    value.Execute(true);
                }
            }

            return value;
        }

        public void Recalculate()
        {
            var groups = new List<LogicGroupWrapper>(_buckets
                .SelectMany(value => value)
                .OrderBy(value => value.Calculator.Current));

            var sums = Enumerable
                .Repeat(0f, _bucketsCount)
                .ToList();

            var newBuckets = new List<List<LogicGroupWrapper>>();
            for (var i = 0; i < _bucketsCount; i++)
            {
                newBuckets.Add(new List<LogicGroupWrapper>());
            }
            
            foreach (var group in groups)
            {
                var index = sums.IndexOf(sums.Min());
                var token = newBuckets[index].AddAsDisposable(group);
                sums[index] += group.Calculator.Current;
                
                group.SetBucketToken(token);
            }
            
            _buckets.Clear();
            _buckets.AddRange(newBuckets);
        }

        public void Execute()
        {
            _index++;
            if (_index == _bucketsCount)
            {
                _index = 0;
            }

            _buckets[_index].ForEach(ExecuteWrapper);

            void ExecuteWrapper(LogicGroupWrapper wrapper)
            {
                wrapper.Execute(IsMeasuring);
            }
        }

        public void Dispose()
        {
            _groups.Clear();
        }
        
        private class LogicGroupWrapper
            : ILogicGroup
        {
            public LogicGroupWrapper(LogicGroup group, ITenseProvider<float> tenseProvider, IAverageCalculator<float> calculator = null, IDisposable groupToken = null, IDisposable bucketToken = null)
            {
                calculator ??= new FloatAverageCalculator();
                
                _group = group;
                _tenseProvider = tenseProvider;
                Calculator = calculator;
                _groupToken = groupToken;
                _bucketToken = bucketToken;
            }
            
            private readonly LogicGroup _group;
            private readonly ITenseProvider<float> _tenseProvider;

            private readonly IRegistryEnumerable<Action<float>> _actions = new RegistryEnumerable<Action<float>>();
            

            private IDisposable _bucketToken;
            private IDisposable _groupToken;

            private float _lastExecutionTime = 0f;

            public IAverageCalculator<float> Calculator
            {
                get;
            }

            public IDisposable RegisterAction(Action action)
            {
                var result = _group.RegisterAction(_ => action.Invoke());
                return result;
            }

            public IDisposable RegisterAction(Action<float> action)
            {
                var result = _group.RegisterAction(action);
                return result;
            }

            public void Execute(bool isMeasuring)
            {
                var now = _tenseProvider.Now();
                var result = now - _lastExecutionTime;
                _lastExecutionTime = now;
                
                _group.Execute(result);

                if (isMeasuring)
                {
                    Calculator.Add(result);
                }
            }

            public void SetGroupToken(IDisposable other)
            {
                _groupToken = other;
            }
            
            public void SetBucketToken(IDisposable other)
            {
                _bucketToken = other;
            }
            
            public void Dispose()
            {
                _groupToken.Dispose();
                _bucketToken.Dispose();
                _actions.Dispose();
            }
        }

        private readonly struct LogicGroup
        {
            public LogicGroup(IRegistryEnumerable<Action<float>> actions = null)
            {
                actions ??= new RegistryEnumerable<Action<float>>();
                
                _actions = actions;
            }
            
            private readonly IRegistryEnumerable<Action<float>> _actions;

            public IDisposable RegisterAction(Action<float> action)
            {
                var result = _actions.Register(action);
                return result;
            }

            public void Execute(float deltaTime)
            {
                var incomingActions = new HashSet<Action<float>>(_actions);
                foreach (var action in incomingActions)
                {
                    action.Invoke(deltaTime);
                }
            }
        }
    }
}