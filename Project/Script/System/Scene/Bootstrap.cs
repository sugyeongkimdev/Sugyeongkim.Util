using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    // Bootstrap, 초기화시 필요한 부분은 수정해서 사용바람

    public class Bootstrap : GlobalSingleton<Bootstrap>
    {
        // bootstarb 시작
        private void Start ()
        {
            BootstrapInit ()
                .Where (isMoveNext => isMoveNext)
                // bootstarb scene -> main scene
                // TODO : 이동할 Scene 이름은 상수가 아니라 유니티 에셋으로 관리되어야함
                .SelectMany (_ => SceneControlManager.LoadScene ("main", false))
                .Subscribe ();
        }

        //============================================//

        /*
        // 최초 실행이 다른 씬에서 시작할 경우 해당 코드를 호출해서 bootstarp 초기화를 시도
        public class SceneMain : MonoBehaviour
        {
            private void Start ()
            {
                Observable.ReturnUnit ()
                    .SelectMany (_ => Bootstrap.instance.BootstrapInit ())
                    .Subscribe (_ =>
                    {
                        // callback
                    });
            }
        }
        */

        // 프로그램 '최초 실행시' 어느씬이든 초기화 가능하도록 초기화를 모아둠, scene 최초 진입시 실행하면 됨
        // bootstrap을 통하지 않고도 실행할 가능하도록 하기 위한 목적
        private static bool isInit = false;
        public static IObservable<bool> BootstrapInit ()
        {
            if (isInit)
            {
                return Observable.Return (false);
            }
            else
            {
                isInit = true;
                return Observable.ReturnUnit ()
                    .DoOnSubscribe (() => UtilLog.Log ("Bootstrap start."))
                    .DoOnCompleted (() => UtilLog.Log ("Bootstrap complete."))

                    // application init
                    .Do (_ =>
                    {
#if UNITY_IOS || UNITY_ANDROID
                        Application.targetFrameRate = 60;
#else
                        QualitySettings.vSyncCount = 1;
#endif
                        Screen.sleepTimeout = SleepTimeout.NeverSleep;
                        Screen.SetResolution ((int)Screen.width, (int)Screen.height, true);
                    })

                    // try load bootstrap scene
                    .SelectMany (_ => TrySceneLoadBootstrap ())
                    // set bootsrap canvas resolution
                    // TODO : 해상도는 상수가 아니라 유니티 에셋으로 관리되어야함
                    .Do (_ => UICanvasManager.instance.scaler.referenceResolution = new Vector2 (1080, 1920))

                    // singleton init
                    .SelectMany (_ => SingletonTool.InitGlobalSingletonAsObservable ())
                    .Select (_ => true);
            }
        }

        // 부트스트랩씬이 없을경우 씬 로드하기
        private static IObservable<Unit> TrySceneLoadBootstrap ()
        {
            if (SceneControlManager.IsLoaded (SceneControlManager.bootstrapSceneName))
            {
                return Observable.ReturnUnit ();
            }
            return SceneControlManager.LoadScene (SceneControlManager.bootstrapSceneName, false, LoadSceneMode.Additive);
        }
    }
}