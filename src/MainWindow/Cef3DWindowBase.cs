using CEF.matrixImpl;
using System.Windows.Forms;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public  class Cef3DWindowBase: Form
{


    internal ToolStrip tollBarLeft;
    internal ToolStripButton btnAddBar, btnClearSel, levelUp, levelDown, 
                            btSnapEnd, btSnapPnt,btSnapCenter, btSnapNear, btSnapPerp;
    internal ToolStripSeparator separator;
    internal ToolStripLabel levelText;
    internal StatusStrip statusBar;
    internal static ToolStripStatusLabel tbStatusText1, tbStatusText2;
    internal static ToolStripProgressBar ProgressStatus;

    internal MenuStrip MainMenuCEF;
    internal ToolStripComboBox tsUnitsSel, tsActCoordSystem, tsActLoadCase;

    public Cef3DWindowBase()
    {
		if (App.mainWindow == null) return;
        IniInitializeComponent();
    }


   internal void IniInitializeComponent()
    {
        mainMenuSetUp();
        otherControlsSetUp();
        mainFormInit();
    }
    internal void mainFormInit(){
	    AutoScaleMode = AutoScaleMode.Font;
	    MainMenuStrip = MainMenuCEF;        
		this.IsMdiContainer = false;
		this.Icon =new Icon(new MemoryStream(Properties.Resources.Icon1));
		this.ClientSize = new Size(1024, 768);
    }

    internal void mainMenuSetUp()
    {
        MainMenuCEF = new();

        // ===== ARCHIVO =====
        var mArchivo = new ToolStripMenuItem(I18n.File);
        mArchivo.DropDownItems.Add(I18n.New, null, App.mainWindow.addNewFile);
        mArchivo.DropDownItems.Add(I18n.Open, null, App.mainWindow.openFile);
        mArchivo.DropDownItems.Add(I18n.Save, null, App.mainWindow.saveFile);
        mArchivo.DropDownItems.Add(I18n.SaveAs, null, App.mainWindow.saveFileAs);

        var mExportar = new ToolStripMenuItem(I18n.Export);
        mExportar.DropDownItems.Add(I18n.ExportToDxf, null, App.mainWindow.exportFileToDxf);
        mArchivo.DropDownItems.Add(mExportar);

        var mImportar = new ToolStripMenuItem(I18n.Import);
        mImportar.DropDownItems.Add(I18n.ImportSap2000S2k, null, App.mainWindow.importFileFromS2K);
        mArchivo.DropDownItems.Add(mImportar);

        mArchivo.DropDownItems.Add(I18n.Exit, null, App.mainWindow.closeApp);

        // ===== EDITAR =====
        var mEditar = new ToolStripMenuItem(I18n.Edit);
        mEditar.DropDownItems.Add(I18n.Copy, null, App.mainWindow.copySelected);
        mEditar.DropDownItems.Add(I18n.Replicate, null, App.mainWindow.replicateSelected);
        mEditar.DropDownItems.Add(I18n.Move, null, App.mainWindow.moveSelected);
        mEditar.DropDownItems.Add(I18n.Rotate, null, App.mainWindow.rotateSelected);
        mEditar.DropDownItems.Add(I18n.Delete, null, App.mainWindow.deleteSelected);

        var mDivide = new ToolStripMenuItem(I18n.Split);
        mDivide.DropDownItems.Add(I18n.SplitByLengthOrElements, null, App.mainWindow.divideByLenOrPartsSelectedBars);
        mDivide.DropDownItems.Add(I18n.SplitAtIntersectionWithSelectedBars, null, App.mainWindow.divideByIntersectsSelectedBars);
        mDivide.DropDownItems.Add(I18n.SplitAtIntersectionWithSelectedNodes, null, App.mainWindow.divideByIntersectsSelectedNodes);

        mEditar.DropDownItems.Add(mDivide);

        mEditar.DropDownItems.Add(I18n.MoveNodeGraphically, null, App.mainWindow.moveNodeOnScreen);
        mEditar.DropDownItems.Add(I18n.RemoveOrphanNodes, null, App.mainWindow.deleteOrfanNodes);
        mEditar.DropDownItems.Add(I18n.RemoveOverlappingNodes, null, App.mainWindow.joinNearNodes);        
        mEditar.DropDownItems.Add(I18n.MergeSelectedBars, null, App.mainWindow.joinSelectedBars);

        mEditar.DropDownItems.Add(I18n.Undo, null, App.mainWindow.undoLastAction);

        // ===== DIBUJAR =====
        var mDraw = new ToolStripMenuItem(I18n.Draw);
        mDraw.DropDownItems.Add(I18n.Node, null, App.mainWindow.addNewNode);
        mDraw.DropDownItems.Add(I18n.NodeGraphically, null, App.mainWindow.addNewNodeOnScreen);
        mDraw.DropDownItems.Add(I18n.Bar, null, App.mainWindow.addNewFrame);


        // ===== select =====
        var mSelect = new ToolStripMenuItem(I18n.Select);
        mSelect.DropDownItems.Add(I18n.Node, null, null);
        mSelect.DropDownItems.Add(I18n.Bar, null, null);
        mSelect.DropDownItems.Add(I18n.SelectOverlappingNodes, null, App.mainWindow.SelectNearNodes);
        mSelect.DropDownItems.Add(I18n.SelectOverlappingBars, null, App.mainWindow.selectOverlapedBars);
        mSelect.DropDownItems.Add(I18n.SelectAll, null, App.mainWindow.selectAllBtn);
        mSelect.DropDownItems.Add(I18n.DeselectAll, null, App.mainWindow.clearSelectionBtn);

        // ===== MODELO =====
        var mModelo = new ToolStripMenuItem(I18n.Model);
        mModelo.DropDownItems.Add(I18n.Materials, null, App.mainWindow.openMaterialDlg);
        mModelo.DropDownItems.Add(I18n.Sections, null, App.mainWindow.openSectionDlg);

        var mNudos = new ToolStripMenuItem(I18n.Nodes);
        mNudos.DropDownItems.Add(I18n.BoundaryConditions, null, App.mainWindow.restraintDlg);
        mNudos.DropDownItems.Add(I18n.Loads, null, App.mainWindow.nodalLoadDlg);
        mNudos.DropDownItems.Add(I18n.Displacements, null, App.mainWindow.nodalDisplDlg);
        mNudos.DropDownItems.Add(I18n.KinematicConstraints, null, App.mainWindow.mfconstraintsDlg);
        mModelo.DropDownItems.Add(mNudos);

        var mBarras = new ToolStripMenuItem(I18n.Bars);
        mBarras.DropDownItems.Add(I18n.Sections, null, App.mainWindow.setBarsSectionDlg);
        mBarras.DropDownItems.Add(I18n.Materials, null, App.mainWindow.setBarsMaterialDlg);

        var mCargasBarra = new ToolStripMenuItem(I18n.Loads);
        mCargasBarra.DropDownItems.Add(I18n.DistributedLoads, null, App.mainWindow.setDistrBarLoadsDlg);
        mCargasBarra.DropDownItems.Add(I18n.barPunctLoads, null, App.mainWindow.setPunctBarLoadsDlg);
        mBarras.DropDownItems.Add(mCargasBarra);

        mBarras.DropDownItems.Add(I18n.ReleaseDegreeOfFreedom, null, App.mainWindow.setBarReleasedDOF);
        mBarras.DropDownItems.Add(I18n.InsertionPoints, null, App.mainWindow.setBarInsertionPoint);
        mModelo.DropDownItems.Add(mBarras);

        var mSistemas = new ToolStripMenuItem(I18n.coordSys);
        mSistemas.DropDownItems.Add(I18n.NewEdit, null, App.mainWindow.setupNewCoordSys);

        var tsActCoordSystem = new ToolStripComboBox();
        tsActCoordSystem.SelectedIndexChanged += App.mainWindow.choosedCoordSysCNChanged;
        tsActCoordSystem.Click += App.mainWindow.loadCoordSysNamesList;

        var mSeleccionarSC = new ToolStripMenuItem(I18n.Select);
        mSeleccionarSC.DropDownItems.Add(tsActCoordSystem);
        mSistemas.DropDownItems.Add(mSeleccionarSC);
        mModelo.DropDownItems.Add(mSistemas);

        var mCasos = new ToolStripMenuItem(I18n.loadCases);
        mCasos.DropDownItems.Add(I18n.EditCreateDelete, null, App.mainWindow.setupNewLoadCase);

        var tsCasoActivo = new ToolStripComboBox();
        tsCasoActivo.Click += App.mainWindow.loadLoadCasesList;
        tsCasoActivo.SelectedIndexChanged += App.mainWindow.chooseActiveLoadCase;

        var mSelCaso = new ToolStripMenuItem(I18n.SelectActiveCase);
        mSelCaso.DropDownItems.Add(tsCasoActivo);
        mCasos.DropDownItems.Add(mSelCaso);
        mModelo.DropDownItems.Add(mCasos);

        mModelo.DropDownItems.Add(new ToolStripMenuItem(I18n.loadCombs));

        var mBases = new ToolStripMenuItem(I18n.Databases);
        mBases.DropDownItems.Add(I18n.TreeView, null, App.mainWindow.showModelTables);
        mBases.DropDownItems.Add(I18n.TableView, null, App.mainWindow.showModelInTreeView);
        mModelo.DropDownItems.Add(mBases);

        var mUnidades = new ToolStripMenuItem(I18n.Units);
        tsUnitsSel = new ToolStripComboBox();
        tsUnitsSel.SelectedIndexChanged += App.mainWindow.setUnitsFromMenu;
        mUnidades.DropDownItems.Add(tsUnitsSel);
        tsUnitsSel.Items.AddRange(uc.supportedUnits);
        mModelo.DropDownItems.Add(mUnidades);

        // ===== ANALISIS =====
        var mAnalisis = new ToolStripMenuItem(I18n.Analysis);
        mAnalisis.DropDownItems.Add(I18n.LinearElasticAnalysis, null, App.mainWindow.runLinearAnalysis);
        mAnalisis.DropDownItems.Add(I18n.LinearAnalysisWithPd, null, App.mainWindow.runLinearPDAnalysis);
        mAnalisis.DropDownItems.Add(I18n.Options, null, App.mainWindow.analysisOptionsDlg);

        // ===== RESULTADOS =====
        var mResultados = new ToolStripMenuItem(I18n.Results);
        MainMenuCEF.Items.Add(mResultados);
        var mTablas = new ToolStripMenuItem(I18n.ViewTables);
        mTablas.DropDownItems.Add(I18n.NodeDisplacements, null, App.mainWindow.nodeDisplTable);
        mTablas.DropDownItems.Add(I18n.NodeReactions, null, App.mainWindow.nodeReactTable);
        mTablas.DropDownItems.Add(I18n.BarInternalForces, null, App.mainWindow.barInternalActionsTable);

        var mDiagramas = new ToolStripMenuItem(I18n.Diagrams);
        mDiagramas.DropDownItems.Add(I18n.Bars, null, App.mainWindow.barInertnalForcesGraphView);

        mResultados.DropDownItems.Add(mTablas);
        mResultados.DropDownItems.Add(mDiagramas);
        mResultados.DropDownItems.Add(I18n.ClearResults, null, App.mainWindow.deleteResultsFromMenu);

        // ===== VER =====
        var mVer = new ToolStripMenuItem(I18n.View);
        mVer.DropDownItems.Add(I18n.DisplayOptions, null, App.mainWindow.setModelViewOptions);
        mVer.DropDownItems.Add(I18n.RestoreView, null, App.mainWindow.mvResteView);

        var mViewFrames = new ToolStripMenuItem(I18n.Viewframes);
        mViewFrames.DropDownItems.Add(I18n.View1, null, App.mainWindow.setViewframes);
        mViewFrames.DropDownItems.Add(I18n.View2, null, App.mainWindow.setViewframes);
        mViewFrames.DropDownItems.Add(I18n.View3, null, App.mainWindow.setViewframes);
        mViewFrames.DropDownItems.Add(I18n.View4, null, App.mainWindow.setViewframes);

        mVer.DropDownItems.Add(mViewFrames);     


        var mVistas = new ToolStripMenuItem(I18n.Views);
        mVistas.DropDownItems.Add(I18n.XY, null, App.mainWindow.setXYView);
        mVistas.DropDownItems.Add(I18n.XZ, null, App.mainWindow.setXZView);
        mVistas.DropDownItems.Add(I18n.YZ, null, App.mainWindow.setYZView);
        mVistas.DropDownItems.Add(I18n.View3D, null, App.mainWindow.set3DView);

        mVer.DropDownItems.Add(mVistas);

        var mZoom = new ToolStripMenuItem(I18n.Zoom);
        mZoom.DropDownItems.Add(I18n.ZoomWindow, null, App.mainWindow.zoomBox);
        mZoom.DropDownItems.Add(I18n.ZoomExtents, null, App.mainWindow.zommAll);
        mZoom.DropDownItems.Add(I18n.ZoomToSelection, null, App.mainWindow.zoomToSelected);
        mVer.DropDownItems.Add(mZoom);

        var mInfoTools = new ToolStripMenuItem(I18n.Info);
        mInfoTools.DropDownItems.Add(I18n.MeasureDistanceBetweenTwoNodes, null, App.mainWindow.measureDist2Nodes);
        mInfoTools.DropDownItems.Add(I18n.SelectedBarsInfo, null, App.mainWindow.showSelectBarsInfo);
        mVer.DropDownItems.Add(mInfoTools);


        // ===== VENTANAS =====
        var mVentanas = new ToolStripMenuItem(I18n.Windows);
        mVentanas.DropDownItems.Add(I18n.NewWindow, null, App.mainWindow.addNewWindow);

       // ===== AYUDA =====
        var mAyuda = new ToolStripMenuItem(I18n.Help);       
        mAyuda.DropDownItems.Add(I18n.ReferenceManual);
        mAyuda.DropDownItems.Add(I18n.License, null, App.mainWindow.abuotDlgShow);
        mAyuda.DropDownItems.Add(I18n.Test, null, App.mainWindow.testDlg);

        MainMenuCEF.Items.AddRange(new ToolStripItem[]
        {
        mArchivo, mVer,mEditar,mSelect ,mDraw , mModelo, mAnalisis, mResultados,  mVentanas, mAyuda
        });

        Controls.Add(MainMenuCEF);
    }
   
    internal void otherControlsSetUp(){
		tollBarLeft =new ();       
		btnAddBar = new ();
        btnClearSel = new ();
		separator = new ();		
		levelUp = new ();
		levelDown = new ();
		levelText = new ();
        tbStatusText1 = new();
        ProgressStatus = new();
        tbStatusText2 = new();
        statusBar = new();

        btSnapPnt = new() { Text = I18n.SnapPoint, BackColor = Color.Gray};
        btSnapPnt.Click +=App.mainWindow.snapBtnClick;
        btSnapEnd = new() { Text = I18n.SnapEnd, BackColor = Color.Gray};
        btSnapEnd.Click +=App.mainWindow.snapBtnClick;
        btSnapCenter =  new() { Text = I18n.SnapCenter, BackColor = Color.Gray };
        btSnapCenter.Click += App.mainWindow.snapBtnClick;
        btSnapNear = new() { Text = I18n.SnapNear, BackColor = Color.Gray };
        btSnapNear.Click += App.mainWindow.snapBtnClick;
        btSnapPerp = new() { Text = I18n.SnapPerpendicular, BackColor = Color.Gray };
        btSnapPerp.Click += App.mainWindow.snapBtnClick;


        tollBarLeft.Items.AddRange(new ToolStripItem[]
		{ btnAddBar, btnClearSel, separator,  levelUp, levelDown, levelText, 
            separator, btSnapEnd, btSnapCenter, btSnapNear, btSnapPerp, btSnapPnt
    });
        tollBarLeft.Dock = DockStyle.Left;
        tollBarLeft.AutoSize = true;
        btnAddBar.DisplayStyle = ToolStripItemDisplayStyle.Image;
        btnAddBar.Image = Properties.Resources.Nbarra;
        btnAddBar.Click += App.mainWindow.addNewFrame;
		btnClearSel.DisplayStyle = ToolStripItemDisplayStyle.Image;
        btnClearSel.Image = Properties.Resources.Refresh;       
        btnClearSel.Click += App.mainWindow.clearSelectionBtn;
		levelUp.DisplayStyle = ToolStripItemDisplayStyle.Image;
        levelUp.Image = Properties.Resources.PlanoMAs;
		levelUp.Click += App.mainWindow.movePlaneUp;
        levelDown.DisplayStyle = ToolStripItemDisplayStyle.Image;
        levelDown.Image = Properties.Resources.PlanoMenos;
        levelDown.Click += App.mainWindow.movePlaneDowm;
		Controls.Add(tollBarLeft);

        tbStatusText2.Alignment = ToolStripItemAlignment.Left;

        statusBar.Items.AddRange(new ToolStripItem[] 
        { tbStatusText1, ProgressStatus,separator, tbStatusText2 });		
        statusBar.Stretch = false;
		Controls.Add(statusBar);
	}   
}