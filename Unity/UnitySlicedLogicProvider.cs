using System;
using System.Collections.Generic;
using Birdhouse.Features.LogicSlicing;
using Birdhouse.Features.LogicSlicing.Interfaces;

namespace Birdhouse.Extended.LogicSlicing.Unity
{
    public static class UnitySlicedLogicProvider
    {
        private static readonly IDictionary<Type, ISlicedLogicFacade> Facades =
            new Dictionary<Type, ISlicedLogicFacade>();

        public static ISlicedLogicFacade GetOrCreate<T>(int priority)
        {
            var result = GetOrCreate(typeof(T), priority);
            return result;
        }

        public static ISlicedLogicFacade GetOrCreate(Type type, int priority)
        {
            var hasValue = Facades.TryGetValue(type, out var result);
            if (hasValue)
            {
                return result;
            }

            result = new UnitySlicedLogicFacade(type, priority);
            return result;
        }
    }
}