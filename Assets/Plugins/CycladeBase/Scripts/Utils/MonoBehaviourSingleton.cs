using UnityEngine;

namespace CycladeBase.Utils
{
    public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        private static T _instance;

        public static T I
        {
            get
            {
                InitializeStatic();
                return _instance;
            }
        }

        private static void InitializeStatic()
        {
            if (_instance != null) 
                return;

            _instance = (T)FindObjectOfType(typeof(T));

            if (_instance != null)
                return;

            var go = new GameObject($"[SINGLETON] {typeof(T).Name}");
            _instance = go.AddComponent<T>();
            _instance.OnInitialize();
            DontDestroyOnLoad(go);
        }

        protected virtual void OnInitialize() { }
    }
}