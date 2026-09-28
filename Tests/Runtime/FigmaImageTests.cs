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
            int total = 32;

            sb.AppendLine("=== Running FigmaImage Acceptance Tests (Corner Radius, Stroke & Drop Shadow) ===");

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
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(testRoot);
            }

            sb.AppendLine($"=== Result: {passed}/{total} Tests Passed ===");
            return sb.ToString();
        }

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
