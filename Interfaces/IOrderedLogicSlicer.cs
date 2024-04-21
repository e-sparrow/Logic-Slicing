using System;

namespace Birdhouse.LogicSlicing.Interfaces
{
    public interface IOrderedLogicSlicer : IDisposable
    {
        ILogicSlice GetOrCreateSlice(int index);

        void Trigger();
    }
}