# Unity Figma-Style Corner Radius — Agent Implementation Brief

## Role

You are a senior Unity UI engineer.

Implement a **Figma-style Corner Radius system for Unity UGUI `Image` components**.

Do not only explain the solution. Inspect the project, implement the required scripts/shaders/editor tooling, resolve compile errors, and verify the feature works in Unity.

---

# Goal

Create a custom Unity UI component that behaves similarly to a normal `UnityEngine.UI.Image`, but adds **Figma-like corner radius controls**.

The final workflow should feel similar to Figma:

- One radius value for all corners.
- Optional independent corner radius values.
- Radius values should visually behave like pixel-based Figma corner radii.
- Radius should remain correct when the RectTransform changes size.
- Excessively large corner radii must be normalized so the shape does not break.
- The component should still behave like a normal Unity UI Image.

Example:

```text
RectTransform:
Width  = 300
Height = 100

Corner Radius:
24
```

The visual result should closely resemble a Figma rectangle sized `300 × 100` with:

```text
Corner radius = 24
```

---

# Target Environment

- Unity 6
- UGUI / `UnityEngine.UI`
- URP-compatible project
- Must work in both Edit Mode and Play Mode
- Must work with normal Canvas UI
- Prefer a shader-based solution instead of baking/modifying textures

Do not convert the feature into UI Toolkit.

---

# Core Component

Create a component named approximately:

```text
FigmaRoundedImage
```

It should inherit from:

```csharp
UnityEngine.UI.Image
```

Do not require the user to add both a normal `Image` and another radius controller component.

The custom component itself should replace the normal Image component.

The component must preserve normal `Image` functionality as much as reasonably possible.

Examples:

- Sprite
- Color
- Material behavior
- Raycast Target
- Maskable
- Preserve Aspect where applicable
- Image Type where compatible
- Buttons using the graphic
- CanvasGroup alpha
- Unity UI masking

---

# Inspector UX

The Inspector should feel simple and similar to Figma.

Provide something conceptually like:

```text
Corner Radius

[ Link Corners ✓ ]

Radius
[ 16 ]

```

When independent corners are enabled:

```text
Corner Radius

[ Link Corners ☐ ]

Top Left
[ 16 ]

Top Right
[ 16 ]

Bottom Right
[ 16 ]

Bottom Left
[ 16 ]
```

Corner order must consistently be:

```text
Top Left
Top Right
Bottom Right
Bottom Left
```

The inspector should update the visual immediately while editing.

Avoid requiring Play Mode to preview changes.

---

# Radius Behavior

Radius values are expressed in UI pixels.

Example:

```text
Image size: 200 × 80
Radius: 20
```

Should visually correspond approximately to:

```text
Figma Rectangle
200 × 80
Corner Radius: 20
```

The result should not depend on the source sprite resolution.

---

# Linked Radius Mode

When corners are linked:

```text
Radius = 24
```

means:

```text
TL = 24
TR = 24
BR = 24
BL = 24
```

Expose a public API similar to:

```text
SetRadius(float radius)
```

The exact method naming may be adapted to the project's conventions.

---

# Independent Corner Mode

Support four independent radii:

```text
Top Left
Top Right
Bottom Right
Bottom Left
```

Example:

```text
TL = 32
TR = 8
BR = 32
BL = 8
```

Expose a runtime API that allows setting all four corners.

Conceptually:

```text
SetCornerRadii(
    topLeft,
    topRight,
    bottomRight,
    bottomLeft
)
```

---

# Radius Normalization

Implement Figma/CSS-like radius normalization.

The component must not visually break if the combined radii exceed the rectangle dimensions.

For example:

```text
Width  = 100
Height = 40

TL = 80
TR = 80
BR = 80
BL = 80
```

Do NOT allow overlapping or inverted corners.

Scale the radii proportionally so all corner combinations fit inside the current rectangle.

Consider these constraints:

```text
TL + TR <= width
BL + BR <= width

TL + BL <= height
TR + BR <= height
```

If one or more constraints are exceeded, calculate a common scale factor and proportionally scale all relevant radii.

Do not simply clamp every radius independently to `min(width, height) / 2`, because independent radii should retain their relative proportions where possible.

---

# Rendering

Prefer a **shader/SDF-style rounded rectangle mask**.

Do not:

- Generate a new Texture2D every frame.
- Modify the original sprite asset.
- Create garbage every frame.
- Depend on Photoshop-generated rounded sprites.
- Require one sprite per radius size.

The rounded corners should have anti-aliasing.

Avoid visibly jagged edges.

Use derivatives such as `fwidth` or another suitable anti-aliasing technique if compatible.

