using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectArea.UI
{
    [AddComponentMenu("UI/Figma Image", 11)]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class FigmaImage : Image, ICanvasRaycastFilter, ISerializationCallbackReceiver
    {
        [SerializeField]
        private FigmaCornerRadiusSettings m_CornerRadius = new FigmaCornerRadiusSettings();

        [SerializeField]
        private FigmaStrokeSettings m_Stroke = new FigmaStrokeSettings();

        [SerializeField]
        private FigmaDropShadowSettings m_DropShadow = new FigmaDropShadowSettings();

        [SerializeField]
        private bool m_UseRoundedRaycast = false;

        // Legacy fields for backward compatibility with scene/prefab data
        [SerializeField, HideInInspector]
        private bool m_LinkCorners = true;

        [SerializeField, HideInInspector]
        private float m_Radius = -1f;

        [SerializeField, HideInInspector]
        private Vector4 m_CornerRadii = Vector4.zero;

        private static Material s_DefaultMaterial;
        private static bool s_ShaderNotFoundLogged = false;

        public FigmaCornerRadiusSettings CornerRadius => m_CornerRadius;
        public FigmaStrokeSettings Stroke => m_Stroke;
        public FigmaDropShadowSettings DropShadow => m_DropShadow;

        #region Public Properties

        public bool LinkCorners
        {
            get => m_CornerRadius.LinkCorners;
            set
            {
                if (m_CornerRadius.LinkCorners != value)
                {
                    m_CornerRadius.LinkCorners = value;
                    SetVerticesDirty();
                }
            }
        }

        public float Radius
        {
            get => m_CornerRadius.Radius;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_CornerRadius.Radius, clamped))
                {
                    m_CornerRadius.Radius = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public Vector4 CornerRadii
        {
            get => m_CornerRadius.CornerRadii;
            set
            {
                Vector4 clamped = new Vector4(
                    Mathf.Max(0f, value.x),
                    Mathf.Max(0f, value.y),
                    Mathf.Max(0f, value.z),
                    Mathf.Max(0f, value.w)
                );
                if (m_CornerRadius.CornerRadii != clamped)
                {
                    m_CornerRadius.CornerRadii = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public float TopLeft
        {
            get => m_CornerRadius.TopLeft;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_CornerRadius.TopLeft, clamped))
                {
                    m_CornerRadius.TopLeft = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public float TopRight
        {
            get => m_CornerRadius.TopRight;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_CornerRadius.TopRight, clamped))
                {
                    m_CornerRadius.TopRight = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public float BottomRight
        {
            get => m_CornerRadius.BottomRight;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_CornerRadius.BottomRight, clamped))
                {
                    m_CornerRadius.BottomRight = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public float BottomLeft
        {
            get => m_CornerRadius.BottomLeft;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_CornerRadius.BottomLeft, clamped))
                {
                    m_CornerRadius.BottomLeft = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public bool StrokeEnabled
        {
            get => m_Stroke.Enabled;
            set
            {
                if (m_Stroke.Enabled != value)
                {
                    m_Stroke.Enabled = value;
                    SetVerticesDirty();
                }
            }
        }

        public float StrokeWidth
        {
            get => m_Stroke.Width;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_Stroke.Width, clamped))
                {
                    m_Stroke.Width = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public Color StrokeColor
        {
            get => m_Stroke.Color;
            set
            {
                if (m_Stroke.Color != value)
                {
                    m_Stroke.Color = value;
                    SetVerticesDirty();
                }
            }
        }

        public FigmaStrokePosition StrokePosition
        {
            get => m_Stroke.Position;
            set
            {
                if (m_Stroke.Position != value)
                {
                    m_Stroke.Position = value;
                    SetVerticesDirty();
                }
            }
        }

        public bool MaskIgnoreStroke
        {
            get => m_Stroke != null && m_Stroke.IgnoreInMask;
            set
            {
                if (m_Stroke != null && m_Stroke.IgnoreInMask != value)
                {
                    m_Stroke.IgnoreInMask = value;
                    SetVerticesDirty();
                }
            }
        }

        public bool DropShadowEnabled
        {
            get => m_DropShadow.Enabled;
            set
            {
                if (m_DropShadow.Enabled != value)
                {
                    m_DropShadow.Enabled = value;
                    SetVerticesDirty();
                }
            }
        }

        public float DropShadowOffsetX
        {
            get => m_DropShadow.OffsetX;
            set
            {
                if (!Mathf.Approximately(m_DropShadow.OffsetX, value))
                {
                    m_DropShadow.OffsetX = value;
                    SetVerticesDirty();
                }
            }
        }

        public float DropShadowOffsetY
        {
            get => m_DropShadow.OffsetY;
            set
            {
                if (!Mathf.Approximately(m_DropShadow.OffsetY, value))
                {
                    m_DropShadow.OffsetY = value;
                    SetVerticesDirty();
                }
            }
        }

        public Vector2 DropShadowOffset
        {
            get => m_DropShadow.Offset;
            set
            {
                if (m_DropShadow.Offset != value)
                {
                    m_DropShadow.Offset = value;
                    SetVerticesDirty();
                }
            }
        }

        public float DropShadowBlur
        {
            get => m_DropShadow.Blur;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (!Mathf.Approximately(m_DropShadow.Blur, clamped))
                {
                    m_DropShadow.Blur = clamped;
                    SetVerticesDirty();
                }
            }
        }

        public float DropShadowSpread
        {
            get => m_DropShadow.Spread;
            set
            {
                if (!Mathf.Approximately(m_DropShadow.Spread, value))
                {
                    m_DropShadow.Spread = value;
                    SetVerticesDirty();
                }
            }
        }

        public Color DropShadowColor
        {
            get => m_DropShadow.Color;
            set
            {
                if (m_DropShadow.Color != value)
                {
                    m_DropShadow.Color = value;
                    SetVerticesDirty();
                }
            }
        }

        public bool UseRoundedRaycast
        {
            get => m_UseRoundedRaycast;
            set => m_UseRoundedRaycast = value;
        }

        #endregion

        #region Public Methods

        public void SetRadius(float radius)
        {
            m_CornerRadius.SetRadius(radius);
            SetVerticesDirty();
        }

        public void SetCornerRadii(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            m_CornerRadius.SetCornerRadii(topLeft, topRight, bottomRight, bottomLeft);
            SetVerticesDirty();
        }

        public void SetCornerRadii(Vector4 radii)
        {
            m_CornerRadius.SetCornerRadii(radii);
            SetVerticesDirty();
        }

        public void SetStrokeEnabled(bool enabled)
        {
            m_Stroke.Enabled = enabled;
            SetVerticesDirty();
        }

        public void SetStrokeWidth(float width)
        {
            m_Stroke.Width = Mathf.Max(0f, width);
            SetVerticesDirty();
        }

        public void SetStrokeColor(Color color)
        {
            m_Stroke.Color = color;
            SetVerticesDirty();
        }

        public void SetMaskIgnoreStroke(bool ignore)
        {
            if (m_Stroke != null && m_Stroke.IgnoreInMask != ignore)
            {
                m_Stroke.IgnoreInMask = ignore;
                SetVerticesDirty();
            }
        }

        public void SetDropShadowEnabled(bool enabled)
        {
            m_DropShadow.Enabled = enabled;
            SetVerticesDirty();
        }

        public void SetDropShadowOffset(float x, float y)
        {
            m_DropShadow.OffsetX = x;
            m_DropShadow.OffsetY = y;
            SetVerticesDirty();
        }

        public void SetDropShadowOffset(Vector2 offset)
        {
            m_DropShadow.Offset = offset;
            SetVerticesDirty();
        }

        public void SetDropShadowBlur(float blur)
        {
            m_DropShadow.Blur = Mathf.Max(0f, blur);
            SetVerticesDirty();
        }

        public void SetDropShadowSpread(float spread)
        {
            m_DropShadow.Spread = spread;
            SetVerticesDirty();
        }

        public void SetDropShadowColor(Color color)
        {
            m_DropShadow.Color = color;
            SetVerticesDirty();
        }

        public Vector4 GetNormalizedRadii()
        {
            return m_CornerRadius.GetNormalizedRadii(GetPixelAdjustedRect());
        }

        #endregion

        #region Material Handling

        public override Material defaultMaterial
        {
            get
            {
                if (s_DefaultMaterial == null)
                {
                    Shader shader = Shader.Find("UI/FigmaImage");
                    if (shader != null)
                    {
                        s_DefaultMaterial = new Material(shader)
                        {
                            name = "FigmaImage-Default",
                            hideFlags = HideFlags.DontSave
                        };
                    }
                    else
                    {
                        if (!s_ShaderNotFoundLogged)
                        {
                            s_ShaderNotFoundLogged = true;
                            Debug.LogError("[FigmaImage] Could not find shader 'UI/FigmaImage'. Falling back to base Image defaultMaterial.");
                        }
                        return base.defaultMaterial;
                    }
                }
                return s_DefaultMaterial;
            }
        }

        #endregion

        #region Lifecycle & Canvas Setup

        protected override void OnEnable()
        {
            base.OnEnable();
            EnsureCanvasChannels();
            RequestHelperUpdates();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CleanupHelpers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CleanupHelpers();
        }

        protected override void Start()
        {
            base.Start();
            EnsureCanvasChannels();
            RequestHelperUpdates();
        }

        protected virtual void LateUpdate()
        {
            if (gameObject.name.StartsWith("[FigmaImage_")) return;

            bool isMasked = TryGetComponent<Mask>(out var mask) && mask.enabled;
            bool needHelpers = isMasked && (
                (m_Stroke != null && m_Stroke.Enabled && m_Stroke.IgnoreInMask) ||
                (m_DropShadow != null && m_DropShadow.Enabled)
            );

            if (needHelpers)
            {
                RequestHelperUpdates();
            }
            else
            {
                CleanupHelpers();
            }
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            EnsureCanvasChannels();
            RequestHelperUpdates();
        }

        protected override void OnCanvasHierarchyChanged()
        {
            base.OnCanvasHierarchyChanged();
            EnsureCanvasChannels();
            RequestHelperUpdates();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            RequestHelperUpdates();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            base.OnDidApplyAnimationProperties();
            SetVerticesDirty();
            RequestHelperUpdates();
        }

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            RequestHelperUpdates();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_CornerRadius == null)
            {
                m_CornerRadius = new FigmaCornerRadiusSettings();
            }
            if (m_Stroke == null)
            {
                m_Stroke = new FigmaStrokeSettings();
            }
            if (m_DropShadow == null)
            {
                m_DropShadow = new FigmaDropShadowSettings();
            }

            m_CornerRadius.Radius = Mathf.Max(0f, m_CornerRadius.Radius);
            m_CornerRadius.CornerRadii = new Vector4(
                Mathf.Max(0f, m_CornerRadius.CornerRadii.x),
                Mathf.Max(0f, m_CornerRadius.CornerRadii.y),
                Mathf.Max(0f, m_CornerRadius.CornerRadii.z),
                Mathf.Max(0f, m_CornerRadius.CornerRadii.w)
            );
            if (m_CornerRadius.LinkCorners)
            {
                m_CornerRadius.CornerRadii = new Vector4(
                    m_CornerRadius.Radius,
                    m_CornerRadius.Radius,
                    m_CornerRadius.Radius,
                    m_CornerRadius.Radius
                );
            }

            m_Stroke.Width = Mathf.Max(0f, m_Stroke.Width);
            m_DropShadow.Blur = Mathf.Max(0f, m_DropShadow.Blur);

            EnsureCanvasChannels();
            SetVerticesDirty();
        }
#endif

        private void EnsureCanvasChannels()
        {
            Canvas c = canvas;
            if (c != null)
            {
                EnableChannelsOnCanvas(c);
                if (c.rootCanvas != null && c.rootCanvas != c)
                {
                    EnableChannelsOnCanvas(c.rootCanvas);
                }
            }
        }

        private static void EnableChannelsOnCanvas(Canvas targetCanvas)
        {
            if (targetCanvas == null) return;

            const AdditionalCanvasShaderChannels required =
                AdditionalCanvasShaderChannels.TexCoord1 |
                AdditionalCanvasShaderChannels.TexCoord2 |
                AdditionalCanvasShaderChannels.TexCoord3 |
                AdditionalCanvasShaderChannels.Normal |
                AdditionalCanvasShaderChannels.Tangent;

            if ((targetCanvas.additionalShaderChannels & required) != required)
            {
                targetCanvas.additionalShaderChannels |= required;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(targetCanvas);
                }
#endif
            }
        }

        #endregion

        #region Mask Helper Overlays & Underlays

        private bool m_IsUpdatingHelpers = false;

        private void RequestHelperUpdates()
        {
            if (gameObject == null || gameObject.name.StartsWith("[FigmaImage_")) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall -= ExecuteHelperUpdates;
                UnityEditor.EditorApplication.delayCall += ExecuteHelperUpdates;
                return;
            }
#endif
            ExecuteHelperUpdates();
        }

        private void ExecuteHelperUpdates()
        {
            if (this == null || gameObject == null || m_IsUpdatingHelpers) return;
            if (gameObject.name.StartsWith("[FigmaImage_")) return;

            m_IsUpdatingHelpers = true;
            try
            {
                bool isMasked = TryGetComponent<Mask>(out var mask) && mask.enabled;
                UpdateOutlineOverlay(isMasked);
                UpdateShadowUnderlay(isMasked);
            }
            finally
            {
                m_IsUpdatingHelpers = false;
            }
        }

        private void UpdateOutlineOverlay(bool isMasked)
        {
            bool needOutline = m_Stroke != null && m_Stroke.Enabled && m_Stroke.IgnoreInMask && isMasked;

            const string overlayName = "[FigmaImage_OutlineOverlay]";
            Transform overlayTr = transform.Find(overlayName);

            if (!needOutline)
            {
                if (overlayTr != null)
                {
                    DestroyHelper(overlayTr);
                }
                return;
            }

            GameObject overlayGo;
            if (overlayTr == null)
            {
                overlayGo = new GameObject(overlayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage));
                overlayGo.hideFlags = HideFlags.DontSave;
                overlayGo.transform.SetParent(transform, false);
            }
            else
            {
                overlayGo = overlayTr.gameObject;
            }

            overlayGo.transform.SetAsLastSibling();

            RectTransform rt = (RectTransform)overlayGo.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;

            FigmaImage overlayImg = overlayGo.GetComponent<FigmaImage>();
            overlayImg.maskable = false;
            overlayImg.raycastTarget = false;
            overlayImg.color = new Color(0, 0, 0, 0);
            overlayImg.CornerRadii = CornerRadii;
            overlayImg.StrokeEnabled = true;
            overlayImg.StrokeWidth = StrokeWidth;
            overlayImg.StrokeColor = StrokeColor;
            overlayImg.DropShadowEnabled = false;
        }

        private void UpdateShadowUnderlay(bool isMasked)
        {
            bool maskShowsGraphic = true;
            if (TryGetComponent<Mask>(out var mask))
            {
                maskShowsGraphic = mask.showMaskGraphic;
            }

            bool needUnderlay = m_DropShadow != null && m_DropShadow.Enabled && isMasked && maskShowsGraphic && transform.parent != null;

            string underlayName = $"[FigmaImage_ShadowUnderlay_{GetInstanceID()}]";
            Transform underlayTr = transform.parent != null ? transform.parent.Find(underlayName) : null;

            if (!needUnderlay)
            {
                if (underlayTr != null)
                {
                    DestroyHelper(underlayTr);
                }
                return;
            }

            GameObject underlayGo;
            if (underlayTr == null)
            {
                underlayGo = new GameObject(underlayName, typeof(RectTransform), typeof(CanvasRenderer), typeof(FigmaImage));
                underlayGo.hideFlags = HideFlags.DontSave;
                underlayGo.transform.SetParent(transform.parent, false);
            }
            else
            {
                underlayGo = underlayTr.gameObject;
            }

            // Ensure underlay is immediately before this GameObject in sibling order
            int myIndex = transform.GetSiblingIndex();
            if (underlayGo.transform.GetSiblingIndex() != myIndex - 1)
            {
                int targetIndex = underlayGo.transform.GetSiblingIndex() < myIndex ? myIndex - 1 : myIndex;
                underlayGo.transform.SetSiblingIndex(targetIndex);
            }

            RectTransform myRt = rectTransform;
            RectTransform underlayRt = (RectTransform)underlayGo.transform;
            underlayRt.anchorMin = myRt.anchorMin;
            underlayRt.anchorMax = myRt.anchorMax;
            underlayRt.pivot = myRt.pivot;
            underlayRt.anchoredPosition = myRt.anchoredPosition;
            underlayRt.sizeDelta = myRt.sizeDelta;
            underlayRt.localRotation = myRt.localRotation;
            underlayRt.localScale = myRt.localScale;

            FigmaImage underlayImg = underlayGo.GetComponent<FigmaImage>();
            underlayImg.maskable = false;
            underlayImg.raycastTarget = false;
            underlayImg.color = new Color(0, 0, 0, 0); // No fill
            underlayImg.StrokeEnabled = false;
            underlayImg.CornerRadii = CornerRadii;
            underlayImg.DropShadowEnabled = true;
            underlayImg.DropShadowOffset = DropShadowOffset;
            underlayImg.DropShadowBlur = DropShadowBlur;
            underlayImg.DropShadowSpread = DropShadowSpread;
            underlayImg.DropShadowColor = DropShadowColor;

            // Sync CanvasGroup if present
            if (TryGetComponent<CanvasGroup>(out var myCg))
            {
                CanvasGroup underlayCg = underlayGo.GetComponent<CanvasGroup>() ?? underlayGo.AddComponent<CanvasGroup>();
                underlayCg.alpha = myCg.alpha;
                underlayCg.interactable = false;
                underlayCg.blocksRaycasts = false;
                underlayCg.ignoreParentGroups = myCg.ignoreParentGroups;
            }
        }

        private void CleanupHelpers()
        {
            if (gameObject == null || gameObject.name.StartsWith("[FigmaImage_")) return;

            Transform overlayTr = transform.Find("[FigmaImage_OutlineOverlay]");
            if (overlayTr != null)
            {
                DestroyHelper(overlayTr);
            }

            if (transform.parent != null)
            {
                string underlayName = $"[FigmaImage_ShadowUnderlay_{GetInstanceID()}]";
                Transform underlayTr = transform.parent.Find(underlayName);
                if (underlayTr != null)
                {
                    DestroyHelper(underlayTr);
                }
            }
        }

        private static void DestroyHelper(Transform helper)
        {
            if (Application.isPlaying)
            {
                bool canDetach = helper.parent == null || helper.parent.gameObject.activeInHierarchy;
                helper.gameObject.SetActive(false);
                if (canDetach)
                {
                    helper.SetParent(null, false);
                }
                Destroy(helper.gameObject);
            }
            else
            {
                DestroyImmediate(helper.gameObject);
            }
        }

        #endregion

        #region Serialization Migration

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();

            // Migrate legacy RoundedImage serialized fields if present
            if (m_Radius >= 0f)
            {
                if (m_CornerRadius == null)
                {
                    m_CornerRadius = new FigmaCornerRadiusSettings();
                }

                m_CornerRadius.LinkCorners = m_LinkCorners;
                m_CornerRadius.Radius = m_Radius;

                if (!m_LinkCorners && m_CornerRadii != Vector4.zero)
                {
                    m_CornerRadius.CornerRadii = m_CornerRadii;
                }

                // Reset legacy field so migration only runs once
                m_Radius = -1f;
            }

            if (m_CornerRadius == null)
            {
                m_CornerRadius = new FigmaCornerRadiusSettings();
            }
            if (m_Stroke == null)
            {
                m_Stroke = new FigmaStrokeSettings();
            }
            if (m_DropShadow == null)
            {
                m_DropShadow = new FigmaDropShadowSettings();
            }
        }

        #endregion

        #region Mesh Generation

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            base.OnPopulateMesh(toFill);

            int count = toFill.currentVertCount;
            if (count == 0) return;

            Rect rect = GetPixelAdjustedRect();
            Vector2 center = rect.center;
            Vector2 halfSize = rect.size * 0.5f;

            if (preserveAspect && type == Type.Simple && count == 4)
            {
                UIVertex temp = new UIVertex();
                toFill.PopulateUIVertex(ref temp, 0);
                Vector2 min = temp.position;
                Vector2 max = temp.position;
                for (int i = 1; i < count; i++)
                {
                    toFill.PopulateUIVertex(ref temp, i);
                    min = Vector2.Min(min, temp.position);
                    max = Vector2.Max(max, temp.position);
                }
                center = (min + max) * 0.5f;
                halfSize = (max - min) * 0.5f;
            }

            Vector4 normalizedRadii = GetNormalizedRadii();

            // Stroke parameters
            bool strokeOn = m_Stroke != null && m_Stroke.Enabled && m_Stroke.Width > 0f;
            float maxStroke = Mathf.Min(halfSize.x, halfSize.y);
            float clampedStrokeWidth = strokeOn ? Mathf.Clamp(m_Stroke.Width, 0f, maxStroke) : 0f;
            Color strokeColor = (m_Stroke != null) ? m_Stroke.Color : Color.white;

            // Drop Shadow parameters
            bool isMaskedGraphic = TryGetComponent<Mask>(out var maskComp) && maskComp.enabled;
            bool shadowOn = m_DropShadow != null && m_DropShadow.Enabled && m_DropShadow.Color.a > 0.0001f;
            // When attached to a Mask, shadow rendering is handled by the sibling underlay so the mask stencil itself is never enlarged!
            bool applyShadowDirectly = shadowOn && !isMaskedGraphic;
            float shadowBlur = (m_DropShadow != null) ? Mathf.Max(0f, m_DropShadow.Blur) : 0f;
            float shadowSpread = (m_DropShadow != null) ? m_DropShadow.Spread : 0f;
            float shadowOffsetX = (m_DropShadow != null) ? m_DropShadow.OffsetX : 0f;
            float shadowOffsetY = (m_DropShadow != null) ? m_DropShadow.OffsetY : 0f; // Figma convention: +Y is down
            Color shadowColor = (m_DropShadow != null) ? m_DropShadow.Color : new Color(0f, 0f, 0f, 0.25f);

            // Expand mesh geometry if Drop Shadow is active directly on this graphic and we have a standard quad
            if (applyShadowDirectly && count == 4)
            {
                float extent = (shadowBlur > 0f ? shadowBlur * 1.5f + 2f : 2f) + Mathf.Max(0f, shadowSpread);
                float leftPad = extent + Mathf.Max(0f, -shadowOffsetX);
                float rightPad = extent + Mathf.Max(0f, shadowOffsetX);
                // Figma +Y is down -> bottom padding expands with positive offsetY
                float bottomPad = extent + Mathf.Max(0f, shadowOffsetY);
                float topPad = extent + Mathf.Max(0f, -shadowOffsetY);

                UIVertex v0 = new UIVertex();
                UIVertex v1 = new UIVertex();
                UIVertex v2 = new UIVertex();
                UIVertex v3 = new UIVertex();
                toFill.PopulateUIVertex(ref v0, 0);
                toFill.PopulateUIVertex(ref v1, 1);
                toFill.PopulateUIVertex(ref v2, 2);
                toFill.PopulateUIVertex(ref v3, 3);

                // In Unity UI Image, vertices are:
                // 0: Bottom-Left, 1: Top-Left, 2: Top-Right, 3: Bottom-Right
                float origW = v2.position.x - v1.position.x;
                float origH = v1.position.y - v0.position.y;

                if (origW > 0.0001f && origH > 0.0001f)
                {
                    Vector4 uv0 = v0.uv0;
                    Vector4 uv1 = v1.uv0;
                    Vector4 uv2 = v2.uv0;
                    Vector4 uv3 = v3.uv0;

                    Vector4 uSpan = uv2 - uv1;
                    Vector4 vSpan = uv1 - uv0;

                    v0.uv0 = uv0 - (leftPad / origW) * uSpan - (bottomPad / origH) * vSpan;
                    v1.uv0 = uv1 - (leftPad / origW) * uSpan + (topPad / origH) * vSpan;
                    v2.uv0 = uv2 + (rightPad / origW) * uSpan + (topPad / origH) * vSpan;
                    v3.uv0 = uv3 + (rightPad / origW) * uSpan - (bottomPad / origH) * vSpan;
                }

                v0.position.x -= leftPad;
                v0.position.y -= bottomPad;

                v1.position.x -= leftPad;
                v1.position.y += topPad;

                v2.position.x += rightPad;
                v2.position.y += topPad;

                v3.position.x += rightPad;
                v3.position.y -= bottomPad;

                toFill.SetUIVertex(v0, 0);
                toFill.SetUIVertex(v1, 1);
                toFill.SetUIVertex(v2, 2);
                toFill.SetUIVertex(v3, 3);
            }

            // Color packing:
            // pStrokeRG: R (8-bit) * 256 + G (8-bit)
            // pStrokeBA: B (8-bit) * 256 + A (8-bit)
            // pShadowRG: R (8-bit) * 256 + G (8-bit)
            // pShadowBA: B (8-bit) * 256 + A (8-bit)
            float pStrokeRG = Mathf.Round(strokeColor.r * 255f) * 256f + Mathf.Round(strokeColor.g * 255f);
            float pStrokeBA = Mathf.Round(strokeColor.b * 255f) * 256f + Mathf.Round(strokeColor.a * 255f);
            float pShadowRG = Mathf.Round(shadowColor.r * 255f) * 256f + Mathf.Round(shadowColor.g * 255f);
            float pShadowBA = Mathf.Round(shadowColor.b * 255f) * 256f + Mathf.Round(shadowColor.a * 255f);

            Vector4 packedColors = new Vector4(pStrokeRG, pStrokeBA, pShadowRG, pShadowBA);

            // Tangent: x = strokeWidth (px), y = fillAlpha (0..1), z = strokeEnabled flag (0=off, 1=on, 2=on + ignore stroke in mask), w = shadowSpread (px)
            float strokeFlag = 0f;
            if (strokeOn)
            {
                strokeFlag = (m_Stroke != null && m_Stroke.IgnoreInMask) ? 2f : 1f;
            }

            Vector4 strokeAndShadowParams = new Vector4(
                clampedStrokeWidth,
                color.a,
                strokeFlag,
                applyShadowDirectly ? shadowSpread : 0f
            );

            // Normal: x = shadowOffsetX, y = shadowOffsetY, z = shadowBlur (-1f if disabled)
            Vector3 shadowParams = new Vector3(
                applyShadowDirectly ? shadowOffsetX : 0f,
                applyShadowDirectly ? shadowOffsetY : 0f,
                applyShadowDirectly ? shadowBlur : -1f
            );

            UIVertex vert = new UIVertex();
            for (int i = 0; i < count; i++)
            {
                toFill.PopulateUIVertex(ref vert, i);
                Vector2 localPos = (Vector2)vert.position - center;

                vert.uv1 = new Vector4(localPos.x, localPos.y, halfSize.x, halfSize.y);
                vert.uv2 = normalizedRadii;
                vert.uv3 = packedColors;
                vert.tangent = strokeAndShadowParams;
                vert.normal = shadowParams;

                // Keep vertex RGB from Image.color, set alpha to 255 so CanvasGroup dynamically scales it
                vert.color = new Color32(vert.color.r, vert.color.g, vert.color.b, 255);

                toFill.SetUIVertex(vert, i);
            }
        }

        #endregion

        #region Raycasting

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (!m_UseRoundedRaycast)
            {
                return base.IsRaycastLocationValid(sp, eventCamera);
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out Vector2 localPoint))
            {
                return false;
            }

            Rect rect = GetPixelAdjustedRect();
            Vector2 center = rect.center;
            Vector2 halfSize = rect.size * 0.5f;

            if (halfSize.x <= 0f || halfSize.y <= 0f)
            {
                return false;
            }

            Vector2 p = localPoint - center;
            Vector4 r = GetNormalizedRadii();

            float rad = (p.x >= 0f) ? ((p.y >= 0f) ? r.y : r.z) : ((p.y >= 0f) ? r.x : r.w);
            rad = Mathf.Min(rad, Mathf.Min(halfSize.x, halfSize.y));

            Vector2 q = new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y)) - halfSize + new Vector2(rad, rad);
            float dist = Mathf.Sqrt(Mathf.Max(q.x, 0f) * Mathf.Max(q.x, 0f) + Mathf.Max(q.y, 0f) * Mathf.Max(q.y, 0f))
                         + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - rad;

            return dist <= 0f;
        }

        #endregion
    }
}
