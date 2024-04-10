using System;
using Birdhouse.Features.LogicSlicing.Interfaces;
using Birdhouse.Tools.Ticks.Interfaces;

namespace Birdhouse.Features.LogicSlicing
{
    public class SlicedTickProvider 
        : ITickProvider
    {
        public SlicedTickProvider(ILogicGroup group)
        {
            _group = group;
        }

        private readonly ILogicGroup _group;
        
        public IDisposable RegisterTick(Action<float> tick)
        {
            var result = _group.RegisterAction(tick);
            return result;
        }
    }
}