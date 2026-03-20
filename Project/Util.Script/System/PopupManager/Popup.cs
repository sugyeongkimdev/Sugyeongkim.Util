using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    public abstract class Popup : MonoBehaviour
    {
        public class PopupSetting { }
        public class PopupResult
        {
            public bool isOkClick;
        }

        //============================================//

        // 해당 팝업이 뒤로가기로 팝업을 닫을 수 있는지 여부
        public bool EnableBackspaceClose_Local { get; set; } = true;

        protected Subject<Unit> onCloseSubject = new Subject<Unit> ();

        //============================================//

        // 팝업이 열리고 닫힐때까지 메세지를 보류함
        public virtual IObservable<Unit> ShowAsObservable ()
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => OnShowAsObservable ())
                .SelectMany (_ => OnCloseAsObservable ());
        }

        // 팝업 닫기
        public virtual void Close ()
        {
            if (PopupManager.TryClosePopup (this))
            {
                onCloseSubject.OnNext (Unit.Default);
                onCloseSubject.OnCompleted ();
            }
        }

        public virtual void Relase ()
        {
            onCloseSubject.Dispose ();
        }

        //============================================//

        public virtual IObservable<Unit> OnShowAsObservable ()
        {
            return Observable.ReturnUnit ();
        }
        public virtual IObservable<Unit> OnCloseAsObservable ()
        {
            return onCloseSubject;
        }
    }

    //============================================//

    public abstract class Popup<Result> : Popup where Result : new()
    {
        // 팝업 닫힐시 결과값
        protected Result result = new Result ();

        // 팝업 닫힐시 결과값 unirx 이벤트 처리
        protected Subject<Result> resultSubject = new Subject<Result> ();

        //============================================//

        // 팝업 열릴시 unirx 이벤트 처리
        public virtual new IObservable<Result> ShowAsObservable ()
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => OnShowAsObservable ())
                .SelectMany (_ => OnCloseAsObservable ());
        }

        public override void Close ()
        {
            if (PopupManager.TryClosePopup (this))
            {
                onCloseSubject.OnNext (Unit.Default);
                resultSubject.OnNext (result);
                onCloseSubject.OnCompleted ();
                resultSubject.OnCompleted ();
            }
        }
        public override void Relase ()
        {
            onCloseSubject.Dispose ();
            resultSubject.Dispose ();
        }

        //============================================//

        // 팝업 닫힐시 unirx 이벤트 처리
        public virtual new IObservable<Result> OnCloseAsObservable ()
        {
            return resultSubject;
        }
    }

    //============================================//

    public abstract class Popup<Result, Setting> : Popup<Result> where Result : new()
    {
        protected Setting setting;

        //============================================//

        // 팝업 데이터 넣기
        public virtual void SetData (Setting setting)
        {
            this.setting = setting;
        }
    }
}