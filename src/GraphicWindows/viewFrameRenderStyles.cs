using System.ComponentModel;
using System.Drawing.Drawing2D;
namespace CEF;
internal class ModelStyles()
{

    [Category(I18n.CategoryScope)]
    [DisplayName(I18n.DisplayApplyToAllWindows)]
    public bool applyToAllWindows { get; set; }

    [Category(I18n.CategoryScope)]
    [DisplayName(I18n.DisplayApplyToNewWindows)]
    public bool applyToNewWindows { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayCanvasBackgroundColor)]
    public Color canvColor { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplayNodeColor)]
    public Color nodeBrushColor { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplayBarShadowColor)]
    public Color barLineSColor { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplayNodeThickness)]
    public float barLineSSize { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplayBarLineColor)]
    public Color barColor { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplayBarLineWidth)]
    public float barLineW { get; set; }

    [Category(I18n.CategorySelect)]
    [DisplayName(I18n.DisplaySelectedColor)]
    public Color barSelectedColor { get; set; }

    [Category(I18n.CategoryAppearance)]
    [DisplayName(I18n.DisplayLoadsColor)]
    internal Brush loadColorBrush { get; set; }

    [Category(I18n.CategoryLoads)]
    [DisplayName(I18n.DisplayPointLoadsColor)]
    public Color loadPenColor { get; set; }

    [Category(I18n.CategoryResults)]
    [DisplayName(I18n.DisplayDeformedShapeColor)]
    public Color deformedShapeColor { get; set; }

    [Category(I18n.CategoryModelElements)]
    [DisplayName(I18n.DisplaySupportBackgroundColor)]
    public Color SuppColorBrush { get; set; }

    [Category(I18n.CategoryFontsAndText)]
    [DisplayName(I18n.DisplayFont)]
    public Font defaultFont { get; set; }

    [Category(I18n.CategoryAuxiliaryElements)]
    [DisplayName(I18n.DisplayGridLineColor)]
    public Color gridLinesColor { get; set; }

    [Category(I18n.CategoryAuxiliaryElements)]
    [DisplayName(I18n.DisplayGridLineWidth)]
    public float gridLineW { get; set; }

    [Category(I18n.CategoryCanvas)]
    [Description(I18n.DescriptionRenderSegments)]
    [DisplayName(I18n.DisplayRenderDivisions)]
    public int sectionsPerElement { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowNodeId)]
    public bool showNodesID { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowGrid)]
    public bool showGrid { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowBarLoads)]
    public bool showBarsLoads { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowDeformedShape)]
    public bool showDeformedShape { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowNodeLoads)]
    public bool showNodalLoads { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowNodes)]
    public bool showNodes { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayShowSelectedNodes)]
    public bool ShowSelectedNodes { get; set; }

    [Category(I18n.CategoryStyles)]
    [Description(I18n.DescriptionRenderOptions)]
    [DisplayName(I18n.DisplayRenderStyle)]
    public RenderMode viewRendeMode_ { get => (RenderMode)viewRenderMode; set { viewRenderMode = (int)value; } }
    internal int viewRenderMode { get; set; }
    [Category(I18n.CategoryAuxiliaryElements)]
    [DisplayName(I18n.DisplayGridLineStyle)]
    public DashStyle GridLineStyle { get; set; }

    [Category(I18n.CategoryStyles)]
    [DisplayName(I18n.DisplayVisualTheme)]
    [Description(I18n.DescriptionCanvasStyle)]
    public CanvasTheme Theme { get => Theme_; set { Theme_ = value; applyTheme(); } }
    internal CanvasTheme Theme_ { get; set; }
    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayDeformScaleLinear)]
    public double deformScale { get; set; }

    [Category(I18n.CategoryCanvas)]
    [DisplayName(I18n.DisplayDeformScaleRotational)]
    public double rotScale { get; set; }

    public enum RenderMode
    {
        Normal,
        Extruded,
        WireFrame,
    }
    public enum CanvasTheme
    {
        BluePrint,
        Tecnic,
        NotePad,
        Clasic,
        DarkTecnic,
        Ligth
    }
    private float fontScale;
    internal ModelStyles(float FontScale,int dfc) : this()
        {
            Theme_ =(CanvasTheme) dfc; 
            fontScale = FontScale;
            sectionsPerElement = 3;
            showNodesID = false;
            showGrid = true;
            showBarsLoads = true;
            showDeformedShape = false;
            showNodalLoads = true;
            showNodes = false;
            ShowSelectedNodes = true;
            applyToAllWindows = false;
            applyToNewWindows = true;
            deformScale = 100.0;   
            rotScale = 10.0;   
            this.applyTheme();
    }   
        internal void applyTheme()
        {
        if (fontScale < 1) fontScale = 3;
        switch ((int)Theme)
            {
                case 0:
                canvColor = Color.FromArgb(15, 35, 65);
                gridLinesColor = Color.FromArgb(90, 140, 190);
                gridLineW = 0.7f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(235, 240, 255); 
                barColor = Color.FromArgb(200, 215, 40);
                barLineW = 1.25f;
                barLineSColor = Color.FromArgb(150, 200, 255);
                barLineSSize = 1.0f;
                barSelectedColor = Color.FromArgb(255, 225, 120);
                deformedShapeColor = Color.FromArgb(240, 160, 110);
                SuppColorBrush = Color.FromArgb(130, 185, 225);
                loadPenColor = Color.FromArgb(145, 170, 200);
                loadColorBrush = Brushes.LightSteelBlue;
                sectionsPerElement = 3;
                defaultFont = new("Consolas", fontScale);
                break;
            case 1:
                canvColor = Color.FromArgb(238, 238, 235);
                gridLinesColor = Color.FromArgb(200, 200, 200);
                gridLineW = 0.6f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(45, 45, 45);
                barColor = Color.FromArgb(30, 30, 30);
                barLineW = 1.4f;
                barLineSColor = Color.FromArgb(120, 120, 120);
                barLineSSize = 1.1f;
                barSelectedColor = Color.FromArgb(70, 120, 200);
                deformedShapeColor = Color.FromArgb(180, 120, 80);
                SuppColorBrush = Color.FromArgb(90, 110, 130);
                loadPenColor = Color.FromArgb(100, 130, 160);
                loadColorBrush = Brushes.SteelBlue;
                sectionsPerElement = 3;
                defaultFont = new("Segoe UI", fontScale);
                break;
            case 2:
                canvColor = Color.FromArgb(250, 245, 220);
                gridLinesColor = Color.FromArgb(180, 180, 170);
                gridLineW = 0.6f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(70, 70, 70);
                barColor = Color.FromArgb(80, 80, 80);
                barLineW = 1.4f;
                barLineSColor = Color.FromArgb(140, 140, 140);
                barLineSSize = 1.2f;
                barSelectedColor = Color.FromArgb(210, 70, 70);
                deformedShapeColor = Color.FromArgb(180, 110, 80);
                SuppColorBrush = Color.FromArgb(90, 130, 90);
                loadPenColor = Color.FromArgb(70, 110, 140);
                loadColorBrush = Brushes.SlateGray;
                sectionsPerElement = 3;
                defaultFont = new("Arial", fontScale);
                break;
            case 3:
                canvColor = Color.DimGray;
                gridLinesColor = Color.DarkSlateGray;
                gridLineW = 0.3f;
                GridLineStyle = DashStyle.DashDotDot;
                nodeBrushColor = Color.FromArgb(245, 245, 245); ;
                barColor = Color.Blue;
                barLineW = 3.0f;
                barLineSColor = Color.GreenYellow;
                barLineSSize = 4.0f;
                barSelectedColor = Color.Red;
                deformedShapeColor = Color.Coral;
                SuppColorBrush = Color.FromArgb(25, 100, 50);
                loadPenColor = Color.AliceBlue;
                loadColorBrush = Brushes.AliceBlue;
                sectionsPerElement = 3;
                defaultFont = new("Arial", fontScale);
                break;
            case 4:
                canvColor = Color.FromArgb(25, 25, 28);
                gridLinesColor = Color.FromArgb(70, 70, 75);
                gridLineW = 0.5f;
                GridLineStyle = DashStyle.Dot;
                nodeBrushColor = Color.FromArgb(220, 220, 220);
                barColor = Color.FromArgb(200, 200, 200);
                barLineW = 1.6f;
                barLineSColor = Color.FromArgb(255, 200, 80);
                barLineSSize = 1.2f;
                barSelectedColor = Color.FromArgb(255, 120, 60);
                deformedShapeColor = Color.FromArgb(220, 150, 90);
                SuppColorBrush = Color.FromArgb(120, 160, 140);
                loadPenColor = Color.FromArgb(160, 190, 210);
                loadColorBrush = Brushes.Orange;
                sectionsPerElement = 3;
                defaultFont = new("Consolas", fontScale);
                break;
            case 5:
                canvColor = Color.FromArgb(245, 245, 245);
                gridLinesColor = Color.FromArgb(210, 210, 210);
                gridLineW = 0.8f;
                GridLineStyle = DashStyle.Solid;
                nodeBrushColor = Color.FromArgb(65, 65, 65);
                barColor = Color.FromArgb(90, 90, 90);
                barLineW = 1.8f;
                barLineSColor = Color.FromArgb(200, 140, 120);
                barLineSSize = 1.4f;
                barSelectedColor = Color.FromArgb(220, 100, 100);
                deformedShapeColor = Color.FromArgb(200, 130, 110);
                SuppColorBrush = Color.FromArgb(150, 180, 160);
                loadPenColor = Color.FromArgb(130, 160, 190);
                loadColorBrush = Brushes.IndianRed;
                sectionsPerElement = 3;
                defaultFont = new("Segoe UI", fontScale);
                break;
        }
    }
    internal ModelStyles Clone()
    {
        var c = new ModelStyles();
        c.Theme = this.Theme;
        c.canvColor = this.canvColor;
        c.gridLinesColor = this.gridLinesColor;
        c.gridLineW = this.gridLineW;
        c.GridLineStyle = this.GridLineStyle;
        c.nodeBrushColor = this.nodeBrushColor;
        c.barColor = this.barColor;
        c.barLineW = this.barLineW;
        c.barLineSColor = this.barLineSColor;
        c.barLineSSize = this.barLineSSize;
        c.barSelectedColor = this.barSelectedColor;
        c.deformedShapeColor = this.deformedShapeColor;
        c.SuppColorBrush = this.SuppColorBrush;
        c.loadPenColor = this.loadPenColor;
        c.loadColorBrush = this.loadColorBrush ?? Brushes.Gray;
        c.sectionsPerElement = this.sectionsPerElement;
        c.fontScale = this.fontScale;
        c.defaultFont = (Font)this.defaultFont.Clone();
        c.showNodesID = showNodesID;
        c.showGrid = showGrid;
        c.showBarsLoads = showBarsLoads;
        c.showDeformedShape = showDeformedShape;
        c.showNodalLoads = showNodalLoads;
        c.showNodes = showNodes;
        c.ShowSelectedNodes = ShowSelectedNodes;
        c.applyToAllWindows = applyToAllWindows;
        c.applyToNewWindows = applyToNewWindows;
        c.deformScale = deformScale;   
        c.rotScale = rotScale;   
        return c;
    }
}