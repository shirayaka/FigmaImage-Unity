# Unity FigmaImage — Corner Radius + Stroke
## Agent Implementation Brief

## Role

You are a senior Unity UI engineer working inside an existing Unity project.

There is already an existing custom rounded-image implementation or an earlier implementation attempt named approximately:

```text
FigmaRoundedImage
```

Your task is to inspect the current project first, then refactor and extend the system into a reusable component named approximately:

```text
FigmaImage
```

Version 1 of this component must support:

```text
1. Normal Unity UI Image behavior
2. Figma-style Corner Radius
3. Figma-style Inside Stroke
```

Do NOT implement gradients yet.

However, structure the code so a future gradient feature can be added without rewriting the whole renderer.

Do not only explain what should be done. Inspect the project, implement it, resolve errors, and verify the result.

---

# Main Goal

Create a single custom Unity UGUI component that behaves like a normal `UnityEngine.UI.Image`, while adding Figma-like shape controls.

The intended final Inspector workflow should conceptually look like:

```text
Figma Image

Source Image
Color
Raycast Target
Maskable
...

────────────────────

Corner Radius

[ Link Corners ✓ ]

Radius
[ 16 ]

────────────────────

Stroke

[ Enabled ✓ ]

Width
[ 2 ]

Color
[ #FFFFFF ]

Position
Inside
```

The component should feel similar to editing a rectangle in Figma.

---

# Architecture Decision

Use ONE rendering component:

```text
FigmaImage
```

Do NOT make the final GameObject require separate components such as:

```text
Image
RoundedCorner
Stroke
Gradient
```

The rounded corners and stroke affect the same shape, so they should be rendered together.

Internally, settings may be separated into serializable data classes.

Recommended conceptual structure:

```text
FigmaImage
│
├── CornerRadiusSettings
├── StrokeSettings
└── Future:
    └── GradientSettings
```

Possible file layout:

```text
Assets/
└── Scripts/
    └── UI/
        └── FigmaImage/
            ├── FigmaImage.cs
            ├── FigmaCornerRadiusSettings.cs
            ├── FigmaStrokeSettings.cs
            └── Editor/
                └── FigmaImageEditor.cs

Assets/
└── Shaders/
    └── UI/
        └── FigmaImage.shader
```

Adapt this to the project's existing folder conventions if appropriate.

Do not create unnecessary abstraction.

---

# Existing Implementation Migration

Before writing new code:

1. Search the project for:
   - `FigmaRoundedImage`
   - Existing rounded UI shaders
   - Custom Image classes
   - Existing references in scenes/prefabs/scripts

2. Reuse working logic where possible.

3. If `FigmaRoundedImage` already exists and is in active use:
   - Refactor safely.
   - Avoid unnecessarily breaking serialized scene/prefab data.
   - Prefer migration-compatible naming/serialization where practical.
   - If renaming the class would break many existing prefabs, consider a compatibility wrapper or Unity serialization migration strategy.

4. Do not leave duplicate competing rounded-image systems unless there is a strong reason.

The final preferred component name is:

```text
FigmaImage
```

---

# Target Environment

- Unity 6
- UGUI / `UnityEngine.UI`
- URP-compatible project
- Mobile game UI
- Android is an important target
- Must work in Edit Mode and Play Mode

Do not convert this to UI Toolkit.

---

# Base Component

The primary component should inherit from:

```csharp
UnityEngine.UI.Image
```

It should preserve normal Image functionality as much as reasonably possible.

Examples:

```text
Sprite
Color
Material behavior
Raycast Target
Maskable
Preserve Aspect where compatible
CanvasGroup alpha
Button Target Graphic usage
Mask
RectMask2D
```

The user should not need to keep a second normal `Image` component.

---

# Feature 1 — Corner Radius

Keep the existing Figma-style corner-radius behavior.

Support:

```text
Linked Radius

or

Top Left
Top Right
Bottom Right
Bottom Left
```

