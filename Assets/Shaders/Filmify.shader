Shader "Custom/Filmify"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength("Noise Strength", Float) = 3
        _Aspect("Aspect Ratio", Float) = 1.777
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // Functions for perlin noise defined in shader to run on GPU
        // This allows all the passes to re-use the same noise generation logic
        // without requiring CPU computation or texture look ups.
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

        // Generate time-sensitive random numbers between 0 and 1.
        float rand(float2 pos)
        {
            return frac(sin(dot(pos + _Time.y, float2(12.9898f, 78.233f))) * 43758.5453123f);
        }

        // Generate a random vector on the unit circle.
        float2 randUnitCircle(float2 pos)
        {
            float randVal = rand(pos);
            float theta = 2.0f * PI * randVal;

            return float2(cos(theta), sin(theta));
        }

        // Quintic interpolation curve.
        float quinterp(float f)
        {
            return f*f*f * (f * (f * 6.0f - 15.0f) + 10.0f);
        }

        // Perlin gradient noise generator.
        float perlin2D(float2 pixel)
        {
            float2 pos00 = floor(pixel);
            float2 pos10 = pos00 + float2(1.0f, 0.0f);
            float2 pos01 = pos00 + float2(0.0f, 1.0f);
            float2 pos11 = pos00 + float2(1.0f, 1.0f);

            float2 rand00 = randUnitCircle(pos00);
            float2 rand10 = randUnitCircle(pos10);
            float2 rand01 = randUnitCircle(pos01);
            float2 rand11 = randUnitCircle(pos11);

            float dot00 = dot(rand00, pos00 - pixel);
            float dot10 = dot(rand10, pos10 - pixel);
            float dot01 = dot(rand01, pos01 - pixel);
            float dot11 = dot(rand11, pos11 - pixel);

            float2 d = frac(pixel);

            float x1 = lerp(dot00, dot10, quinterp(d.x));
            float x2 = lerp(dot01, dot11, quinterp(d.x));
            float y  = lerp(x1, x2, quinterp(d.y));

            return y;
        }
        ENDHLSL

        // Start of actual shader
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

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
            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            // For SRP batcher/optimisation
            CBUFFER_START(UnityPerMaterial)
                float _Strength;
                float _Aspect;
            CBUFFER_END

            // Process vertex data before going to the fragment shader
            v2f vert(appdata v)
            {
                v2f o;
                // Transforms vertex from object space to homogenous clip space
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Original colour of the fragment
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                // Calculates absolute pixel position of the fragment
                float2 pos = i.uv * _ScreenParams.xy;
                // Generate perlin noise for the fragment
                float n = perlin2D(pos);
                // Add the calculated noise 
                return col + ( col * _Strength * n );
            }
            ENDHLSL
        }
    }
}
