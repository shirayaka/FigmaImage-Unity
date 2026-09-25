Shader "UI/RoundedImage"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1; // xy = localPos (pixels), zw = halfSize (pixels)
                float4 texcoord2 : TEXCOORD2; // xyzw = corner radii (TL, TR, BR, BL) in pixels
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 uiParams : TEXCOORD3; // xy = localPos, zw = halfSize
                float4 radii    : TEXCOORD4; // xyzw = TL, TR, BR, BL
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                OUT.uiParams = v.texcoord1;
                OUT.radii = v.texcoord2;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                // Corner radius SDF masking
                float2 p = IN.uiParams.xy;
                float2 b = IN.uiParams.zw;
                float4 r = IN.radii;

                float mask = 1.0;
                if (b.x > 0.0 && b.y > 0.0)
                {
                    // Select radius based on quadrant:
                    // r.x = Top-Left, r.y = Top-Right, r.z = Bottom-Right, r.w = Bottom-Left
                    // In Unity UI rect space: +X is right, +Y is up
                    // TL: x < 0, y >= 0 -> r.x
                    // TR: x >= 0, y >= 0 -> r.y
                    // BR: x >= 0, y < 0  -> r.z
                    // BL: x < 0, y < 0  -> r.w
                    float rad = (p.x >= 0.0) ? ((p.y >= 0.0) ? r.y : r.z) : ((p.y >= 0.0) ? r.x : r.w);

                    // Clamp rad so it cannot exceed halfSize
                    rad = min(rad, min(b.x, b.y));

                    float2 q = abs(p) - b + rad;
                    float dist = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - rad;

                    float aa = fwidth(dist);
                    aa = (aa > 0.0001) ? aa : 1.0;
                    mask = 1.0 - smoothstep(-aa * 0.5, aa * 0.5, dist);

                    color.a *= mask;
                }

                // Discard pixels outside the rounded shape so GPU stencil operations
                // (UnityEngine.UI.Mask) are not written to the outer corners.
                clip(mask - 0.001);

                return color;
            }
        ENDCG
        }
    }
}