---

# Sprite Support

The custom Image must still display the assigned sprite.

The rounded radius should behave as a mask over the normal Image rendering.

The original sprite UV must remain intact.

Do not replace the sprite UV with normalized rectangle UV.

If additional normalized coordinates are required for the rounded-rectangle calculation, pass them separately through another available vertex channel.

Possible approach:

```text
TEXCOORD0 = original sprite UV
TEXCOORD1 = normalized rectangle coordinates
```

Enable the required Canvas additional shader channel automatically if necessary.

---

# RectTransform Resizing

The corner radius must remain visually correct when:

- Width changes
- Height changes
- Anchors resize the object
- Layout Group changes the object
- Content Size Fitter changes the object
- Resolution / Canvas scale changes

Do not assume RectTransform size is static.

Update shader parameters when dimensions change.

Avoid unnecessary per-frame updates.

Prefer event/dirty-driven updates.

---

# Edit Mode

The feature must preview correctly without entering Play Mode.

Changes that should update immediately include:

- Radius
- Individual corner radii
- RectTransform width
- RectTransform height
- Sprite
- Image color

Use Unity-safe Edit Mode behavior.

Avoid leaking temporary materials in the Editor.

---

# Material Management

Be careful with Unity UI materials.

Requirements:

- Do not accidentally modify a shared project material globally.
- Multiple `FigmaRoundedImage` objects must be able to have different corner radii.
- Avoid persistent temporary material assets unless intentionally created.
- Clean up runtime/editor-created material instances correctly.
- Avoid material leaks when entering/exiting Play Mode repeatedly.

If a better implementation using material modifiers, mesh data, vertex streams, or another batching-safe technique is possible, prefer the robust solution.

---

# UI Masking Compatibility

The component should work with common UGUI masking.

At minimum verify compatibility with:

```text
Mask
RectMask2D
CanvasGroup
```

Respect Unity UI stencil properties required by `Mask`.

Do not create a shader that silently breaks standard UGUI masking.

Base relevant shader behavior on Unity's standard UI shader conventions when appropriate.

---

# Raycast Behavior

Default behavior can continue using the rectangular Image raycast area.

Pixel-perfect rounded-corner raycast rejection is optional.

Do not spend significant complexity on rounded raycast detection unless implementation is straightforward.

---

# Image Types

Priority order:

1. `Image.Type.Simple`
2. `Image.Type.Sliced`
3. Other Image modes if reasonably compatible

`Simple` is mandatory.

If `Sliced` cannot be fully supported without compromising the architecture, clearly document the limitation instead of implementing a fragile workaround.

Do not silently break Image modes.

---

# Performance

This feature may be used on many UI elements.

Requirements:

- No per-frame allocations.
- No Texture2D generation.
- Avoid unnecessary `Update()`.
- Avoid unnecessary material recreation.
- Avoid expensive CPU mesh reconstruction unless Unity already dirties the Graphic.
- Keep shader cost appropriate for mobile.

Target devices include Android.

Favor a lightweight implementation suitable for a mobile game UI.

---

# Suggested Project Structure

Use the project's existing folder conventions if they are clear.

Otherwise use something similar to:

```text
Assets/
└── Scripts/
    └── UI/
        └── RoundedImage/
            ├── FigmaRoundedImage.cs
            └── Editor/
                └── FigmaRoundedImageEditor.cs

Assets/
└── Shaders/
    └── UI/
        └── FigmaRoundedImage.shader
```

A custom Inspector is encouraged if it improves the Figma-like workflow.

Do not create unnecessary files.

---

# Public Runtime API

Provide a clean API for runtime changes.

At minimum support equivalent functionality for:

```text
SetRadius(radius)

SetCornerRadii(
    topLeft,
    topRight,
    bottomRight,
    bottomLeft
)
```

Changing values at runtime should update the visual immediately.

Avoid forcing callers to manually invoke Unity dirty methods.

---

# Serialization

Radius settings must serialize normally in scenes and prefabs.

Duplicating a GameObject or prefab should preserve:

- Linked/unlinked state
- Radius
- Individual corner values

Prefab overrides should behave normally.

---

# Error Handling

If the required shader cannot be found:

- Fail gracefully.
- Log a clear actionable error.
- Do not spam the Console every frame.

Avoid NullReferenceExceptions during:

- Compilation
- Scene loading
- Prefab editing
- Enter Play Mode
- Exit Play Mode
- Domain reload

---

# Acceptance Tests

Before considering the task complete, test the following.

## Test 1 — Standard radius

Create:

```text
Image
Size: 300 × 100
Radius: 24
```

