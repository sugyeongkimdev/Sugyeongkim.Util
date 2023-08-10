using System;
using System.Linq;
using System.Reflection;
using UniRx;
using UnityEngine;

namespace BigUtil
{
    // 간단한 싱글톤, 찾기만함
    public abstract class LocalSingleton<T> : MonoBehaviour, SingletonUtil.ISingletonInit where T : Component
    {
        protected static T _instance;
        public static T instance
        {
            get
            {
                return FindCachedInstance ();
            }
        }

        protected static T FindCachedInstance ()
        {
            if (IsValid () == false)
            {
                _instance = FindObjectOfType<T> ();
            }
            return _instance;
        }

        private static bool IsValid ()
        {
            return _instance && ReferenceEquals (_instance, null) == false;
        }


        //==========================================================//

        // 싱글톤 초기화
        public virtual int InitOrder => 0;
        public virtual void Init () { }
        public virtual IObservable<Unit> InitAsObservable ()
        {
            return Observable.Empty<Unit> ();
        }

        //==========================================================//

        public virtual void Awake ()
        {
            _instance = IsValid () ? _instance : transform as T;
        }

        public virtual void OnDestroy ()
        {
            _instance = null;
        }
    }

    //==========================================================//

    // 게임 전역에서 사용되는 싱글톤, 찾고 없으면 만들어서 제공
    public abstract class GlobalSingleton<T> : LocalSingleton<T> where T : Component
    {
        public new static T instance
        {
            get
            {
                if (!FindCachedInstance ())
                {
                    var singleton = new GameObject ($"{typeof (T)}");
                    singleton.transform.SetParent (SingletonUtil.RootTrnas);
                    _instance = singleton.AddComponent<T> ();
                }

                return FindCachedInstance ();
            }
        }
    }

    //==========================================================//
    //==========================================================//

    // 싱글톤 유틸 클래스
    static class SingletonUtil
    {
        private const string INSTANCE_NAME = "instance";
        private const string ROOT_NAME = "Global_Singleton";

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
                    _rootTrnas = new GameObject (ROOT_NAME);
                    MonoBehaviour.DontDestroyOnLoad (RootTrnas);
                }
                return _rootTrnas.transform;
            }
        }

        //==========================================================//

        // 초기화 진행
        private static bool isInit = false;
        public static void InitSingleton (Action initCallback = null)
        {
            if (isInit == false)
            {
                isInit = true;
                InitSingletonAsObservable ()
                    .Subscribe (_ => initCallback?.Invoke ())
                    .AddTo(RootTrnas);
            }
        }

        // unirx 초기화 진행
        public static IObservable<Unit> InitSingletonAsObservable ()
        {
            if (isInit == false)
            {
                isInit = true;

                // 초기화 및 비동기 초기화 대기
                var a = GetGlobalSingletonInstnaceArr ()
                    .Cast<ISingletonInit> ()
                    .OrderBy (target => target.InitOrder)
                    .Select (target => target.InitAsObservable ().DoOnSubscribe (() => target.Init ()))
                    .Concat();

                return Observable.WhenAll (a);
            }

            return Observable.Empty<Unit> ();
        }

        //==========================================================//

        // 글로벌 싱글톤을 상속받는 모든 클래스의 instnace 반환
        private static object[] GetGlobalSingletonInstnaceArr ()
        {
            var singletonType = typeof (GlobalSingleton<>);
            var searchAllSingleton = SearchInheritanceClassType (singletonType);
            return searchAllSingleton
                .Select (currentType =>
                {
                    // 싱글톤 제네릭 타입 생성
                    var currentSingleton = singletonType.MakeGenericType (currentType);
                    // 싱글톤 인스턴스 접근 및 생성
                    var instance = currentSingleton.GetProperty (INSTANCE_NAME).GetValue (null);
                    return instance;
                }).ToArray ();

        }

        // 리플렉션을 사용한 찾을 타입을 상속받는 모든 타입 찾기
        // ignoreAbstract       추상 무시
        // ignoreSearchType     기준이 되는 타입 무시
        private static Type[] SearchInheritanceClassType (Type searchType, bool ignoreAbstract = true, bool ignoreSearchType = true)
        {
            var findType = Assembly.GetAssembly (searchType)
                .GetTypes ()
                .Where (t =>
                {
                    // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
                    Type type = t;
                    while (type != null && type != typeof (object))
                    {
                        var cur = type.IsGenericType ? type.GetGenericTypeDefinition () : type;
                        if (searchType == cur)
                        {
                            bool match = true;
                            // 추상 클래스 무시 여부
                            match = match && ignoreAbstract ? !t.IsAbstract : match;
                            // 찾는 자기 자신은 무시
                            match = match && ignoreSearchType ? t != searchType : match;
                            return match;
                        }
                        type = type.BaseType;
                    }
                    return false;
                });

            return findType.ToArray ();
        }
    }
}