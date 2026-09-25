# Unity FigmaImage — Drop Shadow Addition
## Agent Implementation Brief

## Role

You are a senior Unity UI engineer working inside an existing Unity 6 project.

The project already has a custom UGUI component named approximately:

```text
FigmaImage
```

It already supports:

```text
- Normal Unity UI Image behavior
- Figma-style Corner Radius
- Figma-style Inside Stroke
```

Your task is to add:

```text
Figma-style Drop Shadow
```

Do not rewrite unrelated systems.

Do not implement gradients in this task.

Inspect the current implementation first, then modify it safely, resolve compile/shader errors, and verify the result inside Unity.

---

# Main Goal

Extend `FigmaImage` so it can render ONE configurable Drop Shadow layer similar to Figma.

Conceptual Inspector:

```text
Effects

Drop Shadow
[ Enabled ✓ ]

X
[ 0 ]

Y
[ 4 ]

Blur
[ 8 ]

Spread
[ 0 ]

Color
[ #00000040 ]
```

The shadow must follow the actual `FigmaImage` outer shape, including:

```text
- Uniform Corner Radius
- Independent Corner Radius
- Radius normalization
- Pill shapes
```

The shadow must remain visually separate from Fill and Stroke.

---

# Existing System

Before changing code:

1. Inspect:
   - `FigmaImage.cs`
   - Existing FigmaImage shader
   - Existing custom Inspector
   - Corner Radius settings
   - Stroke settings
   - Mesh generation
   - UV / additional vertex channel usage
   - Material lifecycle / instancing

2. Preserve all existing behavior.

3. Reuse the current rounded-rectangle SDF logic if available.

4. Do not create a separate shadow component if the effect can cleanly belong to `FigmaImage`.

Preferred architecture:

```text
FigmaImage
│
├── Fill
├── Corner Radius
├── Stroke
└── Effects
    └── Drop Shadow
```

A serializable internal settings class such as:

```text
FigmaDropShadowSettings
```

is encouraged if it fits the current codebase.

---

# Scope

Version 1 supports ONE Drop Shadow:

```text
Enabled
Offset X
Offset Y
Blur
Spread
Color
```

Do NOT implement:

```text
Multiple Drop Shadows
Inner Shadow
Gradient Shadow
Blend Modes
Noise
Glow presets
Shadow presets
```

---

# Figma-Style Coordinate Convention

The Inspector must behave like Figma:

```text
Positive X = move shadow right
Negative X = move shadow left

Positive Y = move shadow down
Negative Y = move shadow up
```

Unity UI local Y usually increases upward.

Convert this internally.

Do not expose inverted Unity Y behavior to the user.

---

# Drop Shadow Settings

Recommended conceptual data:

```text
Drop Shadow
    Enabled
    Offset X
    Offset Y
    Blur
    Spread
    Color
```

Constraints:

```text
Blur >= 0
```

Spread may be:

```text
positive
zero
negative
```

if negative spread can be supported robustly.

If negative Spread is unsafe with the chosen implementation, clamp it and document the limitation.

---

# Inspector UX

Add a section below Stroke:

```text
Corner Radius
...

Stroke
...

Effects

Drop Shadow
[ Enabled ]

X
[ 0 ]

Y
[ 4 ]

Blur
[ 8 ]

Spread
[ 0 ]

Color
[ Black, 25% Alpha ]
```

If Drop Shadow is disabled, hide or disable irrelevant fields if practical.

Do not add Gradient controls.

---

# Rendering Architecture

Prefer an analytic rounded-rectangle SDF approach.

Reuse the same outer shape model already used by Corner Radius.

Conceptually:

```text
Rounded Rectangle SDF
        ↓
Shadow Offset
        ↓
Spread
        ↓
Blur Falloff
        ↓
Shadow Color
        ↓
Composite behind Fill and Stroke
```

Do NOT use:

```text
UnityEngine.UI.Shadow
Duplicated Image
Duplicated GameObject
Multiple offset copies
Runtime-generated textures
RenderTexture
Second camera
Post-processing
```

---

# Shadow Source Shape

The shadow source is:

```text
the OUTER rounded rectangle of FigmaImage
```

It is NOT:

```text
the inside edge of Stroke
```

Stroke must not change the basic shadow source silhouette.

Independent corner radius must be respected.

Example:

```text
TL = 32
TR = 8
BR = 24
BL = 0
```

The shadow silhouette must follow those exact corner shapes.

---

# Offset

Offset only applies to the shadow.

The Fill, Sprite, Corner Radius shape, and Stroke must stay in their original positions.

Expected:

```text
X = +10
→ shadow moves 10px right

X = -10
→ shadow moves 10px left

Y = +10
→ shadow moves 10px visually downward

Y = -10
→ shadow moves 10px visually upward
```