Corner order must consistently be:

```text
TL
TR
BR
BL
```

Example:

```text
TL = 32
TR = 8
BR = 24
BL = 0
```

must produce four visibly different corners in the correct positions.

---

# Radius Units

Corner radius values represent UI pixels.

Example:

```text
RectTransform:
300 × 100

Radius:
24
```

should visually resemble a Figma rectangle:

```text
300 × 100
Corner Radius = 24
```

The radius must not depend on source sprite resolution.

---

# Radius Normalization

Retain proper proportional radius normalization.

Constraints:

```text
TL + TR <= width
BL + BR <= width

TL + BL <= height
TR + BR <= height
```

If the requested radii exceed the available size, proportionally scale them so the shape remains valid.

Do not simply clamp each independent radius separately unless mathematically necessary.

Examples that must remain valid:

```text
100 × 40
Radius = 100
```

and:

```text
100 × 40

TL = 80
TR = 60
BR = 40
BL = 20
```

No inverted or overlapping corner geometry.

---

# Feature 2 — Stroke

Add a Figma-style stroke system.

Version 1 only needs:

```text
Enabled
Width
Color
Position = Inside
```

Do NOT implement:

```text
Center stroke
Outside stroke
Gradient stroke
Dashed stroke
Per-side stroke
```

Those are outside current scope.

---

# Stroke Inspector

Conceptually:

```text
Stroke

[ Enabled ✓ ]

Width
[ 2 ]

Color
[ White ]

Position
Inside
```

If Stroke is disabled:

- The shader should behave like the previous rounded Image.
- Stroke settings should not visually affect the object.

If Stroke is enabled:

- Stroke should follow all corner radii.
- Stroke should remain fully inside the existing RectTransform.
- The RectTransform size must not change.

---

# Figma-Style Inside Stroke Behavior

The stroke should behave similarly to an inside stroke in Figma.

Example:

```text
Image:
200 × 80

Radius:
24

Stroke:
4 px
```

Expected:

```text
Outer shape remains 200 × 80.
Stroke occupies the inner edge of the rounded shape.
The border follows the same curved contour.
```

Do not simulate stroke by creating four rectangle objects.

Do not use Unity `Outline`.

Do not render stroke using duplicated offset graphics.

Use the same shape-distance information used by the rounded rectangle renderer.

---

# Stroke Rendering

Prefer a rounded-rectangle SDF / signed-distance approach.

Conceptually:

```text
outer distance = rounded rectangle distance

fill:
inside outer shape

stroke:
inside outer shape
AND
within StrokeWidth from its edge
```

The precise implementation may differ if a better approach exists.

Requirements:

- Smooth anti-aliased outer edge.
- Smooth anti-aliased inner stroke edge.
- Stroke follows each independent radius correctly.
- No visible gaps at corners.
- No hard rectangular stroke corners.

---

# Stroke Width Rules

Stroke Width:

```text
>= 0
```

If:

```text
Width = 0
```

the result should behave like no visible stroke.

If the stroke is wider than the shape can reasonably support:

- Clamp or normalize safely.
- Do not create invalid rendering.
- Do not allow negative fill geometry.
- Avoid obvious artifacts.

Example:

```text
Size = 40 × 40
Stroke = 50
```

should remain visually valid.

A reasonable approach is to cap usable inside stroke thickness based on the current rectangle dimensions.

Document any exact clamp rule used.

---

# Stroke and Corner Radius Relationship

Be careful with inner corner geometry.

For an inside stroke, the inner radius should remain visually coherent.

Conceptually, for many cases:

```text
Inner Radius ≈ Outer Radius - Stroke Width
```

with a minimum of zero.

However:

- Handle independent radii correctly.
- Avoid sharp artifacts when Radius < Stroke Width.
- Do not create negative radius values.

Examples:

```text
Radius = 24
Stroke = 4
```

should produce a smooth rounded border.

```text
Radius = 4
Stroke = 10
```

