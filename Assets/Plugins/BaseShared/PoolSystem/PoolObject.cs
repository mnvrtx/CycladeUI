using Shared.Utils.Logging;

namespace Shared.Utils.PoolSystem
{
    public abstract class PoolObject
    {
        private static readonly Log log = new(nameof(PoolObject));

        public BasePool Pool;

        public virtual void OnReturnToPool() { }

        public void ReturnToPool()
        {
            if (Pool == null)
            {
                log.Warn("ReturnToPool. Pool == null");
                return;
            }

            Pool.ReturnObj(this);
        }
    }
}