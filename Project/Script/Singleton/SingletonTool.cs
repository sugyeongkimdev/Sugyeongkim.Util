using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 싱글톤 유틸 클래스
    public static class SingletonTool
    {
        private const string INSTANCE_PROP_NAME = "instance";
        private const string ROOT_NAME = "__GlobalSingleton__";

        private static Type GlobalType = typeof (GlobalSingleton<>);
        //private static Type LocalType = typeof (LocalSingleton<>);

        //==========================================================//

        // 싱글톤 초기화 인터페이스
        public interface ISingletonInit
        {
            int InitOrder => 0;
            void Init ();
            IObservable<Unit> InitAsObservable ();
        }

        //==========================================================//

        // 싱글톤의 부모 트랜스폼
        private static GameObject _rootTrnas;
        public static Transform RootTrnas
        {
            get
            {
                if (_rootTrnas == false)
                {
                    var parent = GameObject.FindObjectOfType<SingletonParent> ();
                    if (parent)
                    {
                        _rootTrnas = parent.gameObject;
                    }
                    else
                    {
                        _rootTrnas = new GameObject (ROOT_NAME);
                    }
                    MonoBehaviour.DontDestroyOnLoad (_rootTrnas);
                }
                return _rootTrnas.transform;
            }
        }

        //==========================================================//

        // 초기화 진행
        private static bool isInit = false;

        // 초기화 진행 (unirx)
        public static IObservable<Unit> InitSingletonAsObservable ()
        {
            if (isInit == false)
            {
                isInit = true;

                // 초기화 및 비동기 초기화 대기
                var initConcat = GetGlobalSingletonInstnaceArr ()
                    .Cast<ISingletonInit> ()
                    .OrderBy (target => target.InitOrder)
                    .Select (target => target.InitAsObservable ().DoOnSubscribe (() => target.Init ()))
                    .Concat ();

                return Observable.WhenAll (initConcat)
                    .TakeUntilDestroy (RootTrnas);
            }

            return Observable.ReturnUnit ();
        }

        //==========================================================//

        // 글로벌 싱글톤을 상속받는 모든 클래스의 instnace 반환
        private static object[] GetGlobalSingletonInstnaceArr ()
        {
            var searchAllSingleton = SearchInheritanceClassType ();
            return searchAllSingleton
                .Select (currentType =>
                {
                    // 싱글톤 제네릭 타입 생성
                    var currentSingleton = GlobalType.MakeGenericType (currentType);
                    // 싱글톤 인스턴스 접근 및 생성
                    var instance = currentSingleton.GetProperty (INSTANCE_PROP_NAME).GetValue (null);
                    return instance;
                }).ToArray ();

        }

        // 리플렉션을 사용한 찾을 타입을 상속받는 모든 타입 찾기
        // ignoreAbstract       추상 무시
        // ignoreSearchType     기준이 되는 타입 무시
        private static Type[] SearchInheritanceClassType (bool ignoreAbstract = true, bool ignoreSearchType = true)
        {
            var findTypeArr = AppDomain.CurrentDomain.GetAssemblies ()
                .SelectMany (assembly => assembly.GetTypes ())
                .Where (t =>
                {
                    // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
                    Type type = t;
                    while (type != null && type != typeof (object))
                    {
                        var cur = type.IsGenericType ? type.GetGenericTypeDefinition () : type;
                        if (GlobalType == cur)
                        {
                            bool match = true;
                            // 추상 클래스 무시 여부
                            match = match && ignoreAbstract ? !t.IsAbstract : match;
                            // 찾는 자기 자신은 무시
                            match = match && ignoreSearchType ? t != GlobalType : match;
                            return match;
                        }
                        type = type.BaseType;
                    }
                    return false;
                })
                .ToArray ();

            return findTypeArr;
        }
    }
}