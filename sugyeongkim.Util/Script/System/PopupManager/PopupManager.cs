using System;
using UniRx;
using UnityEngine;

namespace SugyeongKim.Util
{
    public class PopupManager : GlobalSingleton<PopupManager>
    {

        public static IObservable<T> OpenAsObservable<T> (string path) where T : PopupBase
        {
            //var popup = Manager_Popup.instance.CreatePopup<T> ();
            //p

            //Addressables.LoadAssetsAsync()

            return AddressablesManager.LoadAsObservable<GameObject> (path, instance)
                .Select (prefab => prefab.GetComponent<T> ());

        }
    }
}