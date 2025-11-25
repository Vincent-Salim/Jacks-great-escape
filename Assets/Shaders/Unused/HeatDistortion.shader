Shader "Custom/HeatDistortion"
{
    Properties 
    {
        _MainTex("Main Texture", 2D) = "white"
        // _BaseTexture("Base Texture", 2D) = "white"
        _Noise("Noise texture", 2D) = "white"
        _Strength("Distort Strength", float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        
        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // SLP batcher compatibility
            CBUFFER_START(UnityPerMaterial)
                float4 _Noise_ST;
                float _Strength;
            CBUFFER_END
            
            // TEXTURE2D(_BaseTexture);
            // SAMPLER(sampler_BaseTexture);
            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            


            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };
            
            v2f vert(appdata v)
            {
                // Ensure not null
                v2f o = (v2f)0;
                
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _Noise);
                o.screenPos = ComputeScreenPos(o.positionCS);
                
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                // float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv);
                float2 noise = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, i.uv).xy;
                noise = ((noise * 2) - 1) * _Strength;
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                screenUV += noise * _Strength;
                float4 distorted = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
                // float4 textureColor = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, i.uv + disp);
                // textureColor.a *= 0.4;
                return float4(distorted.xyz, 1.0);
            }
            ENDHLSL
        }
    }
}