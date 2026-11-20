using System.Drawing;

namespace CEF.Properties;

// Toolbar icons: 24x24 .bmp embedded as CEF.ui.Resources.<name>.
// Requires "partial" in the declaration of Resources in Resources.cs (internal partial class Resources).
// A missing file returns null (the button falls back to text) instead of throwing.
internal partial class Resources
{
    private static Bitmap TryGetBitmap(string fileName)
    {
        if (CurrentAssembly.GetManifestResourceInfo(ResourcePrefix + fileName) == null) return null;
        return GetBitmap(fileName);
    }

    // File
    internal static Bitmap FileNew => TryGetBitmap("FileNew.bmp");
    internal static Bitmap FileOpen => TryGetBitmap("FileOpen.bmp");
    internal static Bitmap FileSave => TryGetBitmap("FileSave.bmp");

    // Edit
    internal static Bitmap EditUndo => TryGetBitmap("EditUndo.bmp");
    internal static Bitmap EditCopy => TryGetBitmap("EditCopy.bmp");
    internal static Bitmap EditReplicate => TryGetBitmap("EditReplicate.bmp");
    internal static Bitmap EditMove => TryGetBitmap("EditMove.bmp");
    internal static Bitmap EditRotate => TryGetBitmap("EditRotate.bmp");
    internal static Bitmap EditDelete => TryGetBitmap("EditDelete.bmp");

    // Draw
    internal static Bitmap DrawNode => TryGetBitmap("DrawNode.bmp");
    internal static Bitmap DrawBar => TryGetBitmap("DrawBar.bmp");
    internal static Bitmap DrawArea => TryGetBitmap("DrawArea.bmp");

    // Selection
    internal static Bitmap SelAll => TryGetBitmap("SelAll.bmp");
    internal static Bitmap SelClear => TryGetBitmap("SelClear.bmp");

    // View
    internal static Bitmap ZoomWindow => TryGetBitmap("ZoomWindow.bmp");
    internal static Bitmap ZoomExtents => TryGetBitmap("ZoomExtents.bmp");
    internal static Bitmap ZoomSelection => TryGetBitmap("ZoomSelection.bmp");
    internal static Bitmap ViewXY => TryGetBitmap("ViewXY.bmp");
    internal static Bitmap ViewXZ => TryGetBitmap("ViewXZ.bmp");
    internal static Bitmap ViewYZ => TryGetBitmap("ViewYZ.bmp");
    internal static Bitmap View3D => TryGetBitmap("View3D.bmp");
}
