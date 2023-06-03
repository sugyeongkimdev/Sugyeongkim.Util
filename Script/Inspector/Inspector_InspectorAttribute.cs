using UnityEngine;

namespace BigUtil
{
    // Editor/InspectorAttributeEditor.cs 필요함

    // 들여쓰기 어트리뷰트 (※ 배열에 사용하면 의도대로 안됨)
    // 생각나서 만들었는데 쓸데가 1도 없는거같다
    public class IndentAttribute : PropertyAttribute
    {
        // 들여쓰기 레벨
        public int level;
        // 필드도 같이 움직일지 결정
        public bool isWithValue;
        public IndentAttribute (int level = 0, bool isWithValue = false)
        {
            this.level = level;
            this.isWithValue = isWithValue;
        }
    }

    //==========================================================//

    // 이름 재정의, 리치텍스트 지원 (※ 배열에 사용하면 의도대로 안됨)
    public class FieldNameAttribute : PropertyAttribute
    {
        public string label;
        public FieldNameAttribute (string label)
        {
            this.label = label;
        }
    }

    //==========================================================//
    // 헤더 빠른 설정 (※ 배열에 사용하면 의도대로 안됨)
    public enum HeaderType { Title, Sub, List }
    public class HeaderCustomAttribute : PropertyAttribute
    {
        public string label;
        public HeaderType type;
        public HeaderCustomAttribute (string label, HeaderType type = HeaderType.Sub)
        {
            this.label = label;
            this.type = type;
        }
    }
}