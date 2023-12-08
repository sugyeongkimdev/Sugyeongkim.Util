using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    public class PopupManager : GlobalSingleton<PopupManager>
    {
        // 팝업을 표시할 layer 설정, bootstrap에 해당 매니저를 생성 후 inspector에 drag drop 가능 
        [field: SerializeField]
        public GameObject popupLayer { get; private set; }
        public static void SetLayer (GameObject setLayer)
        {
            instance.popupLayer = setLayer;
        }

        //============================================//

        // TODO
        // bootstrab 씬 참조 생성
        // 팝업 정렬
        // Layer 정렬

        public static IObservable<T> OpenAsObservable<T> (string path, GameObject onDestroy = null) where T : PopupBase
        {
            return AddressablesManager.InstanctiateAsObservable<T> (path, onDestroy, instance.popupLayer.transform);
        }
    }
}