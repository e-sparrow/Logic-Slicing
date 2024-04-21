using System;

namespace Birdhouse.LogicSlicing.Interfaces
{
    public interface ILogicSlicer : IDisposable
    {
        ILogicSlice RegisterSlice();
        
        void Trigger();
    }
}