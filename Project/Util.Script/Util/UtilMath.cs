using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// System.Random, UnityEngine.Random 차이
// https://discussions.unity.com/t/what-is-the-difference-between-system-random-and-unityengine-random/160442/3

// System.Random
// https://stackoverflow.com/a/768001
namespace SugyeongKim.Util
{
    public static class UtilMath
    {
        // Seed, Rand
        public static int Seed { get; private set; } = NewSeed ();
        public static System.Random Rand { get; private set; }
        public static int NewSeed (int? newSeed = null)
        {
            if (newSeed == null)
            {
                newSeed = new System.Random ().Next (int.MinValue, int.MaxValue);
            }
            Rand = new System.Random (newSeed.Value);
            Seed = newSeed.Value;
            return newSeed.Value;
        }

        //============================================//

        // 랜덤 int
        public static int GetRandomInt (int start, int end)
        {
            return Rand.Next (start, end);
        }

        // 랜덤 float
        // https://stackoverflow.com/a/3365388
        // https://stackoverflow.com/a/610228
        // GetRandomFloat
        public static float GetRandomFloat (float min, float max)
        {
            if (min > max)
            {
                UtilLog.Error ($"min is bigger than max (min:{min}, max:{max}");
                return min;
            }
            double range = (double)max - (double)min;
            double sample = Rand.NextDouble ();
            double scaled = (sample * range) + min;
            float value = (float)scaled;
            return value;
        }

        // 랜덤 float 0f ~ 1f
        public static float GetRandomFloat01 ()
        {
            return (float)Rand.NextDouble ();
        }

        // 랜덤 bool
        public static bool GetRandomBool ()
        {
            return GetRandomInt (0, 2) == 0 ? true : false;
        }

        //============================================//

        // 랜덤 enumerable value
        public static T GetRandomValue<T> (T[] arr)
        {
            return arr[GetRandomInt (0, arr.Length)];
        }
        public static T GetRandomValue<T> (IList<T> list)
        {
            return list[GetRandomInt (0, list.Count)];
        }
        // ※ ElementAt을 사용하기에 성능이 좋지 못함
        public static T GetRandomValue<T> (ICollection<T> collection)
        {
            return collection.ElementAt (GetRandomInt (0, collection.Count));
        }

        //============================================//

        // 랜덤 dic value (성능 최적화)
        public static TValue RandomDicValue<TKey, TValue> (IDictionary<TKey, TValue> dic)
        {
            return RandomDicEnumerable (dic).FirstOrDefault ();
        }
        // 랜덤 dic value Enumerable (성능 최적화)
        // https://stackoverflow.com/questions/1028136/random-entry-from-dictionary
        public static IEnumerable<TValue> RandomDicEnumerable<TKey, TValue> (IDictionary<TKey, TValue> dic)
        {
            List<TValue> values = Enumerable.ToList (dic.Values);
            while (true)
            {
                yield return values[GetRandomInt (0, dic.Count)];
            }
        }

        //============================================//

        // 길이만큼 값을 제한
        public static int ClampCount<T, U> (this List<T> list, U value) where U : struct
        {
            int castInt = (int)Convert.ChangeType (value, typeof (int));
            return Mathf.Clamp (castInt, 0, list.Count);
        }
        public static int ClampLength<T, U> (this T[] arr, U value) where U : struct
        {
            int castInt = (int)Convert.ChangeType (value, typeof (int));
            return Mathf.Clamp (castInt, 0, arr.Length);
        }

    }
}