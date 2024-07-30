using System;

namespace Shared.Utils.PoolSystem
{
    public class Pool<T> : BasePool where T : class
    {
        public Pool(Func<T> getNew, int warmCount = 0) 
            : base(getNew, warmCount)
        {
        }

        public void Return(T obj) => ReturnObj(obj);

        public T Get() => (T)GetObj();
    }
}