using System;
using UnityEngine;

namespace ProjectArea.UI
{
    [Serializable]
    public class FigmaInnerShadowSettings
    {
        [SerializeField]
        private bool m_Enabled = false;

        [SerializeField]
        private float m_OffsetX = 0f;

        [SerializeField]
        private float m_OffsetY = 2f;

        [SerializeField]
        private float m_Blur = 4f;

        [SerializeField]
        private float m_Spread = 0f;

        [SerializeField]
        private Color m_Color = new Color(0f, 0f, 0f, 0.25f);

        public FigmaInnerShadowSettings()
        {
            m_Enabled = false;
            m_OffsetX = 0f;
            m_OffsetY = 2f;
            m_Blur = 4f;
            m_Spread = 0f;
            m_Color = new Color(0f, 0f, 0f, 0.25f);
        }

        public FigmaInnerShadowSettings(bool enabled, float offsetX, float offsetY, float blur, float spread, Color color)
        {
            m_Enabled = enabled;
            m_OffsetX = offsetX;
            m_OffsetY = offsetY;
            m_Blur = Mathf.Max(0f, blur);
            m_Spread = spread;
            m_Color = color;
        }

        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public float OffsetX
        {
            get => m_OffsetX;
            set => m_OffsetX = value;
        }

        public float OffsetY
        {
            get => m_OffsetY;
            set => m_OffsetY = value;
        }

        public Vector2 Offset
        {
            get => new Vector2(m_OffsetX, m_OffsetY);
            set
            {
                m_OffsetX = value.x;
                m_OffsetY = value.y;
            }
        }

        public float Blur
        {
            get => m_Blur;
            set => m_Blur = Mathf.Max(0f, value);
        }

        public float Spread
        {
            get => m_Spread;
            set => m_Spread = value;
        }

        public Color Color
        {
            get => m_Color;
            set => m_Color = value;
        }
    }
}
