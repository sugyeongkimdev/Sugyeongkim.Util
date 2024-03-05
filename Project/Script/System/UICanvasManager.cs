using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SugyeongKim.Util
{
    public class UICanvasManager : GlobalSingleton<UICanvasManager>
    {
        [SerializeField] private Camera uiCamera;
        public static Camera UICamera => instance.uiCamera;

        public Canvas canvas;
        public CanvasScaler scaler;
        public GraphicRaycaster raycaster;

        [field: SerializeField] public Transform backLayer { get; private set; }
        [field: SerializeField] public Transform popupLayer { get; private set; }
        [field: SerializeField] public Transform systemLayer { get; private set; }

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