using R = CEF.Properties.Resources;

namespace CEF;

// Top toolbars: toolBarTop (units / case / coordinate system combos) and toolBarTop2 (action buttons).
// Icons: 24x24 .bmp in the Resources folder (see Resources.toolbar.cs); a missing icon shows the tooltip text.
public partial class Cef3DWindowBase
{
    const int toolIconSize = 24;
    static readonly Color toolIconTransparent = Color.Magenta; // .bmp has no alpha: this color is transparent

    ToolStripButton addToolButton(ToolStrip strip, Bitmap icon, string tip, EventHandler onClick, bool editOnlyBtn = false)
    {
        var b = new ToolStripButton
        {
            Text = tip,
            ToolTipText = tip,
            Image = icon,
            DisplayStyle = icon != null ? ToolStripItemDisplayStyle.Image : ToolStripItemDisplayStyle.Text,
            ImageTransparentColor = toolIconTransparent
        };
        b.Click += onClick;
        strip.Items.Add(b);
        if (editOnlyBtn) editOnlyItems.Add(b);
        return b;
    }

    internal void toolBarsTopSetUp()
    {
        tsCasoActivo2 = new ();
        tsCasoActivo2.Click += mw.LoadCasesList;
        tsCasoActivo2.SelectedIndexChanged += mw.chooseActiveLoadCase;
        tsCasoActivo2.DropDownStyle =ComboBoxStyle.DropDownList;

        tsUnitsSel2 = new ();
        tsUnitsSel2.SelectedIndexChanged += mw.setUnitsFromMenu;
        tsUnitsSel2.Items.AddRange(Uc.supportedUnits);
        tsUnitsSel2.DropDownStyle =ComboBoxStyle.DropDownList;

        tsActCoordSystem2 = new ();
        tsActCoordSystem2.SelectedIndexChanged += mw.choosedCoordSysCNChanged;      
        tsActCoordSystem2.Click += mw.loadCoordSysNamesList;
        tsActCoordSystem2.DropDownStyle =ComboBoxStyle.DropDownList;

        tsBarAction.SelectedIndexChanged+= mw.barActionChange;
        //tsBarAction.TextChanged+=mw.barActionChange;
        tsBarAction.DropDownStyle =ComboBoxStyle.DropDownList;
        tsAreaAction.SelectedIndexChanged+= mw.areaActionChange;
        //tsAreaAction.TextChanged+= mw.areaActionChange;
        tsAreaAction.DropDownStyle =ComboBoxStyle.DropDownList;

        toolBarTop = new();
        toolBarTop.Items.AddRange(new ToolStripItem[] { tsUnitsSel2, tsCasoActivo2, tsActCoordSystem2, tsBarAction,tsAreaAction });

        toolBarTop2Setup();

        Controls.AddRange(new ToolStrip[] { toolBarTop2, toolBarTop, MainMenuCEF });
    }

    void toolBarTop2Setup()
    {
        toolBarTop2 = new() { ImageScalingSize = new Size(toolIconSize, toolIconSize) };
        var t = toolBarTop2;

        // File
        addToolButton(t, R.FileNew, I18n.New, mw.addNewFile);
        addToolButton(t, R.FileOpen, I18n.Open, mw.openFile);
        addToolButton(t, R.FileSave, I18n.Save, mw.saveFile);
        t.Items.Add(new ToolStripSeparator());

        // Edit
        addToolButton(t, R.EditUndo, I18n.Undo, mw.undoLastAction, true);
        addToolButton(t, R.EditCopy, I18n.Copy, mw.copySelected, true);
        addToolButton(t, R.EditReplicate, I18n.Replicate, mw.replicateSelected, true);
        addToolButton(t, R.EditMove, I18n.Move, mw.moveSelected, true);
        addToolButton(t, R.EditRotate, I18n.Rotate, mw.rotateSelected, true);
        addToolButton(t, R.EditDelete, I18n.Delete, mw.deleteSelected, true);
        t.Items.Add(new ToolStripSeparator());

        // Draw
        addToolButton(t, R.DrawNode, I18n.NodeGraphically, mw.addNewNodeOnScreen, true);
        addToolButton(t, R.DrawBar, I18n.Bar, mw.addNewFrame, true);
        addToolButton(t, R.DrawArea, I18n.Area, mw.addNewArea, true);
        t.Items.Add(new ToolStripSeparator());

        // Selection
        addToolButton(t, R.SelAll, I18n.SelectAll, mw.selectAllBtn);
        addToolButton(t, R.SelClear, I18n.DeselectAll, mw.clearSelectionBtn);
        t.Items.Add(new ToolStripSeparator());

        // View
        addToolButton(t, R.ZoomWindow, I18n.ZoomWindow, mw.zoomBox);
        addToolButton(t, R.ZoomExtents, I18n.ZoomExtents, mw.zommAll);
        addToolButton(t, R.ZoomSelection, I18n.ZoomToSelection, mw.zoomToSelected);
        t.Items.Add(new ToolStripSeparator());
        addToolButton(t, R.ViewXY, I18n.XY, mw.setXYView);
        addToolButton(t, R.ViewXZ, I18n.XZ, mw.setXZView);
        addToolButton(t, R.ViewYZ, I18n.YZ, mw.setYZView);
        addToolButton(t, R.View3D, I18n.View3D, mw.set3DView);
    }
}
