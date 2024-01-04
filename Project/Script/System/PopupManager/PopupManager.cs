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
        public static List<PopupBase> popupList { get; private set; }

        // 모든 팝업이 뒤로가기로 닫을 수 있는지 여부
        public static bool enableBackspaceClose { get; set; } = true;

        //============================================//

        private IDisposable updateDisposable;
        public override IObservable<Unit> InitAsObservable ()
        {
            popupList = new List<PopupBase> ();
            // 팝업 뒤로가기로 닫기 기능 추가
            updateDisposable?.Dispose ();
            updateDisposable = this.UpdateAsObservable ()
                .Where (_ => enableBackspaceClose)
                // 뒤로가기 클릭
                .Where (_ => Input.GetKeyDown (KeyCode.Escape) && popupList.Any ())
                // 뒤로가기로 해당 팝업이 닫을 수 있는지 체크
                .Where (_ => popupList[0].enableBackspaceClose)
                // 닫기
                .SelectMany (_ => popupList[0].CloseAsObservable ())
                .Subscribe ()
                .AddTo (this);
            return base.InitAsObservable ();
        }

        //============================================//
        // 팝업 열기, 닫힐때 Result 발행
        public static IObservable<Popup> GetPopupAsObservable<Popup> (string popupAddressPath, GameObject onDestroy = null) where Popup : PopupBase
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => AddressablesManager.InstanctiateAsObservable<Popup> (popupAddressPath, onDestroy, UICanvasManager.instance.popupLayer.transform))
                .Do (popup => { popupList.Add (popup); });
        }

        // 팝업 닫기
        public static void Close (PopupBase popup, bool isReleseFields = true)
        {
            for (int i = 0; i < popupList.Count; i++)
            {
                if (popupList[i] == popup)
                {
                    var closePopup = popupList[i];
                    popupList.RemoveAt (i);

                    if (isReleseFields)
                    {
                        // 내부필드 해제
                        popup.ReleaseFields ();
                    }

                    AddressablesManager.ReleaseInstance (closePopup.gameObject);
                    return;
                }
            }
        }
    }
}