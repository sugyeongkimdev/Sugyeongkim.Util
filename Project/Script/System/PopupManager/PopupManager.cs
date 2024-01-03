using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SugyeongKim.Util
{
    public class PopupManager : GlobalSingleton<PopupManager>
    {
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
        // 팝업 열기
        public static IObservable<T> OpenAsObservable<T> (string addressPath, GameObject onDestroy = null) where T : PopupBase<T>
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => AddressablesManager.InstanctiateAsObservable<T> (
                    addressPath,
                    onDestroy,
                    UICanvasManager.instance.popupLayer.transform))
                .SelectMany (popup => popup.InitAsObservable ().Select (_ => popup))
                .Do (popup => popupList.Add (popup));
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
                        ReleaseFields (popup);
                    }

                    AddressablesManager.ReleaseInstance (closePopup.gameObject);
                    return;
                }
            }
        }
        // 팝업 내부 필드 전부 해제
        public static void ReleaseFields (PopupBase releaseTargetPopup)
        {
            var popupType = releaseTargetPopup.GetType ();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (var field in popupType.GetFields (flags))
            {
                field.SetValue (releaseTargetPopup, null);
            }
            foreach (var prop in popupType.GetProperties (flags))
            {
                prop.SetValue (releaseTargetPopup, null);
            }
        }
    }
}