using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// namespace SugyeongKim.Util 처리 안함
public static class SelectableExtension
{
    //  타이머 Observable
    private static IObservable<Unit> TimeObservable (this Component comp, float time)
    {
        return Observable
            .Timer (TimeSpan.FromSeconds (time))
            .TakeUntilDestroy (comp)
            .AsUnitObservable ();
    }

    //============================================//

    // UniRx 버튼 확장
    // sharedCanExecute 사용법의 경우 아래 주소를 참조 바람
    // https://github.com/neuecc/UniRx?tab=readme-ov-file#reactivecommand-asyncreactivecommand

    /// <summary>
    /// 버튼 이벤트 등록. 버튼 클릭시 clickAction 실행.
    /// ex) btn.AddOnClick(() => {/*clickAction*/});
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="clickAction">Click Callback</param>
    /// <param name="sharedCanExecute">해당 prop을 공유하는 모든 버튼은 공유되는 버튼이 하나라도 사용 불가하면 전부 사용불가</param>
    public static void AddOnClick (this Button btn, Action clickAction,
        IReactiveProperty<bool> sharedCanExecute = null)
    {
        btn.AddOnClick (
                () => Observable.ReturnUnit ().Do (_ => clickAction?.Invoke ()),
                null,
                sharedCanExecute);
    }

    /// <summary>
    /// 버튼 이벤트 등록. 버튼 클릭시 clickAction 실행 후 interval 시간만큼 대기 후 회복됨.
    /// 이후 회복 및 새로고침시 refreshAction가 호출됨
    /// ex) btn.AddOnClick(
    ///     () => {/*clickAction*/},
    ///     1f/*1초 대기*/,
    ///     () => {/*새로고침 Callback. interval 종료 후 및 공유 이벤트 발동*/});
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="clickAction">Click Callback</param>
    /// <param name="interval">interval 시간(s) 만큼 대기 후 클릭 가능</param>
    /// <param name="refreshAction">새로고침. interval 시간이 흐른 뒤 호출되거나, sharedCanExecute이 호출하면 호출됨</param>
    /// <param name="sharedCanExecute">해당 prop을 공유하는 모든 버튼은 공유되는 버튼이 하나라도 사용 불가하면 전부 사용불가</param>
    public static void AddOnClickInterval (this Button btn, Action clickAction, float interval, Action refreshAction,
        IReactiveProperty<bool> sharedCanExecute = null)
    {
        btn.AddOnClick (
                () => Observable.Timer (TimeSpan.FromSeconds (interval)).DoOnSubscribe (() => clickAction?.Invoke ()),
                refreshAction,
                sharedCanExecute);
    }

    /// <summary>
    /// 버튼 이벤트 등록. 버튼 클릭시 clickObservableFunc가 호출되고 Func 내부 observable이 종료될때까지 대기
    /// ex) var ob1 = Observable.Timer(TimeSpan.FromSeconds (1f)).DoOnSubscribe (() => { /*Action1*/});
    ///     var ob2 = Observable.ReturnUnit ().Do (_ => {/*Action2*/});
    ///     btn1.AddOnClick(() => ob1);
    ///     btn2.AddOnClick(() => ob2);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="btn"></param>
    /// <param name="clickObservableFunc">클릭시 발동, Func 내부 observable이 끝날때까지 대기</param>
    /// <param name="sharedCanExecute">해당 prop을 공유하는 모든 버튼은 공유되는 버튼이 하나라도 사용 불가하면 전부 사용불가</param>
    public static void AddOnClick<T> (this Button btn, Func<IObservable<T>> clickObservableFunc,
        IReactiveProperty<bool> sharedCanExecute = null)
    {
        btn.AddOnClick (
                clickObservableFunc,
                null,
                sharedCanExecute);
    }

    /// <summary>
    /// 버튼 이벤트 등록. 버튼 클릭시 clickAction 실행 후 해당 observable 만큼 대기
    /// ex) var waitOb = Observable.Timer (TimeSpan.FromSeconds (1f));
    ///     btn.AddOnClick (() => {/*clickAction*/}, ()=> waitOb);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="btn"></param>
    /// <param name="clickAction">Click Callback</param>
    /// <param name="clickObservableFunc">클릭시 발동, Func 내부 observable이 끝날때까지 대기</param>
    /// <param name="sharedCanExecute">해당 prop을 공유하는 모든 버튼은 공유되는 버튼이 하나라도 사용 불가하면 전부 사용불가</param>
    public static void AddOnClick<T> (this Button btn, Action clickAction, Func<IObservable<T>> clickObservableFunc,
        IReactiveProperty<bool> sharedCanExecute = null)
    {
        btn.AddOnClick (
            () => clickObservableFunc?.Invoke ().DoOnSubscribe (() => clickAction?.Invoke ()),
            null,
            sharedCanExecute);
    }

