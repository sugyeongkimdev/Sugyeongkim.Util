using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public static class UtilUniRx
{
    // 합치기 (※ 연속으로 호출해도 프레임당 첫호출 한번만 호출됨)
    // Merge + ThrottleFirst 1 Frame
    public static IObservable<T> MergeFirstFrame<T> (params IObservable<T>[] observables)
    {
        return Observable.Merge (observables)
            .ThrottleFirstFrame (1)
            .First ();
    }
    public static IObservable<Unit> MergeFirstFrame<T, U> (IObservable<T> ob1, IObservable<U> ob2)
    {
        return MergeFirstFrame (new[] { ob1.AsUnitObservable (), ob2.AsUnitObservable () });
    }
    public static IObservable<Unit> MergeFirstFrame<T, U, V> (IObservable<T> ob1, IObservable<U> ob2, IObservable<V> ob3)
    {
        return MergeFirstFrame (new[] { ob1.AsUnitObservable (), ob2.AsUnitObservable (), ob3.AsUnitObservable () });
    }

    // 합치기 (※ 연속으로 호출해도 프레임 마지막에 한번만 호출함)
    // Merge + BatchFrame
    public static IObservable<T> MergeLastFrame<T> (params IObservable<T>[] observables)
    {
        return Observable.Merge (observables)
            .BatchFrame ()
            .Select (list => list.First ());
    }

    //==========================================================//

    // Observable.Concat 작동이 이상해서 만듬
    public static IObservable<Unit> FixConcat<T> (this IEnumerable<IObservable<T>> observables)
    {
        return FixConcat (observables.ToArray ());
    }
    public static IObservable<Unit> FixConcat<T> (params IObservable<T>[] observableArr)
    {
        return IndexConcat (0);

        IObservable<Unit> IndexConcat (int curIndex)
        {
            if (curIndex >= observableArr.Length)
            {
                return Observable.ReturnUnit ();
            }
            return Observable.Create<Unit> (ob =>
            {
                return observableArr[curIndex]
                    .SelectMany (_ => Observable.Defer (() => IndexConcat (curIndex + 1)))
                    .Subscribe (v =>
                    {
                        ob.OnNext (v);
                        ob.OnCompleted ();
                    });
            });
        }
    }
}
