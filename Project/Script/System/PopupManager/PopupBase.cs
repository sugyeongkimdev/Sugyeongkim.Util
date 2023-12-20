using System;
using UniRx;
using UnityEngine;


namespace SugyeongKim.Util
{
    public abstract class PopupBase : MonoBehaviour
    {
        public void Close ()
        {
            PopupManager.Close (this);
        }
        public virtual IObservable<PopupBase> InitAsObservable ()
        {
            return Observable.Return (this);
        }
    }

    public abstract class PopupBase<T> : PopupBase where T : PopupBase
    {

    }
}