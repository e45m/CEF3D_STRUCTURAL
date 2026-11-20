namespace CEF;

// Main menu. One builder per top-level menu.
// Not implemented features use todoItem(): visible but disabled; change to menuItem() when the handler is ready.
public partial class Cef3DWindowBase
{
    static MainWindow mw => App.mainWindow;

    static ToolStripMenuItem menuItem(string text, EventHandler onClick = null,Keys scKeys= Keys.None)
    {
        var m = new ToolStripMenuItem(text);
        if (onClick != null) m.Click += onClick;
        if(scKeys != Keys.None)  m.ShortcutKeys = scKeys;
        return m;
    }

    static ToolStripMenuItem todoItem(string text, EventHandler onClick = null,Keys scKeys= Keys.None)
    {
        var m = menuItem(text, onClick);
        m.Enabled = false;
        return m;
    }

    static ToolStripMenuItem submenu(string text, params ToolStripItem[] items)
    {
        var m = new ToolStripMenuItem(text);
        m.DropDownItems.AddRange(items);
        return m;
    }

    // Names are kept because other code finds menus by name
    ToolStripMenuItem editOnly(ToolStripMenuItem m, string name)
    {
        m.Name = name;
        editOnlyItems.Add(m);
        return m;
    }

    internal void mainMenuSetUp()
    {
        MainMenuCEF = MainMenuStrip = new();
        mnuResults = buildResultsMenu();

        MainMenuCEF.Items.AddRange(new ToolStripItem[]
        {
            buildFileMenu(), buildViewMenu(), buildEditMenu(), buildSelectMenu(), buildDrawMenu(),
            buildModelMenu(), buildAnalysisMenu(), mnuResults, buildWindowsMenu(), buildHelpMenu()
        });
    }

    ToolStripMenuItem buildFileMenu() => submenu(I18n.File,
        menuItem(I18n.New, mw.addNewFile,Keys.Control|Keys.N),
        menuItem(I18n.Open, mw.openFile,Keys.Control|Keys.O),
        menuItem(I18n.Save, mw.saveFile,Keys.Control|Keys.S),
        menuItem(I18n.SaveAs, mw.saveFileAs,Keys.Control|Keys.Shift|Keys.N),
        submenu(I18n.Export, menuItem(I18n.ExportToDxf, mw.exportFileToDxf)),
        submenu(I18n.Import, menuItem(I18n.ImportSap2000S2k, mw.importFileFromS2K)),
        menuItem(I18n.Exit, mw.closeApp,Keys.Alt|Keys.F4));

    ToolStripMenuItem buildEditMenu()
    {
        var splitMenu = submenu(I18n.Split,
            menuItem(I18n.SplitByLengthOrElements, mw.divideByLenOrPartsSelectedBars),
            menuItem(I18n.SplitAtIntersectionWithSelectedBars, mw.divideByIntersectsSelectedBars),
            menuItem(I18n.SplitAtIntersectionWithSelectedNodes, mw.divideByIntersectsSelectedNodes),
            new ToolStripSeparator(),
            menuItem(I18n.DivideAreasXY, mw.divideAreasXY),
            menuItem(I18n.DivideAreasAtNodes, mw.divideAreasAtSelectedNodes));

         var conectivityMenu = submenu(I18n.Conectivity,
			menuItem(I18n.InvertBarAxes,mw.InvertBarAxes),
            menuItem(I18n.AligneBarWithFirst,mw.AligneBarWithFirst),
            new ToolStripSeparator(),
	        menuItem(I18n.InverAreaAxes,mw.InverAreaAxes),
            menuItem(I18n.AligneAtraWithFist,mw.AligneAtraWithFist),
            menuItem(I18n.ShiftAreaNodes,mw.ShiftAreaNodes));

        return editOnly(submenu(I18n.Edit,
            menuItem(I18n.Copy, mw.copySelected,Keys.Control|Keys.C),   
            menuItem(I18n.Replicate, mw.replicateSelected,Keys.Control|Keys.R),
            menuItem(I18n.Move, mw.moveSelected,Keys.Control|Keys.M),
            menuItem(I18n.Rotate, mw.rotateSelected,Keys.Control|Keys.Shift|Keys.N),
            menuItem(I18n.Delete, mw.deleteSelected,Keys.Delete),
            splitMenu,conectivityMenu,
            menuItem(I18n.MoveNodeGraphically, mw.moveNodeOnScreen),
            menuItem(I18n.RemoveOrphanNodes, mw.deleteOrfanNodes),
            menuItem(I18n.RemoveOverlappingNodes, mw.joinNearNodes),
            menuItem(I18n.MergeSelectedBars, mw.joinSelectedBars),
            menuItem(I18n.Undo, mw.undoLastAction,Keys.Control|Keys.Z)), "Edit");
    }

    ToolStripMenuItem buildDrawMenu() => editOnly(submenu(I18n.Draw,
        menuItem(I18n.Node, mw.addNewNode),
        menuItem(I18n.NodeGraphically, mw.addNewNodeOnScreen),
        menuItem(I18n.Bar, mw.addNewFrame),
        menuItem(I18n.Area, mw.addNewArea)), "Draw");

    ToolStripMenuItem buildSelectMenu() => submenu(I18n.Select,
        menuItem(I18n.SelectOverlappingNodes, mw.SelectNearNodes),
        menuItem(I18n.SelectOverlappingBars, mw.selectOverlapedBars),
        /*todoItem(I18n.SelectOverlappingAreas, mw.selectOverlapedAreas),*/
        menuItem(I18n.SelectAll, mw.selectAllBtn,Keys.Control|Keys.A),
        menuItem(I18n.DeselectAll, mw.clearSelectionBtn,Keys.Control|Keys.Shift|Keys.A));

