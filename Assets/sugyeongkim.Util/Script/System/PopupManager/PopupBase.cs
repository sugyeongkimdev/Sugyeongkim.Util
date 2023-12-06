using UnityEngine;


namespace SugyeongKim.Util
{
    public abstract class PopupBase : MonoBehaviour
    {

        public void Close ()
        {

        }
    }

    public abstract class PopupBase<T> : PopupBase where T : PopupBase
    {

    }
}