using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 게임 전역에서 사용되는 싱글톤, 찾고 없으면 만들어서 제공
    public abstract class GlobalSingleton<T> : LocalSingleton<T>, IGlobalSingletonInit where T : GlobalSingleton<T>
    {
        public static string instanceName => nameof (instance);

        //==========================================================//

        private bool FirstOnce = true;

        // local singleton에서 정의된 instance를 재정의함
        // local에서는 찾을 수 없으면 null이지만 global에서는 찾을 수 없으면 생성해서 유일한 instance가 되려고 함
        // 유일성을 보장하지 않음, 유일성이 보장되지 않는 상황은 이미 일그러진 상황으로 판단
        public new static T instance
        {
            get
            {
                if (FindCachedInstance () == false)
                {
                    UtilLog.Log ($"Create Global Singleton : {typeof (T)}");

                    // 캐시를 찾을 수 없음, 생성함
                    var singleton = new GameObject ($"{typeof (T)}");
                    singleton.transform.SetParent (SingletonTool.RootTrnas);
                    _instance = singleton.AddComponent<T> ();
                    _cachedBool = true;
                }
                if (_instance.FirstOnce)
                {
                    DontDestroyOnLoad (_instance.gameObject);
                    _instance.FirstOnce = false;
                }
                return _instance;
            }
        }

        //==========================================================//

        // 싱글톤 초기화
        public virtual int InitOrder => 0;
        public virtual IObservable<Unit> InitAsObservable ()
        {
            return Observable.ReturnUnit ();
        }
    }
}
