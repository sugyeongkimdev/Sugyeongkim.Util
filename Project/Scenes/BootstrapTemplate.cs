using System;
using SugyeongKim.Util;
using UniRx;
using UnityEngine.SceneManagement;

public abstract class BootstrapTemplate : GlobalSingleton<BootstrapTemplate>
{
    // 프로그램 '최초 실행시' 어느씬이든 초기화 가능하도록 초기화를 모아둠, scene 최초 진입시 실행하면 됨
    // abstract 이므로 bootstrap class를 생성해서 overriding 해서 사용바람. (아래 코드는 예제임)
    // (calss:BootstrapTemplate).instance.BootstrapInit();

    //private void Start ()
    //{
    //    BootstrapInit ()
    //        .Subscribe (_ =>
    //        {
    //            // done callback
    //            // SceneManager.LoadSceneAsync ("A scene");
    //        });
    //}

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

                // 기본 프로그램 초기화
                .Do (_ =>
                {
#if UNITY_IOS || UNITY_ANDROID
                    //Application.targetFrameRate = 60;
#else
                    //QualitySettings.vSyncCount = 1;
#endif
                    //Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    //Screen.SetResolution ((int)Screen.width, (int)Screen.height, true);

                    //DOTween.Init ();
                })

                // 싱글톤 초기화
                .SelectMany (_ => SingletonTool.InitSingletonAsObservable ());
        }
    }
}
