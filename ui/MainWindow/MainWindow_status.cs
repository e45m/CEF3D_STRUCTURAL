namespace CEF;

public partial class MainWindow
{
    internal void enableDisable(bool enable)
    {
        foreach (var f in OwnedForms)
            if (f is modelView)
                f.Enabled = enable;
    }

    // enable = true: model editable (not calculated); false: calculated (results available).
    // Which menus/buttons toggle is decided in Cef3DWindowBase (editOnlyItems + mnuResults).
    //Todo: After calc, design, etc controls enable/disable
    internal void enableDisable_calc(bool enable)
    {
        foreach (var f in OwnedForms)
            if (f is modelView mv)
                mv.setEditState(enable);
    }
}
