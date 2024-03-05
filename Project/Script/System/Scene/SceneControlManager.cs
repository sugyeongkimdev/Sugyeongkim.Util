using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    public class SceneControlManager : GlobalSingleton<SceneControlManager>
    {
        // 예시
        public void Example ()
        {
            // 씬 전환
            SceneControlManager
                .LoadScene (
                    "Lobby",
                    SceneControlManager.emptySceneName
                ).Subscribe ();
            // 씬 전환 및 화면이동 효과
            SceneControlManager
                .LoadScene (
                    "Lobby",
                    SceneControlManager.emptySceneName,
                    inObservable: () => TransitionManager.instance.FadeIn (),
                    outObservable: () => TransitionManager.instance.FadeOut ()
                ).Subscribe ();
        }

        //==========================================================//

        public static string emptySceneName => "sugyeongkim.Util.empty";
        public static string bootstrapSceneName => "sugyeongkim.Util.bootstrap";

        private static Scene emptyScene;
        private static Scene bootstrapScene;
        private static string CurrentSceneName = "";

        public override IObservable<Unit> InitAsObservable ()
        {
            emptyScene = SceneManager.GetSceneByName (emptySceneName);
            bootstrapScene = SceneManager.GetSceneByName (bootstrapSceneName);
            CurrentSceneName = SceneManager.GetActiveScene ().name;
            return base.InitAsObservable ();
        }

        /// <param name="loadTargetSceneName">이동할 씬 이름</param>
        /// <param name="loadingSceneName">중간에 보여줄 씬 이름</param>
        /// <param name="curSceneUnload">지금 씬을 unload할지</param>
        /// <param name="loadTargetSceneMode">이동할 씬 로드 방법</param>
        /// <returns></returns>
        public static IObservable<Unit> LoadScene (
            string loadTargetSceneName,
            string loadingSceneName = null,
            bool curSceneUnload = true,
            LoadSceneMode loadTargetSceneMode = LoadSceneMode.Additive,
            Func<IObservable<Unit>> inObservable = null,
            Func<IObservable<Unit>> outObservable = null)
        {
            loadingSceneName ??= emptySceneName;
            //var curScene = SceneManager.GetActiveScene ();
            //var nextScene = SceneManager.GetSceneByName (nextSceneName)

            // in, out
            inObservable = inObservable == null ? () => Observable.ReturnUnit () : inObservable;
            outObservable = outObservable == null ? () => Observable.ReturnUnit () : outObservable;

            return Observable.ReturnUnit ()
                .SelectMany (_ => inObservable.Invoke ())
                .SelectMany (_ => LoadSceneAsObservable (loadingSceneName, LoadSceneMode.Additive))
                .DelayFrame (33)
                .SelectMany (_ => curSceneUnload ?
                    UnloadSceneAsObservable (CurrentSceneName) :
                    Observable.ReturnUnit ())
                .SelectMany (_ => LoadSceneAsObservable (loadTargetSceneName, loadTargetSceneMode))
                .SelectMany (_ => UnloadSceneAsObservable (loadingSceneName))
                .Do (_ =>
                {
                    if (loadTargetSceneName != bootstrapSceneName)
                    {
                        CurrentSceneName = loadTargetSceneName;
                    }
                })
                .DelayFrame (33)
                .SelectMany (_ => outObservable.Invoke ())
                .AsUnitObservable ();
        }

        //============================================//

        // 씬 로드
        private static IObservable<AsyncOperation> LoadSceneAsObservable (string sceneName, LoadSceneMode mode)
        {
            LoadSceneParameters param = new LoadSceneParameters (mode);

            var operation = SceneManager.LoadSceneAsync (sceneName, param);
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