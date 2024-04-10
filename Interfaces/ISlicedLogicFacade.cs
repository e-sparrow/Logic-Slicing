using System;

namespace Birdhouse.Features.LogicSlicing.Interfaces
{
    public interface ISlicedLogicFacade : IDisposable
    {
        int Priority
        {
            get;
            set;
        }
        
        IDisposable RegisterAction<T>(Action<float> action);
        IDisposable RegisterAction(Type type, Action<float> action);
    }
}