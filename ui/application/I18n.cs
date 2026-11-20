using System.Collections.Frozen;
using Microsoft.VisualBasic.ApplicationServices;

namespace CEF;

// Textos de la UI. Una línea por texto, en el orden exacto de Assign():
//   <carpeta del programa>/Locals/I18n.txt       -> idioma base (siempre se carga)
//   <carpeta del programa>/Locals/<lang>_I18n.txt -> idioma adicional (es_, fr_, ...). Línea vacía = usar el base
// Agregar un texto: declarar el campo, asignarlo AL FINAL de Assign(), subir Count y añadir la línea AL FINAL de cada archivo.
// Nunca reordenar ni borrar entradas. Si falta una línea en el base se muestra "#<índice>".
// csproj:  <None Update="Locals\*.txt"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>
internal static class I18n
{
    public const string SplitChar = ","; //not to be localizable
    public const string GroupingKey = "@"; //not to be localizable
    public const string LocalsDir = "locals";
    public const string BaseFile = "I18n.txt";
    public const int Count = 314;

    public static string Lang { get; private set; } =  "";
    public static FrozenDictionary<string, string> translateTableNames;

    static readonly string[] s = new string[Count];

    static I18n() => Init("");

    // lang = "" -> solo el base; "es" -> base + Locals/es_I18n.txt
    public static void Init(string lang)
    {
        string dir = System.IO.Path.Combine(AppContext.BaseDirectory, LocalsDir);
        var b = Read(System.IO.Path.Combine(dir, BaseFile));
        for (int i = 0; i < Count; i++)
            s[i] = b != null && i < b.Length ? b[i] : "#" + i;

        if (!string.IsNullOrEmpty(lang))
        {
            var t = Read(System.IO.Path.Combine(dir, lang + "_" + BaseFile));
            if (t != null)
                for (int i = 0; i < Count && i < t.Length; i++)
                    if (t[i].Length > 0) s[i] = t[i];
        }
        Lang = lang ?? "";
        Assign();
        
    } 

    private static string[] Read(string path) =>
        System.IO.File.Exists(path) ? System.IO.File.ReadAllLines(path, System.Text.Encoding.UTF8) : null;

