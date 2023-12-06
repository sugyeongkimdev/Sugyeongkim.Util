using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BigUtil
{
    // NGUI 필요함, 근데 내가안써서 이거 비활성화 할거임

    public static class UtilNGUI
    {
#if NEED_NGUI
        // 토글배열에 대한 값 이벤트 등록
        public static void ToggleArrValueBind<T> (UIToggle[] toggleArr, bool[] defaultToggleValueArr, T[] setToggleValueArr, Action<T> onToggleCallback)
        {
            // 모든 배열의 길이는 같아야 한다
            if (toggleArr.Length != defaultToggleValueArr.Length || toggleArr.Length != setToggleValueArr.Length)
            {
                Console.Error ("arrays is not match");
                return;
            }

            // 토글 이벤트 등록
            for (int i = 0; i < toggleArr.Length; i++)
            {
                int closure = i;
                toggleArr[closure].Set (defaultToggleValueArr[closure], false);
                EventDelegate.Add (toggleArr[closure].onChange, () => Callback (toggleArr[closure], setToggleValueArr[closure]));
            }

            // 해당 토글이 true일 경우 콜백에 등록된 값을 실행함
            void Callback (UIToggle toggle, T value)
            {
                if (toggle.value)
                {
                    onToggleCallback (value);
                }
            }
        }

        //==========================================================//

        // NGUI 스프라이트 gray scale 효과주기
        // Resources폴더에 Transparent Colored (Gray) (SoftClip) 쉐이더가 필요함
        private static Dictionary<Material, Material> cachedMatDic = new Dictionary<Material, Material> ();
        public static void GrayEffect (UISprite oriSpr, bool reset)
        {
            if (reset)
            {
                oriSpr.material = null;
            }
            else
            {
                oriSpr.material = GetGrayMaterial (oriSpr.material);
            }

            // 해당 메테리얼을 생성후 쉐이더 변경하기
            Material GetGrayMaterial (Material oriMat)
            {
                if (!oriMat)
                {
                    return null;
                }
                if (!cachedMatDic.TryGetValue (oriMat, out var cachedMat) || !cachedMat)
                {
                    // http://devkorea.co.kr/bbs/board.php?bo_table=m03_lecture&wr_id=3561
                    string SHADER_PATH = "Unlit/Transparent Colored (Gray) (SoftClip)";

                    cachedMat = new Material (oriMat);
                    cachedMat.shader = Shader.Find (SHADER_PATH);
                    cachedMat.name = $"Copy_{SHADER_PATH.Split ('/').Last ()}";
                    cachedMat.SetFloat ("_EffectAmount", 1f);
                    cachedMatDic[oriMat] = cachedMat;
                }
                return cachedMat;
            }
        }

        // NGUI Label gray scale 효과주기
        private static Dictionary<Color, Color> cachedOriColorDic = new Dictionary<Color, Color> ();
        private static Dictionary<Color, Color> cachedGrayColorDic = new Dictionary<Color, Color> ();
        public static void GrayEffect (UILabel oriLabel, bool reset)
        {
            if (reset)
            {
                oriLabel.color = GetResetGrayColor (oriLabel.color);
                oriLabel.effectColor = GetResetGrayColor (oriLabel.effectColor);
            }
            else
            {
                oriLabel.color = GetGrayColor (oriLabel.color);
                oriLabel.effectColor = GetGrayColor (oriLabel.effectColor);
            }

            // 해당 색을 회색으로 변경해서 가져오기
            Color GetGrayColor (Color oriColor)
            {
                if (!cachedGrayColorDic.TryGetValue (oriColor, out var cachedGrayColor) || cachedGrayColor == Color.clear)
                {
                    //float y = oriColor.r * 0.299f + oriColor.g * 0.587f + oriColor.b * 0.114f;
                    float y = oriColor.r * 0.2126f + oriColor.g * 0.7152f + oriColor.b * 0.0722f;
                    cachedGrayColor = new Color (y, y, y);
                    cachedOriColorDic[cachedGrayColor] = oriColor;
                    cachedGrayColorDic[oriColor] = cachedGrayColor;
                }
                return cachedGrayColor;
            }
            // 해당색을 변경한적이 있는지 체크 후 원래 색상 가져오기
            Color GetResetGrayColor (Color grayColor)
            {
                return cachedOriColorDic.TryGetValue (grayColor, out var oriColor) ? oriColor : grayColor;
            }
        }
#endif
    }
}