Shader "Custom/TeleportBeam"
{
    Properties
    {
        [HDR] _BeamColor("Beam Color", Color) = (0.2, 1.0, 1.0, 1.0)
        _NoiseTex("Noise Texture (Energy)", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Float) = -1.5
        _FresnelPower("Fresnel Power (Soft Edges)", Range(0.1, 10.0)) = 2.0
    }

    SubShader
    {
        // Chuyển sang chế độ trong suốt và hòa trộn phát sáng (Additive)
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off // Tắt Cull để nhìn thấy cả mặt trong của Cylinder

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 originalUV : TEXCOORD1; // Dùng để làm mờ 2 đầu Cylinder cố định
                float3 viewDirWS : TEXCOORD2;
                float3 normalWS : NORMAL;
            };

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _BeamColor;
                float4 _NoiseTex_ST;
                float _ScrollSpeed;
                float _FresnelPower;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                // Trượt UV theo thời gian để tạo hiệu ứng hút lên/xuống
                OUT.originalUV = IN.uv; 
                OUT.uv = TRANSFORM_TEX(IN.uv, _NoiseTex);
                OUT.uv.y += _Time.y * _ScrollSpeed;

                // Tính toán hướng nhìn và pháp tuyến để làm mềm viền (Fresnel)
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetCameraPositionWS() - positionWS;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. Lấy dữ liệu nhiễu cuộn
                half noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv).r;

                // 2. Viền mềm Fresnel (Tâm sáng rực, viền mờ ảo)
                float3 viewDir = normalize(IN.viewDirWS);
                float3 normal = normalize(IN.normalWS);
                float fresnel = 1.0 - saturate(dot(viewDir, normal));
                fresnel = 1.0 - pow(fresnel, _FresnelPower); 

                // 3. Làm mờ đỉnh và đáy Cylinder (Mượt mà không bị cắt đứt)
                float fadeBottom = smoothstep(0.0, 0.15, IN.originalUV.y);
                float fadeTop = smoothstep(1.0, 0.85, IN.originalUV.y);
                float verticalFade = fadeBottom * fadeTop;

                // Tổng hợp độ hiển thị
                float alpha = noise * fresnel * verticalFade;

                // Nhân màu HDR
                return half4(_BeamColor.rgb * alpha, alpha);
            }
            ENDHLSL
        }
    }
}