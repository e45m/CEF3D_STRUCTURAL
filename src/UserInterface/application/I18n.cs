using System.Collections.Frozen;

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
    public const int Count = 289;

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

    public static string About;
    public static string File;
    public static string New;
    public static string Open;
    public static string Save;
    public static string SaveAs;
    public static string Export;
    public static string ExportToDxf;
    public static string Import;
    public static string ImportSap2000S2k;
    public static string Exit;
    public static string Edit;
    public static string Copy;
    public static string Replicate;
    public static string Move;
    public static string Rotate;
    public static string Delete;
    public static string Split;
    public static string SplitByLengthOrElements;
    public static string SplitAtIntersectionWithSelectedBars;
    public static string SplitAtIntersectionWithSelectedNodes;
    public static string MoveNodeGraphically;
    public static string RemoveOrphanNodes;
    public static string RemoveOverlappingNodes;
    public static string MergeSelectedBars;
    public static string Undo;
    public static string Draw;
    public static string Node;
    public static string NodeGraphically;
    public static string Bar;
    public static string Select;
    public static string SelectOverlappingNodes;
    public static string SelectOverlappingBars;
    public static string SelectAll;
    public static string DeselectAll;
    public static string Model;
    public static string materials;
    public static string sections;
    public static string Nodes;
    public static string BoundaryConditions;
    public static string Loads;
    public static string Displacements;
    public static string KinematicConstraints;
    public static string Bars;
    public static string LoadsEllipsis;
    public static string DistributedLoads;
    public static string PointLoads;
    public static string ReleaseDegreeOfFreedom;
    public static string InsertionPoints;
    public static string CoordinateSystems;
    public static string NewEdit;
    public static string LoadCases;
    public static string EditCreateDelete;
    public static string SelectActiveCase;
    public static string LoadCombinations;
    public static string Databases;
    public static string TreeView;
    public static string TableView;
    public static string Units;
    public static string Analysis;
    public static string LinearElasticAnalysis;
    public static string LinearAnalysisWithPd;
    public static string Options;
    public static string Results;
    public static string ViewTables;
    public static string NodeDisplacements;
    public static string NodeReactions;
    public static string BarInternalForces;
    public static string Diagrams;
    public static string ClearResults;
    public static string View;
    public static string DisplayOptions;
    public static string RestoreView;
    public static string Viewframes;
    public static string View1;
    public static string View2;
    public static string View3;
    public static string View4;
    public static string Views;
    public static string XY;
    public static string XZ;
    public static string YZ;
    public static string View3D;
    public static string Zoom;
    public static string ZoomWindow;
    public static string ZoomExtents;
    public static string ZoomToSelection;
    public static string Info;
    public static string MeasureDistanceBetweenTwoNodes;
    public static string SelectedBarsInfo;
    public static string Windows;
    public static string NewWindow;
    public static string Help;
    public static string ReferenceManual;
    public static string License;
    public static string Test;
    public static string SnapPoint;//Not usded, nor delete
    public static string SnapEnd; //Not usded, nor delete
    public static string SnapCenter;//Not usded, nor delete
    public static string SnapNear;//Not usded, nor delete
    public static string SnapPerpendicular;//Not usded, nor delete
    public static string gridCoordsX;
    public static string gridCoordsY;
    public static string gridCoordsZ;
    public static string barThermalLoads;
    public static string programParams;
    public static string loadCombs;
    public static string loadCases;
    public static string nodalDisplacements;
    public static string nodalReactions;
    public static string nodalRestraints;
    public static string nodes;
    public static string nodalLoads;
    public static string bars;
    public static string barDistrLoads;
    public static string barPunctLoads;
    public static string barInternForces;
    public static string nodalConstraints;
    public static string Name;
    public static string Type;
    public static string Value;
    public static string Coord;
    public static string coordSys;
    public static string label;
    public static string startNode;
    public static string endNode;
    public static string Case;
    public static string Distance;
    public static string MaterialID;
    public static string SectionID;
    public static string MaterialName;
    public static string SectionName;
    public static string unused;
    public static string Rotation;
    public static string releases;
    public static string offsetY;
    public static string offsetZ;
    public static string masterNodeID;
    public static string nodesIDList;
    public static string NewNode;
    public static string ImputInstr1;
    public static string OutPutInstr1;
    public static string ImputOffests;
    public static string offsetX2;
    public static string offsetY2;
    public static string offsetZ2;
    public static string ImputRotation;
    public static string RotationX;
    public static string RotationY;
    public static string RotationZ;
    public static string ImputDireVect;
    public static string DireVectX;
    public static string DireVectY;
    public static string DireVectZ;
    public static string ImputRotationCent;
    public static string RotCenX;
    public static string RotCenY;
    public static string RotCenZ;
    public static string ImputSpacing;
    public static string ImputSpacingExample;
    public static string Spacing;
    public static string DivideByElemsCuant;
    public static string DivideByEleemLen;
    public static string ImputMsgDivideByElemsCuant;
    public static string ImputMsgDivideByEleemLen;
    public static string NoOverlappedBarsFound;
    public static string DlgOKText;
    public static string DlgCancelText;
    public static string LoadDB;
    public static string EditCreate;
    public static string leftArrow;
    public static string rightArrow;
    public static string CannotLoadDB;
    public static string DiaphragmZ;
    public static string DiaphragmXX;
    public static string DiaphragmYY;
    public static string PlateZ;
    public static string PlateXX;
    public static string PlateYY;
    public static string Body;
    public static string Weld;
    public static string BeamX;
    public static string BeamY;
    public static string BeamZ;
    public static string RodX;
    public static string RodY;
    public static string RodZ;
    public static string Equal;
    public static string CheckInput;
    public static string Replace;
    public static string SetNodalLoads;
    public static string SetPunctBarLoads;
    public static string Fraction;
    public static string Global;
    public static string Local;
    public static string CalProps;
    public static string Add;
    public static string AddNewLoadCase;
    public static string AssignMaterialToBars;
    public static string AssignSectionToBars;
    public static string Restrain;
    public static string Fixed;
    public static string Free;
    public static string Hinge;
    public static string Roller;
    public static string RotationAngle;
    public static string ip_center;
    public static string ip_down;
    public static string ip_up;
    public static string ip_left;
    public static string ip_right;
    public static string min;
    public static string max;
    public static string ValueAtStartNode;
    public static string ValueAtEndNode;
    public static string BatchImput;
    public static string CategoryScope;
    public static string CategoryCanvas;
    public static string CategoryModelElements;
    public static string CategorySelect;
    public static string CategoryAppearance;
    public static string CategoryLoads;
    public static string CategoryResults;
    public static string CategoryFontsAndText;
    public static string CategoryAuxiliaryElements;
    public static string CategoryStyles;
    public static string DisplayApplyToAllWindows;
    public static string DisplayApplyToNewWindows;
    public static string DisplayCanvasBackgroundColor;
    public static string DisplayNodeColor;
    public static string DisplayBarShadowColor;
    public static string DisplayNodeThickness;
    public static string DisplayBarLineColor;
    public static string DisplayBarLineWidth;
    public static string DisplaySelectedColor;
    public static string DisplayLoadsColor;
    public static string DisplayPointLoadsColor;
    public static string DisplayDeformedShapeColor;
    public static string DisplaySupportBackgroundColor;
    public static string DisplayFont;
    public static string DisplayGridLineColor;
    public static string DisplayGridLineWidth;
    public static string DisplayRenderDivisions;
    public static string DisplayShowNodeId;
    public static string DisplayShowGrid;
    public static string DisplayShowBarLoads;
    public static string DisplayShowDeformedShape;
    public static string DisplayShowNodeLoads;
    public static string DisplayShowNodes;
    public static string DisplayShowSelectedNodes;
    public static string DisplayRenderStyle;
    public static string DisplayGridLineStyle;
    public static string DisplayVisualTheme;
    public static string DisplayDeformScaleLinear;
    public static string DisplayDeformScaleRotational;
    public static string DescriptionRenderSegments;
    public static string DescriptionRenderOptions;
    public static string DescriptionCanvasStyle;
    public static string Element;
    public static string ElementSummary;
    public static string Length;
    public static string CaseOrCombination;
    public static string Action;
    public static string NoResultsForSelection;
    public static string ActionSetAll;
    public static string ActionSetPlaneXY;
    public static string ActionSetPlaneXZ;
    public static string ActionSetShears;
    public static string ActionSetMoments;
    public static string ActionAxial;
    public static string ActionShearY;
    public static string ActionShearZ;
    public static string ActionTorsion;
    public static string ActionMomentY;
    public static string ActionMomentZ;
    public static string TableSearch;
    public static string TableCase;
    public static string TableIds;
    public static string TableAll;
    public static string TableHideZeros;
    public static string TableCopy;
    public static string TableRows;
    public static string TableEnterValue;
    public static string TableInvalidValue;
    public static string SelectMaterial;
    public static string MaterialLabel;
    public static string NewMaterial;
    public static string DBFilter;
    public static string NoMaterials;

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
        DiaphragmZ = s[i++];
        DiaphragmXX = s[i++];
        DiaphragmYY = s[i++];
        PlateZ = s[i++];
        PlateXX = s[i++];
        PlateYY = s[i++];
        Body = s[i++];
        Weld = s[i++];
        BeamX = s[i++];
        BeamY = s[i++];
        BeamZ = s[i++];
        RodX = s[i++];
        RodY = s[i++];
        RodZ = s[i++];
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