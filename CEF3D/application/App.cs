namespace CEF;
#pragma warning disable CA2211
internal static class App
{
    public static  MainWindow mainWindow;

    public static ModelDB model; //Main database shared for the program instance.
    public static AppDB appData ;//aux data base, intern -> not to be used by the user


    public static bool initialized = false; // Is the main db initialiced?
    public const string baseLabCinematics = "const_";
    public const int baseIDCombCases = 1000; //base for the added nodes
    public const string baseLabCombCases = "comb_";

    public static string activeFileName;
    public static string currCoordSist;
    public static int currLoadCase;
    public static string currUnits;

    public static int g_gridXPointsCount, g_gridYPointsCount, g_gridZPointsCount;
    public static int materialDefaultID;
    public static int sectionDefaultID;
    public static int currColorScheme;
    internal static ModelStyles visualModelStyles;
    internal static bool visualModelStylesInitialized;
    
    static internal int modelViewInstances = 0;
    //Drawing states
    static internal bool onDrawingNewLine, onDrawingNewNode, onMovingNode;

    //Init functions (static) 
    internal static void setParamProgramDefaults()
    {
        var regs = new List<(string, string)>
            {
            ( Mdta.units, App.currUnits),
            ( Mdta.analysed, "0"),
            ( Mdta.selfWeight, "0"),
            ( Mdta.ActiveCase , "0"),
            ( Mdta.divideIntoSections, "3"),
            ( Mdta.pDeltaIters, "1"),
            ( Mdta.pDeltaLoadComb, "{0}{1}"),
        };
        foreach (var r in regs)
        {
            var row = new programParam
            {
                Name = r.Item1,
                Value = r.Item2
            };
            App.model.programParams.Add(row);
        }
    }


    internal static void setDefaultsToDb()
    {
        App.model = new ModelDB();
        App.appData = new AppDB();
        App.currCoordSist = Mdta.DefaultText;
        App.materialDefaultID = -1;
        App.sectionDefaultID = -1;
        App.currColorScheme = 2;
        App.initialized = true;
        //var CN = new defineNewCoordSys(this);
        //CN.TextBox1.Enabled = false;
        //CN.TextBox1.Text = "Default";
        //CN.ShowDialog();

        App.model.gridCoordsX[0].Coord = 0d;
        App.model.gridCoordsX[0].coordSys = App.currCoordSist;
        App.model.gridCoordsY[0].Coord = 0d;
        App.model.gridCoordsY[0].coordSys = App.currCoordSist;
        App.model.gridCoordsZ[0].Coord = 0d;
        App.model.gridCoordsZ[0].coordSys = App.currCoordSist;

        App.model.loadCases[0].ID = 0;
        App.model.loadCases[0].Name = Mdta.DefaultLoadCaseName;
        App.model.loadCases[0].Type = 0;
        App.model.loadCombs[0].ID = 0;
        App.model.loadCombs[0].Name = Mdta.DefaultCombitationText;
        App.model.loadCombs[0].Case = 0;
        App.model.loadCombs[0].Factor = 1d;
        App.model.loadCombs[0].Type = 0;
    }





    //Undo infra
    public static int max_back_bd = 100;
    private static string[] undoShadows = new string[max_back_bd];
    private static int undoSeek = 0;
    private static bool undoCapturePaused = false;

    public static void pushUndo(in string item)
    {
        if (string.IsNullOrEmpty(item)) return;
        undoSeek++;
        if (undoSeek >= max_back_bd)
            undoSeek = 0;
        undoShadows[undoSeek] = item;
    }

    public static string popUndo()
    {
        var ret = undoShadows[undoSeek];
        if (string.IsNullOrEmpty(ret)|| ret == "") return string.Empty;        
        undoShadows[undoSeek] = string.Empty;
        undoSeek--;
        if (undoSeek < 0) 
            undoSeek = (max_back_bd-1);
        return ret;
    }

    public static void restartUndo()
    {
        undoSeek = 0;
        undoShadows = new string[max_back_bd];
    }


    public static  void stopUndoRecording() { undoCapturePaused = true; }
    public static  void startUndoRecording() { undoCapturePaused = false; }

    public static void takeUndoSnapShot()
    {
        if (!undoCapturePaused)
        {
            pushUndo(model.SaveToStream());
        }
    }
    //End Undo infra
}


//Todo dicts de strings para UI
public enum LoadCaseType //Tipo en la base de datos (int)
{
   Dead=0, 
   Live, 
   Rain,
   Snow,//Granizo en Colombia
   Roof,
   Wind,
   Quake,
   Impact,
   Fatigue,
   Thermic,
   Ice,
   Fluids, //Water
   Construction,
   Accidental, 
   Blast,
   Vehicles,
   Prestress,
   Other,
}
public enum AnalysisType //Aun no se almacena
{
    LinearStatic,
    SecondOrderPD,
    Modal, 
    //GeometricNonlinear,
    //NonlinearMaterial,
    Nonlinear,
    PushOver,
    Buckling,
    ThermicStructural,
    ConstructionStages,
    SoilStructure,
}
public enum sectType
{
    Rect ,
    Circ ,
    HolRec ,
    HolCirc ,
    Angle,
    IShape ,
    WShape ,
    CProfile
}


public enum materialType
{
    Steel ,
    Concrete ,
    Aluminium ,
    Wood ,
    Bambu ,
}
//Multi freedom Restriction/constraints type
public enum MFCType
{
    diaphragm = 0,//Z
    diaphragmXX,
    diaphragmYY,
    plate,//Z
    plateXX,
    plateYY,
    body,
    weld,
    beamX,
    beamY,
    beamZ,
    rodX,
    rodY,
    rodZ,
    equal
}