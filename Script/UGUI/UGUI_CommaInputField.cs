using System.Numerics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BigUtil
{
    // UGUI의 InputField에 ,를 붙이는 최소한의 코드
    // 사용시 자체적으로 커스텀 바람

    public class UGUI_CommaInputField : MonoBehaviour
    {
        private InputField field;

        private void Awake ()
        {
            field = GetComponent<InputField> ();

            // 검증 및 ',' 붙이기
            field.onEndEdit.AddListener (s =>
            {
                if (!string.IsNullOrWhiteSpace (s))
                {
                    field.text = $"{BigInteger.Parse (s.Replace (",", "")):#,###}";
                }
            });

            // 정규식을 이용한 입력 검증
            // 필요시 확장
            field.onValidateInput = (t, i, c) =>
            {
                if (field.text.Replace (",", "").Length <= 15)
                {
                    string pattern = field.isFocused ? @"[^0-9]" : @"[^0-9,]";
                    return Regex.IsMatch ($"{c}", pattern) ? '\0' : c;
                }
                else
                {
                    return '\0';
                }
            };
        }

        private void FixedUpdate ()
        {
            if (!field)
            {
                Debug.LogError ("Component not found.");
                enabled = false;
                return;
            }
            if (field.isFocused)
            {
                field.text = field.text.Replace (",", "");
            }
        }
    }
}