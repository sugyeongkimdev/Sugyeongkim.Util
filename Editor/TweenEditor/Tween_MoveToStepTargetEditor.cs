using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BigUtil
{
    [CustomEditor (typeof (Tween_MoveToStepTarget))]
    public class Tween_MoveToStepTargetEditor : Editor
    {
        public void OnSceneGUI ()
        {
            var anim = target as Tween_MoveToStepTarget;
            if (!anim || !anim.target)
            {
                return;
            }
            if (IsEditable ())
            {
                Undo.RecordObject (anim, nameof (Tween_MoveToStepTarget));
            }

            // key값의 길이가 안맞을 경우 초기화
            // 수정될 경우 기존 키값을 가져와서 평균값을 덮어 씌우는 방법도 생각할 수 있지만 생략함
            int stepLeng = anim.curveStep + 1;
            if (anim.xCurve.keys.Length != stepLeng || anim.yCurve.keys.Length != stepLeng || anim.zCurve.keys.Length != stepLeng)
            {
                var keyArr = Enumerable.Range (0, stepLeng).Select (i => new Keyframe (i / (float)(anim.curveStep), 0));
                anim.xCurve.keys = keyArr.ToArray ();
                anim.yCurve.keys = keyArr.ToArray ();
                anim.zCurve.keys = keyArr.ToArray ();
            }
            var xKeyArr = anim.xCurve.keys;
            var yKeyArr = anim.yCurve.keys;
            var zKeyArr = anim.zCurve.keys;

            Vector3 startPos = IsEditable () ? anim.transform.position : anim.oriPos;
            Vector3 targetPos = anim.target.position;

            // 처음과 마지막 그리기
            Vector3 fistPos = WorldPos (0);
            Vector3 lastPos = WorldPos (anim.curveStep - 1);
            DrawLine (startPos, fistPos, Color.magenta);
            DrawLine (lastPos, targetPos, Color.magenta);

            // step 변경 핸들 그리기
            for (int i = 0; i < anim.curveStep; i++)
            {
                var aWPos = WorldPos (i);
                // 에디터 모드에서만 가능
                if (IsEditable ())
                {
                    var dragPos = Handles.PositionHandle (aWPos, Quaternion.identity);
                    var offPos = dragPos - aWPos;
                    xKeyArr[i].value += offPos.x;
                    yKeyArr[i].value += offPos.y;
                    zKeyArr[i].value += offPos.z;
                }
            }

            // 데이터 수정
            anim.xCurve.keys = xKeyArr;
            anim.yCurve.keys = yKeyArr;
            anim.zCurve.keys = zKeyArr;

            // Gizmo 대용 선 그리기
            float frame = 0.002f;
            for (float t = 0f; t <= anim.mainCurve.keys.Last ().value; t += frame)
            {
                float cVal = anim.mainCurve.Evaluate (t);
                float nVal = anim.mainCurve.Evaluate (t + frame);
                var cLerp = Vector3.Lerp (startPos, targetPos, cVal);
                var nLerp = Vector3.Lerp (startPos, targetPos, nVal);
                var prev = new Vector3 (anim.xCurve.Evaluate (cVal), anim.yCurve.Evaluate (cVal), anim.zCurve.Evaluate (cVal));
                var next = new Vector3 (anim.xCurve.Evaluate (nVal), anim.yCurve.Evaluate (nVal), anim.zCurve.Evaluate (nVal));
                DrawLine (cLerp + prev, nLerp + next);
            }

            //==========================================================/

            // 월드 위치 얻기
            Vector3 WorldPos (int step)
            {
                step = Mathf.Clamp (step, 0, anim.curveStep - 1);
                var stepNormal = 1 / (float)(anim.curveStep) * (step + 0.5f);
                var stepPos = Vector3.Lerp (startPos, targetPos, stepNormal);
                var curvePos = CurvePos (step);
                return stepPos + curvePos;
            }
            // 커브 위치 얻기
            Vector3 CurvePos (int step)
            {
                step = Mathf.Clamp (step, 0, anim.curveStep - 1);
                var keyPos = new Vector3 (xKeyArr[step].value, yKeyArr[step].value, zKeyArr[step].value);
                return keyPos;
            }
            // 선 그리기
            void DrawLine (Vector3 posA, Vector3 posB, Color? color = null)
            {
                Color tempColor = Handles.color;
                Handles.color = color ?? Color.green;
                Handles.DrawLine (posA, posB, 2.5f);
                Handles.color = tempColor;
            }
            // 편집가능한지 체크
            bool IsEditable () => !Application.isPlaying && anim.enabled;
        }
    }
}