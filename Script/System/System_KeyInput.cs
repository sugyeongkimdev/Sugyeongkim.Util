using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigUtil
{
    // 키 입력에 대한 액션

    public class System_KeyInput : MonoBehaviour
    {
        // key group
        public class KeyGroup : Dictionary<KeyCode, Action>
        {
            public new Action this[KeyCode key]
            {
                get => base[key];
                set => base[key] = value;
            }

            public new void Add (KeyCode key, Action keyAction)
            {
                this[key] = keyAction;
            }
        }

        //==========================================================//

        // 키 그룹 타입
        public enum KeyGroupType { None, Scene1, Scene2 }

        //==========================================================//

        // 키 그룹 모음 dic
        public Dictionary<KeyGroupType, KeyGroup> keyGroupSetDic { get; private set; }

        // 현재 키 그룹
        public KeyGroupType currentKeyGroup { get; set; }

        //==========================================================//

        private void OnEnable ()
        {
            // OnEnable은 삭제 후 원하는 시간에 체크를 시작할 수 있음
            StartLoop ();
        }

        // key check loop 시작
        private void StartLoop ()
        {
            StartCoroutine (KeyCheckLoop ());
            IEnumerator KeyCheckLoop ()
            {
                while (true)
                {
                    if (Input.anyKeyDown)
                    {
                        // 키가 눌렸을 경우 해당 키 그룹에서 키가 눌렸는지 체크 후 키 액션 실행
                        foreach (var group in GetGroup (currentKeyGroup).Where (keyKV => Input.GetKeyDown (keyKV.Key)))
                        {
                            group.Value?.Invoke ();
                        }
                    }
                    yield return null;
                }
            }
        }

        //==========================================================//

        // 키 그룹 가져오기
        public KeyGroup GetGroup (KeyGroupType groupType)
        {
            if (keyGroupSetDic == null)
            {
                keyGroupSetDic = new Dictionary<KeyGroupType, KeyGroup> ();
            }
            keyGroupSetDic.TryGetValue (groupType, out KeyGroup gorup);
            return gorup ?? (keyGroupSetDic[groupType] = new KeyGroup ());
        }

        //==========================================================//

        // 해당 그룹의 해당 키에 키 이벤트 넣기
        public void AddKey (KeyGroupType groupType, KeyCode key, Action keyAction)
        {
            GetGroup (groupType).Add (key, keyAction);
        }
        // 해당 그룹에 전체 키 그룹 넣기
        public void AddGroup (KeyGroupType groupType, KeyGroup group)
        {
            keyGroupSetDic[groupType] = group;
        }

        public void RemoveKey (KeyGroupType groupType, KeyCode key)
        {
            GetGroup (groupType).Remove (key);
        }

        //==========================================================//

        // 이하 필요시 확장
        // public void RemoveGroup () { }
        // public void Clear () { }

    }
}