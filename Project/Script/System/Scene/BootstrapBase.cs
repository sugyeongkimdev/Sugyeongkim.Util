using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    public abstract class BootstrapBase : GlobalSingleton<BootstrapBase>
    {
        /*
        public class ExampleScene : MonoBehaviour
        {
            private void Start ()
            {
                BootstrapBase.BootstrapAsObservable ()
                    .Subscribe (_ =>
                    {
                         callback
                    });
            }
        }
        */

        //============================================//

        // 부트스트랩 씬 이름
        public static string BootstrapSceneName = "util.bootstrap";

        // 부트스트랩이 끝난 뒤 이동할 씬 이름
        public abstract string NextSceneName { get; }


        //============================================//

        // 프로그램 최초 실행시 어느씬이든 초기화 가능하도록 초기화를 모아둠, scene 최초 진입시 실행하면 됨
        // bootstrap을 통하지 않고도 실행할 가능하도록 하기 위한 목적
        private static bool isInit = false;
        public static IObservable<bool> BootstrapAsObservable ()
        {
            if (isInit)
            {
                return Observable.Return (false);
            }
            isInit = true;

            return Observable.ReturnUnit ()
                .Do (_ => UtilLog.Log ("Bootstrap start."))

                // boostrap load
                // 부트스트랩씬이 없을경우 씬 로드하기 (※ instance를 사용해선 안됨)
                .SelectMany (_ =>
                {
                    // 부트스트랩이 이미 있으면 무시
                    if (BootstrapBase.IsValid () || BootstrapBase.FindCachedInstance ())
                    {
                        SceneControlManager.CurrentSceneName = BootstrapSceneName;
                        return Observable.ReturnUnit ();
                    }
                    // 부트스트랩 씬 로드
                    return SceneControlManager.LoadSceneAsObservable (BootstrapSceneName);
                })

                // bootstrap Init
                .SelectMany (_ => instance.OnBootstrapAsObservable ())

                // singleton init
                .SelectMany (_ => SingletonTool.InitGlobalSingletonAsObservable ())

                // boostrap unload
                .SelectMany (_ =>
                {
                    if (SceneManager.sceneCount > 1)
                    {
                        return SceneControlManager.UnloadSceneAsObservable (BootstrapSceneName);
                    }
                    return Observable.ReturnUnit ();
                })

                .Do (_ => UtilLog.Log ("Bootstrap complete."))
                .Select (_ => true);
        }

        //============================================//

        // 부트스트랩 내용 구현부
        protected virtual IObservable<Unit> OnBootstrapAsObservable ()
        {
            return Observable.ReturnUnit ()
                // application init
                .Do (_ =>
                {
#if UNITY_IOS || UNITY_ANDROID
                    Application.targetFrameRate = 60;
#else
                    QualitySettings.vSyncCount = 1;
#endif
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                    Screen.SetResolution (Screen.width, Screen.height, true);
                })

                .Do (_ =>
                {
                    // set bootsrap canvas resolution
                    // TODO : 해상도는 상수가 아니라 Scriptableobject로 관리되어야함
                    GlobalCanvasUIManager.SetCanvasScaler (GlobalCanvasUIManager.instance.globalCanvas);

                });

        }
    }
}