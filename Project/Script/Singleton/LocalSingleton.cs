using UnityEngine;

namespace SugyeongKim.Util
{
    // 간단한 싱글톤, 찾기만함
    public abstract class LocalSingleton<T> : MonoBehaviour where T : Component
    {
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
            if (IsValid () == false)
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
            _instance = null;
        }
    }
}
