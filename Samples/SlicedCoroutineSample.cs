using System.Collections.Generic;
using Birdhouse.Extended.LogicSlicing.Unity;
using Birdhouse.Features.LogicSlicing.Unity;
using Birdhouse.Tools.Coroutines;
using Birdhouse.Tools.Coroutines.Instructions;
using Birdhouse.Tools.Coroutines.Interfaces;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Birdhouse.Features.LogicSlicing.Samples
{
    public class SlicedCoroutineSample : MonoBehaviour
    {
        private void Start()
        {
            UnityLogicSlicingHelper
                .SmartLogicSlicers[typeof(Update)]
                .GetOrCreate(5)
                .GetOrRegisterGroup(typeof(SlicedCoroutineSample).GetHashCode())
                .StartCoroutine(GetSampleCoroutine());
        }
        
        private IEnumerator<ICoroutineInstruction> GetSampleCoroutine()
        {
            const float FirstDelay = 2f;
            
            yield return new WaitForSecondsInstruction(FirstDelay);
            Debug.Log($"Waited for {FirstDelay}");
            
            const float SecondDelay = 4f;
            
            yield return new WaitForSecondsInstruction(SecondDelay);
            Debug.Log($"Waited for {SecondDelay}");
            
            const float ThirdDelay = 8f;
            
            yield return new WaitForSecondsInstruction(ThirdDelay);
            Debug.Log($"Waited for {ThirdDelay}");
        }
    }
}