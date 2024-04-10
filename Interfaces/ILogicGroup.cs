using System;

namespace Birdhouse.Features.LogicSlicing.Interfaces
{
    public interface ILogicGroup 
        : IDisposable
    {
        IDisposable RegisterAction(Action action);
        IDisposable RegisterAction(Action<float> action);
    }
}