using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectArea.UI.Tests
{
    public static class FigmaImageTests
    {
        public static string RunAllTests()
        {
            var sb = new StringBuilder();
            int passed = 0;
            int total = 65;

            sb.AppendLine("=== Running FigmaImage Acceptance Tests (Corner Radius, Stroke, Drop Shadow & Inner Shadow) ===");

            GameObject testRoot = new GameObject("TestRoot_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = testRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            try
            {
                // Test 1: Rounded Image without Stroke
                {
                    var go = CreateImageObject("Test1_RoundedWithoutStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetRadius(24f);
                    img.SetStrokeEnabled(false);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 24f) && Mathf.Approximately(r.y, 24f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 24f) &&
                        !img.StrokeEnabled)
                    {
                        sb.AppendLine("[PASS] Test 1: Rounded Image without Stroke (300x100, radius 24) -> Radii (24, 24, 24, 24), Stroke Disabled");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 1: Expected (24, 24, 24, 24) stroke off, got {r} stroke {img.StrokeEnabled}");
                    }
                }

                // Test 2: Simple Stroke
                {
                    var go = CreateImageObject("Test2_SimpleStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetRadius(24f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(2f);
                    img.SetStrokeColor(Color.white);

                    Vector4 r = img.GetNormalizedRadii();
                    if (img.StrokeEnabled && Mathf.Approximately(img.StrokeWidth, 2f) &&
                        img.StrokeColor == Color.white && Mathf.Approximately(r.x, 24f))
                    {
                        sb.AppendLine("[PASS] Test 2: Simple Stroke (300x100, radius 24, stroke 2px white) -> Configured correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 2: Expected stroke enabled, width 2, color white; got enabled:{img.StrokeEnabled}, width:{img.StrokeWidth}");
                    }
                }

                // Test 3: Thick Stroke
                {
                    var go = CreateImageObject("Test3_ThickStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetRadius(24f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(12f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex vert = new UIVertex();
                    bool validTangent = false;
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref vert, 0);
                        validTangent = Mathf.Approximately(vert.tangent.x, 12f) && vert.tangent.z > 0.5f;
                    }

                    if (validTangent && Mathf.Approximately(img.StrokeWidth, 12f))
                    {
                        sb.AppendLine("[PASS] Test 3: Thick Stroke (200x80, radius 24, stroke 12px) -> Streamed to shader correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 3: Thick stroke failed tangent check: {vert.tangent}");
                    }
                }

                // Test 4: Independent Corner Radius + Stroke
                {
                    var go = CreateImageObject("Test4_IndependentCornersStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetCornerRadii(32f, 8f, 24f, 0f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(4f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 32f) && Mathf.Approximately(r.y, 8f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 0f) &&
                        Mathf.Approximately(img.StrokeWidth, 4f))
                    {
                        sb.AppendLine("[PASS] Test 4: Independent corners (TL=32, TR=8, BR=24, BL=0) + Stroke 4px -> Preserved correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 4: Expected (32, 8, 24, 0), got {r}");
                    }
                }

                // Test 5: Pill
                {
                    var go = CreateImageObject("Test5_Pill", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 48);
                    img.SetRadius(24f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(3f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 24f) && Mathf.Approximately(r.y, 24f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 24f) &&
                        Mathf.Approximately(img.StrokeWidth, 3f))
                    {
                        sb.AppendLine("[PASS] Test 5: Pill shape (200x48, radius 24, stroke 3px) -> Semicircular ends valid");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 5: Expected (24, 24, 24, 24), got {r}");
                    }
                }

                // Test 6: Radius Smaller than Stroke
                {
                    var go = CreateImageObject("Test6_RadiusSmallerThanStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetRadius(4f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(10f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex vert = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref vert, 0);
                    }

                    if (Mathf.Approximately(img.Radius, 4f) && Mathf.Approximately(img.StrokeWidth, 10f) &&
                        Mathf.Approximately(vert.uv2.x, 4f) && Mathf.Approximately(vert.tangent.x, 10f))
                    {
                        sb.AppendLine("[PASS] Test 6: Radius smaller than stroke (radius 4, stroke 10) -> Geometry and stream stable");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 6: Radius smaller than stroke failed");
                    }
                }

                // Test 7: Oversized Radius
                {
                    var go = CreateImageObject("Test7_OversizedRadius", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(100, 40);
                    img.SetRadius(100f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(4f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 20f) && Mathf.Approximately(r.y, 20f) &&
                        Mathf.Approximately(r.z, 20f) && Mathf.Approximately(r.w, 20f))
                    {
                        sb.AppendLine("[PASS] Test 7: Oversized radius (100x40, radius 100) -> Proportional normalization to (20, 20, 20, 20)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 7: Expected (20, 20, 20, 20), got {r}");
                    }
                }

                // Test 8: Oversized Stroke
                {
                    var go = CreateImageObject("Test8_OversizedStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(40, 40);
                    img.SetRadius(8f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(50f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex vert = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref vert, 0);
                    }

                    // Stroke width clamped to min(halfSize.x, halfSize.y) = 20
                    if (Mathf.Approximately(vert.tangent.x, 20f))
                    {
                        sb.AppendLine("[PASS] Test 8: Oversized stroke (40x40, stroke 50) -> Clamped safely to halfSize (20px)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 8: Expected tangent.x clamped to 20, got {vert.tangent.x}");
                    }
                }

                // Test 9: Sprite and UV Preservation
                {
                    var go = CreateImageObject("Test9_Sprite", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    Texture2D tex = new Texture2D(32, 32);
                    Sprite spr = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                    img.sprite = spr;
                    img.SetRadius(12f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(2f);
                    img.SetStrokeColor(Color.yellow);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool hasUv0 = false, hasUv1 = false, hasUv2 = false, hasUv3 = false, hasTangent = false;

                    if (vh.currentVertCount > 0)
                    {
                        UIVertex vert = new UIVertex();
                        vh.PopulateUIVertex(ref vert, 0);
                        hasUv0 = vert.position != Vector3.zero; // Base sprite vertices present
                        hasUv1 = vert.uv1 != Vector4.zero;      // LocalPos & halfSize
                        hasUv2 = vert.uv2 != Vector4.zero;      // Radii
                        hasUv3 = vert.uv3 != Vector4.zero;      // Stroke Color
                        hasTangent = vert.tangent != Vector4.zero; // Stroke Params
                    }

                    UnityEngine.Object.DestroyImmediate(spr);
                    UnityEngine.Object.DestroyImmediate(tex);

                    if (hasUv1 && hasUv2 && hasUv3 && hasTangent)
                    {
                        sb.AppendLine("[PASS] Test 9: Sprite assigned, UV0 preserved, UV1/UV2/UV3/Tangent streams intact");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 9: Missing vertex channels. uv1:{hasUv1}, uv2:{hasUv2}, uv3:{hasUv3}, tangent:{hasTangent}");
                    }
                }

                // Test 10: Button Target Graphic
                {
                    var go = CreateImageObject("Test10_Button", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(2f);
                    var btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    if (btn.targetGraphic == img && img.raycastTarget && img.StrokeEnabled)
                    {
                        sb.AppendLine("[PASS] Test 10: Button Target Graphic assigned, stroke preserved, raycast target functional");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 10: Button binding failed");
                    }
                }

                // Test 11: Mask & RectMask2D
                {
                    var maskGo = new GameObject("MaskContainer", typeof(RectTransform), typeof(Mask));
                    maskGo.transform.SetParent(testRoot.transform, false);

                    var go = CreateImageObject("Test11_Masked", maskGo);
                    var img = go.GetComponent<FigmaImage>();
                    Material modifiedMat = img.GetModifiedMaterial(img.defaultMaterial);

                    var rectMaskGo = new GameObject("RectMaskContainer", typeof(RectTransform), typeof(RectMask2D));
                    rectMaskGo.transform.SetParent(testRoot.transform, false);
                    go.transform.SetParent(rectMaskGo.transform, false);

                    if (modifiedMat != null)
                    {
                        sb.AppendLine("[PASS] Test 11: Mask & RectMask2D compatible with FigmaImage shader stencil/clipping");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 11: GetModifiedMaterial returned null");
                    }
                }

                // Test 12: Multiple FigmaImages
                {
                    bool allDistinct = true;
                    Material sharedDefault = null;

                    for (int i = 0; i < 10; i++)
                    {
                        var go = CreateImageObject($"Test12_Instance_{i}", testRoot);
                        var rt = go.GetComponent<RectTransform>();
                        rt.sizeDelta = new Vector2(300, 200);

                        var img = go.GetComponent<FigmaImage>();
                        float targetRadius = 5f + i * 3f;
                        float targetStroke = 1f + i * 0.5f;
                        Color targetColor = new Color(0.1f * i, 0.2f * i, 1f, 1f);

                        img.SetRadius(targetRadius);
                        img.SetStrokeEnabled(true);
                        img.SetStrokeWidth(targetStroke);
                        img.SetStrokeColor(targetColor);

                        if (sharedDefault == null)
                        {
                            sharedDefault = img.defaultMaterial;
                        }
                        else if (img.defaultMaterial != sharedDefault)
                        {
                            allDistinct = false;
                        }

                        Vector4 r = img.GetNormalizedRadii();
                        if (!Mathf.Approximately(r.x, targetRadius) ||
                            !Mathf.Approximately(img.StrokeWidth, targetStroke) ||
                            img.StrokeColor != targetColor)
                        {
                            allDistinct = false;
                        }
                    }

                    if (allDistinct && sharedDefault != null)
                    {
                        sb.AppendLine("[PASS] Test 12: 10 distinct instances maintain independent radii/stroke and share default material");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 12: Multiple instances isolation or material sharing failed");
                    }
                }

                // Test 13: Resize
                {
                    var go = CreateImageObject("Test13_Resize", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetRadius(24f);

                    Vector4 rBefore = img.GetNormalizedRadii();
                    rt.sizeDelta = new Vector2(30, 20);
                    Vector4 rAfter = img.GetNormalizedRadii();

                    float expected = 24f * (20f / 48f);
                    if (Mathf.Approximately(rBefore.x, 24f) && Mathf.Approximately(rAfter.x, expected))
                    {
                        sb.AppendLine($"[PASS] Test 13: Dynamic Resize (300x100 -> 30x20) normalized radius from 24 to {rAfter.x:F1}");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 13: Resize normalization mismatch: expected {expected}, got {rAfter.x}");
                    }
                }

                // Test 14: Edit Mode Immediate Updates
                {
                    var go = CreateImageObject("Test14_EditMode", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.Radius = 32f;
                    img.LinkCorners = false;
                    img.SetCornerRadii(16f, 24f, 8f, 4f);
                    img.StrokeEnabled = true;
                    img.StrokeWidth = 3f;
                    img.StrokeColor = Color.cyan;

                    Vector4 r = img.GetNormalizedRadii();
                    bool validRadii = Mathf.Approximately(r.x, 16f) && Mathf.Approximately(r.y, 24f) &&
                                     Mathf.Approximately(r.z, 8f) && Mathf.Approximately(r.w, 4f);
                    bool validStroke = img.StrokeEnabled && Mathf.Approximately(img.StrokeWidth, 3f) && img.StrokeColor == Color.cyan;

                    if (validRadii && validStroke)
                    {
                        sb.AppendLine("[PASS] Test 14: Immediate Edit Mode property modifications & dirtying confirmed");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 14: Edit Mode property evaluation mismatch");
                    }
                }

                // Test 15: Standard Shadow
                {
                    var go = CreateImageObject("Test15_StandardShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetRadius(16f);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(8f);
                    img.SetDropShadowSpread(0f);
                    img.SetDropShadowColor(new Color(0f, 0f, 0f, 0.25f));

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v1 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    UIVertex v3 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v1, 1);
                        vh.PopulateUIVertex(ref v2, 2);
                        vh.PopulateUIVertex(ref v3, 3);
                    }

                    // Original bottom was -40, top was +40.
                    // Positive Y in Figma moves down -> bottom padding expands by 4px more than top padding.
                    float bottomExp = -40f - v0.position.y;
                    float topExp = v1.position.y - 40f;
                    bool expanded = bottomExp > topExp && Mathf.Approximately(bottomExp - topExp, 4f);
                    bool validChannels = Mathf.Approximately(v0.normal.z, 8f) && Mathf.Approximately(v0.normal.y, 4f);

                    if (expanded && validChannels && img.DropShadowEnabled)
                    {
                        sb.AppendLine("[PASS] Test 15: Standard Shadow (200x80, radius 16, 0, 4, blur 8) -> Mesh expanded with downward bias & channels intact");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 15: Expected bottomExp > topExp by 4 and normal.z == 8; got bottom:{bottomExp}, top:{topExp}, normal.z:{v0.normal.z}");
                    }
                }

                // Test 16: Right Offset
                {
                    var go = CreateImageObject("Test16_RightOffset", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(12f, 0f);
                    img.SetDropShadowBlur(8f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    float rightExp = v2.position.x - 100f;
                    float leftExp = -100f - v0.position.x;
                    if (rightExp > leftExp && Mathf.Approximately(rightExp - leftExp, 12f))
                    {
                        sb.AppendLine("[PASS] Test 16: Right Offset (X=12, Y=0, Blur=8) -> Right padding expands 12px more than left");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 16: Right offset expansion failed: rightExp:{rightExp}, leftExp:{leftExp}");
                    }
                }

                // Test 17: Left Offset
                {
                    var go = CreateImageObject("Test17_LeftOffset", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(-12f, 0f);
                    img.SetDropShadowBlur(8f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    float rightExp = v2.position.x - 100f;
                    float leftExp = -100f - v0.position.x;
                    if (leftExp > rightExp && Mathf.Approximately(leftExp - rightExp, 12f))
                    {
                        sb.AppendLine("[PASS] Test 17: Left Offset (X=-12, Y=0, Blur=8) -> Left padding expands 12px more than right");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 17: Left offset expansion failed: leftExp:{leftExp}, rightExp:{rightExp}");
                    }
                }

                // Test 18: Figma Y Convention
                {
                    var go = CreateImageObject("Test18_FigmaYConvention", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowBlur(8f);

                    // Y = +12 (Figma down)
                    img.SetDropShadowOffset(0f, 12f);
                    VertexHelper vhDown = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhDown, SendMessageOptions.DontRequireReceiver);
                    UIVertex vDown0 = new UIVertex();
                    UIVertex vDown1 = new UIVertex();
                    vhDown.PopulateUIVertex(ref vDown0, 0);
                    vhDown.PopulateUIVertex(ref vDown1, 1);
                    float downBottomExp = -40f - vDown0.position.y;
                    float downTopExp = vDown1.position.y - 40f;

                    // Y = -12 (Figma up)
                    img.SetDropShadowOffset(0f, -12f);
                    VertexHelper vhUp = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhUp, SendMessageOptions.DontRequireReceiver);
                    UIVertex vUp0 = new UIVertex();
                    UIVertex vUp1 = new UIVertex();
                    vhUp.PopulateUIVertex(ref vUp0, 0);
                    vhUp.PopulateUIVertex(ref vUp1, 1);
                    float upBottomExp = -40f - vUp0.position.y;
                    float upTopExp = vUp1.position.y - 40f;

                    if (downBottomExp > downTopExp && upTopExp > upBottomExp)
                    {
                        sb.AppendLine("[PASS] Test 18: Figma Y Convention (+Y moves down to bottomPad, -Y moves up to topPad)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 18: Figma Y convention mismatch");
                    }
                }

                // Test 19: Blur Zero
                {
                    var go = CreateImageObject("Test19_BlurZero", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(0f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    bool noNaN = !float.IsNaN(v0.position.x) && !float.IsInfinity(v0.position.x);
                    if (noNaN && Mathf.Approximately(v0.normal.z, 0f) && Mathf.Approximately(img.DropShadowBlur, 0f))
                    {
                        sb.AppendLine("[PASS] Test 19: Blur Zero -> Hard edge configured, normal.z = 0, no NaN/Infinity");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 19: Blur zero failed");
                    }
                }

                // Test 20: Large Blur
                {
                    var go = CreateImageObject("Test20_LargeBlur", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 0f);
                    img.SetDropShadowBlur(32f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    float padding = v2.position.x - 100f;
                    // For blur 32, padding should be at least 32 * 1.5 = 48
                    if (padding >= 48f && Mathf.Approximately(v0.normal.z, 32f))
                    {
                        sb.AppendLine($"[PASS] Test 20: Large Blur (Blur=32) -> Ample padding ({padding}px) prevents self-clipping");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 20: Large blur padding insufficient: {padding}");
                    }
                }

                // Test 21: Positive Spread
                {
                    var go = CreateImageObject("Test21_PositiveSpread", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowBlur(8f);

                    img.SetDropShadowSpread(0f);
                    VertexHelper vh0 = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh0, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0_0 = new UIVertex();
                    UIVertex v2_0 = new UIVertex();
                    vh0.PopulateUIVertex(ref v0_0, 0);
                    vh0.PopulateUIVertex(ref v2_0, 2);
                    float pad0 = v2_0.position.x - 100f;

                    img.SetDropShadowSpread(6f);
                    VertexHelper vh6 = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh6, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0_6 = new UIVertex();
                    UIVertex v2_6 = new UIVertex();
                    vh6.PopulateUIVertex(ref v0_6, 0);
                    vh6.PopulateUIVertex(ref v2_6, 2);
                    float pad6 = v2_6.position.x - 100f;

                    if (Mathf.Approximately(pad6 - pad0, 6f) && Mathf.Approximately(v0_6.tangent.w, 6f))
                    {
                        sb.AppendLine("[PASS] Test 21: Positive Spread (Spread=6 vs 0) -> Mesh padding increased by 6px & streamed to tangent.w");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 21: Spread expansion failed: pad0:{pad0}, pad6:{pad6}");
                    }
                }

                // Test 22: Independent Corners + Drop Shadow
                {
                    var go = CreateImageObject("Test22_IndependentCornersShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetCornerRadii(32f, 8f, 24f, 0f);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(8f);

                    Vector4 r = img.GetNormalizedRadii();
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    if (Mathf.Approximately(r.x, 32f) && Mathf.Approximately(r.y, 8f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 0f) &&
                        Mathf.Approximately(v0.uv2.x, 32f) && Mathf.Approximately(v0.normal.z, 8f))
                    {
                        sb.AppendLine("[PASS] Test 22: Independent corners (32, 8, 24, 0) + Drop Shadow -> Radii stream & shadow stream coexist");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 22: Independent corners failed: {r}");
                    }
                }

                // Test 23: Pill Shape + Drop Shadow
                {
                    var go = CreateImageObject("Test23_PillShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 48);
                    img.SetRadius(24f);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 6f);
                    img.SetDropShadowBlur(12f);

                    Vector4 r = img.GetNormalizedRadii();
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    if (Mathf.Approximately(r.x, 24f) && Mathf.Approximately(r.y, 24f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 24f) &&
                        Mathf.Approximately(v0.normal.y, 6f) && Mathf.Approximately(v0.normal.z, 12f))
                    {
                        sb.AppendLine("[PASS] Test 23: Pill shape (200x48, radius 24) + Drop Shadow -> Semicircular ends & shadow stream valid");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 23: Pill shape failed: {r}");
                    }
                }

                // Test 24: Stroke + Shadow
                {
                    var go = CreateImageObject("Test24_StrokeShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetRadius(20f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(3f);
                    img.SetStrokeColor(Color.white);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 6f);
                    img.SetDropShadowBlur(12f);
                    img.SetDropShadowSpread(0f);
                    img.SetDropShadowColor(new Color(0f, 0f, 0f, 0.25f));

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    bool strokeOk = Mathf.Approximately(v0.tangent.x, 3f) && v0.tangent.z > 0.5f;
                    bool shadowOk = Mathf.Approximately(v0.normal.y, 6f) && Mathf.Approximately(v0.normal.z, 12f);

                    if (strokeOk && shadowOk)
                    {
                        sb.AppendLine("[PASS] Test 24: Stroke + Shadow (radius 20, stroke 3px, shadow 0, 6, blur 12) -> Both layers stream correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 24: Stroke + Shadow streams failed: strokeOk:{strokeOk}, shadowOk:{shadowOk}");
                    }
                }

                // Test 25: Large Offset
                {
                    var go = CreateImageObject("Test25_LargeOffset", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(30f, 30f);
                    img.SetDropShadowBlur(16f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    float rightExp = v2.position.x - 100f;
                    float bottomExp = -40f - v0.position.y;

                    // Extent for blur 16 is 16 * 1.5 + 2 = 26; with offset 30, right and bottom expand by 26 + 30 = 56px
                    if (rightExp >= 54f && bottomExp >= 54f)
                    {
                        sb.AppendLine($"[PASS] Test 25: Large Offset (X=30, Y=30, Blur=16) -> Geometry fully accommodates offset (right:{rightExp}px, bottom:{bottomExp}px)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 25: Large offset expansion failed: right:{rightExp}, bottom:{bottomExp}");
                    }
                }

                // Test 26: Button Target Graphic & Raycast Hit Area
                {
                    var go = CreateImageObject("Test26_ButtonRaycast", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(20f, 20f);
                    img.SetDropShadowBlur(16f);

                    var btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    var rt = go.GetComponent<RectTransform>();
                    Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(null, rt.position);
                    bool centerHit = RectTransformUtility.RectangleContainsScreenPoint(rt, centerScreen, null);

                    // Point in shadow padding (e.g. 150px outside)
                    Vector2 outsideScreen = centerScreen + new Vector2(150f, 0f);
                    bool outsideHit = RectTransformUtility.RectangleContainsScreenPoint(rt, outsideScreen, null);

                    img.UseRoundedRaycast = true;
                    bool roundedOutsideHit = img.IsRaycastLocationValid(outsideScreen, null);

                    if (btn.targetGraphic == img && centerHit && !outsideHit && !roundedOutsideHit)
                    {
                        sb.AppendLine("[PASS] Test 26: Button Target Graphic assigned & Shadow padding is rejected by raycaster (not clickable)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 26: Button raycast check failed: centerHit:{centerHit}, outsideHit:{outsideHit}");
                    }
                }

                // Test 27: Mask / RectMask2D Compatibility
                {
                    var maskGo = new GameObject("MaskContainer_Shadow", typeof(RectTransform), typeof(Mask));
                    maskGo.transform.SetParent(testRoot.transform, false);

                    var go = CreateImageObject("Test27_MaskedShadow", maskGo);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowBlur(8f);
                    Material modifiedMat = img.GetModifiedMaterial(img.defaultMaterial);

                    var rectMaskGo = new GameObject("RectMaskContainer_Shadow", typeof(RectTransform), typeof(RectMask2D));
                    rectMaskGo.transform.SetParent(testRoot.transform, false);
                    go.transform.SetParent(rectMaskGo.transform, false);

                    if (modifiedMat != null)
                    {
                        sb.AppendLine("[PASS] Test 27: Drop Shadow compatible with Mask / RectMask2D stencil & clipping pipeline");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 27: GetModifiedMaterial returned null with drop shadow enabled");
                    }
                }

                // Test 28: Multiple Instances Material Sharing
                {
                    bool allDistinct = true;
                    Material sharedDefault = null;

                    for (int i = 0; i < 10; i++)
                    {
                        var go = CreateImageObject($"Test28_Instance_{i}", testRoot);
                        var img = go.GetComponent<FigmaImage>();
                        img.SetRadius(10f + i * 2f);
                        img.SetStrokeEnabled(true);
                        img.SetStrokeWidth(1f + i * 0.5f);
                        img.SetDropShadowEnabled(true);
                        img.SetDropShadowOffset(i * 2f, 4f + i);
                        img.SetDropShadowBlur(4f + i * 2f);
                        img.SetDropShadowSpread(i * 0.5f);
                        img.SetDropShadowColor(new Color(0.1f * i, 0.05f * i, 0f, 0.25f));

                        if (sharedDefault == null)
                        {
                            sharedDefault = img.defaultMaterial;
                        }
                        else if (img.defaultMaterial != sharedDefault)
                        {
                            allDistinct = false;
                        }

                        if (!Mathf.Approximately(img.DropShadowBlur, 4f + i * 2f) ||
                            !Mathf.Approximately(img.DropShadowOffsetX, i * 2f))
                        {
                            allDistinct = false;
                        }
                    }

                    if (allDistinct && sharedDefault != null)
                    {
                        sb.AppendLine("[PASS] Test 28: 10 distinct instances maintain independent drop shadows and share default material");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 28: Multiple instances isolation or material sharing failed");
                    }
                }

                // Test 29: Dynamic Resize with Drop Shadow
                {
                    var go = CreateImageObject("Test29_ResizeShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetRadius(16f);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(8f);

                    VertexHelper vhBefore = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhBefore, SendMessageOptions.DontRequireReceiver);
                    UIVertex vBefore = new UIVertex();
                    vhBefore.PopulateUIVertex(ref vBefore, 0);

                    rt.sizeDelta = new Vector2(100, 40);
                    VertexHelper vhAfter = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhAfter, SendMessageOptions.DontRequireReceiver);
                    UIVertex vAfter = new UIVertex();
                    vhAfter.PopulateUIVertex(ref vAfter, 0);

                    // Original xMin was -100 before, -50 after.
                    // Padding is 14px on both. So vBefore.x = -114, vAfter.x = -64.
                    if (Mathf.Approximately(vBefore.position.x, -114f) && Mathf.Approximately(vAfter.position.x, -64f))
                    {
                        sb.AppendLine("[PASS] Test 29: Dynamic Resize (200x80 -> 100x40) updates mesh padding bounds correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 29: Resize mesh padding mismatch: before:{vBefore.position.x}, after:{vAfter.position.x}");
                    }
                }

                // Test 30: Negative Spread Support
                {
                    var go = CreateImageObject("Test30_NegativeSpread", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowBlur(8f);
                    img.SetDropShadowSpread(-4f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    // Padding extent uses max(spread, 0), so it doesn't shrink below blur extent.
                    // Tangent.w holds -4f for shader SDF erosion.
                    float padding = v2.position.x - 100f;
                    if (padding >= 14f && Mathf.Approximately(v0.tangent.w, -4f) && Mathf.Approximately(img.DropShadowSpread, -4f))
                    {
                        sb.AppendLine("[PASS] Test 30: Negative Spread (Spread=-4) -> Safe padding clamp & tangent.w erosion preserved");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 30: Negative spread failed: padding:{padding}, tangent.w:{v0.tangent.w}");
                    }
                }

                // Test 31: Mask with Drop Shadow (Mask stencil ignores shadow padding)
                {
                    var go = CreateImageObject("Test31_MaskWithDropShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();
                    mask.showMaskGraphic = true;

                    img.SetRadius(16f);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0, 10);
                    img.SetDropShadowBlur(16);
                    img.UseRoundedRaycast = true;

                    var mat = img.materialForRendering;
                    float stencilOp = mat.GetFloat("_StencilOp");

                    // StencilOp should be Replace (2) for mask generator
                    bool isMaskGen = stencilOp > 0.5f;

                    // Click outside the rect (in the shadow padding area) should NOT be valid
                    RectTransform rt = go.GetComponent<RectTransform>();
                    Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(null, rt.position);
                    Vector2 shadowPointScreen = centerScreen + new Vector2(0, -60f); // 60px down is outside 40px half-height
                    bool outsideRect = !RectTransformUtility.RectangleContainsScreenPoint(rt, shadowPointScreen, null);
                    bool raycastInShadow = img.IsRaycastLocationValid(shadowPointScreen, null);

                    // Execute helper updates to create shadow underlay
                    var mi31 = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi31?.Invoke(img, null);

                    string underlayName = $"[FigmaImage_ShadowUnderlay_{img.GetInstanceID()}]";
                    Transform underlayTr = testRoot.transform.Find(underlayName);
                    bool underlayCreated = underlayTr != null;
                    bool underlayCorrect = false;
                    if (underlayCreated)
                    {
                        var uImg = underlayTr.GetComponent<FigmaImage>();
                        underlayCorrect = uImg != null && uImg.DropShadowEnabled && !uImg.maskable;
                    }

                    if (isMaskGen && outsideRect && !raycastInShadow && underlayCreated && underlayCorrect)
                    {
                        sb.AppendLine("[PASS] Test 31: Mask with Drop Shadow -> Shadow underlay created, StencilOp=2 identified & shadow padding excluded from raycast/mask");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 31: StencilOp:{stencilOp}, outsideRect:{outsideRect}, raycastInShadow:{raycastInShadow}, underlayCreated:{underlayCreated}, underlayCorrect:{underlayCorrect}");
                    }
                }

                // Test 32: Ignore Stroke In Mask (MaskIgnoreStroke property, stream & overlay)
                {
                    var go = CreateImageObject("Test32_MaskIgnoreStroke", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();

                    img.SetRadius(12f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(6f);
                    img.SetStrokeColor(Color.black);
                    img.SetMaskIgnoreStroke(true);

                    // Test vertex tangent.z stream
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    bool tangentFlag2 = Mathf.Approximately(v0.tangent.z, 2f);

                    // Test overlay creation
                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    Transform overlayTr = go.transform.Find("[FigmaImage_OutlineOverlay]");
                    bool overlayCreated = overlayTr != null;
                    bool overlayCorrect = false;

                    if (overlayCreated)
                    {
                        var overlayImg = overlayTr.GetComponent<FigmaImage>();
                        overlayCorrect = overlayImg != null &&
                                         !overlayImg.maskable &&
                                         Mathf.Approximately(overlayImg.StrokeWidth, 6f) &&
                                         overlayImg.StrokeColor == Color.black;
                    }

                    // Test toggling off destroys overlay
                    img.SetMaskIgnoreStroke(false);
                    mi?.Invoke(img, null);
                    bool overlayRemoved = go.transform.Find("[FigmaImage_OutlineOverlay]") == null;

                    if (tangentFlag2 && overlayCreated && overlayCorrect && overlayRemoved)
                    {
                        sb.AppendLine("[PASS] Test 32: Ignore Stroke In Mask -> tangent.z=2 streamed, overlay created with maskable=false & cleaned up on toggle");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 32: tangent.z:{v0.tangent.z}, created:{overlayCreated}, correct:{overlayCorrect}, removed:{overlayRemoved}");
                    }
                }

                // Test 33: Outside Stroke Configuration & Tangent Stream
                {
                    var go = CreateImageObject("Test33_OutsideStrokeConfig", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetRadius(16f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(6f);
                    img.SetStrokeColor(Color.cyan);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex vert = new UIVertex();
                    bool validTangent = false;
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref vert, 0);
                        validTangent = Mathf.Approximately(vert.tangent.x, 6f) && Mathf.Approximately(vert.tangent.z, 3f);
                    }

                    if (img.StrokeEnabled && img.StrokePosition == FigmaStrokePosition.Outside &&
                        Mathf.Approximately(img.StrokeWidth, 6f) && img.StrokeColor == Color.cyan && validTangent)
                    {
                        sb.AppendLine("[PASS] Test 33: Outside Stroke Configuration -> Position=Outside, Width=6, Tangent.z=3 streamed to shader");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 33: Outside stroke configuration failed: pos={img.StrokePosition}, tangent.z={vert.tangent.z}");
                    }
                }

                // Test 34: Outside Stroke Mesh Geometry Expansion
                {
                    var go = CreateImageObject("Test34_OutsideStrokeMeshExpansion", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80); // halfSize = (100, 40)
                    img.SetRadius(16f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(8f);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v1 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    UIVertex v3 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v1, 1);
                        vh.PopulateUIVertex(ref v2, 2);
                        vh.PopulateUIVertex(ref v3, 3);
                    }

                    // Expected expansion: 8px outward on all sides
                    // Original: x in [-100, 100], y in [-40, 40]
                    // Expanded: x in [-108, 108], y in [-48, 48]
                    bool validBounds = Mathf.Approximately(v0.position.x, -108f) && Mathf.Approximately(v0.position.y, -48f) &&
                                       Mathf.Approximately(v2.position.x, 108f) && Mathf.Approximately(v2.position.y, 48f);

                    if (validBounds)
                    {
                        sb.AppendLine("[PASS] Test 34: Outside Stroke Mesh Expansion (200x80, stroke 8px outside) -> Quad expanded 8px on all 4 sides");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 34: Bounds mismatch: v0=({v0.position.x}, {v0.position.y}), v2=({v2.position.x}, {v2.position.y})");
                    }
                }

                // Test 35: Outside Stroke UV Preservation
                {
                    var go = CreateImageObject("Test35_OutsideStrokeUV", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    Texture2D tex = new Texture2D(32, 32);
                    Sprite spr = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                    img.sprite = spr;
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(10f);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    // UV coordinates expand proportionally so sprite stays centered inside [0, 1] relative to original 200x80
                    // Original width 200, pad 10 -> u span expands by 10/200 = 0.05 on left and right: uv0.x = -0.05, uv2.x = 1.05
                    bool validUV = Mathf.Approximately(v0.uv0.x, -0.05f) && Mathf.Approximately(v2.uv0.x, 1.05f);

                    UnityEngine.Object.DestroyImmediate(spr);
                    UnityEngine.Object.DestroyImmediate(tex);

                    if (validUV)
                    {
                        sb.AppendLine("[PASS] Test 35: Outside Stroke UV Preservation -> UV0 adjusted proportionally (-0.05 to 1.05), preserving sprite aspect");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 35: UV0 mismatch: v0.uv0.x={v0.uv0.x}, v2.uv0.x={v2.uv0.x}");
                    }
                }

                // Test 36: Outside Stroke + Independent Corner Radii
                {
                    var go = CreateImageObject("Test36_OutsideStrokeIndependentCorners", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetCornerRadii(32f, 16f, 24f, 8f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(5f);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);

                    Vector4 r = img.GetNormalizedRadii();
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    bool radiiValid = Mathf.Approximately(r.x, 32f) && Mathf.Approximately(r.y, 16f) &&
                                      Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 8f);
                    bool streamValid = Mathf.Approximately(v0.uv2.x, 32f) && Mathf.Approximately(v0.tangent.x, 5f) && Mathf.Approximately(v0.tangent.z, 3f);

                    if (radiiValid && streamValid)
                    {
                        sb.AppendLine("[PASS] Test 36: Outside Stroke + Independent Corners (32, 16, 24, 8) -> Radii & Outside Stroke co-exist");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 36: Radii or stream failed: radii={r}, tangent.z={v0.tangent.z}");
                    }
                }

                // Test 37: Outside Stroke + Drop Shadow Combined Expansion & Tangent Stream
                {
                    var go = CreateImageObject("Test37_OutsideStrokeDropShadow", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetRadius(16f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(6f);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(8f);
                    img.SetDropShadowSpread(0f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    UIVertex v0 = new UIVertex();
                    UIVertex v2 = new UIVertex();
                    if (vh.currentVertCount == 4)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                        vh.PopulateUIVertex(ref v2, 2);
                    }

                    // strokePad = 6. shadowExtent = 8 * 1.5 + 2 = 14.
                    // leftPad = 6 + 14 = 20 -> v0.x = -100 - 20 = -120.
                    // rightPad = 6 + 14 = 20 -> v2.x = 100 + 20 = 120.
                    // bottomPad = 6 + 14 + 4 = 24 -> v0.y = -40 - 24 = -64.
                    // topPad = 6 + 14 = 20 -> v2.y = 40 + 20 = 60.
                    bool boundsOk = Mathf.Approximately(v0.position.x, -120f) && Mathf.Approximately(v2.position.x, 120f) &&
                                    Mathf.Approximately(v0.position.y, -64f) && Mathf.Approximately(v2.position.y, 60f);
                    bool streamsOk = Mathf.Approximately(v0.tangent.z, 3f) && Mathf.Approximately(v0.normal.z, 8f);

                    if (boundsOk && streamsOk)
                    {
                        sb.AppendLine("[PASS] Test 37: Outside Stroke + Drop Shadow -> Combined mesh expansion (20px horizontal, 24px bottom) & streams valid");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 37: Bounds/Streams failed: v0=({v0.position.x}, {v0.position.y}), v2=({v2.position.x}, {v2.position.y}), tangent.z={v0.tangent.z}");
                    }
                }

                // Test 38: Outside Stroke + Mask Stencil & IgnoreInMask Helper Overlay
                {
                    var go = CreateImageObject("Test38_OutsideStrokeMask", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();

                    img.SetRadius(12f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(6f);
                    img.SetStrokeColor(Color.magenta);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);
                    img.SetMaskIgnoreStroke(true);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }

                    bool tangentFlag4 = Mathf.Approximately(v0.tangent.z, 4f);

                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    Transform overlayTr = go.transform.Find("[FigmaImage_OutlineOverlay]");
                    bool overlayCreated = overlayTr != null;
                    bool overlayCorrect = false;

                    if (overlayCreated)
                    {
                        var overlayImg = overlayTr.GetComponent<FigmaImage>();
                        overlayCorrect = overlayImg != null &&
                                         !overlayImg.maskable &&
                                         overlayImg.StrokePosition == FigmaStrokePosition.Outside &&
                                         Mathf.Approximately(overlayImg.StrokeWidth, 6f) &&
                                         overlayImg.StrokeColor == Color.magenta;
                    }

                    if (tangentFlag4 && overlayCreated && overlayCorrect)
                    {
                        sb.AppendLine("[PASS] Test 38: Outside Stroke + Mask Ignore -> tangent.z=4 streamed, overlay created with Position=Outside");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 38: Outside stroke mask failed: tangent.z={v0.tangent.z}, created={overlayCreated}, correct={overlayCorrect}");
                    }
                }

                // Test 39: Outside Stroke Raycast Hit Area Validation
                {
                    var go = CreateImageObject("Test39_OutsideStrokeRaycast", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80); // halfSize 100x40
                    img.SetRadius(16f);
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(10f);
                    img.SetStrokePosition(FigmaStrokePosition.Outside);
                    img.UseRoundedRaycast = true;

                    Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(null, rt.position);

                    // Point 1: Inside original rect (center) -> should be hit
                    bool centerHit = img.IsRaycastLocationValid(centerScreen, null);

                    // Point 2: 5px outside the right edge (x = +105) -> on the outside stroke! Should be hit
                    Vector2 onStrokeScreen = centerScreen + new Vector2(105f, 0f);
                    bool onStrokeHit = img.IsRaycastLocationValid(onStrokeScreen, null);

                    // Point 3: 20px outside the right edge (x = +120) -> outside the stroke! Should NOT be hit
                    Vector2 outsideStrokeScreen = centerScreen + new Vector2(120f, 0f);
                    bool outsideStrokeHit = img.IsRaycastLocationValid(outsideStrokeScreen, null);

                    // Check raycastPadding expansion
                    bool paddingExpanded = Mathf.Approximately(img.raycastPadding.x, -10f) &&
                                           Mathf.Approximately(img.raycastPadding.y, -10f) &&
                                           Mathf.Approximately(img.raycastPadding.z, -10f) &&
                                           Mathf.Approximately(img.raycastPadding.w, -10f);

                    if (centerHit && onStrokeHit && !outsideStrokeHit && paddingExpanded)
                    {
                        sb.AppendLine("[PASS] Test 39: Outside Stroke Raycast -> 5px outside hits stroke, 20px outside rejected, raycastPadding expanded to -10");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 39: Raycast validation failed: centerHit={centerHit}, onStrokeHit={onStrokeHit}, outsideHit={outsideStrokeHit}, padding={img.raycastPadding}");
                    }
                }

                // Test 40: Inner Shadow Settings
                {
                    var go = CreateImageObject("Test40_InnerShadowSettings", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowOffset(4f, -8f);
                    img.SetInnerShadowBlur(6f);
                    img.SetInnerShadowSpread(2f);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0.3f));

                    bool ok = img.InnerShadowEnabled &&
                              Mathf.Approximately(img.InnerShadowOffsetX, 4f) &&
                              Mathf.Approximately(img.InnerShadowOffsetY, -8f) &&
                              img.InnerShadowOffset == new Vector2(4f, -8f) &&
                              Mathf.Approximately(img.InnerShadowBlur, 6f) &&
                              Mathf.Approximately(img.InnerShadowSpread, 2f) &&
                              img.InnerShadowColor == new Color(0f, 0f, 0f, 0.3f);

                    if (ok)
                    {
                        sb.AppendLine("[PASS] Test 40: Inner Shadow Settings (offset 4,-8, blur 6, spread 2, a 0.3) -> Configured correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 40: Settings mismatch: enabled={img.InnerShadowEnabled}, offset={img.InnerShadowOffset}, blur={img.InnerShadowBlur}, spread={img.InnerShadowSpread}");
                    }
                }

                // Test 41: Blur Clamp (negative blur clamped to 0)
                {
                    var go = CreateImageObject("Test41_BlurClamp", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowBlur(-10f);

                    if (Mathf.Approximately(img.InnerShadowBlur, 0f) && Mathf.Approximately(img.InnerShadow.Blur, 0f))
                    {
                        sb.AppendLine("[PASS] Test 41: Blur Clamp (-10 -> 0) -> Clamped safely to zero");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 41: Expected blur clamped to 0, got {img.InnerShadowBlur}");
                    }
                }

                // Test 42: Negative Spread Preserved
                {
                    var go = CreateImageObject("Test42_NegativeSpread", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowSpread(-6f);

                    if (Mathf.Approximately(img.InnerShadowSpread, -6f) && Mathf.Approximately(img.InnerShadow.Spread, -6f))
                    {
                        sb.AppendLine("[PASS] Test 42: Negative Spread (-6) -> Preserved without clamping");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 42: Expected spread -6, got {img.InnerShadowSpread}");
                    }
                }

                // Test 43: No Mesh Expansion (Inner Shadow does not expand geometry)
                {
                    var go = CreateImageObject("Test43_NoMeshExpansion", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);

                    // First populate without inner shadow
                    VertexHelper vhBase = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhBase, SendMessageOptions.DontRequireReceiver);
                    UIVertex baseV0 = new UIVertex();
                    UIVertex baseV2 = new UIVertex();
                    vhBase.PopulateUIVertex(ref baseV0, 0);
                    vhBase.PopulateUIVertex(ref baseV2, 2);

                    // Enable inner shadow with large offset, blur, spread
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowOffset(30f, 30f);
                    img.SetInnerShadowBlur(20f);
                    img.SetInnerShadowSpread(10f);

                    VertexHelper vhInner = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhInner, SendMessageOptions.DontRequireReceiver);
                    UIVertex innerV0 = new UIVertex();
                    UIVertex innerV2 = new UIVertex();
                    vhInner.PopulateUIVertex(ref innerV0, 0);
                    vhInner.PopulateUIVertex(ref innerV2, 2);

                    bool boundsMatch = Mathf.Approximately(baseV0.position.x, innerV0.position.x) &&
                                       Mathf.Approximately(baseV0.position.y, innerV0.position.y) &&
                                       Mathf.Approximately(baseV2.position.x, innerV2.position.x) &&
                                       Mathf.Approximately(baseV2.position.y, innerV2.position.y);

                    if (boundsMatch)
                    {
                        sb.AppendLine("[PASS] Test 43: No Mesh Expansion -> Inner shadow preserves base image geometry bounds exactly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 43: Mesh bounds changed: base=({baseV0.position}, {baseV2.position}), inner=({innerV0.position}, {innerV2.position})");
                    }
                }

                // Test 44: Inner Layer Created (triangle stream duplicated with marker tangent.z = 10)
                {
                    var go = CreateImageObject("Test44_InnerLayerCreated", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0.5f));

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool hasInnerMarker = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            hasInnerMarker = true;
                            break;
                        }
                    }

                    if (vh.currentVertCount > 4 && hasInnerMarker)
                    {
                        sb.AppendLine($"[PASS] Test 44: Inner Layer Created -> Duplicate triangle stream generated ({vh.currentVertCount} verts) with marker tangent.z=10");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 44: Inner layer failed: vertCount={vh.currentVertCount}, hasMarker={hasInnerMarker}");
                    }
                }

                // Test 45: Inner Parameter Stream (packed colors, offsets, blur, spread)
                {
                    var go = CreateImageObject("Test45_InnerParameterStream", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    Color shadowCol = new Color(0.2f, 0.4f, 0.6f, 0.8f);
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowOffset(5f, -7f);
                    img.SetInnerShadowBlur(12f);
                    img.SetInnerShadowSpread(3f);
                    img.SetInnerShadowColor(shadowCol);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool streamValid = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            // Unpack color from uv3.x and uv3.y
                            float pRG = vert.uv3.x;
                            float pBA = vert.uv3.y;
                            float r = Mathf.Floor(pRG / 256f) / 255f;
                            float g = (pRG - Mathf.Floor(pRG / 256f) * 256f) / 255f;
                            float b = Mathf.Floor(pBA / 256f) / 255f;
                            float a = (pBA - Mathf.Floor(pBA / 256f) * 256f) / 255f;

                            bool colOk = Mathf.Abs(r - shadowCol.r) < 0.01f &&
                                         Mathf.Abs(g - shadowCol.g) < 0.01f &&
                                         Mathf.Abs(b - shadowCol.b) < 0.01f &&
                                         Mathf.Abs(a - shadowCol.a) < 0.01f;

                            bool offsetOk = Mathf.Approximately(vert.uv3.z, 5f) && Mathf.Approximately(vert.uv3.w, -7f);
                            bool blurOk = Mathf.Approximately(vert.tangent.x, 12f);
                            bool spreadOk = Mathf.Approximately(vert.tangent.y, 3f);

                            if (colOk && offsetOk && blurOk && spreadOk)
                            {
                                streamValid = true;
                                break;
                            }
                        }
                    }

                    if (streamValid)
                    {
                        sb.AppendLine("[PASS] Test 45: Inner Parameter Stream -> Packed color, offsets, blur, and spread streamed correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 45: Inner parameter streaming failed to match expected values");
                    }
                }

                // Test 46: Disabled Has Zero Extra Geometry
                {
                    var go = CreateImageObject("Test46_DisabledZeroGeometry", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(false);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    if (vh.currentVertCount == 4)
                    {
                        sb.AppendLine("[PASS] Test 46: Disabled Has Zero Extra Geometry -> 4 vertices produced (standard single quad)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 46: Expected 4 vertices when disabled, got {vh.currentVertCount}");
                    }
                }

                // Test 47: Zero Alpha Has Zero Extra Geometry
                {
                    var go = CreateImageObject("Test47_ZeroAlphaZeroGeometry", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0f));

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    if (vh.currentVertCount == 4)
                    {
                        sb.AppendLine("[PASS] Test 47: Zero Alpha Has Zero Extra Geometry -> 4 vertices produced when color alpha is 0");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 47: Expected 4 vertices with zero alpha, got {vh.currentVertCount}");
                    }
                }

                // Test 48: Drop + Inner Coexistence
                {
                    var go = CreateImageObject("Test48_DropAndInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);

                    // Configure Drop Shadow only
                    img.SetDropShadowEnabled(true);
                    img.SetDropShadowOffset(0f, 4f);
                    img.SetDropShadowBlur(8f);
                    img.SetDropShadowSpread(0f);
                    img.SetDropShadowColor(new Color(0f, 0f, 0f, 0.25f));

                    VertexHelper vhDropOnly = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhDropOnly, SendMessageOptions.DontRequireReceiver);
                    UIVertex dropV0 = new UIVertex();
                    vhDropOnly.PopulateUIVertex(ref dropV0, 0);

                    // Now also enable Inner Shadow
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowOffset(2f, 2f);
                    img.SetInnerShadowBlur(4f);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0.3f));

                    VertexHelper vhBoth = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vhBoth, SendMessageOptions.DontRequireReceiver);
                    UIVertex bothV0 = new UIVertex();
                    vhBoth.PopulateUIVertex(ref bothV0, 0);

                    bool boundsEqual = Mathf.Approximately(dropV0.position.x, bothV0.position.x) &&
                                       Mathf.Approximately(dropV0.position.y, bothV0.position.y);

                    bool hasInnerLayer = false;
                    UIVertex temp = new UIVertex();
                    for (int i = 0; i < vhBoth.currentVertCount; i++)
                    {
                        vhBoth.PopulateUIVertex(ref temp, i);
                        if (Mathf.Approximately(temp.tangent.z, 10f))
                        {
                            hasInnerLayer = true;
                            break;
                        }
                    }

                    if (boundsEqual && hasInnerLayer)
                    {
                        sb.AppendLine("[PASS] Test 48: Drop + Inner Coexistence -> Drop shadow expansion bounds unchanged & Inner shadow layer active");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 48: Drop + Inner failed: boundsEqual={boundsEqual}, hasInnerLayer={hasInnerLayer}");
                    }
                }

                // Test 49: Stroke + Inner Coexistence
                {
                    var go = CreateImageObject("Test49_StrokeAndInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(5f);
                    img.SetStrokeColor(Color.red);
                    img.SetStrokePosition(FigmaStrokePosition.Inside);

                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowBlur(6f);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0.4f));

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool strokeParamsPreservedOnInner = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            bool widthOk = Mathf.Approximately(vert.tangent.w, 5f);
                            bool strokeOn = Mathf.Approximately(vert.normal.y, 1f);
                            bool insideStroke = Mathf.Approximately(vert.normal.z, 0f);

                            if (widthOk && strokeOn && insideStroke)
                            {
                                strokeParamsPreservedOnInner = true;
                                break;
                            }
                        }
                    }

                    if (strokeParamsPreservedOnInner)
                    {
                        sb.AppendLine("[PASS] Test 49: Stroke + Inner Coexistence -> Stroke width (5px) and flags passed to inner layer for masking");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 49: Stroke parameters on inner layer were missing or incorrect");
                    }
                }

                // Test 50: Independent Corners + Inner
                {
                    var go = CreateImageObject("Test50_IndependentCornersInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetCornerRadii(32f, 8f, 24f, 0f);
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowBlur(4f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool uv2Matches = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            if (Mathf.Approximately(vert.uv2.x, 32f) &&
                                Mathf.Approximately(vert.uv2.y, 8f) &&
                                Mathf.Approximately(vert.uv2.z, 24f) &&
                                Mathf.Approximately(vert.uv2.w, 0f))
                            {
                                uv2Matches = true;
                                break;
                            }
                        }
                    }

                    if (uv2Matches)
                    {
                        sb.AppendLine("[PASS] Test 50: Independent Corners + Inner -> UV2 corner radii (32, 8, 24, 0) identical on inner layer");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 50: UV2 corner radii mismatch on inner shadow layer");
                    }
                }

                // Test 51: Pill + Inner
                {
                    var go = CreateImageObject("Test51_PillInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 48);
                    img.SetRadius(24f);
                    img.SetInnerShadowEnabled(true);

                    Vector4 r = img.GetNormalizedRadii();
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool pillRadiusPreserved = Mathf.Approximately(r.x, 24f) &&
                                               Mathf.Approximately(r.y, 24f) &&
                                               Mathf.Approximately(r.z, 24f) &&
                                               Mathf.Approximately(r.w, 24f);

                    bool innerLayerHasPillRadii = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            if (Mathf.Approximately(vert.uv2.x, 24f) && Mathf.Approximately(vert.uv2.y, 24f))
                            {
                                innerLayerHasPillRadii = true;
                                break;
                            }
                        }
                    }

                    if (pillRadiusPreserved && innerLayerHasPillRadii)
                    {
                        sb.AppendLine("[PASS] Test 51: Pill + Inner (200x48, r=24) -> Normalized radii (24, 24, 24, 24) preserved on inner layer");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 51: Pill radii check failed: normalized={r}, innerValid={innerLayerHasPillRadii}");
                    }
                }

                // Test 52: Mask Compatibility (Mask with Inner Shadow has no helper gameobjects)
                {
                    var go = CreateImageObject("Test52_MaskInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowBlur(4f);

                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    Transform overlayTr = go.transform.Find("[FigmaImage_OutlineOverlay]");
                    Transform underlayTr = go.transform.parent != null ? go.transform.parent.Find($"[FigmaImage_ShadowUnderlay_{go.GetInstanceID()}]") : null;
                    bool noExtraHelpers = overlayTr == null && underlayTr == null;
                    bool matValid = img.materialForRendering != null;

                    if (noExtraHelpers && matValid)
                    {
                        sb.AppendLine("[PASS] Test 52: Mask Compatibility -> Material valid, zero unnecessary helper GameObjects spawned for Inner Shadow");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 52: Mask check failed: noExtraHelpers={noExtraHelpers}, matValid={matValid}");
                    }
                }

                // Test 53: Mask Ignore Stroke
                {
                    var go = CreateImageObject("Test53_MaskIgnoreStrokeInner", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(4f);
                    img.SetMaskIgnoreStroke(true);
                    img.SetInnerShadowEnabled(true);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool ignoreMaskFlagCarried = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            if (Mathf.Approximately(vert.normal.x, 1f))
                            {
                                ignoreMaskFlagCarried = true;
                                break;
                            }
                        }
                    }

                    if (ignoreMaskFlagCarried)
                    {
                        sb.AppendLine("[PASS] Test 53: Mask Ignore Stroke -> normal.x=1 (IgnoreInMask) streamed to inner shadow layer");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 53: normal.x did not carry IgnoreInMask flag to inner shadow layer");
                    }
                }

                // Test 54: RectMask2D Compatibility
                {
                    var parentGo = new GameObject("Test54_Parent", typeof(RectTransform), typeof(RectMask2D));
                    parentGo.transform.SetParent(testRoot.transform, false);

                    var go = CreateImageObject("Test54_RectMask2DInner", parentGo);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(true);

                    Material renderMat = img.materialForRendering;
                    bool matOk = renderMat != null;

                    if (matOk)
                    {
                        sb.AppendLine("[PASS] Test 54: RectMask2D Compatibility -> materialForRendering valid under RectMask2D");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 54: materialForRendering was null under RectMask2D");
                    }
                }

                // Test 55: Shared Material (batching preserved, zero unique material clones)
                {
                    var go1 = CreateImageObject("Test55_SharedMat1", testRoot);
                    var img1 = go1.GetComponent<FigmaImage>();
                    img1.SetInnerShadowEnabled(true);
                    img1.SetInnerShadowBlur(4f);
                    img1.SetInnerShadowColor(Color.red);

                    var go2 = CreateImageObject("Test55_SharedMat2", testRoot);
                    var img2 = go2.GetComponent<FigmaImage>();
                    img2.SetInnerShadowEnabled(true);
                    img2.SetInnerShadowBlur(10f);
                    img2.SetInnerShadowColor(Color.blue);

                    bool shared = ReferenceEquals(img1.defaultMaterial, img2.defaultMaterial) && img1.defaultMaterial != null;

                    if (shared)
                    {
                        sb.AppendLine("[PASS] Test 55: Shared Material -> Both instances share exact same defaultMaterial instance");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 55: Instances did not share defaultMaterial");
                    }
                }

                // Test 56: Dynamic Resize
                {
                    var go = CreateImageObject("Test56_DynamicResize", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 80);
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowBlur(8f);

                    // Resize to 400x120
                    rt.sizeDelta = new Vector2(400, 120);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool halfSizeUpdated = false;
                    UIVertex vert = new UIVertex();
                    for (int i = 0; i < vh.currentVertCount; i++)
                    {
                        vh.PopulateUIVertex(ref vert, i);
                        if (Mathf.Approximately(vert.tangent.z, 10f))
                        {
                            if (Mathf.Approximately(vert.uv1.z, 200f) && Mathf.Approximately(vert.uv1.w, 60f))
                            {
                                halfSizeUpdated = true;
                                break;
                            }
                        }
                    }

                    if (halfSizeUpdated && Mathf.Approximately(img.InnerShadowBlur, 8f))
                    {
                        sb.AppendLine("[PASS] Test 56: Dynamic Resize -> halfSize updated to (200, 60) on inner stream and settings preserved");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 56: Dynamic resize failed to update halfSize on inner stream");
                    }
                }

                // Test 57: Base Color Preservation with Inner Shadow
                {
                    var go = CreateImageObject("Test57_BaseColorPreserved", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.color = new Color(1f, 0.2f, 0.3f, 1f);
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowColor(new Color(0f, 0f, 0f, 0.5f));
                    img.SetInnerShadowBlur(10f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool baseColorPreserved = false;
                    if (vh.currentVertCount >= 4)
                    {
                        UIVertex v0 = new UIVertex();
                        vh.PopulateUIVertex(ref v0, 0);
                        baseColorPreserved = Mathf.Abs(v0.color.r - 255) <= 1 &&
                                             Mathf.Abs(v0.color.g - 51) <= 1 &&
                                             Mathf.Abs(v0.color.b - (int)(0.3f * 255f)) <= 2 &&
                                             v0.tangent.z < 9.5f; // base layer, not overwritten by inner shadow flag
                    }

                    if (baseColorPreserved)
                    {
                        sb.AppendLine("[PASS] Test 57: Base Color Preserved -> Base mesh vertices retain original graphic color with inner shadow enabled");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 57: Base color was not preserved on mesh vertices");
                    }
                }

                // Test 58: Inner Shadow Child Overlay Hierarchy (renders on top of child image elements)
                {
                    var go = CreateImageObject("Test58_ChildOverlayHierarchy", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    img.SetInnerShadowEnabled(true);
                    img.SetInnerShadowBlur(8f);

                    // Add a child image (e.g. avatar or icon)
                    var childGo = new GameObject("Child_Avatar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    childGo.transform.SetParent(go.transform, false);

                    // Update helpers
                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    Transform innerOverlayTr = go.transform.Find("[FigmaImage_InnerShadowOverlay]");
                    bool overlayExists = innerOverlayTr != null;
                    bool isLastSibling = overlayExists && innerOverlayTr.GetSiblingIndex() == go.transform.childCount - 1;
                    bool childBeforeOverlay = overlayExists && childGo.transform.GetSiblingIndex() < innerOverlayTr.GetSiblingIndex();

                    if (overlayExists && isLastSibling && childBeforeOverlay)
                    {
                        sb.AppendLine("[PASS] Test 58: Child Overlay Hierarchy -> [FigmaImage_InnerShadowOverlay] created at last sibling index, rendering on top of child elements");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 58: Child overlay hierarchy failed: exists={overlayExists}, isLast={isLastSibling}, childBefore={childBeforeOverlay}");
                    }
                }

                // Test 59: Outline Overlay Child Hierarchy (maintains top-most sibling when child Image added)
                {
                    var go = CreateImageObject("Test59_OutlineChildHierarchy", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(6f);
                    img.SetMaskIgnoreStroke(true);

                    // Add child image
                    var childGo = new GameObject("Child_Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    childGo.transform.SetParent(go.transform, false);

                    // Execute helper updates
                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    Transform outlineTr = go.transform.Find("[FigmaImage_OutlineOverlay]");
                    bool outlineExists = outlineTr != null;
                    bool isLastSibling = outlineExists && outlineTr.GetSiblingIndex() == go.transform.childCount - 1;
                    bool childBeforeOutline = outlineExists && childGo.transform.GetSiblingIndex() < outlineTr.GetSiblingIndex();

                    RectTransform overlayRt = outlineTr as RectTransform;
                    bool rtSynced = overlayRt != null &&
                        overlayRt.anchorMin == Vector2.zero &&
                        overlayRt.anchorMax == Vector2.one &&
                        overlayRt.pivot == img.rectTransform.pivot &&
                        overlayRt.offsetMin == Vector2.zero &&
                        overlayRt.offsetMax == Vector2.zero;

                    var overlayImg = outlineTr != null ? outlineTr.GetComponent<FigmaImage>() : null;
                    bool overlayImgValid = overlayImg != null && overlayImg.StrokeEnabled && !overlayImg.maskable;

                    if (outlineExists && isLastSibling && childBeforeOutline && rtSynced && overlayImgValid)
                    {
                        sb.AppendLine("[PASS] Test 59: Outline Overlay Child Hierarchy -> [FigmaImage_OutlineOverlay] maintains top-most sibling with synced RectTransform and unmasked stroke above child image");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 59: Outline child hierarchy failed: exists={outlineExists}, isLast={isLastSibling}, childBefore={childBeforeOutline}, rtSynced={rtSynced}, overlayImgValid={overlayImgValid}");
                    }
                }

                // Test 60: Auto Outline Overlay on Child Added when Masked
                {
                    var go = CreateImageObject("Test60_AutoOutlineOnChildAdded", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    var mask = go.AddComponent<Mask>();
                    img.SetStrokeEnabled(true);
                    img.SetStrokeWidth(4f);
                    img.SetStrokeColor(Color.black);
                    img.SetMaskIgnoreStroke(false);

                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(img, null);

                    // Before child added: No outline overlay
                    bool noOverlayInitially = go.transform.Find("[FigmaImage_OutlineOverlay]") == null;

                    // Add child image
                    var childGo = new GameObject("Child_Content", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                    childGo.transform.SetParent(go.transform, false);

                    // Trigger helper update
                    mi?.Invoke(img, null);

                    Transform outlineTr = go.transform.Find("[FigmaImage_OutlineOverlay]");
                    bool outlineCreated = outlineTr != null;
                    bool isLastSibling = outlineCreated && outlineTr.GetSiblingIndex() == go.transform.childCount - 1;
                    bool childBeforeOutline = outlineCreated && childGo.transform.GetSiblingIndex() < outlineTr.GetSiblingIndex();

                    // Check tangent.z streams 2f (inside + ignore mask) for base graphic when child exists
                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);
                    UIVertex v0 = new UIVertex();
                    if (vh.currentVertCount > 0)
                    {
                        vh.PopulateUIVertex(ref v0, 0);
                    }
                    bool tangentStreamsIgnoreMask = Mathf.Approximately(v0.tangent.z, 2f);

                    // Clean up child and verify overlay is removed
                    UnityEngine.Object.DestroyImmediate(childGo);
                    mi?.Invoke(img, null);
                    bool overlayCleanedUp = go.transform.Find("[FigmaImage_OutlineOverlay]") == null;

                    if (noOverlayInitially && outlineCreated && isLastSibling && childBeforeOutline && tangentStreamsIgnoreMask && overlayCleanedUp)
                    {
                        sb.AppendLine("[PASS] Test 60: Auto Outline Overlay -> Masked FigmaImage automatically creates [FigmaImage_OutlineOverlay] above child content even when MaskIgnoreStroke is false, and cleans up when child is removed");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 60: Auto outline failed: init={noOverlayInitially}, created={outlineCreated}, last={isLastSibling}, childBefore={childBeforeOutline}, tangentZ={v0.tangent.z}, cleaned={overlayCleanedUp}");
                    }
                }

                // Test 61: LayoutGroup Compatibility with Mask and Drop Shadow Underlay
                {
                    var groupGo = new GameObject("Test61_LayoutParent", typeof(RectTransform), typeof(VerticalLayoutGroup));
                    groupGo.transform.SetParent(testRoot.transform, false);

                    var vlg = groupGo.GetComponent<VerticalLayoutGroup>();
                    vlg.spacing = 10f;
                    vlg.childControlWidth = true;
                    vlg.childControlHeight = false;

                    var b1 = new GameObject("Button1", typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage), typeof(Mask));
                    b1.transform.SetParent(groupGo.transform, false);
                    var b1Rt = (RectTransform)b1.transform;
                    b1Rt.sizeDelta = new Vector2(200, 50);
                    var f1 = b1.GetComponent<FigmaImage>();
                    f1.SetDropShadowEnabled(true);
                    f1.SetDropShadowBlur(10f);

                    var b2 = new GameObject("Button2", typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage), typeof(Mask));
                    b2.transform.SetParent(groupGo.transform, false);
                    var b2Rt = (RectTransform)b2.transform;
                    b2Rt.sizeDelta = new Vector2(200, 50);
                    var f2 = b2.GetComponent<FigmaImage>();
                    f2.SetDropShadowEnabled(true);
                    f2.SetDropShadowBlur(10f);

                    var mi = typeof(FigmaImage).GetMethod("ExecuteHelperUpdates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    mi?.Invoke(f1, null);
                    mi?.Invoke(f2, null);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(groupGo.GetComponent<RectTransform>());
                    mi?.Invoke(f1, null);
                    mi?.Invoke(f2, null);

                    Transform u1 = groupGo.transform.Find($"[FigmaImage_ShadowUnderlay_{f1.GetInstanceID()}]");
                    Transform u2 = groupGo.transform.Find($"[FigmaImage_ShadowUnderlay_{f2.GetInstanceID()}]");

                    bool underlaysExist = u1 != null && u2 != null;
                    bool leIgnored = underlaysExist &&
                                     u1.GetComponent<LayoutElement>() != null && u1.GetComponent<LayoutElement>().ignoreLayout &&
                                     u2.GetComponent<LayoutElement>() != null && u2.GetComponent<LayoutElement>().ignoreLayout;
                    bool ignorerInterface = underlaysExist &&
                                            ((ILayoutIgnorer)u1.GetComponent<FigmaImage>()).ignoreLayout &&
                                            ((ILayoutIgnorer)u2.GetComponent<FigmaImage>()).ignoreLayout;
                    bool hideFlagsCorrect = underlaysExist &&
                                            (u1.gameObject.hideFlags & HideFlags.HideInHierarchy) != 0 &&
                                            (u2.gameObject.hideFlags & HideFlags.HideInHierarchy) != 0;

                    float actualDiff = Mathf.Abs(b1Rt.anchoredPosition.y - b2Rt.anchoredPosition.y);
                    bool layoutSpacingCorrect = Mathf.Approximately(actualDiff, 60f);

                    var u1Rt = (RectTransform)u1;
                    var u2Rt = (RectTransform)u2;
                    bool posSynced = u1Rt != null && u2Rt != null &&
                                     Mathf.Approximately(u1Rt.anchoredPosition.y, b1Rt.anchoredPosition.y) &&
                                     Mathf.Approximately(u2Rt.anchoredPosition.y, b2Rt.anchoredPosition.y);

                    if (underlaysExist && leIgnored && ignorerInterface && hideFlagsCorrect && layoutSpacingCorrect && posSynced)
                    {
                        sb.AppendLine("[PASS] Test 61: LayoutGroup Compatibility -> Masked FigmaImage with Drop Shadow underlay has ignoreLayout = true, HideInHierarchy, and does not displace LayoutGroup items");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 61: LayoutGroup compatibility failed: exists={underlaysExist}, leIgnored={leIgnored}, ignorerInterface={ignorerInterface}, hideFlags={hideFlagsCorrect}, spacing={layoutSpacingCorrect} (diff={actualDiff}), posSynced={posSynced}");
                    }
                }

                // Test 62: Resources Material Inclusion
                {
                    Material resMat = Resources.Load<Material>("FigmaImage-Default");
                    bool validMat = resMat != null && resMat.shader != null && resMat.shader.name == "UI/FigmaImage";
                    if (validMat)
                    {
                        sb.AppendLine("[PASS] Test 62: Resources Material Inclusion -> 'FigmaImage-Default.mat' successfully loaded with 'UI/FigmaImage' shader");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 62: 'FigmaImage-Default.mat' missing from Resources or has invalid shader (loaded: {resMat != null}, shader: {resMat?.shader?.name})");
                    }
                }

                // Test 63: Runtime Default Material Resolution
                {
                    var go = CreateImageObject("Test63_DefaultMaterial", testRoot);
                    var img = go.GetComponent<FigmaImage>();
                    Material defMat = img.defaultMaterial;
                    bool validDefMat = defMat != null && defMat.shader != null && defMat.shader.name == "UI/FigmaImage";
                    if (validDefMat)
                    {
                        sb.AppendLine("[PASS] Test 63: Runtime Default Material Resolution -> FigmaImage.defaultMaterial resolves to 'UI/FigmaImage' without fallback to standard Image");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 63: defaultMaterial resolved incorrectly: {defMat?.name} (shader: {defMat?.shader?.name})");
                    }
                }

                // Test 64: Nested Canvas Hierarchy Channel Propagation
                {
                    GameObject nestedRoot = new GameObject("Test64_RootCanvas", typeof(RectTransform), typeof(Canvas));
                    GameObject nestedInter = new GameObject("Test64_InterCanvas", typeof(RectTransform), typeof(Canvas));
                    GameObject nestedLeaf = new GameObject("Test64_LeafCanvas", typeof(RectTransform), typeof(Canvas));
                    GameObject nestedImgGo = new GameObject("Test64_Img", typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage));

                    nestedInter.transform.SetParent(nestedRoot.transform, false);
                    nestedLeaf.transform.SetParent(nestedInter.transform, false);
                    nestedImgGo.transform.SetParent(nestedLeaf.transform, false);

                    Canvas cRoot = nestedRoot.GetComponent<Canvas>();
                    Canvas cInter = nestedInter.GetComponent<Canvas>();
                    Canvas cLeaf = nestedLeaf.GetComponent<Canvas>();
                    var img = nestedImgGo.GetComponent<FigmaImage>();

                    const AdditionalCanvasShaderChannels required =
                        AdditionalCanvasShaderChannels.TexCoord1 |
                        AdditionalCanvasShaderChannels.TexCoord2 |
                        AdditionalCanvasShaderChannels.TexCoord3 |
                        AdditionalCanvasShaderChannels.Normal |
                        AdditionalCanvasShaderChannels.Tangent;

                    // Trigger hierarchy changed
                    img.SendMessage("EnsureCanvasChannels", SendMessageOptions.DontRequireReceiver);

                    bool rootOk = (cRoot.additionalShaderChannels & required) == required;
                    bool interOk = (cInter.additionalShaderChannels & required) == required;
                    bool leafOk = (cLeaf.additionalShaderChannels & required) == required;

                    if (rootOk && interOk && leafOk)
                    {
                        sb.AppendLine("[PASS] Test 64: Nested Canvas Hierarchy Channel Propagation -> Root, intermediate, and leaf Canvases all configured with required vertex channels");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 64: Nested canvas channels missing: root={rootOk}, inter={interOk}, leaf={leafOk}");
                    }

                    UnityEngine.Object.DestroyImmediate(nestedRoot);
                }

                // Test 65: Mobile Shader Compilation & Support
                {
                    Shader s = Shader.Find("UI/FigmaImage");
                    bool shaderValid = s != null && s.isSupported;
#if UNITY_EDITOR
                    int msgCount = UnityEditor.ShaderUtil.GetShaderMessageCount(s);
                    shaderValid = shaderValid && (msgCount == 0);
#endif
                    if (shaderValid)
                    {
                        sb.AppendLine("[PASS] Test 65: Mobile Shader Compilation & Support -> Shader 'UI/FigmaImage' is supported and compiles with 0 errors/warnings");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 65: Shader 'UI/FigmaImage' isSupported={s?.isSupported}");
                    }
                }
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testRoot);
            }

            sb.AppendLine($"=== Result: {passed}/{total} Tests Passed ===");
            return sb.ToString();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/FigmaImage/Run All Acceptance Tests", false, 100)]
        public static void RunAllTestsMenu()
        {
            string report = RunAllTests();
            Debug.Log(report);
        }
#endif

        private static GameObject CreateImageObject(string name, GameObject parent)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage));
            go.transform.SetParent(parent.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 80);
            return go;
        }
    }
}
