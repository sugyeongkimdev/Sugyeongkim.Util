using System;
using SugyeongKim.Util;
using UniRx;
using UnityEngine;

public class TransitionManager : GlobalSingleton<TransitionManager>
{
    // 예시
    public void Example ()
    {
        // 씬 전환시 화면이동 효과
        SceneControlManager
            .LoadScene (
                "Lobby",
                SceneControlManager.emptySceneName,
                inObservable: () => TransitionManager.instance.FadeIn (),
                outObservable: () => TransitionManager.instance.FadeOut ()
            ).Subscribe ();
    }

    //==========================================================//

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
    public IObservable<Unit> FadeIn ()
    {
        Clear ();
        transitionCanvasGroup.gameObject.SetActive (true);

        return Observable
            .Create<Unit> (ob =>
            {
                return Observable.IntervalFrame (1)
                    .AsUnitObservable ()
                    .Subscribe (_ =>
                    {
                        transitionCanvasGroup.alpha += 5f * Time.deltaTime;
                        if (transitionCanvasGroup.alpha >= 1f)
                        {
                            ob.OnNext (Unit.Default);
                            ob.OnCompleted ();
                        }
                    });
            });
    }
    public IObservable<Unit> FadeOut ()
    {
        transitionCanvasGroup.alpha = 1f;
        return Observable
            .Create<Unit> (ob =>
            {
                return Observable.IntervalFrame (1, FrameCountType.Update)
                    .TakeWhile (_ => transitionCanvasGroup.alpha > 0f)
                    .AsUnitObservable ()
                    .Subscribe (_ =>
                    {
                        transitionCanvasGroup.alpha -= 5f * Time.deltaTime;
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
            });
    }
}
