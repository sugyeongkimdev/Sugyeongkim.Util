
#if USE_DOTWEEN

using UniRx;
using System;
using DG.Tweening;
using UnityEngine.Events;

// DOTween과 UniRx 결합(DoTween + UniRx)
// https://qiita.com/t-matsunaga/items/4a6a41b8720c4763e59a
static public partial class DOTweenUniRxExtensions
{
    static public IObservable<Tween> OnCompleteAsObservable (this Tween tweener)
    {
        return Observable.Create<Tween> (o =>
        {
            tweener.OnComplete (() =>
            {
                o.OnNext (tweener);
                o.OnCompleted ();
            });
            return Disposable.Create (() =>
            {
                tweener.Kill ();
            });
        });
    }

    //static public IObservable<Tween> OnCompleteAsObservable (this Tween tweener)
    //{
    //    return Observable.FromEvent<TweenCallback, Tween> (
    //            x => () => x.Invoke (tweener),
    //            x => tweener.onComplete += x,
    //            x =>
    //            {
    //                tweener.Kill ();
    //                tweener.onComplete -= x;
    //            });
    //}
}

#endif