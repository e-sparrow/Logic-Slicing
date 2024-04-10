using System;
using System.Collections.Generic;
using Birdhouse.Abstractions.Disposables;
using Birdhouse.Common.Extensions;
using Birdhouse.Features.LogicSlicing.Interfaces;
using Birdhouse.Features.LogicSlicing.Unity;

namespace Birdhouse.Features.LogicSlicing
{
    public abstract class UnitySlicedLogicFacadeBase
        : ISlicedLogicFacade
    {
        protected UnitySlicedLogicFacadeBase(int priority)
        {
            _priority = priority;
        }
        
        private readonly List<LogicInfo> _logics = new List<LogicInfo>();

        private int _priority;

        public int Priority
        {
            get => _priority;
            set => ChangePriority(value);
        }

        protected abstract int GetGroupKey();
        
        public IDisposable RegisterAction<T>(Action<float> action)
        {
            var result = RegisterAction(typeof(T), action);
            return result;
        }

        public IDisposable RegisterAction(Type type, Action<float> action)
        {
            var key = GetGroupKey();
            
            var token = UnityLogicSlicingHelper
                .SmartLogicSlicers[type]
                .GetOrCreate(_priority)
                .GetOrRegisterGroup(key)
                .RegisterAction(action);

            var logic = new LogicInfo(type, action, token);
            var logicToken = _logics.AddAsDisposable(logic);

            token = token.Append(logicToken);
            return token;
        }

        private void ChangePriority(int priority)
        {
            var incomingLogics = new List<LogicInfo>(_logics);
            foreach (var logic in incomingLogics)
            {
                RegisterAction(logic.Type, logic.Action);
            }

            Dispose();

            _priority = priority;
        }

        public void Dispose()
        {
            _logics.ForEach(value => value.Token.Dispose());
            _logics.Clear();
        }

        private readonly struct LogicInfo
        {
            public LogicInfo(Type type, Action<float> action, IDisposable token)
            {
                Type = type;
                Action = action;
                Token = token;
            }

            public Type Type
            {
                get;
            }

            public Action<float> Action
            {
                get;
            }

            public IDisposable Token
            {
                get;
            }
        }
    }
}