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
        public static List<BasePopup> popupList { get; private set; }

        // 모든 팝업이 뒤로가기로 닫을 수 있는지 여부
        public static bool enableBackspaceClose { get; set; } = true;

        public static Action popupBackspaceCloseAction { get; set; }

        //============================================//

        private IDisposable updateDisposable;
        public override IObservable<Unit> InitAsObservable ()
        {
            popupList = new List<BasePopup> ();

            // 팝업 뒤로가기로 닫기 기능 추가
            updateDisposable?.Dispose ();
            updateDisposable = this.UpdateAsObservable ()
                .Where (_ => enableBackspaceClose)
                // 뒤로가기 클릭
                .Where (_ => Input.GetKeyDown (KeyCode.Escape) && popupList.Any ())
                // 뒤로가기로 해당 팝업이 닫을 수 있는지 체크
                .Where (_ => popupList.Last ().EnableBackspaceClose)
                // 뒤로가기로 팝업이 닫힌 경우 해당 이벤트 실행, 보통 뒤로가기 닫기 사운드 출력용일듯
                .Do (_ => popupBackspaceCloseAction?.Invoke ())
                // 닫기
                .SelectMany (_ => popupList.Last ().CloseAsObservable ())
                .Subscribe ()
                .AddTo (this);

            return base.InitAsObservable ();
        }

        //============================================//

        // 생성된 팝업을 팝업 매니저에 추가
        // TODO : 팝업 중복추가에 대한 처리를 해야함
        public static IObservable<Result> AddPopup<Result> (BasePopup<Result> popup) where Result : new()
        {
            popupList.Add (popup);
            popup.transform.SetParent (GlobalCanvasUIManager.instance.PopupLayer);
            return popup.ShowAsObservable ();
        }

        public static void GetPopup<Popup> () where Popup : BasePopup
        {

        }

        //============================================//

        // 팝업 열기, 닫힐때 Result 발행
        private static bool IsCreatingPopup;
        public static IObservable<Popup> GetPopupAsObservable<Popup> (string popupAddressPath, GameObject onDestroy = null) where Popup : BasePopup
        {
            return Observable.ReturnUnit ()
                .Do (_ =>
                {
                    IsCreatingPopup = true;
                })
                .SelectMany (_ => AddressablesManager.InstanctiateAsObservable<Popup> (
                    popupAddressPath,
                    onDestroy,
                    GlobalCanvasUIManager.instance.PopupLayer))
                .Do (popup =>
                {
                    IsCreatingPopup = false;
                });
        }

        // 팝업 닫기
        public static bool TryClosePopup (BasePopup popup, bool isReleseFields = true)
        {
            if (IsCreatingPopup)
            {
                return false;
            }
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

                    // addrsable instnace 해제
                    AddressablesManager.ReleaseInstance (closePopup.gameObject);
                    return true;
                }
            }
            return false;
        }
    }
}