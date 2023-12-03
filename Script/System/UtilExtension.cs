using System;
using System.Collections.Generic;

namespace SugyeongKim.Unity
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
        //==========================================================/

    }
}