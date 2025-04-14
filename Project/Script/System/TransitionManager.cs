using System;
using SugyeongKim.Util;
using UniRx;
using UnityEngine;

// 전환 매니저
public class TransitionManager : GlobalSingleton<TransitionManager>
{
    public CanvasGroup transitionCanvasGroup;

    //==========================================================//
    public enum TransitionType { Straight, Fade }
    private static SerialDisposable serialDisposable = new SerialDisposable ();

    public override IObservable<Unit> InitAsObservable ()
    {
        transitionCanvasGroup.gameObject.SetActive (false);

        return base.InitAsObservable ();
    }

    //==========================================================//

    // 전환 실행
    public static void Transition (TransitionType transitionType, Action transitionAction, Action callback = null)
    {
        serialDisposable.Disposable = TransitionAsObservable (transitionType, transitionAction)
            .Subscribe (_ => { callback?.Invoke (); });
    }
    public static IObservable<Unit> TransitionAsObservable (TransitionType transitionType, Action transitionAction)
    {
        switch (transitionType)
        {
            case TransitionType.Straight:
                return instance.StraightAsObservable (transitionAction);
            case TransitionType.Fade:
                return instance.FadeAnimationAsObservable (transitionAction);
        }
        return Observable.ReturnUnit ();
    }

    // 정리
    private void Clear ()
    {
        transitionCanvasGroup.gameObject.SetActive (false);
        transitionCanvasGroup.alpha = 0f;
    }

    //==========================================================//
    // Straight, 곧바로
    private IObservable<Unit> StraightAsObservable (Action transitionAction)
    {
        return Observable.ReturnUnit ()
            .SelectMany (_ => StraightIn ())
            .Do (_ => transitionAction?.Invoke ())
            .SelectMany (_ => StraightOut ());
    }
    public IObservable<Unit> StraightIn ()
    {
        Clear ();
        transitionCanvasGroup.gameObject.SetActive (true);
        transitionCanvasGroup.alpha = 1f;
        return Observable.ReturnUnit ();
    }
    public IObservable<Unit> StraightOut ()
    {
        transitionCanvasGroup.alpha = 0f;
        Clear ();
        return Observable.ReturnUnit ();
    }

    //==========================================================//
    // Fade, 점진적
    private IObservable<Unit> FadeAnimationAsObservable (Action transitionAction)
    {
        return Observable.ReturnUnit ()
            .SelectMany (_ => FadeIn ())
            .Do (_ => transitionAction?.Invoke ())
            .SelectMany (_ => FadeOut ());
    }

    public IObservable<Unit> FadeIn (float speed = 25f)
    {
        // 구독시 실행
        return Observable.Defer (() => Observable
            .Create<Unit> (ob =>
            {
                // 초기화
                Clear ();
                transitionCanvasGroup.gameObject.SetActive (true);

                // 애니메이션
                return Observable.IntervalFrame (1)
                    .AsUnitObservable ()
                    .Subscribe (_ =>
                    {
                        transitionCanvasGroup.alpha += speed * Time.deltaTime;
                        if (transitionCanvasGroup.alpha >= 1f)
                        {
                            ob.OnNext (Unit.Default);
                            ob.OnCompleted ();
                        }
                    });
            }));
    }

    public IObservable<Unit> FadeOut (float speed = 25f)
    {
        // 구독시 실행
        return Observable.Defer (() => Observable
            .Create<Unit> (ob =>
            {
                // 초기화
                transitionCanvasGroup.alpha = 1f;

                // 애니메이션
                return Observable.IntervalFrame (1)
                    .AsUnitObservable ()
                    .Subscribe (_ =>
                    {
                        transitionCanvasGroup.alpha -= speed * Time.deltaTime;
                        if (transitionCanvasGroup.alpha <= 0f)
                        {
                            ob.OnNext (Unit.Default);
                            ob.OnCompleted ();
                        }
                    });
            })
            .Do (_ =>
            {
                Clear ();
            }));
    }
}
