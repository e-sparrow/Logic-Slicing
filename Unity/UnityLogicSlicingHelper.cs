using System;
using Birdhouse.Common.Collections;
using Birdhouse.Common.Collections.Interfaces;
using Birdhouse.LogicSlicing.Interfaces;
using Birdhouse.Tools.UnityMessages;
using UnityEngine.LowLevel;

namespace Birdhouse.LogicSlicing.Unity
{
    public static class UnityLogicSlicingHelper
    {
        public static readonly ILazyBuffer<Type, IOrderedLogicSlicer> LogicSlicers
            = new LazyBuffer<Type, IOrderedLogicSlicer>(CreateLogicSlicer);
        
        public static readonly ILazyBuffer<Type, ILazyBuffer<int, ISmartLogicSlicer<int>>> SmartLogicSlicers 
            = new LazyBuffer<Type, ILazyBuffer<int, ISmartLogicSlicer<int>>>(CreateSmartLogicSlicersBuffer);

        private static IOrderedLogicSlicer CreateLogicSlicer(Type type)
        {
            var result = CreatePlayerLoopSlicer(type);
            return result;
        }

        private static ILazyBuffer<int, ISmartLogicSlicer<int>> CreateSmartLogicSlicersBuffer(Type type)
        {
            var result = new LazyBuffer<int, ISmartLogicSlicer<int>>(priority => CreateSmartPlayerLoopSlicer(type, priority));
            return result;
        }
        
        private static IOrderedLogicSlicer CreatePlayerLoopSlicer<T>()
        {
            var result = CreatePlayerLoopSlicer(typeof(T));
            return result;
        }

        private static IOrderedLogicSlicer CreatePlayerLoopSlicer(Type type)
        {
            var result = new OrderedLogicSlicer();
            
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            ref var system = ref loop.Find(type);
            system.updateDelegate += Invoke;
            PlayerLoop.SetPlayerLoop(loop);
            
            return result;

            void Invoke()
            {
                result.Trigger();
            }
        }

        private static ISmartLogicSlicer<int> CreateSmartPlayerLoopSlicer<T>
            (int bucketsCount, bool isMeasuring = true)
        {
            var result = CreateSmartPlayerLoopSlicer(typeof(T), bucketsCount, isMeasuring);
            return result;
        }

        private static ISmartLogicSlicer<int> CreateSmartPlayerLoopSlicer
            (Type type, int bucketsCount, bool isMeasuring = true)
        {
            var result = new SmartLogicSlicer<int>(bucketsCount);
            result.IsMeasuring = isMeasuring;
            
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            ref var system = ref loop.Find(type);
            
            system.updateDelegate += Invoke;
            PlayerLoop.SetPlayerLoop(loop);
            
            return result;

            void Invoke()
            {
                result.Execute();
            }
        }
    }
}