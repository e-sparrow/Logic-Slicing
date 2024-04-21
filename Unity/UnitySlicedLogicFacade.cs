using System;

namespace Birdhouse.LogicSlicing
{
    public class UnitySlicedLogicFacade 
        : UnitySlicedLogicFacadeBase
    {
        public UnitySlicedLogicFacade(object target, int priority) 
            : base(priority)
        {
            _keyFunc = target.GetHashCode;
        }
        
        public UnitySlicedLogicFacade(Func<int> keyFunc, int priority) 
            : base(priority)
        {
            _keyFunc = keyFunc;
        }

        private readonly Func<int> _keyFunc;

        protected override int GetGroupKey()
        {
            var result = _keyFunc.Invoke();
            return result;
        }
    }
}