using UnityEngine;

namespace BigUtil
{
    // Editor/VFX_MoveToStepTargetEditor.cs 필요
    // 각 스탭을 지정해서 애니메이션 기능을 실행하는 컴포넌트
    // 의도한만큼 기능이 안나와서 생각보다 구림

    public class Tween_MoveToStepTarget : Tween_MoveToTarget
    {
        [Header ("Animation Step")]
        public AnimationCurve xCurve = AnimationCurve.Linear (0, 0, 1f, 1f);
        public AnimationCurve yCurve = AnimationCurve.Linear (0, 0, 1f, 1f);
        public AnimationCurve zCurve = AnimationCurve.Linear (0, 0, 1f, 1f);
        [Range (1, 32)] public int curveStep = 1;

        public override void Cacaulate ()
        {
            aniTime += Time.deltaTime;
            if (aniTime < moveTime + delayTime)
            {
                if (aniTime < delayTime)
                {
                    // 대기
                    return;
                }
                // 곡선 계산
                float cVale = mainCurve.Evaluate ((aniTime - delayTime) / moveTime);
                float xVale = xCurve.Evaluate (cVale);
                float yVale = yCurve.Evaluate (cVale);
                float zVale = zCurve.Evaluate (cVale);
                Vector3 mPos = Vector3.Lerp (oriPos, target.position, cVale);
                Vector3 cPos = new Vector3 (xVale, yVale, zVale);

                transform.position = mPos + cPos;
            }
            else
            {
                transform.position = target.position;
                Stop ();
            }
        }

    }
}