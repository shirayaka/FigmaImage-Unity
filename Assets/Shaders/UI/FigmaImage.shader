Shader "UI/FigmaImage"
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
                float4 vertex       : POSITION;
                float4 color        : COLOR;
                float2 texcoord     : TEXCOORD0;
                float4 texcoord1    : TEXCOORD1; // xy = localPos (pixels), zw = halfSize (pixels)
                float4 texcoord2    : TEXCOORD2; // xyzw = corner radii (TL, TR, BR, BL) in pixels
                float4 texcoord3    : TEXCOORD3; // xy = packed stroke color (RG, BA), zw = packed shadow color (RG, BA)
                float4 tangent      : TANGENT;   // x = strokeWidth (pixels), y = fillAlpha (0..1), z = strokeEnabled (0/1), w = shadowSpread (pixels)
                float3 normal       : NORMAL;    // x = shadowOffsetX, y = shadowOffsetY, z = shadowBlur (-1 if disabled)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 uiParams      : TEXCOORD3; // xy = localPos, zw = halfSize
                float4 radii         : TEXCOORD4; // xyzw = TL, TR, BR, BL
                float4 packedColors  : TEXCOORD5; // xy = stroke, zw = shadow
                float4 strokeParams  : TEXCOORD6; // x = strokeWidth, y = fillAlpha, z = strokeEnabled, w = shadowSpread
                float3 shadowParams  : TEXCOORD7; // x = shadowOffsetX, y = shadowOffsetY, z = shadowBlur
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _StencilOp;

            half4 UnpackColor(float pRG, float pBA)
            {
                half r = floor(pRG / 256.0) / 255.0;
                half g = (pRG - floor(pRG / 256.0) * 256.0) / 255.0;
                half b = floor(pBA / 256.0) / 255.0;
                half a = (pBA - floor(pBA / 256.0) * 256.0) / 255.0;
                return half4(r, g, b, a);
            }

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
                OUT.packedColors = v.texcoord3;
                OUT.strokeParams = v.tangent;
                OUT.shadowParams = v.normal;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // UI local geometry in pixels
                float2 p = IN.uiParams.xy;
                float2 b = IN.uiParams.zw;
                float4 r = IN.radii;

                float shapeMask = 1.0;
                float shapeDist = 0.0;
                float aa = 1.0;

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

                    // Inigo Quilez 2D rounded box SDF
                    float2 q = abs(p) - b + rad;
                    shapeDist = length(max(q, 0.0)) + min(max(q.x, q.y), 0.0) - rad;

                    aa = fwidth(shapeDist);
                    aa = (aa > 0.0001) ? aa : 1.0;
                    shapeMask = 1.0 - smoothstep(-aa * 0.5, aa * 0.5, shapeDist);
                }

                // Inherited alpha from CanvasGroup / CanvasRenderer (passed through IN.color.a)
                float inheritedAlpha = IN.color.a;

                // 1. Fill color calculation
                // IN.strokeParams.y holds base Image fill alpha (0..1)
                float fillAlpha = IN.strokeParams.y * inheritedAlpha;
                half4 fillSample = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;
                half4 fillColor = half4(fillSample.rgb * IN.color.rgb, fillSample.a * fillAlpha);

                // 2. Inside stroke calculation
                float strokeWidth = min(IN.strokeParams.x, min(b.x, b.y));
                bool strokeEnabled = (IN.strokeParams.z > 0.5) && (strokeWidth > 0.001);

                half4 shapeColor = fillColor;

                if (strokeEnabled)
                {
                    // Inner edge of stroke is at shapeDist = -strokeWidth
                    // Inside stroke region is shapeDist in [-strokeWidth, 0]
                    float innerDist = shapeDist + strokeWidth;
                    float strokeFactor = smoothstep(-aa * 0.5, aa * 0.5, innerDist);

                    half4 stroke = UnpackColor(IN.packedColors.x, IN.packedColors.y);
                    stroke.a *= inheritedAlpha;

                    // Standard "Over" blend: Stroke layered over Fill
                    float sa = stroke.a * strokeFactor;
                    float outA = sa + fillColor.a * (1.0 - sa);
                    float3 outRGB = (outA > 0.0001)
                        ? (stroke.rgb * sa + fillColor.rgb * fillColor.a * (1.0 - sa)) / outA
                        : stroke.rgb;

                    shapeColor = half4(outRGB, outA);
                }

                // Outer shape alpha
                float shapeFinalAlpha = shapeColor.a * shapeMask;
                half4 finalColor = half4(shapeColor.rgb, shapeFinalAlpha);

                // 3. Drop Shadow calculation
                float shadowBlur = IN.shadowParams.z;
                bool shadowEnabled = (shadowBlur >= 0.0);

                if (shadowEnabled && (b.x > 0.0 && b.y > 0.0))
                {
                    float2 shadowOffset = IN.shadowParams.xy;
                    float shadowSpread = IN.strokeParams.w;
                    half4 shadowColor = UnpackColor(IN.packedColors.z, IN.packedColors.w);
                    shadowColor.a *= inheritedAlpha;

                    // Convert Figma coordinates (+Y down) to Unity UI local space (+Y up):
                    // p_shadow = p - (offsetX, -offsetY)
                    float2 p_shadow = p - float2(shadowOffset.x, -shadowOffset.y);

                    // Shadow corner radius selection for p_shadow
                    float rad_shadow = (p_shadow.x >= 0.0) ? ((p_shadow.y >= 0.0) ? r.y : r.z) : ((p_shadow.y >= 0.0) ? r.x : r.w);
                    rad_shadow = min(rad_shadow, min(b.x, b.y));

                    float2 q_shadow = abs(p_shadow) - b + rad_shadow;
                    float dist_shadow = length(max(q_shadow, 0.0)) + min(max(q_shadow.x, q_shadow.y), 0.0) - rad_shadow - shadowSpread;

                    float shadowFactor = 0.0;
                    if (shadowBlur <= 0.0001)
                    {
                        // Hard shadow with subpixel anti-aliasing
                        shadowFactor = 1.0 - smoothstep(-aa * 0.5, aa * 0.5, dist_shadow);
                    }
                    else
                    {
                        // Soft shadow: continuous cubic Hermite falloff over [-shadowBlur, +shadowBlur]
                        float blurRange = max(shadowBlur, aa * 0.5);
                        float t = clamp((dist_shadow + blurRange) / (2.0 * blurRange), 0.0, 1.0);
                        shadowFactor = 1.0 - t * t * (3.0 - 2.0 * t);
                    }

                    float shadowFinalAlpha = shadowColor.a * shadowFactor;

                    // "Over" composite: Shape layered OVER Shadow
                    // outAlpha = shapeAlpha + shadowAlpha * (1 - shapeAlpha)
                    // outRGB = (shapeRGB * shapeAlpha + shadowRGB * shadowAlpha * (1 - shapeAlpha)) / outAlpha
                    float shadowUnderAlpha = shadowFinalAlpha * (1.0 - shapeFinalAlpha);
                    float outAlpha = shapeFinalAlpha + shadowUnderAlpha;

                    float3 outRGB = (outAlpha > 0.0001)
                        ? (shapeColor.rgb * shapeFinalAlpha + shadowColor.rgb * shadowUnderAlpha) / outAlpha
                        : shapeColor.rgb;

                    finalColor = half4(outRGB, outAlpha);
                }

                // Stencil Mask Generator detection:
                // When _StencilOp > 0.5 (Unity UI Mask uses StencilOp.Replace = 2), this graphic is acting as a mask stencil generator.
                bool isMaskGenerator = (_StencilOp > 0.5);

                if (isMaskGenerator)
                {
                    bool maskIgnoreStroke = (IN.strokeParams.z > 1.5) && strokeEnabled;
                    if (maskIgnoreStroke)
                    {
                        // Clip stencil strictly to the inside boundary of the stroke
                        float innerDist = shapeDist + strokeWidth;
                        float innerMask = 1.0 - smoothstep(-aa * 0.5, aa * 0.5, innerDist);
                        clip(innerMask - 0.001);
                    }
                    else
                    {
                        // Clip stencil strictly to the card's shape boundary, ignoring drop shadow padding
                        clip(shapeMask - 0.001);
                    }
                }
                else
                {
                    // Discard pixels completely outside both the outer shape and drop shadow
                    clip(finalColor.a - 0.001);
                }

                #ifdef UNITY_UI_CLIP_RECT
                finalColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                if (!isMaskGenerator)
                {
                    clip(finalColor.a - 0.001);
                }
                #endif

                return finalColor;
            }
        ENDCG
        }
    }
}
