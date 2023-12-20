using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SugyeongKim.Util
{
    public class PopupManager : GlobalSingleton<PopupManager>
    {
        // 팝업을 표시할 layer 설정, bootstrap에 해당 매니저를 생성 후 inspector에 drag drop 가능 
        [field: SerializeField]
        public GameObject popupLayer { get; private set; }
        public static List<PopupBase> popupList { get; private set; }

        //============================================//

        public override IObservable<Unit> InitAsObservable ()
        {
            popupList = new List<PopupBase> ();
            this.UpdateAsObservable ()
                .Do (_ =>
                {
                    if (Input.GetKeyDown (KeyCode.Escape))
                    {
                        if (popupList.Any ())
                        {
                            Close (popupList[0]);
                        }
                    }
                })
                .Subscribe ()
                .AddTo (this);
            return base.InitAsObservable ();
        }

        //============================================//

        public static void SetLayer (GameObject setLayer)
        {
            instance.popupLayer = setLayer;
        }

        //============================================//
        // 팝업 열기
        public static IObservable<T> OpenAsObservable<T> (string addressPath, GameObject onDestroy = null) where T : PopupBase<T>
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => AddressablesManager.InstanctiateAsObservable<T> (addressPath, onDestroy, instance.popupLayer.transform))
                .SelectMany (popup => popup.InitAsObservable ().Select (_ => popup))
                .Do (popup => popupList.Add (popup));
        }
        // 팝업 닫기
        public static void Close (PopupBase popup)
        {
            for (int i = 0; i < popupList.Count; i++)
            {
                if (popupList[i] == popup)
                {
                    var closePopup = popupList[i];
                    popupList.RemoveAt (i);
                    AddressablesManager.ReleaseInstance (closePopup.gameObject);
                }
            }
        }
    }
}