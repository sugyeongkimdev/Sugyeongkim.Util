using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 팝업 만들기 예시, 해당 컴포넌트를 게임오브젝트에 부착함
    public class ExamplePopup : Popup<ExamplePopup.Result, ExamplePopup.Setting>
    {
        // 팝업 열기 예시
        public static void OpenMethod ()
        {
            var setting = new ExamplePopup.Setting ()
            {
                setData1 = "Title",
                setData2 = "Content",
                setData3 = "etc",
            };
            ShowPopupExampleAsObservable (setting).Subscribe (result =>
            {
                // cloase callbask
                if (result.isOkClick)
                {
                    // popup ok click action
                }
                Debug.Log (result.restunData1);
                Debug.Log (result.restunData2);
                Debug.Log (result.restunData3);
            });
        }
        public static IObservable<ExamplePopup.Result> ShowPopupExampleAsObservable (ExamplePopup.Setting setting)
        {
            return PopupManager.GetPopupAsObservable<ExamplePopup> ("PopupExampleAddressPath")
                .Do (popup => popup.SetData (setting))
                .SelectMany (popup => popup.ShowAsObservable ());
        }

        //============================================//

        public class Setting
        {
            public string setData1;
            public string setData2;
            public string setData3;
        }
        public class Result
        {
            public string restunData1;
            public string restunData2;
            public string restunData3;
            public bool isOkClick;
        }

        //============================================//

        public override IObservable<Unit> OnShowAsObservable ()
        {
            // -init popup action
            // -show popup action
            Debug.Log (setting.setData1);
            Debug.Log (setting.setData2);
            Debug.Log (setting.setData3);
            return base.OnShowAsObservable ();
        }
        public override IObservable<Result> OnCloseAsObservable ()
        {
            // -close popup action
            result.restunData1 = $"return1 {setting.setData1}";
            result.restunData2 = $"return2 {setting.setData2}";
            result.restunData3 = $"return3 {setting.setData3}";
            result.isOkClick = true;
            return base.OnCloseAsObservable ();
        }
    }
}