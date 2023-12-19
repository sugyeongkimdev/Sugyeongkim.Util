using System;

namespace SugyeongKim.Util
{
    // global singleton 초기화, 자동생성 모두 무시하는 어트리뷰트
    [AttributeUsage (AttributeTargets.Class)]
    public class GlobalSingletonIgnoreAttribute : Attribute
    {
        public bool isIgnore;
        public GlobalSingletonIgnoreAttribute (bool isIgnore = false)
        {
            this.isIgnore = isIgnore;
        }
    }
}