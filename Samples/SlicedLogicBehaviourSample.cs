using System;
using Birdhouse.LogicSlicing.Interfaces;
using Birdhouse.LogicSlicing.Unity;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Birdhouse.LogicSlicing.Samples
{
    public abstract class SlicedLogicBehaviourSample
        : MonoBehaviour
    {
        private IDisposable _token;

        private int _priority = 3;

        protected int Priority
        {
            get => _priority;
            set
            {
                Unsubscribe();
                _priority = value;
                Subscribe();
            }
        }

        private static int Index = 0;

        protected abstract void TickInternal(float deltaTime);

        [ContextMenu("Recalculate")]
        private void Recalculate()
        {
            GetGroup()
                .Recalculate();
        }

        [ContextMenu("Enable Measuring")]
        private void EnableMeasuring()
        {
            GetGroup()
                .IsMeasuring = true;
        }

        [ContextMenu("Disable Measuring")]
        private void DisableMeasuring()
        {
            GetGroup()
                .IsMeasuring = false;
        }

        private void Tick(float deltaTime)
        {
            TickInternal(deltaTime);
        }

        private void Subscribe()
        {
            _token = GetGroup()
                .GetOrRegisterGroup(++Index)
                .RegisterAction(Tick);
        }

        private void Unsubscribe()
        {
            _token?.Dispose();
            _token = null;
        }

        private ISmartLogicSlicer<int> GetGroup()
        {
            var result = UnityLogicSlicingHelper
                .SmartLogicSlicers[typeof(Update)]
                .GetOrCreate(Priority);

            return result;
        }
        
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }
    }
}