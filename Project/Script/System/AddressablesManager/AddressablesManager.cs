using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#if USE_ADDESSABLE
using UnityEngine.AddressableAssets;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
#endif

namespace SugyeongKim.Util
{
    public class AddressablesManager : GlobalSingleton<AddressablesManager>
    {
        // <path, load asset>, 로드된 에셋
        private static Dictionary<string, object> cachedLoadAssetDic;
        // <path, load observable>, 실행중인 로드 참조
        static Dictionary<string, IObservable<object>> loadRuntimeObservableDic;

        // 초기화
        public override IObservable<Unit> InitAsObservable ()
        {
#if USE_ADDESSABLE
            //return Addressables.InitializeAsync (true).ToUniTask ().ToObservable ()
            //    .Do (_ =>
            //    {
            //        cachedLoadAssetDic?.Clear ();
            //        cachedLoadAssetDic = new Dictionary<string, object> ();
            //        loadRuntimeObservableDic?.Clear ();
            //        loadRuntimeObservableDic = new Dictionary<string, IObservable<object>> ();
            //    })
            //    .AsUnitObservable ();
            cachedLoadAssetDic?.Clear ();
            cachedLoadAssetDic = new Dictionary<string, object> ();
            loadRuntimeObservableDic?.Clear ();
            loadRuntimeObservableDic = new Dictionary<string, IObservable<object>> ();
#endif
            return Observable.ReturnUnit ();
        }

        //============================================//

        // key(or path)로 에셋 로드
        public static IObservable<T> LoadAssetAsObservable<T> (string key, GameObject onDestroy = null)
        {
#if USE_ADDESSABLE
            if (cachedLoadAssetDic.TryGetValue (key, out object cachedAsset) && cachedAsset != null)
            {
                return Observable.Return ((T)cachedAsset);
            }
            else
            {
                // 로드중인 에셋 이벤트 반환
                if (loadRuntimeObservableDic.TryGetValue (key, out var currentRuntimeObservable))
                {
                    return currentRuntimeObservable.Cast<object, T> ();
                }
                else
                {
                    // 로드 이벤트 생성
                    var loadObservable = Addressables.LoadAssetAsync<T> (key).ToUniTask ().ToObservable ()
                        .Do (loadAsset =>
                        {
                            cachedLoadAssetDic.Add (key, loadAsset);
                            loadRuntimeObservableDic.Remove (key);
                        })
                        .Do (loadAsset =>
                        {
                            if (onDestroy)
                            {
                                onDestroy.OnDestroyAsObservable ()
                                    .Subscribe (_ => UnloadAssetAsObservable (loadAsset))
                                    .AddTo (onDestroy);
                            }
                        });
                    // 로드 이벤트 저장 및 반환
                    loadRuntimeObservableDic.Add (key, loadObservable.Cast<T, object> ());
                    return loadObservable;
                }
            }
#else
            return Observable.Return<T> (default);
#endif
        }
        public static IObservable<T> LoadAssetAsObservable<T> (string key, Component onDestroy = null)
        {
#if USE_ADDESSABLE
            return LoadAssetAsObservable<T> (key, onDestroy.gameObject);
#else
            return Observable.Return<T> (default);
#endif
        }

        // 로드된 에셋 해제
        public static void UnloadAssetAsObservable (object unloadAsset)
        {
#if USE_ADDESSABLE
            Addressables.Release (unloadAsset);
#endif
        }
        // 로드된 에셋 key로 해제
        public static void UnloadAssetAsObservable (string unloadAssetKey)
        {
#if USE_ADDESSABLE
            if (cachedLoadAssetDic.TryGetValue (unloadAssetKey, out object unloadAsset))
            {
                Addressables.Release (unloadAsset);
                cachedLoadAssetDic.Remove (unloadAssetKey);
            }
#endif
        }

        //============================================//

        // 게임 오브젝트 인스턴스<T>
        public static IObservable<T> InstanctiateAsObservable<T> (string key, GameObject onDestroy,
            Transform parent = null, bool instantiateInWorldSpace = false, Vector3 position = default, Quaternion rotation = default)
            where T : Component
        {
            return InstanctiateAsObservable (key, onDestroy, parent, instantiateInWorldSpace, position, rotation)
                .Select (instance => instance ? instance.GetComponent<T> () : default);
        }
        // 게임 오브젝트 인스턴스
        public static IObservable<GameObject> InstanctiateAsObservable (string key, GameObject onDestroy,
            Transform parent = null, bool instantiateInWorldSpace = false, Vector3 position = default, Quaternion rotation = default)
        {
#if USE_ADDESSABLE
            return Observable.ReturnUnit ()
                    .SelectMany (Addressables.InstantiateAsync (key, parent, instantiateInWorldSpace).ToUniTask ().ToObservable ())
                    .Do (instance =>
                    {
                        if (position != default) { instance.transform.position = position; }
                        if (rotation != default) { instance.transform.rotation = rotation; }
                        if (onDestroy)
                        {
                            onDestroy.OnDestroyAsObservable ()
                                .Subscribe (_ => ReleaseInstance (instance))
                                .AddTo (onDestroy);
                        }
                    });
#else
            return Observable.Return<GameObject> (default);
#endif
        }
        // 게임 오브젝트 인스턴스 해제
        public static void ReleaseInstance (GameObject releaseTarget)
        {
#if USE_ADDESSABLE
            Addressables.ReleaseInstance (releaseTarget);
#endif
        }
    }
}