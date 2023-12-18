using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 게임 전역에서 사용되는 싱글톤, 찾고 없으면 만들어서 제공
    public abstract class GlobalSingleton<T> : LocalSingleton<T>, SingletonTool.ISingletonInit where T : Component
    {
        public new static T instance
        {
            get
            {
                if (!FindCachedInstance ())
                {
                    var singleton = new GameObject ($"{typeof (T)}");
                    singleton.transform.SetParent (SingletonTool.RootTrnas);
                    _instance = singleton.AddComponent<T> ();
                }

                return FindCachedInstance ();
            }
        }

        //==========================================================//

        // 싱글톤 초기화
        public virtual int InitOrder => 0;
        public virtual void Init () { }
        public virtual IObservable<Unit> InitAsObservable ()
        {
            return Observable.ReturnUnit ();
        }
    }

}