should still remain stable and visually sensible.

---

# Fill Color vs Stroke Color

The component should support:

```text
Image / Fill Color
Stroke Color
```

These are separate concepts.

Example:

```text
Image Color:
#F8F3ED

Stroke Color:
#61563A
```

The stroke color must not overwrite the base Image color.

The source sprite should still render normally inside the shape.

---

# Sprite Support

The assigned Image sprite must continue to work.

Requirements:

- Preserve original sprite UV.
- Do not replace sprite UV with shape UV.
- Rounded clipping and stroke must be layered over normal Image rendering.
- Do not distort the sprite just to support the stroke.

If normalized rectangle coordinates are required, use a separate vertex stream.

Conceptually:

```text
TEXCOORD0 = original Image sprite UV
TEXCOORD1 = normalized shape coordinate
```

Enable required Canvas additional shader channels automatically if needed.

---

# Rendering Composition

Conceptually, Version 1 rendering should work like:

```text
Source Sprite / Image Color
          ↓
Rounded Rectangle Shape
          ↓
Inside Stroke
          ↓
Canvas Masking / UI clipping
          ↓
Final UI Output
```

Gradient is NOT part of this version.

---

# Future Gradient Compatibility

Do not implement gradient rendering yet.

However, avoid architecture that would make gradient support difficult later.

Future target concept:

```text
FigmaImage
│
├── Fill
│   ├── Solid
│   └── Linear Gradient
│
├── Corner Radius
└── Stroke
```

For now, normal `Image.color` / sprite rendering remains the Fill.

Do not add unused gradient UI fields.

Do not add unfinished gradient shader code.

Only keep architecture reasonably extensible.

---

# Edit Mode Preview

The following changes should update immediately without entering Play Mode:

```text
Radius
Linked / independent radius
Stroke Enabled
Stroke Width
Stroke Color
RectTransform size
Image Color
Sprite
```

Avoid requiring manual refresh.

Use Unity-safe Edit Mode logic.

---

# RectTransform Resizing

Corner radius and stroke must remain correct when size changes through:

```text
Manual RectTransform resizing
Anchors
Layout Group
Content Size Fitter
Different resolutions
Canvas scaling
```

Avoid per-frame polling if unnecessary.

Prefer dirty/event-driven updates.

---

# Shader Requirements

The shader must follow Unity UI conventions where appropriate.

Support:

```text
UGUI stencil masking
Mask
RectMask2D
CanvasGroup alpha
UI clipping
Sprite atlas usage where applicable
```

Do not make a shader that looks correct alone but breaks standard Unity UI masking.

---

# Material Handling

This is important.

Multiple FigmaImage instances may have different:

```text
Radius
Stroke Enabled
Stroke Width
Stroke Color
```

Changing one component must not affect another.

Requirements:

- Do not mutate a shared project material globally.
- Avoid creating new materials every frame.
- Avoid leaking materials in the Editor.
- Avoid leaking materials through repeated Play Mode entry/exit.
- Reuse or manage material instances safely.
- Consider batching implications.

If using per-instance material properties is difficult in UGUI, choose the most robust practical architecture.

Correct behavior is more important than forcing batching at all costs.

---

# Performance

This component will be used frequently in Pocket Acres UI.

Target use cases include:

```text
Inventory slots
Buttons
Shop cards
Dialog panels
Tooltips
Menus
Settings panels
HUD elements
```

Requirements:

- No Texture2D generation.
- No runtime sprite baking.
- No per-frame allocations.
- Avoid Update() unless genuinely required.
- No unnecessary mesh rebuild loops.
- Keep fragment shader cost lightweight.
- Suitable for Android mobile devices.

---

# Anti-Aliasing

Rounded corners and stroke edges must be smooth.

Use derivative-based anti-aliasing such as:

```text
fwidth
```

or another suitable lightweight method.

Avoid visibly jagged corners on common mobile UI resolutions.

---

# Raycast Behavior

