using SugyeongKim.Util;
using UnityEngine;

// 메세지 매니저
public class ToastManager : GlobalSingleton<ToastManager>
{
    //============================================//

    // 메세지 표시
    public static void ShowMessage (string message)
    {
        if (string.IsNullOrEmpty (message))
        {
            return;
        }
        // 메세지 표시
        Debug.Log (message);
    }
}