Expected:

- Four corners visually match.
- Result resembles Figma radius 24.
- No visible distortion.

---

## Test 2 — Pill shape

Create:

```text
Size: 200 × 48
Radius: 24
```

Expected:

- Ends become semicircular.
- No corner overlap.

---

## Test 3 — Oversized radius

Create:

```text
Size: 100 × 40
Radius: 100
```

Expected:

- Shape remains valid.
- Radius is normalized.
- No inverted geometry.

---

## Test 4 — Independent corners

Use:

```text
TL = 32
TR = 8
BR = 24
BL = 0
```

Expected:

- All four corners are visibly different.
- Corner mapping is correct.
- No corner is accidentally swapped.

---

## Test 5 — Resize

Start with:

```text
300 × 100
Radius = 24
```

Resize the RectTransform repeatedly.

Expected:

- Rounded corners remain stable.
- Radius stays pixel-based.
- No stale dimensions.

---

## Test 6 — Sprite

Assign a non-white sprite.

Expected:

- Sprite remains visible.
- Sprite UV is correct.
- Rounded clipping only affects the outer shape.
- Sprite is not stretched because of the radius implementation.

---

## Test 7 — Button

Use the component as the Target Graphic of a Unity Button.

Expected:

- Button state tinting works normally.
- Clicking still works.
- Radius remains intact.

---

## Test 8 — Mask

Place the rounded image under:

```text
Mask
```

and separately under:

```text
RectMask2D
```

Expected:

- Standard UI clipping still works.
- No stencil-related visual errors.

---

## Test 9 — Multiple instances

Create at least 10 rounded images with different radii.

Expected:

- Each object keeps its own radius.
- Changing one object does not alter another.
- No unexpected shared-material state.

---

## Test 10 — Edit Mode

Without entering Play Mode:

- Change radius.
- Resize RectTransform.
- Toggle linked corners.

Expected:

- Scene View/Game View updates immediately.

---

# Mobile Considerations

The project targets Android.

Keep the shader lightweight.

Avoid expensive loops, branching-heavy solutions, and unnecessary texture sampling.

One additional rounded-rectangle distance calculation per UI pixel is acceptable.

The implementation should remain practical for menus containing many rounded panels/buttons.

---

# Visual Accuracy Priority

Priority:

```text
Figma-like appearance
    >
clean Unity workflow
    >
performance
    >
supporting unusual Image modes
```

Do not sacrifice basic runtime performance, but prioritize matching Figma's standard circular corner-radius appearance.

---

# Corner Smoothing

Do NOT implement Figma Corner Smoothing / squircle behavior in the first version.

Version 1 only needs standard circular Figma corner radius.

Architect the implementation so corner smoothing could potentially be added later, but do not add unnecessary complexity now.

---

# Do Not Do

Do not solve this by:

- Creating rounded PNG sprites.
- Asking the user to manually make 9-sliced rounded assets.
- Using SpriteMask.
- Using RenderTexture.
- Generating textures at runtime.
- Adding a second camera.
- Rebuilding textures whenever radius changes.
- Using an `Update()` loop just to resend unchanged shader properties.
- Modifying Unity package source code.
- Installing external packages unless absolutely necessary.

---

# Implementation Workflow

Follow this order:

1. Inspect the existing project structure and Unity version.
2. Check whether there is already a custom UI shader or Image extension that should be reused.
3. Design the simplest robust architecture.
4. Implement the rounded Image component.
5. Implement the UI shader.
6. Add a custom Inspector only if useful.
7. Resolve all compile/shader errors.
8. Test in Edit Mode.
9. Test in Play Mode.
10. Test masking and multiple component instances.
11. Check Console for errors/warnings introduced by the implementation.
12. Clean up temporary/test objects if they are not useful project assets.

Do not stop after writing files. Verify that the implementation actually works.

---

# Completion Report

When finished, provide a concise report containing:

```text
Implemented:
- ...

Files created:
- ...

Files modified:
- ...

Supported:
- Simple Image
- ...

Known limitations:
- ...

Validation performed:
- ...
```

If something cannot be fully supported, state it explicitly.

Do not claim a feature was tested unless it was actually verified in the project.

---

# Definition of Done

The task is complete when I can:

1. Add `FigmaRoundedImage` to a Canvas.
2. Assign a sprite/color like a normal Image.
3. Enter a radius such as `16`, `24`, or `32`.
4. See the result immediately in the Editor.
5. Toggle independent corners.
6. Give each corner a different value.
7. Resize the Image without breaking the corners.
8. Use the component on normal mobile UI without generating additional rounded sprites.
