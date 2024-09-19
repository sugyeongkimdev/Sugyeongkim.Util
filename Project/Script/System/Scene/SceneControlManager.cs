using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    public class SceneControlManager : GlobalSingleton<SceneControlManager>
    {
        private static string CurrentSceneName = "";

        //==========================================================//

        public static IObservable<Unit> LoadSceneAsObservable (
            string loadScene,
            bool unloadBefore = true,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive,
            IObservable<Unit> inObservable = null,
            IObservable<Unit> outObservable = null)
        {
            // in, out
            //inObservable = inObservable ?? Observable.Defer (() => inObservable);
            //outObservable = outObservable ?? Observable.Defer (() => outObservable);
            inObservable = inObservable ?? Observable.ReturnUnit ();
            outObservable = outObservable ?? Observable.ReturnUnit ();

            //return Observable.ReturnUnit ();

            return Observable.ReturnUnit ()
                // TransitionManager in 실행
                .SelectMany (_ => inObservable)
                .Do (_ => SceneManager.CreateScene ("EmptyScene"))

                .DelayFrame (33)
                // 이전 씬 해제
                .SelectMany (_ => UnloadSceneAsObservable (unloadBefore ? CurrentSceneName : ""))
                // 목표 씬 로드
                .SelectMany (_ => LoadSceneAsObservable (loadScene, loadSceneMode))
                // 로딩 씬 해제
                .SelectMany (_ => UnloadSceneAsObservable ("EmptyScene"))
                //.Do (_ =>
                //{
                //    if (loadTargetSceneName != bootstrapSceneName)
                //    {
                //        CurrentSceneName = loadTargetSceneName;
                //    }
                //})
                .DelayFrame (33)
                .SelectMany (_ => outObservable)
                .AsUnitObservable ();
        }

        //============================================//

        // 씬 로드
        private static IObservable<AsyncOperation> LoadSceneAsObservable (string sceneName, LoadSceneMode mode)
        {
            var operation = SceneManager.LoadSceneAsync (sceneName, mode);
            operation.allowSceneActivation = true;
            return operation.AsAsyncOperationObservable ();
        }

        // 씬 해제
        private static IObservable<Unit> UnloadSceneAsObservable (string sceneName)
        {
            if (string.IsNullOrWhiteSpace (sceneName))
            {
                return Observable.ReturnUnit ();
            }
            UnloadSceneOptions param = UnloadSceneOptions.None;

            var operation = SceneManager.UnloadSceneAsync (sceneName, param);
            operation.allowSceneActivation = true;
            return operation.AsAsyncOperationObservable ().AsUnitObservable ();
        }

        //============================================//

        // 해당 씬이 로드되어 있는지
        public static bool IsLoaded (string sceneName)
        {
            var checkScene = SceneManager.GetSceneByName (sceneName);
            return checkScene.isLoaded;
        }
    }
}