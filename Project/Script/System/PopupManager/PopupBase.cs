using System;
using UniRx;
using UnityEngine;


namespace SugyeongKim.Util
{
    public abstract class PopupBase : MonoBehaviour
    {
        public virtual IObservable<PopupBase> InitAsObservable ()
        {
            return Observable.Return (this);
        }

        public virtual void Close (bool isReleseFields = true)
        {
            PopupManager.Close (this, isReleseFields);
        }
    }

    public abstract class PopupBase<T> : PopupBase where T : PopupBase
    {
        public new virtual void Close (bool isReleseFields = true)
        {
            base.Close (isReleseFields);
        }
    }
}