using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 해당 class는 예제이므로 초기화 및 생성을 무시함, 복사시 해당 어트리뷰트를 빼야함
    [GlobalSingletonIgnore (true)]
    // Bootstrap 구현 예제
    public class BootstrapTemplate : Bootstrap<BootstrapTemplate>, IGlobalSingletonInit
    {
        private void Start ()
        {
            // 씬 이동 예시
            BootstrapInit ()
                .Where (isInit => isInit)
                .SelectMany (_ => SceneControlManager.LoadScene ("main", false))
                .Subscribe ();
        }

        // 다른 씬에서 시작할 경우 이렇게 호출해서 초기화를 시도
        /*
        public class SceneMain : MonoBehaviour
        {
            private void Start ()
            {
                Observable.ReturnUnit ()
                    .SelectMany (_ => BootstrapTemplate.instance.BootstrapInit ())
                    .Subscribe (_ =>
                    {
                        // callback
                    });
            }
        }
        */
        //============================================//

        public override IObservable<bool> BootstrapInit ()
        {
            if (isBootstrapInit)
            {
                return Observable.Return (false);
            }
            else
            {
                isBootstrapInit = true;
                return Observable.ReturnUnit ()
                    .DoOnSubscribe (() => UtilLog.Log ("Bootstrap start."))
                    .DoOnCompleted (() => UtilLog.Log ("Bootstrap complete."))

                    // 시스템 초기화
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

                    // 싱글톤 초기화
                    .SelectMany (_ => SingletonTool.InitGlobalSingletonAsObservable ())
                    .Select (_ => true);
            }
        }
    }
}