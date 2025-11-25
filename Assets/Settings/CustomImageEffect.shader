Shader "Custom/CustomImageEffect"
{

    Properties 
    {
        _MainTex("Base Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off Zwrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 frag(v2f i) : SV_Target
            {
                float4 col = float4(i.uv.x, i.uv.y, 0, 1);
                return col;
            }

            ENDHLSL
        }
    }
}