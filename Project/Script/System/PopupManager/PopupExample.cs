using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 팝업 만들기 예시, 해당 컴포넌트를 게임오브젝트에 부착함
    /*
    // 팝업 열기 예시
    PopupManager.GetPopupAsObservable<PopupExample> ("PopupExampleAddressPath")
        .Do (popup => popup.SetData (new PopupExample.Setting ()
        {
            setTile = "Title",
            setContent = "Content",
            setData1 = 1
        }))
        .SelectMany (pop => pop.ShowAsObservable ())
        .Subscribe (result =>
        {
            UnityEngine.Debug.Log (result.restunData1);
            UnityEngine.Debug.Log (result.restunData2);
            UnityEngine.Debug.Log (result.restunData3);
        });
    */
    public class PopupExample : PopupBase<PopupExample.Setting, PopupExample.Result>
    {
        public class Setting
        {
            public string setTile;
            public string setContent;
            public int setData1;
        }
        public class Result
        {
            public int restunData1;
            public int restunData2;
            public int restunData3;
        }
        public override IObservable<Unit> OnShowAsObservable ()
        {
            // -init action /OR/ -show action
            // setting.setTile;
            // setting.setContent;
            // setting.setData1;
            return base.OnShowAsObservable ();
        }
        public override IObservable<Result> OnCloseAsObservable ()
        {
            // -close action
            // result.restunData1;
            // result.restunData2;
            // result.restunData3;
            return base.OnCloseAsObservable ();
        }
    }
}