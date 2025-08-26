Shader "Rasen/DepthCapture_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NearDistance ("Near Distance", Float) = 0.5
        _FarDistance ("Far Distance", Float) = 20.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float _NearDistance;
            float _FarDistance;

            v2f vert(float4 positionOS : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(positionOS.xyz);
                o.uv = uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // スクリーン座標からUVを計算
                float2 UV = i.positionHCS.xy / _ScaledScreenParams.xy;
                
                // 深度をサンプリング（0-1の範囲）
                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(UV);
                #else
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(UV));
                #endif
                
                // LinearEyeDepthで実際の距離に変換
                float linearDepth = LinearEyeDepth(depth, _ZBufferParams);
                
                // 指定範囲で0-1に正規化
                float normalizedDepth = (linearDepth - _NearDistance) / (_FarDistance - _NearDistance);
                normalizedDepth = saturate(normalizedDepth);
                
                return half4(normalizedDepth, normalizedDepth, normalizedDepth, 1);
            }
            ENDHLSL
        }
    }
}