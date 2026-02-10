using System;
using UniRx;

namespace SugyeongKim.Util
{
    // 싱글톤 초기화 인터페이스
    public interface IGlobalSingletonInit
    {
        // 초기화 순서
        int InitOrder { get; }

        // 비동기 초기화
        IObservable<Unit> InitAsObservable ();
    }
}