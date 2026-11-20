namespace CEF;
#pragma warning disable CA2211
internal static class App
{
    public static  MainWindow mainWindow;


    public static bool initialized = false; // Is the main db initialiced?
    public const string baseLabCinematics = "const_";
    public const int baseIDCombCases = 1000; //base for the added nodes
    public const string baseLabCombCases = "comb_";

    public static string activeFileName;
    public static string currCoordSist;
    public static int currLoadCase;
    public static string currUnits;

    public static int g_gridXPointsCount, g_gridYPointsCount, g_gridZPointsCount;
    
    public static int barSectionDefaultID;
    public static int areaSectionDefaultID;

    public static int currColorScheme;
    internal static UISettings UserSettings;
    
    
    static internal int modelViewInstances = 0;
    //Drawing states
    static internal bool onDrawingNewLine, onDrawingNewNode, onMovingNode,  onDrawingNewArea;

    //Init functions (static) 
    internal static void setParamProgramDefaults()
    {
       Engine.setParamProgramDefaults(currUnits);

    }


    internal static void setDefaultsToDb()
    {

        Engine.setDefaultsToDb(currCoordSist);
        Engine.setUpUndo();
    
       currCoordSist = paramDefaults.DefaultText;
       areaSectionDefaultID = -1;
       barSectionDefaultID = -1;
       currColorScheme = 2;

       initialized = true;

    }


    public static void takeUndoSnapShot(){

        Undo.takeUndoSnapShot();
    }
    public static void stopUndoRecording(){

        Undo.stopUndoRecording();
    }
    public static void startUndoRecording(){

        Undo.startUndoRecording();
    }
    public static void restartUndo(){

        Undo.restartUndo();
    }
    public static string popUndo(){

        return Undo.popUndo();
    }



}



