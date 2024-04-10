using System;
using Birdhouse.Features.LogicSlicing.Interfaces;

namespace Birdhouse.Features.LogicSlicing
{
    public static class LogicSlicingExtensions
    {
#region Facades
        public static IDisposable RegisterAction<T>(this ISlicedLogicFacade self, Action action)
        {
            var result = self.RegisterAction(typeof(T), action);
            return result;
        }

        public static IDisposable RegisterAction(this ISlicedLogicFacade self, Type type, Action action)
        {
            var input = new Action<float>(_ => action.Invoke());
            
            var result = self.RegisterAction(type, input);
            return result;
        }
#endregion
    }
}