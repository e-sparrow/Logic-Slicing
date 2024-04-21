using System.Threading;
using Birdhouse.LogicSlicing.Unity;
using Birdhouse.LogicSlicing.Interfaces;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Birdhouse.LogicSlicing.Samples
{
    public class SlicedLogicSample : MonoBehaviour
    {
        [SerializeField] private int priority;
        [SerializeField] private int delay;

        private readonly ISlicedLogicFacade _updateFacade 
            = UnitySlicedLogicProvider.GetOrCreate<SlicedLogicSample>(2);

        private void CheckPriority()
        {
            if (_updateFacade.Priority != priority)
            {
                _updateFacade.Priority = priority;
            }
        }

        private void ApplyDelay()
        {
            Thread.Sleep(delay);
        }

        private void OnEnable()
        {
            _updateFacade.Priority = priority;
            
            _updateFacade.RegisterAction<Update>(ApplyDelay);
            _updateFacade.RegisterAction<Update>(CheckPriority);
        }

        private void OnDisable()
        {
            _updateFacade.Dispose();
        }
    }
}