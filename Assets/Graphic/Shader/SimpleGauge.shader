Shader "Original/SimpleGauge"
{
    Properties
    {
        _MainTex("Sprite Texture (Grayscale Gradient)", 2D) = "white" {}
        _Threshold("Threshold", Range(0,1)) = 1.0
        _FullColor("Full Color", Color) = (0,1,0,1)
        _EmptyColor("Empty Color", Color) = (1,0,0,1)
        _BGColor("Background Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "HealthBar"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
            float4 _FullColor;
            float4 _EmptyColor;
            float4 _BGColor;
            float _Threshold;
            CBUFFER_END

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            v2f vert(appdata_t v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // 透明部分はBG色
                if (texCol.a < 0.0001)
                    return _BGColor;

                // グレースケール値
                half gray = texCol.r;

                // 閾値比較: gray <= _Threshold ならFullColor、超えてたらEmptyColor
                half4 barColor = lerp(_EmptyColor, _FullColor, step(gray, _Threshold));

                // アルファ合成
                barColor.a *= texCol.a * i.color.a;
                return barColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
