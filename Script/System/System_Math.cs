namespace SugyeongKim.Unity
{
    public class System_Math
    {
        //============================================//

        // 랜덤
        public static int RandomInt (int start, int end)
        {
            return UnityEngine.Random.Range (start, end);
        }
        // 시드 랜덤
        public static int RandomIntSeed (int start, int end, int seed)
        {
            return new System.Random (seed).Next (start, end);
        }

        // 랜덤
        public static bool RandomBool ()
        {
            return RandomInt (0, 1) % 1 == 0 ? true : false;
        }
        // 시드 랜덤
        public static bool RandomBoolSeed (int seed)
        {
            return RandomIntSeed (0, 1, seed) % 1 == 0 ? true : false;
        }

        //============================================//
    }
}