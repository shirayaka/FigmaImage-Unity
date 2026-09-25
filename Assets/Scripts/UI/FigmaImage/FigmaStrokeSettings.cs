using System;
using UnityEngine;

namespace ProjectArea.UI
{
    public enum FigmaStrokePosition
    {
        Inside = 0
    }

    [Serializable]
    public class FigmaStrokeSettings
    {
        [SerializeField]
        private bool m_Enabled = false;

        [SerializeField]
        private float m_Width = 1f;

        [SerializeField]
        private Color m_Color = Color.white;

        [SerializeField]
        private FigmaStrokePosition m_Position = FigmaStrokePosition.Inside;

        [SerializeField]
        private bool m_IgnoreInMask = false;

        public FigmaStrokeSettings()
        {
            m_Enabled = false;
            m_Width = 1f;
            m_Color = Color.white;
            m_Position = FigmaStrokePosition.Inside;
            m_IgnoreInMask = false;
        }

        public FigmaStrokeSettings(bool enabled, float width, Color color, FigmaStrokePosition position = FigmaStrokePosition.Inside, bool ignoreInMask = false)
        {
            m_Enabled = enabled;
            m_Width = Mathf.Max(0f, width);
            m_Color = color;
            m_Position = position;
            m_IgnoreInMask = ignoreInMask;
        }

        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public float Width
        {
            get => m_Width;
            set => m_Width = Mathf.Max(0f, value);
        }

        public Color Color
        {
            get => m_Color;
            set => m_Color = value;
        }

        public FigmaStrokePosition Position
        {
            get => m_Position;
            set => m_Position = value;
        }

        public bool IgnoreInMask
        {
            get => m_IgnoreInMask;
            set => m_IgnoreInMask = value;
        }
    }
}
