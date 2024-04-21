using System;

namespace Birdhouse.LogicSlicing.Interfaces
{
    public interface ILogicSlice : IDisposable
    {
        IDisposable RegisterAction(Action action);
    }
}