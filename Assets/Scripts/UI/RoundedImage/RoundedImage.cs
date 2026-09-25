using System;
using UnityEngine;

namespace ProjectArea.UI
{
    [Obsolete("RoundedImage is deprecated. Please use FigmaImage instead.")]
    [AddComponentMenu("UI/Legacy/Rounded Image (Deprecated)", 12)]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class RoundedImage : FigmaImage
    {
        // Inherits all Corner Radius, Stroke, and Image functionality from FigmaImage.
    }
}
