using UnityEngine;

namespace BigUtil
{
    // 이 스크립트가 달린 오브젝트는 타겟을 향해 설정된 애니메이션을 실행

    public class Tween_MoveToTarget : MonoBehaviour
    {
        //============================================================//

        [Header ("Animation Default")]
        public Transform target;
        [Range (0, 10)] public float moveTime = 1f;
        [Range (0, 10)] public float delayTime = 0f;
        [Range (-10, 10)] public float dirVal = 0f;
        public AnimationCurve mainCurve = AnimationCurve.Linear (0, 0, 1f, 1f);

        //============================================================//

        [HideInInspector] public Vector3 oriPos;
        public bool pause;

        //============================================================//

        private void Awake ()
        {
            oriPos = transform.position;
        }

        private void OnEnable ()
        {
            Play ();
        }

        private void OnDisable ()
        {
            Stop ();
        }

        private void FixedUpdate ()
        {
            if (!target)
            {
                enabled = false;
                return;
            }

            if (!pause)
            {
                Cacaulate ();
            }
        }


        //============================================================//

        // 재생
        public void Play ()
        {
            aniTime = 0;
            transform.position = oriPos;
            enabled = true;
        }

        // 정지
        public void Stop ()
        {
            enabled = false;
        }

        //============================================================//

        protected float aniTime;
        public virtual void Cacaulate ()
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

                // 방향 계산
                Vector3 tPos = target.position;
                Vector3 dPos = tPos - oriPos;
                Vector3 offPos = new Vector3 (-dPos.y, dPos.x, 0) / Mathf.Sqrt (Mathf.Pow (dPos.x, 2) + Mathf.Pow (dPos.y, 2)) * dirVal;

                // 베지어 커브 계산
                Vector3 bPos = Vector3.Lerp (tPos - offPos, oriPos - offPos, 0.5f);
                Vector3 bPos1 = Vector3.Lerp (oriPos, bPos, cVale);
                Vector3 bPos2 = Vector3.Lerp (bPos, tPos, cVale);

                transform.position = Vector3.Lerp (bPos1, bPos2, cVale);
            }
            else
            {
                transform.position = target.position;
                Stop ();
            }
        }
    }
}