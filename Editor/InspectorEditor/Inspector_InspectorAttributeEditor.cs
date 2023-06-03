using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BigUtil
{
    // 들여쓰기 어트리뷰트 에디터
    [CustomPropertyDrawer (typeof (IndentAttribute))]
    public class IndentAttributeEditor : PropertyDrawer
    {
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as IndentAttribute;

            EditorGUI.BeginProperty (position, label, property);
            int tempLevel = EditorGUI.indentLevel;

            EditorGUI.indentLevel = att.level;
            EditorGUI.LabelField (position, label);
            if (att.isWithValue)
            {
                position.position = position.position + new Vector2 (att.level * EditorGUIUtility.singleLineHeight, 0);
            }
            EditorGUI.PropertyField (position, property);
            EditorGUI.indentLevel = tempLevel;
            EditorGUI.EndProperty ();
        }
    }

    // 리치 텍스트 헤더 에디터
    [CustomPropertyDrawer (typeof (FieldNameAttribute))]
    public class HeaderRichAttributeEditor : PropertyDrawer
    {
        // rich text 폰트 크기 찾기
        private int sizeCache = -1;
        private int FontSize ()
        {
            if (sizeCache == -1)
            {
                var att = attribute as FieldNameAttribute;
                var noEmptyStr = Regex.Replace (att.label, @"\s+", "");
                var findIndex = noEmptyStr.IndexOf ("<size=");

                string resultStr = string.Empty;
                if (findIndex != -1)
                {
                    char[] charArr = noEmptyStr.ToArray ();
                    for (int i = findIndex; i < charArr.Length - (findIndex + 1); i++)
                    {
                        if (int.TryParse (charArr[i].ToString (), out int findSizeChar))
                        {
                            resultStr += findSizeChar;
                        }
                    }
                }
                sizeCache = int.TryParse (resultStr, out int size) ? size : 10;
            }

            return sizeCache;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as FieldNameAttribute;
            var fieldRect = new Rect (position);

            // 스타일 재정의
            GUIStyle newStyle = new GUIStyle (EditorStyles.label);
            newStyle.richText = true;

            // offset
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.y += FontSize () / 1.2f;
            position.y -= FontSize () / 10f;

            EditorGUI.BeginProperty (position, label, property);
            EditorGUI.LabelField (position, att.label, newStyle);
            EditorGUI.PropertyField (fieldRect, property);
            EditorGUI.EndProperty ();
        }
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + FontSize () / 1.2f;
        }
    }

    // 헤더 빠른 설정 에디터
    [CustomPropertyDrawer (typeof (HeaderCustomAttribute))]
    public class HeaderCustomAttributeEditor : PropertyDrawer
    {
        // 빠른 셋팅
        private (int fontSize, Color fontColor) Setting (HeaderType type)
        {
            switch (type)
            {
                case HeaderType.Title: return (24, Color.red);
                case HeaderType.Sub: return (18, Color.green);
                case HeaderType.List:
                default: return (14, Color.white);
            }
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            var att = attribute as HeaderCustomAttribute;
            var set = Setting (att.type);
            var fieldRect = new Rect (position);

            // offset
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            fieldRect.y += GetPropertyHeight (property, label) - EditorGUIUtility.singleLineHeight;
            position.y -= set.fontSize / 4f;

            // 스타일 재정의
            GUIStyle newStyle = new GUIStyle (EditorStyles.boldLabel);
            newStyle.fontSize = set.fontSize;
            newStyle.normal.textColor = set.fontColor;

            EditorGUI.BeginProperty (position, label, property);
            EditorGUI.LabelField (fieldRect, label.text);
            EditorGUI.PropertyField (fieldRect, property);
            EditorGUI.EndProperty ();

            EditorGUI.BeginProperty (position, label, property);
            EditorGUI.LabelField (position, att.label, newStyle);
            EditorGUI.PropertyField (fieldRect, property);
            EditorGUI.EndProperty ();
        }
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            var att = attribute as HeaderCustomAttribute;
            var set = Setting (att.type);
            return base.GetPropertyHeight (property, label) + set.fontSize + 10;
        }
    }
}