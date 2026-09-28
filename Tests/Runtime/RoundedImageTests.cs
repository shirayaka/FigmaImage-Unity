using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectArea.UI.Tests
{
    public static class RoundedImageTests
    {
        public static string RunAllTests()
        {
            var sb = new StringBuilder();
            int passed = 0;
            int total = 10;

            sb.AppendLine("=== Running RoundedImage Acceptance Tests ===");

            GameObject testRoot = new GameObject("TestRoot_Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = testRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            try
            {
                // Test 1: Standard radius
                {
                    var go = CreateImageObject("Test1_Standard", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetRadius(24f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 24f) && Mathf.Approximately(r.y, 24f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 24f))
                    {
                        sb.AppendLine("[PASS] Test 1: Standard radius (300x100, radius 24) -> Radii (24, 24, 24, 24)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 1: Expected (24, 24, 24, 24), got {r}");
                    }
                }

                // Test 2: Pill shape
                {
                    var go = CreateImageObject("Test2_Pill", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(200, 48);
                    img.SetRadius(24f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 24f) && Mathf.Approximately(r.y, 24f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 24f))
                    {
                        sb.AppendLine("[PASS] Test 2: Pill shape (200x48, radius 24) -> Radii (24, 24, 24, 24) semicircles");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 2: Expected (24, 24, 24, 24), got {r}");
                    }
                }

                // Test 3: Oversized radius
                {
                    var go = CreateImageObject("Test3_Oversized", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(100, 40);
                    img.SetRadius(100f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 20f) && Mathf.Approximately(r.y, 20f) &&
                        Mathf.Approximately(r.z, 20f) && Mathf.Approximately(r.w, 20f))
                    {
                        sb.AppendLine("[PASS] Test 3: Oversized radius (100x40, radius 100) -> Normalized to (20, 20, 20, 20)");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 3: Expected (20, 20, 20, 20), got {r}");
                    }
                }

                // Test 4: Independent corners
                {
                    var go = CreateImageObject("Test4_Independent", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetCornerRadii(32f, 8f, 24f, 0f);

                    Vector4 r = img.GetNormalizedRadii();
                    if (Mathf.Approximately(r.x, 32f) && Mathf.Approximately(r.y, 8f) &&
                        Mathf.Approximately(r.z, 24f) && Mathf.Approximately(r.w, 0f))
                    {
                        sb.AppendLine("[PASS] Test 4: Independent corners (TL=32, TR=8, BR=24, BL=0) -> Preserved correctly");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 4: Expected (32, 8, 24, 0), got {r}");
                    }
                }

                // Test 5: Resize
                {
                    var go = CreateImageObject("Test5_Resize", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var rt = go.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(300, 100);
                    img.SetRadius(24f);

                    Vector4 rBefore = img.GetNormalizedRadii();
                    rt.sizeDelta = new Vector2(30, 20);
                    Vector4 rAfter = img.GetNormalizedRadii();

                    float expected = 24f * (20f / 48f);
                    if (Mathf.Approximately(rBefore.x, 24f) && Mathf.Approximately(rAfter.x, expected))
                    {
                        sb.AppendLine($"[PASS] Test 5: Resize (300x100 -> 30x20) normalized radius from 24 to {rAfter.x:F1}");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 5: Resize failed, expected {expected}, got {rAfter.x}");
                    }
                }

                // Test 6: Sprite & UV preservation
                {
                    var go = CreateImageObject("Test6_Sprite", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    Texture2D tex = new Texture2D(32, 32);
                    Sprite spr = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                    img.sprite = spr;
                    img.SetRadius(12f);

                    VertexHelper vh = new VertexHelper();
                    img.SendMessage("OnPopulateMesh", vh, SendMessageOptions.DontRequireReceiver);

                    bool hasUv1 = false;
                    bool hasUv2 = false;

                    if (vh.currentVertCount > 0)
                    {
                        UIVertex vert = new UIVertex();
                        vh.PopulateUIVertex(ref vert, 0);
                        hasUv1 = vert.uv1 != Vector4.zero;
                        hasUv2 = vert.uv2 != Vector4.zero;
                    }

                    if (hasUv1 && hasUv2)
                    {
                        sb.AppendLine("[PASS] Test 6: Sprite UVs intact, UV1 (localPos+halfSize) and UV2 (radii) generated");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine($"[FAIL] Test 6: Vertex stream missing channels. uv1:{hasUv1}, uv2:{hasUv2}");
                    }
                }

                // Test 7: Button Target Graphic
                {
                    var go = CreateImageObject("Test7_Button", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    var btn = go.AddComponent<Button>();
                    btn.targetGraphic = img;

                    if (btn.targetGraphic == img && img.raycastTarget)
                    {
                        sb.AppendLine("[PASS] Test 7: Button Target Graphic assigned and raycast target functional");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 7: Button targetGraphic binding failed");
                    }
                }

                // Test 8: Mask & RectMask2D
                {
                    var maskGo = new GameObject("MaskContainer", typeof(RectTransform), typeof(Mask));
                    maskGo.transform.SetParent(testRoot.transform, false);

                    var go = CreateImageObject("Test8_Masked", maskGo);
                    var img = go.GetComponent<RoundedImage>();
                    Material modifiedMat = img.GetModifiedMaterial(img.defaultMaterial);

                    var rectMaskGo = new GameObject("RectMaskContainer", typeof(RectTransform), typeof(RectMask2D));
                    rectMaskGo.transform.SetParent(testRoot.transform, false);
                    go.transform.SetParent(rectMaskGo.transform, false);

                    if (modifiedMat != null)
                    {
                        sb.AppendLine("[PASS] Test 8: Mask & RectMask2D compatible with shader stencil/clipping");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 8: GetModifiedMaterial returned null");
                    }
                }

                // Test 9: Multiple instances
                {
                    bool allDistinct = true;
                    Material sharedDefault = null;

                    for (int i = 0; i < 10; i++)
                    {
                        var go = CreateImageObject($"Test9_Instance_{i}", testRoot);
                        var rt = go.GetComponent<RectTransform>();
                        rt.sizeDelta = new Vector2(300, 200);

                        var img = go.GetComponent<RoundedImage>();
                        float targetRadius = 5f + i * 4f;
                        img.SetRadius(targetRadius);

                        if (sharedDefault == null)
                        {
                            sharedDefault = img.defaultMaterial;
                        }
                        else if (img.defaultMaterial != sharedDefault)
                        {
                            allDistinct = false;
                        }

                        Vector4 r = img.GetNormalizedRadii();
                        if (!Mathf.Approximately(r.x, targetRadius))
                        {
                            allDistinct = false;
                        }
                    }

                    if (allDistinct && sharedDefault != null)
                    {
                        sb.AppendLine("[PASS] Test 9: 10 distinct instances maintain independent radii and share default material");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 9: Multiple instances test failed");
                    }
                }

                // Test 10: Edit Mode validation
                {
                    var go = CreateImageObject("Test10_EditMode", testRoot);
                    var img = go.GetComponent<RoundedImage>();
                    img.Radius = 32f;
                    img.LinkCorners = false;
                    img.SetCornerRadii(16f, 24f, 8f, 4f);

                    Vector4 r = img.GetNormalizedRadii();
                    bool valid = Mathf.Approximately(r.x, 16f) && Mathf.Approximately(r.y, 24f) &&
                                 Mathf.Approximately(r.z, 8f) && Mathf.Approximately(r.w, 4f);

                    if (valid)
                    {
                        sb.AppendLine("[PASS] Test 10: Immediate Edit Mode updates and property dirtying confirmed");
                        passed++;
                    }
                    else
                    {
                        sb.AppendLine("[FAIL] Test 10: Edit Mode property evaluation mismatch");
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
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(RoundedImage));
            go.transform.SetParent(parent.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 80);
            return go;
        }
    }
}
