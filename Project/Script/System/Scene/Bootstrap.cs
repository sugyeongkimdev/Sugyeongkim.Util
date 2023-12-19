using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    public abstract class Bootstrap<T> : GlobalSingleton<T> where T : Bootstrap<T>
    {
        // 프로그램 '최초 실행시' 어느씬이든 초기화 가능하도록 초기화를 모아둠, scene 최초 진입시 실행하면 됨
        // 다른 bootstrap class를 생성해서 overriding 해서 사용바람 (BootstrapTemplate.txt 참조)
        protected static bool isBootstrapInit = false;
        public virtual IObservable<Unit> BootstrapInit ()
        {
            if (isBootstrapInit)
            {
                return Observable.ReturnUnit ();
            }
            else
            {
                isBootstrapInit = true;
                return Observable.ReturnUnit ()
                    .DoOnSubscribe (() => UtilLog.Log ("Bootstrap start."))
                    .DoOnTerminate (() => UtilLog.Log ("Bootstrap done."))

                    // 싱글톤 초기화
                    .SelectMany (_ => SingletonTool.InitGlobalSingletonAsObservable ());
            }
        }
    }
}