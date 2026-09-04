Shader "Custom/PurpleBrushArrow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Arrow Mask (Brush)", 2D) = "white" {}
        [Toggle] _InvertMask ("Invert Mask", Float) = 0
        
        _ArrowColor ("Arrow Color (Background Layer)", Color) = (0.5, 0.0, 0.8, 1.0)
        _DarkenColor ("Darken Background Color", Color) = (0, 0, 0, 0.8) 
        _FadeColor ("Final Fade Color (Foreground Layer)", Color) = (0.0, 0.0, 0.0, 1.0)
        _FadeProgress ("Progress (0-1)", Range(0, 1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MaskTex;
            float _InvertMask;
            float4 _ArrowColor;
            float4 _DarkenColor;
            float4 _FadeColor;
            float _FadeProgress;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Hàm lấy mask cơ bản
            float GetBaseMask(float2 uv, float2 tiling, float speed, float xOffset)
            {
                float2 texUV = uv * tiling;
                // Chỉ di chuyển ngang (X), giữ nguyên dọc (Y) để đảm bảo "trật tự"
                texUV.x -= _FadeProgress * speed + xOffset; 
                
                float maskVal = tex2D(_MaskTex, texUV).r;
                return lerp(maskVal, 1.0 - maskVal, _InvertMask);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 1. Phủ kính râm (Làm tối nền nhẹ lúc ban đầu)
                float darkenFactor = smoothstep(0.0, 0.2, _FadeProgress);
                fixed4 baseDarken = _DarkenColor;
                baseDarken.a *= darkenFactor; 

                // --- GIAI ĐOẠN 2: LỚP TÍM (NỀN) ---
                // Tạo một lớp mũi tên mờ mờ màu tím chạy trước. 
                // Scale (2, 2) cho mũi tên vừa vặn.
                float purpleLayer = GetBaseMask(i.uv, float2(2.0, 2.0), 3.5, 0.0);
                
                // Mảng quét kiểm soát việc hiển thị từ trái qua phải (cho lớp Tím)
                float sweepPurple = smoothstep(1.2, -0.2, i.uv.x - (_FadeProgress * 3.5) + 0.8);
                float purpleAlpha = purpleLayer * sweepPurple;
                
                // Chỉ bắt đầu hiện lớp Tím khi FadeProgress lớn hơn 0.1
                purpleAlpha *= smoothstep(0.1, 0.3, _FadeProgress);


                // --- GIAI ĐOẠN 3: LỚP ĐEN (TIỀN CẢNH & HỖN LOẠN) ---
                // Tạo một lớp mũi tên khác, scale hơi khác một chút (1.8, 1.8) 
                // và chạy nhanh hơn, lệch nhịp đi để tạo ra độ đan xen "hỗn loạn" trên nền cái trật tự.
                float blackLayer = GetBaseMask(i.uv, float2(1.8, 1.8), 3.5, 0.4);
                
                // Mảng quét cho lớp đen (chạy sau lớp tím một chút)
                float sweepBlack = smoothstep(1.3, -0.1, i.uv.x - (_FadeProgress * 3.5) + 1.2);
                float blackAlpha = blackLayer * sweepBlack;

                // Giai đoạn cuối: Màn hình ngập trong màu đen
                float finalBlackout = smoothstep(0.7, 1.0, _FadeProgress);
                
                // --- TỔNG HỢP MÀU ---
                fixed4 finalColor = baseDarken;
                
                // Đè lớp Tím lên trên nền mờ
                finalColor.rgb = lerp(finalColor.rgb, _ArrowColor.rgb, purpleAlpha);
                finalColor.a = saturate(finalColor.a + purpleAlpha);
                
                // Đè tiếp lớp Đen lên trên cùng
                finalColor.rgb = lerp(finalColor.rgb, _FadeColor.rgb, max(blackAlpha, finalBlackout));
                finalColor.a = saturate(finalColor.a + blackAlpha + finalBlackout);

                return finalColor;
            }
            ENDCG
        }
    }
}