using System;
using System.Collections.Generic;

namespace Shared.Utils.PoolSystem
{
    public class BasePool
    {
        public int Count => _pool.Count;
        public int TrackedObjects;
        
        private readonly Stack<object> _pool = new();
        private readonly Func<object> _getNew;

        protected BasePool(Func<object> getNew, int warmCount = 0)
        {
            _getNew = getNew;
            for (int i = 0; i < warmCount; i++) 
                _pool.Push(GetNew());
        }

        public void ReturnObj(object obj)
        {
            TrackedObjects--;
            if (obj is PoolObject poolObject)
                poolObject.OnReturnToPool();
            _pool.Push(obj);
        }

        protected object GetObj()
        {
            TrackedObjects++;
            return _pool.Count > 0 ? _pool.Pop() : GetNew();
        }

        private object GetNew()
        {
            var newObj = _getNew.Invoke();
            if (newObj is PoolObject poolObject)
                poolObject.Pool = this;
            return newObj;
        }
    }
}