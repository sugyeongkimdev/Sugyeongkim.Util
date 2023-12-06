// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// NGUIUtil.GrayEffect() 에서 사용
// http://devkorea.co.kr/bbs/board.php?bo_table=m03_lecture&wr_id=3561
Shader "Unlit/Transparent Colored (Gray) (SoftClip)"
{
    Properties
    {
        _MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
        _EffectAmount("Effect Amount", Range(0, 1)) = 0.0
        _Intensity("Intencity", float) = 1.0
    }

        SubShader
        {
            LOD 100

            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag

                    #include "UnityCG.cginc"

                    struct appdata_t
                    {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                    };

                    struct v2f
                    {
                        float4 vertex : SV_POSITION;
                        half2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                        float2 worldPos : TEXCOORD1;
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    fixed _EffectAmount;
                    half _Intensity;
                    float2 _ClipSharpness = float2(20.0, 20.0);

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.color = v.color;
                        o.texcoord = v.texcoord;
                        o.worldPos = TRANSFORM_TEX(v.vertex.xy, _MainTex);
                        return o;
                    }

                    fixed4 frag(v2f i) : COLOR
                    {
                        // Softness factor
                        float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipSharpness;

                        fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                        col.rgb = lerp(col.rgb, dot(col.rgb, float3(0.3, 0.59, 0.11)), _EffectAmount) * _Intensity;
                        float fade = clamp(min(factor.x, factor.y), 0.0, 1.0);
                        col.a *= fade;
                        col.rgb = lerp(half3(0.0, 0.0, 0.0), col.rgb, fade);
                        return col;
                    }
                ENDCG
            }
        }

            SubShader
                    {
                        LOD 100

                        Tags
                        {
                            "Queue" = "Transparent"
                            "IgnoreProjector" = "True"
                            "RenderType" = "Transparent"
                        }

                        Pass
                        {
                            Cull Off
                            Lighting Off
                            ZWrite Off
                            Fog { Mode Off }
                            Offset -1, -1
                            ColorMask RGB
                            AlphaTest Greater .01
                            Blend SrcAlpha OneMinusSrcAlpha
                            ColorMaterial AmbientAndDiffuse

                            SetTexture[_MainTex]
                            {
                                Combine Texture * Primary
                            }
                        }
                    }
}