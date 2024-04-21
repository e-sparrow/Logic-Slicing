using System.Collections;
using System.Collections.Generic;
using Birdhouse.LogicSlicing;
using Birdhouse.LogicSlicing.Interfaces;
using Birdhouse.Tools.Coroutines;
using Birdhouse.Tools.Coroutines.Interfaces;
using Birdhouse.Tools.Coroutines.Unity;

namespace Birdhouse.LogicSlicing.Unity
{
    public static class UnityLogicSlicingExtensions
    {
        public static void StartCoroutine(this ILogicGroup self, IEnumerator<ICoroutineInstruction> coroutine)
        {
            var tickProvider = new SlicedTickProvider(self);
            
            var coroutineStarter = new TickCoroutineStarter(tickProvider);
            coroutineStarter.Start(coroutine);
        }

        public static void StartCoroutine(this ILogicGroup self, IEnumerator coroutine)
        {
            var tickProvider = new SlicedTickProvider(self);
            
            var coroutineStarter = new CoroutineStarterWrapper<IEnumerator>(new TickCoroutineStarter(tickProvider), UnityCoroutinesHelper.CoroutineWrapper);
            coroutineStarter.Start(coroutine);
        }
    }
}