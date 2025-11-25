Shader "Custom/HeatDistortion1"
{
    Properties
    {
        _Noise("Noise", 2D) = "white" 
        _StrengthFilter("Strength Filter", 2D) = "white" 
        _Strength("Distort Strength", Float) = 1.0
        _Speed("Distort Speed", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "Distort"
            Tags { "LightMode" = "UniversalForward" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Noise);
            SAMPLER(sampler_Noise);
            TEXTURE2D(_StrengthFilter);
            SAMPLER(sampler_StrengthFilter);
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _Strength;
            float _Speed;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Sample distortion sources
                float noise = SAMPLE_TEXTURE2D(_Noise, sampler_Noise, IN.uv).r;
                float filt = SAMPLE_TEXTURE2D(_StrengthFilter, sampler_StrengthFilter, IN.uv).r;

                // Time comes from URP’s _Time
                float t = _Time.y * _Speed;

                // Offset distortion
                float2 offset = float2(cos(noise * t), sin(noise * t)) * filt * _Strength;

                // Distorted UV for screen sample
                float2 screenUV = (IN.screenPos.xy / IN.screenPos.w) + offset;

                // Sample the background (camera opaque texture)
                float4 col = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
                return col;
                return float4(IN.uv, 0, 1); // should display UV gradient
            }

            ENDHLSL
        }
    }
}