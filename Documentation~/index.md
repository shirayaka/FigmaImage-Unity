# Figma Image

Figma Image is a uGUI `Image` replacement for interfaces that need Figma-style corner radii, inside strokes, and drop shadows without baking those effects into sprites.

## Create a Figma Image

Use **GameObject > UI > Figma Image**. The command creates a Canvas and EventSystem when the current scene does not already contain one.

## Corner Radius

Enable **Link Corners** for a uniform radius, or disable it to edit the top-left, top-right, bottom-right, and bottom-left values independently. Radii are normalized when their combined size exceeds the rectangle dimensions.

## Stroke

Enable **Stroke** to draw a stroke. Select the **Position** alignment:
- **Inside**: Goresan digambar di dalam batas kontur shape.
- **Outside**: Goresan digambar di luar batas kontur shape, dengan drop shadow otomatis menyesuaikan batas luar stroke.

Configure width and color. **Ignore In Mask** keeps the stroke visible while child content is clipped inside its inner edge.

## Drop Shadow

Enable **Drop Shadow** and configure X/Y offset, blur, spread, and color. Positive Y follows Figma's convention and moves the shadow downward.

## Inner Shadow

Enable **Inner Shadow** and configure X/Y offset, blur, spread, and color. Inner shadows seamlessly cast inside the shape's inner contour and render on top of nested child elements via an automatic overlay helper.

## Rounded Raycast

Enable **Rounded Raycast** to reject pointer events outside the rounded shape.

## Runtime API

```csharp
using ProjectArea.UI;

FigmaImage image = GetComponent<FigmaImage>();
image.SetCornerRadii(24f, 12f, 24f, 12f);

// Stroke configuration
image.SetStrokeEnabled(true);
image.SetStrokeWidth(2f);
image.SetStrokePosition(FigmaStrokePosition.Outside);

// Drop Shadow configuration
image.SetDropShadowEnabled(true);
image.SetDropShadowOffset(0f, 6f);
image.SetDropShadowBlur(12f);

// Inner Shadow configuration
image.SetInnerShadowEnabled(true);
image.SetInnerShadowOffset(0f, 2f);
image.SetInnerShadowBlur(8f);
image.SetInnerShadowSpread(2f);
```

## Sample

Import **Basic Usage** from Package Manager to install the demonstration scene into the project's `Assets/Samples` folder.
