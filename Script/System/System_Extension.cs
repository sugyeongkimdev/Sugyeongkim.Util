using System;
using System.Collections.Generic;

namespace BigUtil
{
    public static class System_Extension
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