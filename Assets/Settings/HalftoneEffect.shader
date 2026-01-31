Shader "Hidden/HalftoneEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DotSize ("Dot Size", Range(1, 20)) = 5
        _DotIntensity ("Dot Intensity", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "HalftonePass"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float _DotSize;
            float _DotIntensity;
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            float halftone(float2 uv, float luminance)
            {
                float2 gridUV = frac(uv * _ScreenParams.xy / _DotSize);
                float2 center = float2(0.5, 0.5);
                float dist = distance(gridUV, center);
                float radius = luminance * 0.7;
                return smoothstep(radius, radius + 0.1, dist);
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // 转换为灰度
                float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
                
                // 应用半色调效果
                float dotPattern = halftone(input.uv, luminance);
                
                // 混合原图和圆点效果
                float3 finalColor = lerp(float3(0, 0, 0), float3(1, 1, 1), dotPattern);
                finalColor = lerp(color.rgb, finalColor, _DotIntensity);
                
                return half4(finalColor, color.a);
            }
            ENDHLSL
        }
    }
}