Default rectangular Image raycast behavior is acceptable for Version 1.

Rounded-shape-aware raycast rejection is optional.

Do not spend unnecessary complexity on raycast shape filtering.

---

# Image Type Support

Priority:

```text
1. Image.Type.Simple — mandatory
2. Image.Type.Sliced — desirable
3. Other Image types — optional
```

Do not silently claim unsupported Image modes work.

If a mode cannot be supported reliably, document that limitation.

---

# Public API

Provide clean runtime APIs.

At minimum equivalent functionality for:

```text
SetRadius(float radius)

SetCornerRadii(
    float topLeft,
    float topRight,
    float bottomRight,
    float bottomLeft
)

SetStrokeEnabled(bool enabled)

SetStrokeWidth(float width)

SetStrokeColor(Color color)
```

Exact naming may follow project conventions.

Changing values at runtime must update visuals automatically.

The caller should not need to manually call Unity dirty functions.

---

# Serialization

Scene and prefab serialization must preserve:

```text
Linked corner state
Radius
Independent radii
Stroke enabled state
Stroke width
Stroke color
```

Prefab overrides should behave normally.

---

# Custom Inspector

A custom Inspector is recommended.

Keep it simple.

Possible layout:

```text
Image
  Source Image
  Color
  Material
  Raycast Target
  Maskable
  ...

Corner Radius
  Link Corners
  Radius

or

  Top Left
  Top Right
  Bottom Right
  Bottom Left

Stroke
  Enabled
  Width
  Color
  Position: Inside
```

Do not clutter the Inspector with future gradient controls yet.

If possible, hide irrelevant stroke fields while Stroke is disabled.

---

# Validation Tests

Perform the following tests.

---

## Test 1 — Rounded Image without Stroke

```text
Size:
300 × 100

Radius:
24

Stroke:
Disabled
```

Expected:

- Same visual behavior as existing rounded Image.
- No visual regression.

---

## Test 2 — Simple Stroke

```text
Size:
300 × 100

Radius:
24

Stroke:
Enabled

Width:
2

Color:
White
```

Expected:

- 2px inside border.
- Smooth corners.
- Stroke follows rounded shape.

---

## Test 3 — Thick Stroke

```text
Size:
200 × 80

Radius:
24

Stroke:
12
```

Expected:

- Stable inner border.
- No gaps.
- No inverted geometry.

---

## Test 4 — Independent Corner Radius + Stroke

```text
TL = 32
TR = 8
BR = 24
BL = 0

Stroke = 4
```

Expected:

- Stroke correctly follows every unique corner.
- Corner mapping is correct.

---

## Test 5 — Pill

```text
Size:
200 × 48

Radius:
24

Stroke:
3
```

Expected:

- Smooth pill shape.
- Stroke follows semicircular ends.

---

## Test 6 — Radius Smaller than Stroke

```text
Radius:
4

Stroke:
10
```

Expected:

- No broken corners.
- No negative inner radius artifacts.
- Stable rendering.

---

## Test 7 — Oversized Radius

```text
Size:
100 × 40

Radius:
100

Stroke:
4
```

Expected:

- Radius normalization works.
- Stroke remains valid.

---

## Test 8 — Oversized Stroke

```text
Size:
40 × 40

Radius:
8

Stroke:
50
```

Expected:

- No shader explosion.
- No invalid values.
- Result remains visually stable.

---

## Test 9 — Sprite

Assign a textured sprite.

Expected:

- Sprite UV remains correct.
- Rounded clipping works.
- Stroke is independent from sprite color.
- No unexpected stretching.

---

## Test 10 — Button

Use `FigmaImage` as a Unity Button Target Graphic.

Expected:

- Button interaction works.
- Tint transitions still work.
- Stroke remains visible.
- Radius remains correct.

---

## Test 11 — Mask

Test under:

```text
Mask
```

and:

```text
RectMask2D
```

Expected:

- Standard UGUI masking remains functional.
- No stencil artifacts.

