using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SugyeongKim.Util
{
    public class GlobalCanvasUIManager : GlobalSingleton<GlobalCanvasUIManager>
    {
        [field: SerializeField, Header ("Common")] public Camera globalCanvasCamera { get; private set; }
        [field: SerializeField] public Canvas globalCanvas { get; private set; }

        //[field: SerializeField, Header("ETC")] public CanvasScaler scaler { get; private set; }
        //[field: SerializeField] public GraphicRaycaster raycaster { get; private set; }
        //[field: SerializeField] public EventSystem uiEventSystem { get; private set; }

        [field: SerializeField, Header("Layers")] public Transform BackLayer { get; private set; }
        [field: SerializeField] public Transform PopupLayer { get; private set; }
        [field: SerializeField] public Transform SystemLayer { get; private set; }

        //============================================//

        public static Camera Camera => instance.globalCanvasCamera;
        public static Canvas Cavnas => instance.globalCanvas;

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