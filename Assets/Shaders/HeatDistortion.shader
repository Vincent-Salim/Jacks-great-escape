Shader "Custom/HeatDistortion"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DistortionTex("Distortion Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 0.2)) = 0.005
        _Speed("Speed", Float) = 0.8
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // appdata: structure received from the mesh vertex buffer (object space)
            // POSITION = vertex position, TEXCOORD0 = base UV coordinates
            struct appdata
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            // v2f: data passed from the vertex shader to the fragment shader
            // SV_POSITION = clip-space position used for rasterization
            // TEXCOORD0 = main UVs for sampling textures
            // TEXCOORD1 = additional UVs for distortion map sampling
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float2 distortionUV : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DistortionTex);
            SAMPLER(sampler_DistortionTex);
            // For SRP batcher/optimisation
            CBUFFER_START(UnityPerMaterial)
                float _DistortionStrength;
                float _Speed;
            CBUFFER_END

            // Process vertex data before going to the fragment shader
            v2f vert(appdata v)
            {
                v2f o;
                // Transforms vertex from object space to homogenous clip space
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                // Calculates the scrolling UV for the distortion/noise texture
                o.distortionUV = v.uv + float2(0, -_Time.y * _Speed);
                return o;
            }

            // Process fragment data before sending to screen/pass
            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.distortionUV;
                uv.y = frac(uv.y);
                // Calculate the distortion vector by sampling the noise texture using the scrolling UVs
                float2 distortion = SAMPLE_TEXTURE2D(_DistortionTex, sampler_DistortionTex, uv).rg * _DistortionStrength;
                // Sample the main texture by offsetting its original UVs with the calculated distortion vector
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + distortion);
                return col;
            }
            ENDHLSL
        }
    }
}