---

## Test 12 — Multiple FigmaImages

Create at least 10 instances with different:

```text
Radius
Stroke Width
Stroke Color
```

Expected:

- No shared-state bugs.
- Changing one does not change others.

---

## Test 13 — Resize

Resize RectTransform continuously.

Expected:

- Radius stays pixel-based.
- Stroke stays pixel-based.
- Shape remains stable.
- No stale shader size parameters.

---

## Test 14 — Edit Mode

Without entering Play Mode:

- Change radius.
- Toggle linked radius.
- Enable/disable Stroke.
- Change Stroke Width.
- Change Stroke Color.
- Resize RectTransform.

Expected:

- Visual updates immediately.

---

# Console / Error Requirements

Before completion:

- No new C# compile errors.
- No shader compile errors.
- No recurring warnings introduced by the component.
- No NullReferenceException on scene load.
- No material leak warnings.
- No error spam if shader lookup fails.

If shader is missing:

- Fail gracefully.
- Log one clear actionable error.
- Do not spam every frame.

---

# Do Not Do

Do NOT solve Stroke using:

```text
Unity Outline
Duplicated UI Image objects
Four child rectangles
9-sliced border sprites
Runtime-generated PNGs
Texture2D generation
RenderTexture
SpriteMask
Second camera
Per-frame material recreation
Per-frame texture modification
```

Do not install third-party packages unless absolutely necessary.

Do not modify Unity package source code.

---

# Scope Exclusions

Do NOT implement yet:

```text
Gradient fill
Gradient stroke
Corner smoothing / squircle
Outside stroke
Center stroke
Dashed stroke
Shadow
Blur
Per-side stroke
Multiple strokes
```

These can be future versions.

---

# Implementation Order

Follow this order:

1. Inspect current `FigmaRoundedImage` implementation.
2. Inspect shader and editor code.
3. Identify existing usages in scenes/prefabs/scripts.
4. Decide safe migration path to `FigmaImage`.
5. Preserve working rounded-corner behavior.
6. Add Stroke settings.
7. Add inside-stroke shader rendering.
8. Add Inspector controls.
9. Test Edit Mode.
10. Test Play Mode.
11. Test Sprite.
12. Test Button.
13. Test Mask and RectMask2D.
14. Test multiple FigmaImage instances.
15. Check material lifecycle.
16. Check Console.
17. Remove temporary test artifacts unless useful.
18. Report exactly what was changed.

Do not stop after simply generating code.

Verify the implementation inside the project.

---

# Completion Report

When finished, respond using a concise report:

```text
Implemented:
- FigmaImage component
- Corner Radius
- Inside Stroke
- ...

Migrated:
- ...

Files created:
- ...

Files modified:
- ...

Supported:
- Image.Type.Simple
- Mask
- RectMask2D
- Button
- ...

Known limitations:
- ...

Validation performed:
- ...
```

Do not claim something was tested unless it was actually tested.

---

# Definition of Done

The task is complete when I can create a UI object with one component:

```text
FigmaImage
```

and do all of the following:

```text
1. Assign a normal sprite.
2. Change Image color.
3. Set Corner Radius = 16 / 24 / 32.
4. Use different values for each corner.
5. Enable Stroke.
6. Set Stroke Width in pixels.
7. Set Stroke Color independently.
8. Resize the RectTransform.
9. Preview everything in Edit Mode.
10. Use the object as a normal Unity UI Button graphic.
11. Use it inside Mask / RectMask2D.
12. Have multiple FigmaImage objects with different settings.
```

The final visual should feel like a lightweight Figma rectangle renderer built directly into Unity UGUI.

---

# Future Direction

Do not implement this now, but keep the architecture compatible with a future Version 2:

```text
FigmaImage

Fill
├── Solid
└── Linear Gradient

Corner Radius

Stroke
```

For the current task, stop after a robust:

```text
Corner Radius + Inside Stroke
```

implementation.
