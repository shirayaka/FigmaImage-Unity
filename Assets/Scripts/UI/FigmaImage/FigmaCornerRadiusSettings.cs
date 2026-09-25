using System;
using UnityEngine;

namespace ProjectArea.UI
{
    [Serializable]
    public class FigmaCornerRadiusSettings
    {
        [SerializeField]
        private bool m_LinkCorners = true;

        [SerializeField]
        private float m_Radius = 16f;

        // Corner order: Top-Left (x), Top-Right (y), Bottom-Right (z), Bottom-Left (w)
        [SerializeField]
        private Vector4 m_CornerRadii = new Vector4(16f, 16f, 16f, 16f);

        public FigmaCornerRadiusSettings()
        {
            m_LinkCorners = true;
            m_Radius = 16f;
            m_CornerRadii = new Vector4(16f, 16f, 16f, 16f);
        }

        public FigmaCornerRadiusSettings(float uniformRadius)
        {
            m_LinkCorners = true;
            m_Radius = Mathf.Max(0f, uniformRadius);
            m_CornerRadii = new Vector4(m_Radius, m_Radius, m_Radius, m_Radius);
        }

        public FigmaCornerRadiusSettings(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            m_LinkCorners = false;
            m_Radius = Mathf.Max(0f, topLeft);
            m_CornerRadii = new Vector4(
                Mathf.Max(0f, topLeft),
                Mathf.Max(0f, topRight),
                Mathf.Max(0f, bottomRight),
                Mathf.Max(0f, bottomLeft)
            );
        }

        public bool LinkCorners
        {
            get => m_LinkCorners;
            set
            {
                if (m_LinkCorners != value)
                {
                    m_LinkCorners = value;
                    if (m_LinkCorners)
                    {
                        m_CornerRadii = new Vector4(m_Radius, m_Radius, m_Radius, m_Radius);
                    }
                }
            }
        }

        public float Radius
        {
            get => m_Radius;
            set
            {
                float clamped = Mathf.Max(0f, value);
                m_Radius = clamped;
                if (m_LinkCorners)
                {
                    m_CornerRadii = new Vector4(m_Radius, m_Radius, m_Radius, m_Radius);
                }
            }
        }

        public Vector4 CornerRadii
        {
            get => m_CornerRadii;
            set
            {
                Vector4 clamped = new Vector4(
                    Mathf.Max(0f, value.x),
                    Mathf.Max(0f, value.y),
                    Mathf.Max(0f, value.z),
                    Mathf.Max(0f, value.w)
                );
                m_CornerRadii = clamped;
                if (m_LinkCorners)
                {
                    m_Radius = clamped.x;
                }
            }
        }

        public float TopLeft
        {
            get => m_LinkCorners ? m_Radius : m_CornerRadii.x;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (m_LinkCorners)
                {
                    Radius = clamped;
                }
                else
                {
                    m_CornerRadii.x = clamped;
                }
            }
        }

        public float TopRight
        {
            get => m_LinkCorners ? m_Radius : m_CornerRadii.y;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (m_LinkCorners)
                {
                    Radius = clamped;
                }
                else
                {
                    m_CornerRadii.y = clamped;
                }
            }
        }

        public float BottomRight
        {
            get => m_LinkCorners ? m_Radius : m_CornerRadii.z;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (m_LinkCorners)
                {
                    Radius = clamped;
                }
                else
                {
                    m_CornerRadii.z = clamped;
                }
            }
        }

        public float BottomLeft
        {
            get => m_LinkCorners ? m_Radius : m_CornerRadii.w;
            set
            {
                float clamped = Mathf.Max(0f, value);
                if (m_LinkCorners)
                {
                    Radius = clamped;
                }
                else
                {
                    m_CornerRadii.w = clamped;
                }
            }
        }

        public void SetRadius(float radius)
        {
            m_LinkCorners = true;
            Radius = radius;
        }

        public void SetCornerRadii(float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            m_LinkCorners = false;
            CornerRadii = new Vector4(topLeft, topRight, bottomRight, bottomLeft);
        }

        public void SetCornerRadii(Vector4 radii)
        {
            m_LinkCorners = false;
            CornerRadii = radii;
        }

        public Vector4 GetNormalizedRadii(Rect rect)
        {
            float width = Mathf.Max(0f, rect.width);
            float height = Mathf.Max(0f, rect.height);

            Vector4 raw = m_LinkCorners
                ? new Vector4(m_Radius, m_Radius, m_Radius, m_Radius)
                : m_CornerRadii;

            float tl = Mathf.Max(0f, raw.x);
            float tr = Mathf.Max(0f, raw.y);
            float br = Mathf.Max(0f, raw.z);
            float bl = Mathf.Max(0f, raw.w);

            float f = 1f;

            float topSum = tl + tr;
            if (topSum > width && topSum > 0f)
            {
                f = Mathf.Min(f, width / topSum);
            }

            float bottomSum = bl + br;
            if (bottomSum > width && bottomSum > 0f)
            {
                f = Mathf.Min(f, width / bottomSum);
            }

            float leftSum = tl + bl;
            if (leftSum > height && leftSum > 0f)
            {
                f = Mathf.Min(f, height / leftSum);
            }

            float rightSum = tr + br;
            if (rightSum > height && rightSum > 0f)
            {
                f = Mathf.Min(f, height / rightSum);
            }

            return new Vector4(tl * f, tr * f, br * f, bl * f);
        }
    }
}
