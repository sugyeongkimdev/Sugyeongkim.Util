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
    }
}