using System;
using System.Linq;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 간단한 싱글톤, 찾기만함
    public abstract class LocalSingleton<T> : MonoBehaviour, IDisposable where T : Component
    {
        // GameObject bool check는 성능이 좋지 못함,
        // onDestroy시 토글되는 bool로 성능 최적화 시도
        protected static bool _cachedBool;

        protected static T _instance { get; set; }
        public static T instance
        {
            get
            {
                return FindCachedInstance ();
            }
        }

        //==========================================================//

        // 캐시 찾기
        public static T FindCachedInstance ()
        {
            if (_cachedBool)
            {
                return _instance;
            }

            _cachedBool = IsValid ();
            if (_cachedBool == false)
            {
                var findAll = FindObjectsOfType<T> (true);
                if(findAll.Length > 1)
                {
                    UtilLog.Error ($"Many Same Global Sington Type : {typeof(T)} [{findAll.Length}]");
                }
                _instance = findAll.FirstOrDefault();
            }
            return _instance;
        }

        // 유효성 체크
        public static bool IsValid ()
        {
            return _instance && (ReferenceEquals (_instance, null) == false);
        }

        //==========================================================//

        public virtual void Awake ()
        {
            _instance = IsValid () ? _instance : transform as T;
        }


        public virtual void OnDestroy ()
        {
            Dispose ();
        }

        public virtual void Dispose ()
        {
            _cachedBool = false;
            _instance = null;
        }
    }
}