    public static string 
    About, //1
    File,
    New,
    Open,
    Save,
    SaveAs,
    Export,
    ExportToDxf,
    Import,
    ImportSap2000S2k,
    Exit,//11
    Edit,
    Copy,
    Replicate,
    Move,
    Rotate,
    Delete,
    Split,
    SplitByLengthOrElements,
    SplitAtIntersectionWithSelectedBars,
    SplitAtIntersectionWithSelectedNodes,//21
    MoveNodeGraphically,
    RemoveOrphanNodes,
    RemoveOverlappingNodes,
    MergeSelectedBars,
    Undo,
    Draw,
    Node,
    NodeGraphically,
    Bar,
    Select,//31
    SelectOverlappingNodes,
    SelectOverlappingBars,
    SelectAll,
    DeselectAll,
    Model,
    materials,
    sections,
    Nodes,
    BoundaryConditions,
    Loads,//41
    Displacements,
    KinematicConstraints,
    Bars,
    LoadsEllipsis,
    DistributedLoads,
    PointLoads,
    ReleaseDegreeOfFreedom,
    InsertionPoints,
    CoordinateSystems,
    NewEdit,//51
    LoadCases,
    EditCreateDelete,
    SelectActiveCase,
    LoadCombinations,
    Databases,
    TreeView,
    TableView,
    Units,
    Analysis,
    LinearElasticAnalysis,//61
    LinearAnalysisWithPd,
    Options,
    Results,
    ViewTables,
    NodeDisplacements,
    NodeReactions,
    BarInternalForces,
    Diagrams,
    ClearResults,
    View,//71
    DisplayOptions,
    RestoreView,
    Viewframes,
    View1,
    View2,
    View3,
    View4,
    Views,
    XY,
    XZ,//81
    YZ,
    View3D,
    Zoom,
    ZoomWindow,
    ZoomExtents,
    ZoomToSelection,
    Info,
    MeasureDistanceBetweenTwoNodes,
    SelectedBarsInfo,
    Windows,//91
    NewWindow,
    Help,
    ReferenceManual,
    License,
    Test,
    SnapPoint,
    SnapEnd, //Not usded, nor delete
    SnapCenter,//Not usded, nor delete
    SnapNear,//Not usded, nor delete
    SnapPerpendicular,//Not usded, nor delete 101
    gridCoordsX,
    gridCoordsY,
    gridCoordsZ,
    barThermalLoads,
    programParams,
    loadCombs,
    loadCases,
    nodalDisplacements,
    nodalReactions,
    nodalRestraints,//111
    nodes,
    nodalLoads,
    bars,
    barDistrLoads,
    barPunctLoads,
    barInternForces,
    nodalConstraints,
    Name,
    Type,
    Value,//121
    Coord,
    coordSys,
    label,
    startNode,
    endNode,
    Case,
    Distance,
    MaterialID,
    SectionID,
    MaterialName,//131
    SectionName,
    unused,
    Rotation,
    releases,
    offsetY,
    offsetZ,
    masterNodeID,
    nodesIDList,
    NewNode,
    ImputInstr1,//141
    OutPutInstr1,
    ImputOffests,
    offsetX2,
    offsetY2,
    offsetZ2,
    ImputRotation,
    RotationX,
    RotationY,
    RotationZ,
    ImputDireVect,//151
    DireVectX,
    DireVectY,
    DireVectZ,
    ImputRotationCent,
    RotCenX,
    RotCenY,
    RotCenZ,
    ImputSpacing,
    ImputSpacingExample,
    Spacing,//161
    DivideByElemsCuant,
    DivideByEleemLen,
    ImputMsgDivideByElemsCuant,
    ImputMsgDivideByEleemLen,
    NoOverlappedBarsFound,
    DlgOKText,
    DlgCancelText,
    LoadDB,
    EditCreate,
    leftArrow,//171
    rightArrow,
    CannotLoadDB,
    Diaphragm,    
    Plate,
    Body,
    Weld,
    Beam,    
    Equal,
    CheckInput,
    Replace,//181
    SetNodalLoads,
    SetPunctBarLoads,
    Fraction,
    Global,
    Local,
    CalProps,
    Add,
    AddNewLoadCase,
    AssignMaterialToBars,
    AssignSectionToBars,//191
    Restrain,
    Fixed,
    Free,
    Hinge,
    Roller,
    RotationAngle,
    ip_center,
    ip_down,
    ip_up,
    ip_left,//201
    ip_right,
    min,
    max,
    ValueAtStartNode,
    ValueAtEndNode,
    BatchImput,
    CategoryScope,
    CategoryCanvas,
    CategoryModelElements,
    CategorySelect,//211
    CategoryAppearance,
    CategoryLoads,
    CategoryResults,
    CategoryFontsAndText,
    CategoryAuxiliaryElements,
    CategoryStyles,
    DisplayApplyToAllWindows,
    DisplayApplyToNewWindows,
    DisplayCanvasBackgroundColor,
    DisplayNodeColor,//221
    DisplayBarShadowColor,
    DisplayNodeThickness,
    DisplayBarLineColor,
    DisplayBarLineWidth,
    DisplaySelectedColor,
    DisplayLoadsColor,
    DisplayPointLoadsColor,
    DisplayDeformedShapeColor,
    DisplaySupportBackgroundColor,
    DisplayFont,//231
    DisplayGridLineColor,
    DisplayGridLineWidth,
    DisplayRenderDivisions,
    DisplayShowNodeId,
    DisplayShowGrid,
    DisplayShowBarLoads,
    DisplayShowDeformedShape,
    DisplayShowNodeLoads,
    DisplayShowNodes,
    DisplayShowSelectedNodes,//241
    DisplayRenderStyle,
    DisplayGridLineStyle,
    DisplayVisualTheme,
    DisplayDeformScaleLinear,
    DisplayDeformScaleRotational,
    DescriptionRenderSegments,
    DescriptionRenderOptions,
    DescriptionCanvasStyle,
    Element,
    ElementSummary,//251
    Length,
    CaseOrCombination,
    Action,
    NoResultsForSelection,
    ActionSetAll,
    ActionSetPlaneXY,
    ActionSetPlaneXZ,
    ActionSetShears,
    ActionSetMoments,
    ActionAxial,
    ActionShearY,
    ActionShearZ,
    ActionTorsion,
    ActionMomentY,
    ActionMomentZ,
    TableSearch,
    TableCase,
    TableIds,
    TableAll,
    TableHideZeros,
    TableCopy,
    TableRows,
    TableEnterValue,
    TableInvalidValue,
    SelectMaterial,
    MaterialLabel,
    NewMaterial,
    DBFilter,
    NoMaterials,
    Area,    
    AreaSections, 
    Areas,
    DivideAreasXY,
    DivideAreasAtNodes,
    SelectOverlappingAreas,
    AssignSectionToAreas,
    AreaCoordPatternLoads,
    AreaEdgeReleases,
    AreaForces,
    SelectedAreasInfo,//291
    AreaDivisionsX,
    AreaDivisionsY,
    AnalysisOptions,
    AreaAutomesh,
    BarAutomesh,
    Factor,
    AreaInternalForces,
    InvertBarAxes,
    AligneBarWithFirst,
    InverAreaAxes,//301
    AligneAtraWithFist,
    ShiftAreaNodes,
    Conectivity

