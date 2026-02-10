
#if USE_SPINE

using System;
using UniRx;
using static Spine.AnimationState;
using Spine;

// spine events
// https://ko.esotericsoftware.com/spine-unity#Processing-AnimationState-Events
// unirx events sample
// https://github.com/neuecc/UniRx/blob/master/Assets/Plugins/UniRx/Examples/Sample09_EventHandling.cs

// 스파인과 UniRx 결합 (Spine + UniRx) 
static public partial class SpineUniRxExtensions
{
    // 애니메이션 시작시 호출
    static public IObservable<TrackEntry> OnStartAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryDelegate, TrackEntry> (
                // x => t => x.Invoke (t),
                x => x.Invoke,
                x => state.Start += x,
                x => state.Start -= x);
    }
    // 트랙을 지우거나 새 애니메이션을 설정하는 등의 이유로 애니메이션이 중단된 경우 호출
    static public IObservable<TrackEntry> OnInterruptAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryDelegate, TrackEntry> (
                x => x.Invoke,
                x => state.Interrupt += x,
                x => state.Interrupt -= x);
    }
    // 애니메이션 완료시 호출 (Interrupt 호출시 호출되지 않음)
    static public IObservable<TrackEntry> OnCompleteAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryDelegate, TrackEntry> (
                x => x.Invoke,
                x => state.Complete += x,
                x => state.Complete -= x);
    }
    // 애니메이션 종료시 호출 (Interrupt 호출시 호출됨)
    static public IObservable<TrackEntry> OnEndAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryDelegate, TrackEntry> (
                x => x.Invoke,
                x => state.End += x,
                x => state.End -= x);
    }
    // 애니메이션이나 TackEntry가 해제된 경우
    static public IObservable<TrackEntry> OnDisposeAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryDelegate, TrackEntry> (
                x => x.Invoke,
                x => state.Dispose += x,
                x => state.Dispose -= x);
    }
    // 사용자 정의 이벤트 발생시 호출
    static public IObservable<(TrackEntry t, Event e)> OnEventAsObservable (this AnimationState state)
    {
        return Observable.FromEvent<TrackEntryEventDelegate, (TrackEntry, Event)> (
                x => (t, e) => x.Invoke ((t, e)),
                x => state.Event += x,
                x => state.Event -= x);
    }
}
#endif