Verify this visually in Unity.

Do not assume sign convention without testing.

---

# Spread

Spread changes the shadow source shape before blur.

Expected behavior:

```text
Spread > 0
→ larger shadow body

Spread = 0
→ based on original outer shape

Spread < 0
→ smaller shadow body
```

An SDF implementation may conceptually use:

```text
shadowDistance = shapeDistance - spread
```

or the inverse depending on the existing distance sign convention.

Verify the result visually.

---

# Blur

Blur should soften the shadow edge.

Expected:

```text
Blur = 0
→ hard edge

Blur = 4
→ slightly soft

Blur = 8
→ medium soft

Blur = 16
→ clearly soft

Blur = 24+
→ wide soft shadow
```

Exact Gaussian equivalence to Figma is NOT required.

However, common values should feel visually similar.

Prefer a lightweight SDF falloff.

Do NOT use expensive multi-sample Gaussian loops.

Target Android performance.

---

# Critical Requirement — Shadow Must Render Outside RectTransform

A standard Unity `Image` quad normally only covers its RectTransform.

A Drop Shadow with:

```text
Offset
Blur
Spread
```

must extend outside the original shape.

The implementation MUST account for this.

Do NOT accept a solution where the shadow is clipped at the Image's own mesh boundary.

---

# Expand Render Geometry, Not Layout

Preferred approach:

Expand the rendered UI mesh / geometry around the original RectTransform.

Do NOT resize the actual RectTransform.

Example:

```text
RectTransform:
200 × 80
```

With shadow enabled, layout systems must still see:

```text
200 × 80
```

even if the visible rendering extends to:

```text
approximately 230 × 110
```

because of shadow blur/offset.

The shadow geometry expansion must NOT alter:

```text
Layout Group calculations
Anchors
Preferred size
Button hit area
Original visual Fill size
```

---

# Shadow Padding

Calculate mesh expansion independently on every side.

Conceptually determine:

```text
leftPadding
rightPadding
topPadding
bottomPadding
```

using:

```text
Offset X
Offset Y
Blur
positive Spread
```

Example concept:

```text
extent = Blur + max(Spread, 0)

leftPadding =
extent + max(-OffsetX, 0)

rightPadding =
extent + max(OffsetX, 0)
```

Vertical logic must account for Figma's positive-Y-down convention.

Do not blindly copy these formulas if the current SDF mapping requires additional safety margin.

Verify no shadow cutoff at large Blur values.

---

# Fill and Stroke Must Not Expand

Expanded shadow geometry is only rendering space.

The actual:

```text
Fill
Sprite
Stroke
Rounded rectangle outer boundary
```

must stay aligned to the original RectTransform.

Do not stretch the visible Image into the shadow padding.

---

# Sprite UV Preservation

This is critical.

If the mesh is expanded:

- Original sprite UV must remain correct.
- Do not stretch sprite UV into shadow padding.
- Do not break Sprite Atlas UVs.
- Do not modify the original sprite asset.

If the existing system uses:

```text
TEXCOORD0 = sprite UV
TEXCOORD1 = normalized shape coordinate
```

preserve that design or extend it carefully.

The shader may need explicit local shape coordinates or original-shape bounds.

---

# Shape Coordinate System

Keep a clear distinction between:

```text
Original RectTransform shape
Expanded render mesh
Shadow position
```

The shader should know enough information to reconstruct:

```text
original shape center
original shape size
corner radii
shadow offset
spread
blur
```

Do not center the Fill on the expanded mesh accidentally.

---

# Render Order

Within one `FigmaImage`, visual order must be:

```text
1. Drop Shadow
2. Fill / Sprite
3. Inside Stroke
```

Shadow must render behind the Fill.

Stroke must remain crisp above both.

Example:

```text
Fill: cream
Stroke: white 2px
Shadow: black 25%, Y 6, Blur 12
```

Expected:

```text
soft black shadow
cream panel above it
white stroke on top
```

---

# Alpha Composition

Be careful with alpha.

Avoid:

```text
dark halo artifacts
double-darkening under opaque Fill
incorrect premultiplied alpha
shadow showing on top of Fill
```

Composite shadow under the shape correctly.

Use alpha math consistent with the shader's current blend mode.

---

# Shadow Color

Support full RGBA.

Do not force black-only shadow.

Examples that must work:

```text
Black 25%
Brown 20%
Blue 30%
```

Even though the feature is called Drop Shadow, colored shadow should remain possible.

---

# Interaction with Stroke

Stroke remains unaffected by shadow settings.

Example:

```text
Radius = 24

Stroke:
Enabled
Width = 3
Color = White

Shadow:
X = 0
Y = 6
Blur = 12
Spread = 0
Color = Black 25%
```

Expected:

```text
shadow behind outer shape
white stroke remains crisp
```