    /// <summary>
    /// 버튼 이벤트 등록. 버튼 클릭시 clickObservableFunc가 호출되고 Func 내부 observable이 종료될때까지 대기.
    /// 이후 회복 및 새로고침시 refreshAction가 호출됨
    /// ex) btn.AddOnClick (
    ///         () => Observable.ReturnUnit().Do (_ => {/*Action*/}).Delay (TimeSpan.FromSeconds (1f)),
    ///         () => {/*새로고침 Callback. interval 종료 후 및 공유 이벤트 발동*/});
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="btn"></param>
    /// <param name="clickObservableFunc">클릭시 발동, Func 내부 observable이 끝날때까지 대기</param>
    /// <param name="refreshAction">새로고침. clickObservableFunc 내부 Observalbe 클릭 처리가 '끝나고(Complete)' 호출되거나, sharedCanExecute이 호출하면 호출됨</param>
    /// <param name="sharedCanExecute">해당 prop을 공유하는 모든 버튼은 공유되는 버튼이 하나라도 사용 불가하면 전부 사용불가</param>
    public static void AddOnClick<T> (this Button btn,
        Func<IObservable<T>> clickObservableFunc,
        Action refreshAction,
        IReactiveProperty<bool> sharedCanExecute = null)
    {
        // UniRx.BindToOnClick를 참조한 ReactiveCommand
        sharedCanExecute ??= new BoolReactiveProperty (true);
        var command = sharedCanExecute.ToAsyncReactiveCommand ();
        var d1 = command.CanExecute.SubscribeToInteractable (btn);
        var d2 = btn.OnClickAsObservable ().SubscribeWithState (command, (x, c) => c.Execute (x));
        var d3 = command.Subscribe (_ => clickObservableFunc?.Invoke ().AsUnitObservable ());
        // add refresh action
        // ※ refreshAction Callback에 {btn.interactable = false;} 같은 코드를 넣을경우 false 처리됨.
        // clickObservableFunc처리 이후 조건에 따라 버튼이 활성화 되면 안되는 경우를 위한 Callback처리
        var d4 = command.CanExecute.Subscribe (_ => { refreshAction?.Invoke (); });

        StableCompositeDisposable.Create (d1, d2, d3, d4);
    }

    //============================================//
    //============================================//
    //============================================//

    // ※ 아래부터는 성능 및 최적화 되지 않은 코드들임, 확인 후 첨삭될 가능성 있음 ※

    // 해당 UI를 클릭했는지 체크
    //public static IObservable<(bool isHit, GameObject[] hitArr)> ClickCheckEventSystem<T> (this T comp, Camera debugCamera, IEnumerable<UIBehaviour> targetArr) where T : Component
    //{
    //    List<RaycastResult> raycastResults = new List<RaycastResult> ();
    //    return comp.UpdateAsObservable ()
    //        .TakeUntilDisable (comp)
    //        .Where (_ => Input.GetMouseButtonDown (0))
    //        .Select (_ =>
    //        {
    //            PointerEventData pointer = new PointerEventData (EventSystem.current);
    //            pointer.position = Input.mousePosition;
    //            EventSystem.current.RaycastAll (pointer, raycastResults);
    //            if (debugCamera)
    //            {
    //                Ray ray = debugCamera.ScreenPointToRay (Input.mousePosition);
    //                Debug.DrawLine (ray.origin, ray.origin + ray.direction * 1000f, Color.red, 3f);
    //            }
    //            var uiGOArr = targetArr.Select (ui => ui.gameObject);
    //            var hitGOArr = raycastResults.Select (hit => hit.gameObject);
    //            var interect = uiGOArr.Intersect (hitGOArr).ToArray ();
    //            return (interect.Any (), interect);
    //        });
    //}

    // 해당 컴포넌트를 클릭했는지 collider 체크
    //public static IObservable<(bool isHit, Component[] hitArr)> ClickCheckPhysics<T> (this T comp, Camera camera, bool drawDebug, IEnumerable<Component> targetArr) where T : Component
    //{
    //    List<RaycastResult> raycastResults = new List<RaycastResult> ();
    //    return comp.UpdateAsObservable ()
    //        .TakeUntilDisable (comp)
    //        .Where (_ => Input.GetMouseButtonDown (0))
    //        .Select (_ =>
    //        {
    //            Ray ray = camera.ScreenPointToRay (Input.mousePosition);
    //            RaycastHit[] hitArr = Physics.RaycastAll (ray.origin, ray.direction);
    //            RaycastHit2D[] hit2DArr = Physics2D.RaycastAll (ray.origin, ray.direction);

