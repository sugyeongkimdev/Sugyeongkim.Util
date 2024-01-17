using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SugyeongKim.Util
{
    public class UICanvasManager : GlobalSingleton<UICanvasManager>
    {
        public Canvas canvas;
        public CanvasScaler scaler;
        public GraphicRaycaster raycaster;

        [field: SerializeField] public GameObject popupLayer { get; private set; }

        //============================================//

        public override IObservable<Unit> InitAsObservable ()
        {
            return base.InitAsObservable ();
        }

        // canvas 해상도 설정
        public static void SetCanvasScaler (Canvas targetCanvas)
        {
            var targetScaler = targetCanvas.GetComponent<CanvasScaler> ();
            targetScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            targetScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            targetScaler.referenceResolution = new Vector2 (1080, 1920);
            targetScaler.matchWidthOrHeight = Screen.width > Screen.height ? 0 : 1;
        }
    }
}