    ToolStripMenuItem buildModelMenu()
    {
        var nodes = editOnly(submenu(I18n.Nodes,
            menuItem(I18n.BoundaryConditions, mw.restraintDlg),
            menuItem(I18n.Loads, mw.nodalLoadDlg),
            menuItem(I18n.Displacements, mw.nodalDisplDlg),
            menuItem(I18n.KinematicConstraints, mw.mfconstraintsDlg)), "Nodes");

        var bars = editOnly(submenu(I18n.Bars,
            menuItem(I18n.sections, mw.setBarsSectionDlg),
            submenu(I18n.Loads,
                menuItem(I18n.DistributedLoads, mw.setDistrBarLoadsDlg),
                menuItem(I18n.barPunctLoads, mw.setPunctBarLoadsDlg)),
            menuItem(I18n.ReleaseDegreeOfFreedom, mw.setBarReleasedDOF),
            menuItem(I18n.InsertionPoints, mw.setBarInsertionPoint)
           ), "Bars");

        var areas = editOnly(submenu(I18n.Areas,
            menuItem(I18n.AssignSectionToAreas, mw.setAreasSectionDlg)/*,
            submenu(I18n.Loads,
                todoItem(I18n.DistributedLoads, mw.setDistrAreaLoadsDlg),
                todoItem(I18n.PointLoads, mw.setPunctAreaLoadsDlg),
                todoItem(I18n.AreaCoordPatternLoads, mw.setCoordPatAreaLoadsDlg)),
            todoItem(I18n.AreaEdgeReleases, mw.setAreaReleasesDlg)*/), "Areas");
       
    
        var databases = submenu(I18n.Databases,
            menuItem(I18n.TreeView, mw.showModelTree,Keys.Control|Keys.T),
            menuItem(I18n.TableView, mw.showModelInTable,Keys.Control|Keys.Shift|Keys.T));

        return submenu(I18n.Model,
            menuItem(I18n.materials, mw.openMaterialDlg),
            menuItem(I18n.sections, mw.openSectionDlg),
            menuItem(I18n.AreaSections, mw.openAreaSectionDlg),
            nodes, bars, areas,
            editOnly(menuItem(I18n.coordSys, mw.setupNewCoordSys), "CoordSys"),
            editOnly(menuItem(I18n.loadCases, mw.setupNewLoadCase), "CreateCases"),
            editOnly(menuItem(I18n.loadCombs,mw.loadCombDlg),"CreateCases"),
            databases);
    }

    ToolStripMenuItem buildAnalysisMenu() => submenu(I18n.Analysis,
        menuItem(I18n.LinearElasticAnalysis, mw.runLinearAnalysis,Keys.F5),
        menuItem(I18n.LinearAnalysisWithPd, mw.runLinearPDAnalysis, Keys.Shift|Keys.F5),
        menuItem(I18n.Options, mw.analysisOptionsDlg));

    ToolStripMenuItem buildResultsMenu()
    {
        var m = submenu(I18n.Results,
            submenu(I18n.ViewTables,
                menuItem(I18n.NodeDisplacements, mw.nodeDisplTable),
                menuItem(I18n.NodeReactions, mw.nodeReactTable),
                menuItem(I18n.BarInternalForces, mw.barInternalActionsTable),
                menuItem(I18n.AreaForces, mw.areaForcesTable)),
            submenu(I18n.Diagrams,
                menuItem(I18n.Bars, mw.barInertnalForcesGraphView),
                menuItem(I18n.Areas, mw.areaForcesGraphView)),
            menuItem(I18n.ClearResults, mw.deleteResultsFromMenu));
        m.Name = "Results";
        return m;
    }

    ToolStripMenuItem buildViewMenu() => submenu(I18n.View,
        menuItem(I18n.DisplayOptions, mw.setModelViewOptions,Keys.Control|Keys.F2),
        menuItem(I18n.RestoreView, mw.mvResteView),
        submenu(I18n.Viewframes,
            menuItem(I18n.View1, mw.setViewframes),
            menuItem(I18n.View2, mw.setViewframes),
            menuItem(I18n.View3, mw.setViewframes),
            menuItem(I18n.View4, mw.setViewframes)),
        submenu(I18n.Views,
            menuItem(I18n.XY, mw.setXYView),
            menuItem(I18n.XZ, mw.setXZView),
            menuItem(I18n.YZ, mw.setYZView),
            menuItem(I18n.View3D, mw.set3DView)),
        submenu(I18n.Zoom,
            menuItem(I18n.ZoomWindow, mw.zoomBox),
            menuItem(I18n.ZoomExtents, mw.zommAll),
            menuItem(I18n.ZoomToSelection, mw.zoomToSelected)),
        submenu(I18n.Info,
            menuItem(I18n.MeasureDistanceBetweenTwoNodes, mw.measureDist2Nodes),
            menuItem(I18n.SelectedBarsInfo, mw.showSelectBarsInfo),
            menuItem(I18n.SelectedAreasInfo, mw.showSelectAreasInfo)));

    ToolStripMenuItem buildWindowsMenu() => submenu(I18n.Windows,
        menuItem(I18n.NewWindow, mw.addNewWindow));

    ToolStripMenuItem buildHelpMenu() => submenu(I18n.Help,
        menuItem(I18n.ReferenceManual),
        menuItem(I18n.License, mw.abuotDlgShow),
        menuItem(I18n.Test, mw.testDlg));
}
