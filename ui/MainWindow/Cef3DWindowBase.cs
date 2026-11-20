namespace CEF;

// Base window: fields, left toolbar, status bar and edit/calculated state.
// Menus  -> Cef3DWindowBase.menu.cs
// Top toolbars -> Cef3DWindowBase.toolbar.cs
public partial class Cef3DWindowBase : Form
{
    internal ToolStrip toolBarLeft, toolBarTop2, toolBarTop;
    internal ToolStripButton btnAddBar, btnClearSel, levelUp, levelDown,
                            btSnapEnd, btSnapPnt, btSnapCenter, btSnapNear, btSnapPerp;
    internal ToolStripSeparator separator;
    internal ToolStripLabel levelText;
    internal StatusStrip statusBar;
    internal ToolStripLabel tbStatusText1, tbStatusText2;

    internal MenuStrip MainMenuCEF;
    internal static ToolStripComboBox tsActCoordSystem2, tsUnitsSel2, tsCasoActivo2, tsBarAction, tsAreaAction;
    
    // Menus and buttons that only work while the model is editable (not calculated)
    readonly List<ToolStripItem> editOnlyItems = [];
    internal ToolStripMenuItem mnuResults;

    public Cef3DWindowBase()
    {
        if (App.mainWindow == null) return;
        IniInitializeComponent();
    }

    internal void IniInitializeComponent()
    {
        otherControlsSetUp();
        mainMenuSetUp();
        toolBarsTopSetUp();
        mainFormInit();
    }

    internal void mainFormInit()
    {
        AutoScaleMode = AutoScaleMode.Font;
        MainMenuStrip = MainMenuCEF;
        this.IsMdiContainer = false;
        this.Icon = new Icon(new MemoryStream(Properties.Resources.Icon1));
        this.ClientSize = new Size(1024, 768);
    }

    // editable = true: model not calculated (edit/draw enabled, results disabled); false: calculated
    internal void setEditState(bool editable)
    {
        foreach (var it in editOnlyItems)
            it.Enabled = editable;
        if (mnuResults != null)
            mnuResults.Enabled = !editable;
    }

    internal void otherControlsSetUp()
    {
        toolBarLeft = new();
        btnAddBar = new();
        btnClearSel = new();
        separator = new();
        levelUp = new();
        levelDown = new();
        levelText = new();
        tbStatusText1 = new();
        tbStatusText2 = new();
        statusBar = new();
        tsBarAction = new();
        tsAreaAction = new();

        btSnapPnt = new() { Text = I18n.SnapPoint, BackColor = Color.Gray };
        btSnapPnt.Click += App.mainWindow.snapBtnClick;
        btSnapEnd = new() { Text = I18n.SnapEnd, BackColor = Color.Gray };
        btSnapEnd.Click += App.mainWindow.snapBtnClick;
        btSnapCenter = new() { Text = I18n.SnapCenter, BackColor = Color.Gray };
        btSnapCenter.Click += App.mainWindow.snapBtnClick;
        btSnapNear = new() { Text = I18n.SnapNear, BackColor = Color.Gray };
        btSnapNear.Click += App.mainWindow.snapBtnClick;
        btSnapPerp = new() { Text = I18n.SnapPerpendicular, BackColor = Color.Gray };
        btSnapPerp.Click += App.mainWindow.snapBtnClick;

        toolBarLeft.Items.AddRange(new ToolStripItem[]
        { /*btnAddBar, btnClearSel, separator,*/ levelUp, levelDown, levelText,
          separator, btSnapEnd, btSnapCenter, btSnapNear, btSnapPerp, btSnapPnt
        });

        toolBarLeft.Dock = DockStyle.Left;
        toolBarLeft.AutoSize = true;

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
        Controls.Add(toolBarLeft);

        statusBar.Items.AddRange(new ToolStripItem[]
        { tbStatusText1, tbStatusText2 });

        Controls.Add(statusBar);
    }

    internal void setComboBoxes()
    {
        App.mainWindow.loadCoordSysNamesList(tsActCoordSystem2, null);
        App.mainWindow.LoadCasesList(tsCasoActivo2, null);
        tsUnitsSel2.SelectedItem = App.currUnits;
        tsActCoordSystem2.SelectedItem = App.currCoordSist;
        tsCasoActivo2.SelectedIndex = Engine.getModel().loadCases.FindIndexByID(App.currLoadCase);
        tsBarAction.Items.AddRange(Enum.GetNames(typeof(BarActions)).ToArray());
        tsAreaAction.Items.AddRange(Enum.GetNames(typeof(AreaAction)).ToArray());
        tsAreaAction.Visible = false;
        tsBarAction.Visible=false;
        tsBarAction.SelectedIndex = 0;
        tsAreaAction.SelectedIndex = 0;
    }
}
