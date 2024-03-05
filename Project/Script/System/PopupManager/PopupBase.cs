using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    public abstract class PopupBase : MonoBehaviour
    {
        public class BaseSetting
        {
        }
        public class BaseResult
        {
            public bool isOkClick;
        }

        // 해당 팝업이 뒤로가기로 팝업을 닫을 수 있는지 여부
        public bool enableBackspaceClose { get; protected set; } = true;

        // 팝업 닫기 (unirx)
        public virtual IObservable<Unit> CloseAsObservable ()
        {
            PopupManager.TryClosePopup (this);
            return Observable.ReturnUnit ();
        }
        // 팝업 닫기
        public virtual void Close ()
        {
            PopupManager.TryClosePopup (this);
        }
    }

    //============================================//

    public abstract class PopupBase<Result> : PopupBase where Result : new()
    {
        // 팝업 닫힐시 결과값
        protected Result result = new Result ();
        // 팝업 닫힐시 결과값 unirx 이벤트 처리
        protected Subject<Result> resultSubject = new Subject<Result> ();

        //============================================//

        // 팝업 열기 (unirx), 열리고 닫힐때까지 메세지를 보류함
        public virtual IObservable<Result> ShowAsObservable ()
        {
            return Observable.ReturnUnit ()
                .SelectMany (_ => OnShowAsObservable ())
                .SelectMany (_ => OnCloseAsObservable ());
        }

        //============================================//

        // 팝업 닫기 (unirx)
        private bool isClosing;
        public override IObservable<Unit> CloseAsObservable ()
        {
            return Observable.ReturnUnit ()
                .Do (_ =>
                {
                    // 팝업 삭제 시도
                    if (PopupManager.TryClosePopup (this))
                    {
                        // 결과 반환
                        if (resultSubject.HasObservers)
                        {
                            resultSubject.OnNext (result);
                            resultSubject.OnCompleted ();
                            resultSubject.Dispose ();
                        }
                    }
                });
        }

        //============================================//

        // 팝업 열릴시 unirx 이벤트 처리
        public virtual IObservable<Unit> OnShowAsObservable ()
        {
            return Observable.ReturnUnit ();
        }
        // 팝업 닫힐시 unirx 이벤트 처리
        public virtual IObservable<Result> OnCloseAsObservable ()
        {
            return resultSubject;
        }
    }

    //============================================//

    public abstract class PopupBase<Setting, Result> : PopupBase<Result> where Result : new()
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