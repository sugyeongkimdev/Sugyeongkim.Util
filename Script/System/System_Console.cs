using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// 호출자 정보 추적 특성을 이용한 로그 시스템
// 필요시 알아서 확장
// ex) Console.Log("Log") or Console.Error("Error")

namespace BigUtil
{
    public static class Console
    {
        //==========================================================//

        // Conditional ("UNITY_EDITOR")]
        public static void Log (object logStr = default,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            string logHeader = $"[{GetPathLastString (sourceFilePath).ToColor (Color.green)}]";
            Debug.Log ($"{logHeader} : {logStr}");
        }

        // [Conditional ("UNITY_EDITOR")]
        public static void Error (object logStr = default,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            string logHeader = $"[{GetPathLastString (sourceFilePath).ToColor (Color.red)}]";
            Debug.LogError ($"{logHeader} : {logStr}");
        }

        //==========================================================//

        // 파일 경로 잘라서 마지막 가져오기
        private static string GetPathLastString (string path)
        {
            return path.Split ('\\').Last ().Split ('.').First ();
        }

        // string rich text, 색상 서식 칠하기
        private static string ToColor (this string str, Color color = new Color ())
        {
            string hexColorStr = color.a == 0 ? "red" : $"#{ColorUtility.ToHtmlStringRGBA (color)}";
            return $"<color={hexColorStr}>{str}</color>";
        }

        //==========================================================//

    }
}