    ;


    private static void Assign()
    {
       int i = 0;
        About = s[i++];
        File = s[i++];
        New = s[i++];
        Open = s[i++];
        Save = s[i++];
        SaveAs = s[i++];
        Export = s[i++];
        ExportToDxf = s[i++];
        Import = s[i++];
        ImportSap2000S2k = s[i++];
        Exit = s[i++];
        Edit = s[i++];
        Copy = s[i++];
        Replicate = s[i++];
        Move = s[i++];
        Rotate = s[i++];
        Delete = s[i++];
        Split = s[i++];
        SplitByLengthOrElements = s[i++];
        SplitAtIntersectionWithSelectedBars = s[i++];
        SplitAtIntersectionWithSelectedNodes = s[i++];
        MoveNodeGraphically = s[i++];
        RemoveOrphanNodes = s[i++];
        RemoveOverlappingNodes = s[i++];
        MergeSelectedBars = s[i++];
        Undo = s[i++];
        Draw = s[i++];
        Node = s[i++];
        NodeGraphically = s[i++];
        Bar = s[i++];
        Select = s[i++];
        SelectOverlappingNodes = s[i++];
        SelectOverlappingBars = s[i++];
        SelectAll = s[i++];
        DeselectAll = s[i++];
        Model = s[i++];
        materials = s[i++];
        sections = s[i++];
        Nodes = s[i++];
        BoundaryConditions = s[i++];
        Loads = s[i++];
        Displacements = s[i++];
        KinematicConstraints = s[i++];
        Bars = s[i++];
        LoadsEllipsis = s[i++];
        DistributedLoads = s[i++];
        PointLoads = s[i++];
        ReleaseDegreeOfFreedom = s[i++];
        InsertionPoints = s[i++];
        CoordinateSystems = s[i++];
        NewEdit = s[i++];
        LoadCases = s[i++];
        EditCreateDelete = s[i++];
        SelectActiveCase = s[i++];
        LoadCombinations = s[i++];
        Databases = s[i++];
        TreeView = s[i++];
        TableView = s[i++];
        Units = s[i++];
        Analysis = s[i++];
        LinearElasticAnalysis = s[i++];
        LinearAnalysisWithPd = s[i++];
        Options = s[i++];
        Results = s[i++];
        ViewTables = s[i++];
        NodeDisplacements = s[i++];
        NodeReactions = s[i++];
        BarInternalForces = s[i++];
        Diagrams = s[i++];
        ClearResults = s[i++];
        View = s[i++];
        DisplayOptions = s[i++];
        RestoreView = s[i++];
        Viewframes = s[i++];
        View1 = s[i++];
        View2 = s[i++];
        View3 = s[i++];
        View4 = s[i++];
        Views = s[i++];
        XY = s[i++];
        XZ = s[i++];
        YZ = s[i++];
        View3D = s[i++];
        Zoom = s[i++];
        ZoomWindow = s[i++];
        ZoomExtents = s[i++];
        ZoomToSelection = s[i++];
        Info = s[i++];
        MeasureDistanceBetweenTwoNodes = s[i++];
        SelectedBarsInfo = s[i++];
        Windows = s[i++];
        NewWindow = s[i++];
        Help = s[i++];
        ReferenceManual = s[i++];
        License = s[i++];
        Test = s[i++];
        SnapPoint = "Pnt";i++;//Not localizable but not remove line in text file s[i++];
        SnapEnd = "End";i++;//Not localizable but not remove line in text file s[i++];
        SnapCenter ="Cen";i++;//Not localizable  but not remove line in text file s[i++];
        SnapNear ="Nea";i++;//Not localizable  but not remove line in text file s[i++];
        SnapPerpendicular = "Per";i++;//Not localizables but not remove line in text file s[i++];
        gridCoordsX = s[i++];
        gridCoordsY = s[i++];
        gridCoordsZ = s[i++];
        barThermalLoads = s[i++];
        programParams = s[i++];
        loadCombs = s[i++];
        loadCases = s[i++];
        nodalDisplacements = s[i++];
        nodalReactions = s[i++];
        nodalRestraints = s[i++];
        nodes = s[i++];
        nodalLoads = s[i++];
        bars = s[i++];
        barDistrLoads = s[i++];
        barPunctLoads = s[i++];
        barInternForces = s[i++];
        nodalConstraints = s[i++];
        Name = s[i++];
        Type = s[i++];
        Value = s[i++];
        Coord = s[i++];
        coordSys = s[i++];
        label = s[i++];
        startNode = s[i++];
        endNode = s[i++];
        Case = s[i++];
        Distance = s[i++];
        MaterialID = s[i++];
        SectionID = s[i++];
        MaterialName = s[i++];
        SectionName = s[i++];
        unused = s[i++];
        Rotation = s[i++];
        releases = s[i++];
        offsetY = s[i++];
        offsetZ = s[i++];
        masterNodeID = s[i++];
        nodesIDList = s[i++];
        NewNode = s[i++];
        ImputInstr1 = s[i++];
        OutPutInstr1 = s[i++];
        ImputOffests = s[i++];
        offsetX2 = s[i++];
        offsetY2 = s[i++];
        offsetZ2 = s[i++];
        ImputRotation = s[i++];
        RotationX = s[i++];
        RotationY = s[i++];
        RotationZ = s[i++];
        ImputDireVect = s[i++];
        DireVectX = s[i++];
        DireVectY = s[i++];
        DireVectZ = s[i++];
        ImputRotationCent = s[i++];
        RotCenX = s[i++];
        RotCenY = s[i++];
        RotCenZ = s[i++];
        ImputSpacing = s[i++];
        ImputSpacingExample = s[i++];
        Spacing = s[i++];
        DivideByElemsCuant = s[i++];
        DivideByEleemLen = s[i++];
        ImputMsgDivideByElemsCuant = s[i++];
        ImputMsgDivideByEleemLen = s[i++];
        NoOverlappedBarsFound = s[i++];
        DlgOKText = s[i++];
        DlgCancelText = s[i++];
        LoadDB = s[i++];
        EditCreate = s[i++];
        leftArrow = s[i++];
        rightArrow = s[i++];
        CannotLoadDB = s[i++];
        Diaphragm = s[i++];       
        Plate = s[i++];        
        Body = s[i++];
        Weld = s[i++];       
        Beam = s[i++];       
        Equal = s[i++];
        CheckInput = s[i++];
        Replace = s[i++];
        SetNodalLoads = s[i++];
        SetPunctBarLoads = s[i++];
        Fraction = s[i++];
        Global = s[i++];
        Local = s[i++];
        CalProps = s[i++];
        Add = s[i++];
        AddNewLoadCase = s[i++];
        AssignMaterialToBars = s[i++];
        AssignSectionToBars = s[i++];
        Restrain = s[i++];
        Fixed = s[i++];
        Free = s[i++];
        Hinge = s[i++];
        Roller = s[i++];
        RotationAngle = s[i++];
        ip_center = s[i++];
        ip_down = s[i++];
        ip_up = s[i++];
        ip_left = s[i++];
        ip_right = s[i++];
        min = s[i++];
        max = s[i++];
        ValueAtStartNode = s[i++];
        ValueAtEndNode = s[i++];
        BatchImput = s[i++];
        CategoryScope = s[i++];
        CategoryCanvas = s[i++];
        CategoryModelElements = s[i++];
        CategorySelect = s[i++];
        CategoryAppearance = s[i++];
        CategoryLoads = s[i++];
        CategoryResults = s[i++];
        CategoryFontsAndText = s[i++];
        CategoryAuxiliaryElements = s[i++];
        CategoryStyles = s[i++];
        DisplayApplyToAllWindows = s[i++];
        DisplayApplyToNewWindows = s[i++];
        DisplayCanvasBackgroundColor = s[i++];
        DisplayNodeColor = s[i++];
        DisplayBarShadowColor = s[i++];
        DisplayNodeThickness = s[i++];
        DisplayBarLineColor = s[i++];
        DisplayBarLineWidth = s[i++];
        DisplaySelectedColor = s[i++];
        DisplayLoadsColor = s[i++];
        DisplayPointLoadsColor = s[i++];
        DisplayDeformedShapeColor = s[i++];
        DisplaySupportBackgroundColor = s[i++];
        DisplayFont = s[i++];
        DisplayGridLineColor = s[i++];
        DisplayGridLineWidth = s[i++];
        DisplayRenderDivisions = s[i++];
        DisplayShowNodeId = s[i++];
        DisplayShowGrid = s[i++];
        DisplayShowBarLoads = s[i++];
        DisplayShowDeformedShape = s[i++];
        DisplayShowNodeLoads = s[i++];
        DisplayShowNodes = s[i++];
        DisplayShowSelectedNodes = s[i++];
        DisplayRenderStyle = s[i++];
        DisplayGridLineStyle = s[i++];
        DisplayVisualTheme = s[i++];
        DisplayDeformScaleLinear = s[i++];
        DisplayDeformScaleRotational = s[i++];
        DescriptionRenderSegments = s[i++];
        DescriptionRenderOptions = s[i++];
        DescriptionCanvasStyle = s[i++];
        Element = s[i++];
        ElementSummary = s[i++];
        Length = s[i++];
        CaseOrCombination = s[i++];
        Action = s[i++];
        NoResultsForSelection = s[i++];
        ActionSetAll = s[i++];
        ActionSetPlaneXY = s[i++];
        ActionSetPlaneXZ = s[i++];
        ActionSetShears = s[i++];
        ActionSetMoments = s[i++];
        ActionAxial = s[i++];
        ActionShearY = s[i++];
        ActionShearZ = s[i++];
        ActionTorsion = s[i++];
        ActionMomentY = s[i++];
        ActionMomentZ = s[i++];
        TableSearch= s[i++];
        TableCase= s[i++];
        TableIds= s[i++];
        TableAll= s[i++];
        TableHideZeros= s[i++];
        TableCopy= s[i++];
        TableRows= s[i++];
        TableEnterValue= s[i++];
        TableInvalidValue= s[i++];
        SelectMaterial= s[i++];
        MaterialLabel= s[i++];
        NewMaterial= s[i++];
        DBFilter= s[i++];
        NoMaterials= s[i++];
        Area= s[i++];
        AreaSections= s[i++];
        Areas = s[i++];
        DivideAreasXY = s[i++];
        DivideAreasAtNodes = s[i++];
        SelectOverlappingAreas = s[i++];
        AssignSectionToAreas = s[i++];
        AreaCoordPatternLoads = s[i++];
        AreaEdgeReleases = s[i++];
        AreaForces = s[i++];
        SelectedAreasInfo = s[i++];
        AreaDivisionsX = s[i++];
        AreaDivisionsY = s[i++];
        AreaSections = s[i++];
        AnalysisOptions = s[i++];
        AreaAutomesh = s[i++];
        BarAutomesh = s[i++];
        Factor = s[i++];
        AreaInternalForces = s[i++];
        InvertBarAxes= s[i++];
        AligneBarWithFirst= s[i++];
        InverAreaAxes= s[i++];
        AligneAtraWithFist= s[i++];
        ShiftAreaNodes = s[i++];
         Conectivity = s[i++];
   
        

        translateTableNames = new Dictionary<string, string>
        {
            ["gridCoordsX"] = gridCoordsX,
            ["gridCoordsY"] = gridCoordsY,
            ["gridCoordsZ"] = gridCoordsZ,
            ["barThermalLoads"] = barThermalLoads,
            ["programParams"] = programParams,
            ["loadCombs"] = loadCombs,
            ["loadCases"] = loadCases,
            ["nodalDisplacements"] = nodalDisplacements,
            ["nodalReactions"] = nodalReactions,
            ["nodalRestraints"] = nodalRestraints,
            ["nodes"] = nodes,
            ["nodalLoads"] = nodalLoads,
            ["materials"] = materials,
            ["sections"] = sections,
            ["bars"] = bars,
            ["barDistrLoads"] = barDistrLoads,
            ["barPunctLoads"] = barPunctLoads,
            ["barInternForces"] = barInternForces,
            ["nodalConstraints"] = nodalConstraints,
            ["Name"] = Name,
            ["Type"] = Type,
            ["Value"] = Value,
            ["Coord"] = Coord,
            ["coordSys"] = coordSys,
            ["label"] = label,
            ["startNode"] = startNode,
            ["endNode"] = endNode,
            ["Case"] = Case,
            ["Distance"] = Distance,
            ["MaterialID"] = MaterialID,
            ["SectionID"] = SectionID,
            ["MaterialName"] = MaterialName,
            ["SectionName"] = SectionName,
            ["unused"] = unused,
            ["Rotation"] = Rotation,
            ["releases"] = releases,
            ["offsetY"] = offsetY,
            ["offsetZ"] = offsetZ,
            ["masterNodeID"] = masterNodeID,
            ["nodesIDList"] = nodesIDList,
        }.ToFrozenDictionary();
    }
}