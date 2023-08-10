using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace BigUtil
{
    // UniRx를 이용한 유니티 애니메이션 이벤트 등록
    // 해당 컴포넌트를 부착한 뒤 애니메이션에서 OnAnimationEvent(string key)를 이용하여
    // 애니메이션 string 이벤트를 실행하면 Add를 통해 등록한 액션을 실행하는 코드
    // 즉 애니메이션에 subject를 반환해서 Subscribe 구독이 가능함

    public class UniRx_AnimationEventSubject : MonoBehaviour
    {
        // 사용 예시
        // thisComp.Add("ani_Key").Subscribe(_=>{  });

        private Dictionary<string, Subject<string>> _eventHandlerDic = new Dictionary<string, Subject<string>> ();

        //============================================================//

        public Subject<string> Add (string key, Action<string> eventAction)
        {
            Get (key).Subscribe (k => eventAction (k));
            return Get (key);
        }
        public Subject<string> Get (string key)
        {
            _eventHandlerDic.TryGetValue (key, out Subject<string> value);
            return value ?? (_eventHandlerDic[key] = new Subject<string> ());
        }

        public void Remove (string key)
        {
            _eventHandlerDic.Remove (key);
        }

        public void Clear ()
        {
            _eventHandlerDic.Clear ();
        }

        //============================================================//

        // 유니티 애니메이션에서 등록할 키값을 넣어서 호출해야함 
        [UsedImplicitly]
        public void OnAnimationEvent (string key)
        {
            _eventHandlerDic.TryGetValue (key, out Subject<string> animEvent);
            animEvent?.OnNext (key);
        }
    }
}