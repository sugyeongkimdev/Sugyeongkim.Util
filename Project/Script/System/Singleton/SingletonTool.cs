using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    // 싱글톤 유틸 클래스
    public static class SingletonTool
    {
        private const string INSTANCE_NAME = "instance";
        private const string ROOT_NAME = "__GlobalSingleton__";

        private static Type GlobalType = typeof (GlobalSingleton<>);
        //private static Type LocalType = typeof (LocalSingleton<>);

        //==========================================================//

        // 싱글톤의 부모 트랜스폼
        private static GameObject _rootTrnas;
        public static Transform RootTrnas
        {
            get
            {
                if (_rootTrnas == false)
                {
                    var parent = GameObject.FindObjectOfType<GlobalSingletonParent> ();
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
        public static IObservable<Unit> InitGlobalSingletonAsObservable ()
        {
            if (isInit == false)
            {
                isInit = true;

                // 순서대로 비동기 초기화
                // 낮은 InitOrder부터 초기화, 같은 순서가 여러개면 같이 실행되지만 같은 순서끼리는 순서가 보장되지 않음
                // InitOrder가 끝나면 다음 InitOrder가 실행됨
                var initConcat = GetGlobalSingletonInstnaceArr ()
                    .Cast<IGlobalSingletonInit> ()
                    .GroupBy (target => target.InitOrder)
                    .OrderBy (group => group.Key)
                    .Select (group =>
                        Observable.Defer (() =>
                            Observable.WhenAll (group.Select (singleton =>
                                Observable.Defer (() => singleton.InitAsObservable ())
                                    //.Do (_ => { UtilLog.Log (singleton); })

                                    //.Catch ((Exception e) =>
                                    //{
                                    //    // InitAsObservable() 어느 한 싱글톤에 에러 발생, 로그를 띄우고 나머지 마저 실행
                                    //    UtilLog.Error (singleton);
                                    //    UtilLog.Error (e);
                                    //    return Observable.ReturnUnit ();
                                    //})
                                    ))))

                    .Concat ();
                return Observable.WhenAll (initConcat)
                    .TakeUntilDestroy (RootTrnas);
            }

            return Observable.ReturnUnit ();
        }

        //==========================================================//

        // 글로벌 싱글톤을 상속받는 모든 클래스의 instnace 반환
        private static IEnumerable<object> GetGlobalSingletonInstnaceArr ()
        {
            var searchAllSingleton = SearchInheritanceClassType ();
            return searchAllSingleton
                // GlobalSingletonIgnoreAttribute 어트리뷰트를 받은 global sington은 초기화, 생성을 모두 무시함
                .Where (currentType =>
                {
                    var ignoreAtt = currentType.GetCustomAttribute<GlobalSingletonIgnoreAttribute> ();
                    return ignoreAtt == null || ignoreAtt.isIgnore == false;
                })
                .Select (currentType =>
                {
                    // global 싱글톤 제네릭 타입 생성
                    var currentSingleton = GlobalType.MakeGenericType (currentType);
                    // global 싱글톤 인스턴스 접근 및 생성, 접근시 property에 의해 자동생성됨
                    var instance = currentSingleton.GetProperty (INSTANCE_NAME).GetValue (null);
                    return instance;
                });

        }
        // 리플렉션을 사용한 찾을 타입을 상속받는 모든 타입 찾기
        // ignoreAbstract       추상 무시
        // ignoreSearchType     기준이 되는 타입 무시
        private static IEnumerable<Type> SearchInheritanceClassType (bool ignoreAbstract = true, bool ignoreSearchType = true)
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
                });

            return findTypeArr;
        }
    }
}