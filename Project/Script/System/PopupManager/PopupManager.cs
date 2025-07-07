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
        public static OrderedDictionary<Type, Popup> popupDic { get; private set; } = new ();

        // 모든 팝업이 뒤로가기로 닫을 수 있는지 여부
        public static bool EnableBackspaceClose_Global { get; set; } = true;

        //============================================//

        private IDisposable updateDisposable;
        public override IObservable<Unit> InitAsObservable ()
        {
            // 팝업 뒤로가기로 닫기 기능 추가
            updateDisposable?.Dispose ();
            updateDisposable = this.UpdateAsObservable ()
                .Where (_ => EnableBackspaceClose_Global)
                // 뒤로가기 클릭
                .Where (_ => Input.GetKeyDown (KeyCode.Escape) && popupDic.Any ())
                // 뒤로가기로 해당 팝업이 닫을 수 있는지 체크
                .Where (_ => popupDic.LastValue.EnableBackspaceClose_Local)
                // 뒤로가기로 팝업이 닫힌 경우, 닫기 사운드 실행
                .Do (_ => SoundManager.PlaySFX (SFXType.PopupClose))
                // 닫기
                .SelectMany (_ => popupDic.LastValue.CloseAsObservable ())
                .Subscribe ()
                .AddTo (this);

            return base.InitAsObservable ();
        }

        //============================================//

        // 팝업 매니저에 추가
        public static void AddPopup (Popup popup, bool ignoreAlert = false)
        {
            popup.transform.SetParent (GlobalCanvasUIManager.instance.PopupLayer);
            if(popup.transform is RectTransform rectTrans)
            {
                rectTrans.localScale = Vector3.one;         // 팝업의 스케일을 초기화
                rectTrans.anchoredPosition= Vector2.zero;   // 위치 초기화
                //rectTrans.anchorMin = Vector2.zero;         // 앵커 최소값 초기화
                //rectTrans.anchorMax = Vector2.one;          // 앵커 최대값 초기화
                rectTrans.offsetMin = Vector2.zero;
                rectTrans.offsetMax = Vector2.zero;
                rectTrans.pivot = new Vector2(0.5f, 0.5f);  // 피벗을 중앙으로 설정
            }
            if (popupDic.TryAdd (popup.GetType(), popup) == false)
            {
                if (ignoreAlert == false)
                {
                    // 이미 존재하는 팝업인 경우, 기존 팝업을 닫고 새로 생성된 팝업을 추가
                    UtilLog.Error ($"AddPopup : {popup.GetType ()} is already in PopupManager");
                }
            }
        }

        // 팝업 가져오기
        public static IObservable<Popup> GetPopupAsObservable<Popup> (string popupAddressPath) where Popup : Util.Popup
        {
            if (popupDic.TryGetValue (typeof(Popup), out var popup))
            {
                // 팝업이 이미 존재하는 경우, 팝업을 반환
                return Observable.Return (popup as Popup);
            }
            else
            {
                // 팝업이 없을 경우, 팝업을 생성하고 팝업 매니저에 추가
                return CreatePopupAsObservable<Popup> (popupAddressPath)
                    .Do (popup =>
                    {
                        AddPopup (popup);
                    });
            }
        }

        //============================================//

        // 팝업 생성
        private static bool IsCreatingPopup;
        public static IObservable<Popup> CreatePopupAsObservable<Popup> (string popupAddressPath) where Popup : Util.Popup
        {
            return Observable.ReturnUnit ()
                .Do (_ =>
                {
                    IsCreatingPopup = true;
                })
                // 팝업 생성
                .SelectMany (_ => AddressablesManager.InstanctiateAsObservable<Popup> (
                    popupAddressPath,
                    instance.gameObject,
                    GlobalCanvasUIManager.instance.PopupLayer))
                .Do (popup =>
                {
                    //popup.PopupPath = popupAddressPath;
                    IsCreatingPopup = false;
                });
        }

        // 팝업 닫기 시도
        public static bool TryClosePopup (Popup popup, bool isReleseFields = true)
        {
            if (IsCreatingPopup)
            {
                // 팝업 생성중에 닫기 시도한 경우
                UtilLog.Error ($"TryClosePopup : {popup.GetType ()} is creating popup");
                return false;
            }

            if (popupDic.ContainsKey (popup.GetType()) == false)
            {
                // 팝업매니저에 없던 팝업임
                UtilLog.Error ($"TryClosePopup : {popup.GetType ()} is not in PopupManager");
            }
            else
            {
                popupDic.Remove (popup.GetType());
            }

            // 내부필드 해제
            if (isReleseFields)
            {
                popup.ReleaseFields ();
            }

            // addrsable instnace 해제
            AddressablesManager.ReleaseInstance (popup.gameObject);

            return true;
        }
    }
}