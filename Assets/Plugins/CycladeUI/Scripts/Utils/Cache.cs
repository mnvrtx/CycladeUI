using System;

namespace CycladeUI.Utils
{
    public class Cache<T>
    {
        public bool UseNullCheck;

        private T _cachedObject;
        private bool _isCached;

        public T Value
        {
            get
            {
                var condition = UseNullCheck ? _cachedObject == null || _cachedObject.Equals(null) : !_isCached;
                if (condition)
                {
                    _cachedObject = _onCreate.Invoke();
                    _isCached = true;
                }

                return _cachedObject;
            }
        }

        private readonly Func<T> _onCreate;

        public Cache(Func<T> onCreate, bool instantInvoke = false, bool useNullCheck = false)
        {
            _onCreate = onCreate;
            UseNullCheck = useNullCheck;
            if (instantInvoke)
                _onCreate.Invoke();
        }

        public static implicit operator T(Cache<T> v) => v.Value;

        public void Warmup()
        {
            _onCreate.Invoke();
        }

        public void ResetCache()
        {
            _isCached = false;
            _cachedObject = default;
        }
    }
}