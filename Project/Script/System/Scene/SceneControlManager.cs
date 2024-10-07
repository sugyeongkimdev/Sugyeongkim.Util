using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    public class SceneControlManager : GlobalSingleton<SceneControlManager>
    {
        // Empty 씬 이름
        public const string EmptySceneName = "EmptyScene";

        //==========================================================//

        // 현재 씬 이름
        public static string CurrentSceneName { get; set; } = "";

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
                // Empty 씬 생성
                .Do (_ => SceneManager.CreateScene (EmptySceneName))
                .DelayFrame (33)
                // 이전 씬 해제
                .SelectMany (_ => UnloadSceneAsObservable (unloadBefore ? CurrentSceneName : ""))
                // 목표 씬 로드
                .SelectMany (_ => LoadSceneAsObservable (loadScene, loadSceneMode))
                .Do (_ => { CurrentSceneName = loadScene; })
                // Empty 씬 해제
                .SelectMany (_ => UnloadSceneAsObservable (EmptySceneName))
                .DelayFrame (33)
                // TransitionManager out 실행
                .SelectMany (_ => outObservable)
                .AsUnitObservable ();
        }

        //============================================//

        // 씬 로드
        public static IObservable<AsyncOperation> LoadSceneAsObservable (string sceneName, LoadSceneMode mode)
        {
            var operation = SceneManager.LoadSceneAsync (sceneName, mode);
            if (operation == null)
            {
                UtilLog.Error ($"invalid sceneName : {sceneName}");
                return Observable.Return<AsyncOperation> (default);
            }

            operation.allowSceneActivation = true;
            return operation.AsAsyncOperationObservable ();
        }

        // 씬 해제
        public static IObservable<Unit> UnloadSceneAsObservable (string sceneName)
        {
            if (string.IsNullOrWhiteSpace (sceneName))
            {
                return Observable.ReturnUnit ();
            }

            var operation = SceneManager.UnloadSceneAsync (sceneName);
            if (operation == null)
            {
                UtilLog.Error ($"invalid sceneName : {sceneName}");
                return Observable.ReturnUnit ();
            }

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