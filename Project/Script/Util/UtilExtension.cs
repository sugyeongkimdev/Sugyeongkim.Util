using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SugyeongKim.Util
{
    public static class UtilExtension
    {
        // ex) (new int[0]).ForEach ((c, i) => { /*function*/ });
        public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T> loop)
        {
            foreach (var item in enumerable)
            {
                loop.Invoke (item);
            }
        }
        public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T, int> loop)
        {
            int index = 0;
            foreach (var item in enumerable)
            {
                loop.Invoke (item, index++);
            }
        }

        //============================================================//

        // target component class 필드/프로퍼티 전부 해제
        public static void ReleaseFields<T> (this T releaseTarget) where T : Component
        {
            var popupType = releaseTarget.GetType ();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (var field in popupType.GetFields (flags))
            {
                field.SetValue (releaseTarget, null);
            }
            foreach (var prop in popupType.GetProperties (flags))
            {
                if (prop.CanWrite)
                {
                    prop.SetValue (releaseTarget, null);
                }
            }
        }

    }
}