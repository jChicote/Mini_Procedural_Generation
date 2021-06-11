Shader "Custom/HeightMapShader"
{
    Properties
    {
        _ColorA("Color A", Color) = (1,1,1,1)
        _ColorB("Color B", Color) = (1,1,1,1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _LerpTest("LerpTest", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _MainTex("Base (RGB)", 2D) = "white" {}

        _LerpPower("Lerp Power", Int) = 1
        _MaxHeight("Max Height", Float) = 1
        _MinHeight("Min Height", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };

        sampler2D _MainTex;

        fixed4 _ColorA;
        fixed4 _ColorB;
        float _MaxHeight;
        float _MinHeight;
        float _LerpTest;
        int _LerpPower;

        void surf(Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

            // calculates lerp heat value
            float lerpVal = IN.worldPos.y / (_MaxHeight - _MinHeight);
            lerpVal = pow(lerpVal, _LerpPower);
            fixed4 lerpColor = lerp(_ColorA, _ColorB, lerpVal);

            o.Albedo *= lerpColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
