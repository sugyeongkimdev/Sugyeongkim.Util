using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace BigUtil
{
    // 간단한 싱글톤, 찾기만함
    public abstract class SimpleSingleton<T> : MonoBehaviour where T : Component
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
            if (!IsValid ())
            {
                _instance = FindObjectOfType<T> ();
            }
            return _instance;
        }

        private static bool IsValid ()
        {
            return _instance && !ReferenceEquals (_instance, null);
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
    public abstract class GlobalSingleton<T> : SimpleSingleton<T>, SingletonUtil.IInit where T : Component
    {
        public new static T instance
        {
            get
            {
                if (!FindCachedInstance ())
                {
                    var singleton = new GameObject ($"{typeof (T)}");
                    singleton.transform.SetParent (SingletonUtil.ParentTrnas);
                    _instance = singleton.AddComponent<T> ();
                }

                return FindCachedInstance ();
            }
        }

        //==========================================================//

        // 전역 싱글톤 초기화 virtual 지원
        public virtual void Init () { }
        public virtual async Task InitAsync ()
        {
            // 게임 시작시 한번만 호출하면서 완료까지 대기하므로
            // 상식적으로 무식하게 큰 작업을 걸거나 Delay 걸면 안됨
            await Task.CompletedTask;
        }
    }

    //==========================================================//
    //==========================================================//

    // 싱글톤 유틸 클래스
    public static class SingletonUtil
    {
        //==========================================================//

        // 싱글톤 초기화 지원 인터페이스
        public interface IInit
        {
            void Init ();
            Task InitAsync ();
        }

        //==========================================================//

        // 리플렉션에서 사용되는 이름들
        private const string InstanceName = "instance";

        // 싱글톤의 부모 트랜스폼
        private static GameObject _parentTrnas;
        public static Transform ParentTrnas
        {
            get
            {
                if (!_parentTrnas)
                {
                    _parentTrnas = new GameObject ($"__Global_Singleton__");
                    MonoBehaviour.DontDestroyOnLoad (_parentTrnas);
                }

                return _parentTrnas.transform;
            }
        }

        //==========================================================//

        // 초기화 진행
        private static bool isInit = false;         // 초기화 여부
        private static bool isRunning = false;      // 초기화 진행 여부
        public static void SingletonInit (Action initDoneCallback)
        {
            if (!isInit)
            {
                isInit = true;
                isRunning = true;

                // 한번만 초기화 진행
                SingletonInitAsync (initDoneCallback);
            }
            else
            {
                // 이미 초기화를 했으면 바로 callback처리하고 진행중이면 무시
                if (!isRunning)
                {
                    initDoneCallback?.Invoke ();
                }
            }
        }

        // 비동기 초기화 진행
        private static async void SingletonInitAsync (Action initDoneCallback)
        {
            //// 초기화 및 비동기 초기화 대기
            await Task.WhenAll (GetGlobalSingletonInstnaceArr ()
                    // 초기화를 위한 IInit 캐스팅
                    .Cast<IInit> ()
                    .Select (init =>
                    {
                        init.Init ();
                        return init.InitAsync ();
                    }));
            // 초기화 종료 콜백
            isRunning = false;
            initDoneCallback?.Invoke ();
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
                    var instance = currentSingleton.GetProperty (InstanceName).GetValue (null);
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