Shadow must not derive color from Stroke.

---

# Interaction with Corner Radius

Shadow must work with:

```text
Linked radius
Independent corners
Pill radius
Oversized radius normalization
```

Do not downgrade independent corners to one average radius.

---

# RectTransform Resizing

Shadow must update correctly when size changes because of:

```text
Manual resizing
Anchors
Layout Group
Content Size Fitter
Resolution change
Canvas scaling
```

Update:

```text
mesh padding
shape size
shader data
```

when needed.

Avoid unnecessary per-frame polling.

Prefer Unity dirty callbacks / geometry rebuilds.

---

# Edit Mode

Everything must preview without entering Play Mode.

Changing any of these should update immediately:

```text
Drop Shadow Enabled
Offset X
Offset Y
Blur
Spread
Color
Corner Radius
Stroke
RectTransform size
Sprite
Image color
```

Avoid material leaks in Edit Mode.

---

# Mask and RectMask2D

Standard hierarchy clipping must still work.

If a parent:

```text
Mask
RectMask2D
```

clips the shadow, that is acceptable and expected.

Do not bypass normal UGUI parent clipping.

Maintain stencil compatibility.

---

# Raycast

Drop Shadow must NOT increase the clickable area.

Raycast behavior should remain based on the original RectTransform / Image area.

Expanded render geometry must not make invisible shadow padding clickable.

---

# Material Management

Different `FigmaImage` instances may have unique:

```text
Corner Radius
Stroke
Shadow Offset
Shadow Blur
Shadow Spread
Shadow Color
```

Changing one must not affect others.

Do not:

```text
modify shared material globally
recreate materials every frame
leak materials in Edit Mode
leak materials during Play Mode transitions
```

Reuse the existing robust material strategy.

---

# Performance

This is intended for Pocket Acres mobile UI.

Potential use cases:

```text
Inventory cards
Shop cards
Buttons
Dialogue panels
HUD
Settings panels
Tooltips
Menus
```

Requirements:

```text
No Texture2D generation
No RenderTexture
No multi-pass Gaussian blur
No duplicated UI objects
No per-frame allocations
No unnecessary Update()
No per-frame material recreation
```

Prefer one lightweight analytic SDF shadow evaluation.

---

# Runtime API

Provide runtime API equivalent to:

```text
SetDropShadowEnabled(bool enabled)

SetDropShadowOffset(float x, float y)

SetDropShadowBlur(float blur)

SetDropShadowSpread(float spread)

SetDropShadowColor(Color color)
```

Exact naming may follow existing project style.

Runtime changes must update visuals automatically.

Caller should not need to manually call Unity dirty methods.

---

# Serialization

Drop Shadow values must serialize properly in:

```text
Scenes
Prefabs
Prefab variants
Duplicated GameObjects
```

Preserve:

```text
Enabled
Offset X
Offset Y
Blur
Spread
Color
```

Prefab overrides should work normally.

---

# Validation Tests

## Test 1 — Standard Shadow

```text
Size:
200 × 80

Radius:
16

Shadow:
Enabled

X:
0

Y:
4

Blur:
8

Spread:
0

Color:
Black 25%
```

Expected:

```text
soft shadow mostly visible below
no clipping by FigmaImage's own mesh
```

---

## Test 2 — Right Offset

```text
X = 12
Y = 0
Blur = 8
```

Expected:

```text
shadow moves right
Fill does not move
```

---

## Test 3 — Left Offset

```text
X = -12
```

Expected:

```text
shadow moves left
```

---

## Test 4 — Figma Y Convention

```text
Y = 12
```

Expected:

```text
shadow moves visually downward
```

```text
Y = -12
```

Expected:

```text
shadow moves visually upward
```

---

## Test 5 — Blur Zero

```text
Blur = 0
```

Expected:

```text
hard shadow
no divide-by-zero
no NaN
```

---

## Test 6 — Large Blur

```text
Blur = 32
```

Expected:

```text
wide soft shadow
no self-clipping
```

---

## Test 7 — Positive Spread

Compare:

```text
Spread = 0
```

with:

```text
Spread = 6
```

Expected:

```text
Spread 6 creates a larger shadow body
```

---

## Test 8 — Independent Corners

```text
TL = 32
TR = 8
BR = 24
BL = 0
```

Expected:

```text
shadow follows every corner correctly
```

---

## Test 9 — Pill

```text
Size:
200 × 48

Radius:
24

Shadow:
0, 6, 12, 0
```

Expected:

```text
shadow follows pill silhouette smoothly
```

---

## Test 10 — Stroke + Shadow

```text
Radius:
20

Stroke:
Enabled
Width = 3
Color = White

Shadow:
X = 0
Y = 6
Blur = 12
Spread = 0
Color = Black 25%
```

