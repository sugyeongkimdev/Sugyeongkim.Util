using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SugyeongKim.Util
{
    public class SceneControlManager : GlobalSingleton<SceneControlManager>
    {
        public static string emptySceneName = "sugyeongkim.Util.empty";
        public static string bootstrapSceneName = "sugyeongkim.Util.bootstrap";

        private static Scene emptyScene;
        private static Scene bootstrapScene;

        public override IObservable<Unit> InitAsObservable ()
        {
            emptyScene = SceneManager.GetSceneByName (emptySceneName);
            bootstrapScene = SceneManager.GetSceneByName (bootstrapSceneName);
            return base.InitAsObservable ();
        }

        /// <param name="nextSceneName">이동할 이름</param>
        /// <param name="nextMode">이동할 씬 로드 방법</param>
        /// <param name="curSceneUnload">지금 씬을 unload할지</param>
        /// <returns></returns>
        public static IObservable<Unit> LoadScene (string nextSceneName,
            bool curSceneUnload = true,
            LoadSceneMode nextMode = LoadSceneMode.Additive)
        {
            var curScene = SceneManager.GetActiveScene ();
            //var nextScene = SceneManager.GetSceneByName (nextSceneName);

            return Observable.ReturnUnit ()
                .SelectMany (_ => LoadSceneAsObservable (emptySceneName, LoadSceneMode.Additive))
                .SelectMany (_ => LoadSceneAsObservable (nextSceneName, nextMode))
                .SelectMany (_ => curSceneUnload ?
                    UnloadSceneAsObservable (curScene.name).AsUnitObservable () :
                    Observable.ReturnUnit ())
                .SelectMany (_ => UnloadSceneAsObservable (emptySceneName))
                .AsUnitObservable ();
        }

        //============================================//

        // 씬 로드
        private static IObservable<AsyncOperation> LoadSceneAsObservable (string sceneName, LoadSceneMode mode)
        {
            LoadSceneParameters param = new (mode);

            var operation = SceneManager.LoadSceneAsync (sceneName, param);
            operation.allowSceneActivation = true;
            return operation.AsAsyncOperationObservable ();
        }
        // 씬 해제
        private static IObservable<AsyncOperation> UnloadSceneAsObservable (string sceneName)
        {
            UnloadSceneOptions param = UnloadSceneOptions.None;

            var operation = SceneManager.UnloadSceneAsync (sceneName, param);
            operation.allowSceneActivation = true;
            return operation.AsAsyncOperationObservable ();
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