    //            var hit = targetArr.Intersect (hitArr.Select (h => h.transform));
    //            var hit2d = targetArr.Intersect (hit2DArr.Select (h => h.transform));
    //            var all = hit.Concat (hit2d);

    //            // draw debug line
    //            if (drawDebug)
    //            {
    //                Debug.DrawLine (ray.origin, ray.direction * 1000f, Color.red, 3f);
    //            }
    //            // hit check
    //            return (all.Any (), all.ToArray ());
    //            //return
    //            //    hitArr.Any (hit => hit.collider && hit.transform == target.transform) ||
    //            //    hit2DArr.Any (hit => hit.collider && hit.transform == target.transform);
    //        });
    //}

    //============================================//

    // 툴팁 창 바깥 클릭시 자동 닫기 등록 (닫을 툴팁, 클릭 무시할 GameObject)
    // ※ EventSystem에 영향을 받는 UIBehaviour 하위 GameObject만 사용해야함
    //public static void SubscribeTooltipAutoClose (this GameObject go, params Selectable[] ignoreArr)
    //{
    //    go.SetActive (true);
    //    go.UpdateAsObservable ()
    //        .TakeUntilDisable (go)
    //        .Where (_ => Input.GetMouseButtonDown (0))
    //        .Where (_ => EventSystem.current.currentSelectedGameObject != go)
    //        .Where (_ => ignoreArr.All (ignore => EventSystem.current.currentSelectedGameObject != ignore?.gameObject))
    //        .Subscribe (_ =>
    //        {
    //            go?.SetActive (false);
    //        })
    //        .AddTo (go);
    //}

    //============================================//
    // 이하 Animation Component 확장

    // 애니메이션 컴포넌트 index 실행
    public static IObservable<bool> PlayAsObservable (this Animation anim, int animIndex)
    {
        return Observable.Create<bool> (ob =>
        {
            // 실행
            var animState = anim.GetState (animIndex);
            if (animState)
            {
                animState.speed = 1f;
                anim.Play (animState.clip.name);
                return Observable.Timer (TimeSpan.FromSeconds (animState.clip.length))
                    .Subscribe (_ =>
                    {
                        ob.OnNext (true);
                        ob.OnCompleted ();
                    });
            }
            else
            {
                ob.OnNext (false);
                ob.OnCompleted ();
                return Disposable.Empty;
            }
        });
    }

    // 애니메이션 컴포넌트 첫번쨰 실행
    public static IObservable<bool> PlayFirstAsObservable (this Animation anim)
    {
        return anim.PlayAsObservable (0);
    }

    // AnimationState에서 첫 프레임으로 설정
    public static void SetFirstFrame (this AnimationState animState)
    {
        if (animState)
        {
            animState.time = 0f;
            animState.speed = 0f;
        }
    }
    // AnimationState에서 첫 프레임으로 설정
    public static void SetFirstFrame (this Animation anim, int animIndex)
    {
        anim.GetState (animIndex).SetFirstFrame ();
    }

    // Animation Compoent에서 AnimationState 가져오기
    public static AnimationState GetState (this Animation anim, int animIndex)
    {
        var animState = anim.Cast<AnimationState> ().ElementAtOrDefault (animIndex);
        return animState;
    }

    //============================================//
    // text 자동 줄바꿈이 layout 크기를 잡아먹는 경우가 있으므로
    // 강제 개행 기능으로 layout을 수동으로 잡음
    //public static void ForceUpdateNewline (this Text text, string str)
    //{
    //    string oriStr = str;
    //    string editStr = str;
    //    Canvas.ForceUpdateCanvases ();
    //    TextGenerator textGenerator = text.cachedTextGenerator;
    //    if (textGenerator.characterCount == 0)
    //    {
    //        textGenerator = text.cachedTextGeneratorForLayout;
    //    }
    //    var lines = textGenerator.lines;
    //    try
    //    {
    //        for (int i = 1; i < lines.Count; i++)
    //        {
    //            var charIndex = lines[i].startCharIdx - 1;
    //            // 줄바꿈에 마지막이 띄어쓰기일 경우 \n으로 변경
    //            if (editStr[charIndex] == ' ')
    //            {
    //                editStr = editStr.Remove (charIndex, 1).Insert (charIndex, "\n");
    //            }
    //        }
    //        text.text = editStr;
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError (e.Message);
    //        text.text = oriStr;
    //    }
    //}
}