Expected:

```text
shadow behind shape
Stroke remains sharp
```

---

## Test 11 — Large Offset

```text
X = 30
Y = 30
Blur = 16
```

Expected:

```text
shadow remains fully visible
no cutoff caused by local mesh bounds
```

---

## Test 12 — Resize

Resize the RectTransform repeatedly.

Expected:

```text
Fill remains aligned
Corner Radius remains correct
Stroke remains correct
Shadow remains aligned
mesh padding updates correctly
```

---

## Test 13 — Button

Use FigmaImage as a Unity Button Target Graphic.

Expected:

```text
Button interaction still works
Tint transitions still work
Shadow remains behind
Click area does not expand to shadow padding
```

---

## Test 14 — Mask / RectMask2D

Test under:

```text
Mask
RectMask2D
```

Expected:

```text
normal clipping behavior
no stencil artifacts
```

---

## Test 15 — Multiple Instances

Create at least 10 FigmaImages with different:

```text
Offset
Blur
Spread
Color
Radius
Stroke
```

Expected:

```text
no shared-state bugs
```

---

## Test 16 — Edit Mode

Without entering Play Mode, change:

```text
Enabled
X
Y
Blur
Spread
Color
RectTransform size
```

Expected:

```text
immediate visual update
```

---

# Edge Cases

Handle safely:

```text
Blur = 0
Very large Blur
Spread = 0
Large positive Spread
Negative Spread
Very large Offset
Very small RectTransform
Large Corner Radius
Large Stroke
Shadow alpha = 0
Shadow disabled
```

Never allow:

```text
NaN
Infinity
negative blur
broken geometry
material leaks
error spam
```

---

# Do Not Do

Do NOT implement this using:

```text
Unity UI Shadow component
Unity UI Outline component
Duplicated child Image
Duplicated GameObject
Multiple shadow copies
Runtime-generated PNG
Texture2D generation
RenderTexture
Camera blur
Post-processing
Third-party shadow package
```

Do not modify Unity package source code.

Do not install external dependencies unless absolutely necessary.

---

# Scope Exclusions

Do NOT implement:

```text
Inner Shadow
Multiple shadows
Gradient fill
Gradient shadow
Corner smoothing / squircle
Glow
Background blur
Multi-pass Gaussian blur
Outside Stroke
Center Stroke
```

This task is ONLY:

```text
Add one Figma-style Drop Shadow to the existing FigmaImage.
```

---

# Implementation Order

1. Inspect existing `FigmaImage`.
2. Inspect current shader.
3. Inspect current mesh and UV handling.
4. Inspect material lifecycle.
5. Add Drop Shadow settings structure.
6. Determine mesh padding requirements.
7. Expand render geometry without changing RectTransform layout.
8. Preserve original Fill and Sprite mapping.
9. Implement shadow shape from existing rounded-rectangle SDF.
10. Implement X/Y offset.
11. Implement Spread.
12. Implement Blur.
13. Implement RGBA shadow color.
14. Composite shadow behind Fill and Stroke.
15. Add Inspector controls.
16. Add runtime API.
17. Test Edit Mode.
18. Test Play Mode.
19. Test large Blur.
20. Test large Offset.
21. Test independent Corner Radius.
22. Test Stroke + Shadow.
23. Test Button.
24. Test Mask and RectMask2D.
25. Test multiple instances.
26. Inspect Console.
27. Verify no material leak.
28. Remove temporary test artifacts if not useful.
29. Report exactly what changed.

Do not stop after writing code.

Verify the implementation in the actual Unity project.

---

# Completion Report

When finished, respond with:

```text
Implemented:
- Figma-style Drop Shadow
- X / Y Offset
- Blur
- Spread
- RGBA Color
- Expanded render geometry
- ...

Files created:
- ...

Files modified:
- ...

Existing behavior preserved:
- Corner Radius
- Inside Stroke
- Sprite rendering
- Button
- Mask
- RectMask2D
- ...

Validation performed:
- ...

Known limitations:
- ...
```

Do not claim something was tested unless it was actually verified.

---

# Definition of Done

The task is complete when an existing `FigmaImage` can be configured like:

```text
Corner Radius
24

Stroke
Enabled
2 px
White

Drop Shadow
Enabled

X
0

Y
6

Blur
12

Spread
0

Color
Black / 25%
```

and the result:

```text
- resembles a Figma-style rounded card
- has a smooth shadow behind it
- does not clip against its own render mesh
- does not change RectTransform layout size
- does not expand the Button click area
- preserves Sprite and Fill
- preserves Stroke
- works in Edit Mode
- works in Play Mode
- respects Mask / RectMask2D
- remains practical for Android mobile UI
```

The Drop Shadow should feel like a native effect built into `FigmaImage`, not an external workaround.
