# Figma Image for Unity

`FigmaImage` extends Unity's uGUI `Image` with Figma-style visual controls:

- Uniform or per-corner radius
- Inside stroke with configurable width and color
- Drop shadow with offset, blur, spread, and color
- Rounded raycast filtering
- `Mask` and `RectMask2D` compatibility
- Legacy `RoundedImage` migration support

## Requirements

- Unity 6 (`6000.0`) or newer
- Unity UI (uGUI) 2.0.0
- Input System 1.14.0 for the included sample scene

## Install

In Unity, open **Window > Package Management > Package Manager**, select **Install package from git URL**, and enter:

```text
https://github.com/shirayaka/FigmaImage-Unity.git
```

## Usage

1. Create a component from **GameObject > UI > Figma Image**.
2. Configure the standard Image fields.
3. Configure Corner Radius, Stroke, Effects, and Rounded Raycast in the Inspector.

The public API remains in the `ProjectArea.UI` namespace for compatibility with existing scenes and scripts.

## Sample

Import **Basic Usage** from the package's Samples tab in Package Manager.

## License

MIT. See [LICENSE](LICENSE).
