using System;
using Birdhouse.Features.Executions.Interfaces;

namespace Birdhouse.LogicSlicing.Interfaces
{
    public interface ISmartLogicSlicer<in TKey> 
        : IExecutor, IDisposable
    {
        bool IsMeasuring
        {
            get;
            set;
        }
        
        ILogicGroup GetOrRegisterGroup(TKey key);

        void Recalculate();
    }
}