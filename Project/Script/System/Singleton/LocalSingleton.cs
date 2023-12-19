using UnityEngine;

namespace SugyeongKim.Util
{
    // 간단한 싱글톤, 찾기만함
    public abstract class LocalSingleton<T> : MonoBehaviour where T : Component
    {
        // GameObject bool check는 성능이 좋지 못함,
        // onDestroy시 토글되는 bool로 성능 최적화 시도
        protected static bool _cachedInstanceBool;

        protected static T _instance;
        public static T instance
        {
            get
            {
                return FindCachedInstance ();
            }
        }

        protected static T FindCachedInstance ()
        {
            if (_cachedInstanceBool)
            {
                return _instance;
            }

            _cachedInstanceBool = IsValid ();
            if (_cachedInstanceBool == false)
            {
                _instance = FindObjectOfType<T> ();
            }
            return _instance;
        }

        private static bool IsValid ()
        {
            return _instance && ReferenceEquals (_instance, null) == false;
        }

        //==========================================================//

        public virtual void Awake ()
        {
            _instance = IsValid () ? _instance : transform as T;
        }

        public virtual void OnDestroy ()
        {
            _cachedInstanceBool = false;
            _instance = null;
        }
    }
}
