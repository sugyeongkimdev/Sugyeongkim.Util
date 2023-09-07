using System;
using System.Collections.Generic;
using BigUtil;
using UniRx;
using UnityEngine;

#if USE_ADDESSABLE
using UnityEngine.AddressableAssets;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
#endif

// https://planek.tistory.com/30

public class AddressResource : GlobalSingleton<AddressResource>
{
    // <path, load asset>, 로드된 에셋
    private static Dictionary<string, object> cachedLoadAssetDic;
    // <path, load observable>, 실행중인 로드 참조
    static Dictionary<string, IObservable<object>> loadRuntimeObservableDic;

    // 초기화
    public override IObservable<Unit> InitAsObservable ()
    {
#if USE_ADDESSABLE
        return Addressables.InitializeAsync (this).ToObservable ()
            .Do (_ =>
            {
                cachedLoadAssetDic?.Clear ();
                cachedLoadAssetDic = new ();
                loadRuntimeObservableDic?.Clear ();
                loadRuntimeObservableDic = new ();
            });
#else
        return Observable.Empty<Unit> ();
#endif
    }

    //============================================//

    // load
    public static IObservable<T> LoadAsObservable<T> (string key, GameObject onDestroy)
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
                        cachedLoadAssetDic.TryAdd (key, loadAsset);
                        loadRuntimeObservableDic.Remove (key);
                    })
                    .Do (loadAsset =>
                    {
                        onDestroy.OnDestroyAsObservable ()
                            .Subscribe (_ => UnloadAsObservable (loadAsset))
                            .AddTo (onDestroy);
                    });
                // 로드 이벤트 저장 및 반환
                loadRuntimeObservableDic.Add (key, loadObservable.Cast<T, object> ());
                return loadObservable;
            }
        }
#else
        return Observable.Empty<T> ();
#endif
    }
    public static IObservable<T> LoadAsObservable<T> (string key, Component onDestroy)
    {
#if USE_ADDESSABLE
        return LoadAsObservable<T> (key, onDestroy.gameObject);
#else
        return Observable.Empty<T> ();
#endif
    }

    // unload
    public static void UnloadAsObservable (object unloadAsset)
    {
#if USE_ADDESSABLE
        Addressables.Release (unloadAsset);
#endif
    }
    public static void UnloadAsObservable (string unloadAssetKey)
    {
#if USE_ADDESSABLE
        if (cachedLoadAssetDic.TryGetValue (unloadAssetKey, out object unloadAsset))
        {
            Addressables.Release (unloadAsset);
        }
#endif
    }

    //============================================//

    // 게임오브젝트
    public static IObservable<GameObject> InstanctiateAsObservable (string key, Transform parent = null)
    {
#if USE_ADDESSABLE
        return Addressables.InstantiateAsync (key, parent).ToUniTask ().ToObservable ();
#else
        return Observable.Empty<GameObject> ();
#endif
    }
    public static void Release (GameObject releaseTarget)
    {
#if USE_ADDESSABLE
        Addressables.ReleaseInstance (releaseTarget);
#endif
    }
}
