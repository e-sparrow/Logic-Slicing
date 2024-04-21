using System;

namespace Birdhouse.LogicSlicing.Interfaces
{
    public interface ILogicGroup 
        : IDisposable
    {
        IDisposable RegisterAction(Action action);
        IDisposable RegisterAction(Action<float> action);
    }
}