# Changelog

All notable changes to this package are documented in this file.

## [1.1.1] - 2026-09-30

### Fixed

- **Outline Overlay Auto-Maintenance**: Automatically creates and maintains `[FigmaImage_OutlineOverlay]` when a masked `FigmaImage` with stroke contains child elements (e.g. child `Image`), ensuring the stroke renders above child content.
- **Hierarchy Order & Transform Synchronization**: Guaranteed sibling ordering (`Child Content` -> `InnerShadowOverlay` -> `OutlineOverlay`) on hierarchy changes, and fully synchronized RectTransform dimensions, pivots, and CanvasGroup states.
- **Shader Color Precision**: Added rounding offset to `UnpackColor` in `FigmaImage.shader` to prevent 8-bit color truncation issues across different GPU architectures.
- **Acceptance Test Suite**: Expanded to 60 comprehensive unit tests covering child overlay ordering, auto-creation, and cleanup.

## [1.1.0] - 2026-09-29

### Added

- **Inner Shadow**: Figma-style inner shadow effect with configurable Offset (X/Y), Blur, Spread, and Color.
- **Outside Stroke**: Support for outside stroke alignment in addition to inside stroke, with automated drop shadow expansion and masking adaptation.
- **Child Overlay for Inner Shadow**: Inner shadow automatically renders over nested child elements via managed overlay helper.
- **Expanded Test Suite**: Acceptance test suite extended to 58 automated unit tests covering outside stroke, inner shadow, and graphic color preservation.
- Updated Basic Usage sample scene showcasing inside/outside stroke and inner shadow configurations.

## [1.0.0] - 2026-09-28

### Added

- Unity Package Manager compatible repository structure.
- Figma-style corner radius, stroke, drop shadow, masking, and rounded raycast controls.
- Custom inspectors and legacy `RoundedImage` migration support.
- Basic Usage sample scene.
