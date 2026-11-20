using System.Reflection.Metadata;

namespace CEF;
public static class Engine//<- Because is what is saw from the ui Engine.Start() etc..
{

    public static ModelDB model; //Main database shared for the program instance.
    public static AppDB appData ;//aux data base, intern -> not to be used by the user
    
    public static ModelDB getModel() => model;
    public static AppDB getAppData() => appData;



     public static void setParamProgramDefaults( string currUnits)
    {
        var regs = new List<(string, string)>
            {
            ( paramDefaults.units, currUnits),
            ( paramDefaults.analysed, "0"),
            ( paramDefaults.selfWeight, "0"),
            ( paramDefaults.ActiveCase , "0"),
            ( paramDefaults.divideIntoSections, "3"),            
            ( paramDefaults.pDeltaIters, "1"),
            ( paramDefaults.pDeltaLoadComb, "{0}{1}"),
            ( paramDefaults.BarAutoMesh, "1"),
            ( paramDefaults.AreaAutoMeh, "1"),
        };
        foreach (var r in regs)
        {
            var row = new programParam
            {
                Name = r.Item1,
                Value = r.Item2
            };
            model.programParams.Add(row);
        }
    }

    public static void clearModelDbs()
    {
       model = new ModelDB();
       appData = new AppDB();
    }
    public static void setDefaultsToDb(string currCoordSist)
    {
       model = new ModelDB();
       appData = new AppDB();

      model.gridCoordsX.Add(new Coordinate() {  
      Coord = 0d,
      coordSys = currCoordSist
        });
      model.gridCoordsY.Add(new Coordinate() {  
      Coord = 0d,
      coordSys = currCoordSist
        });
      model.gridCoordsZ.Add(new Coordinate() {  
      Coord = 0d,
      coordSys = currCoordSist
        });

        model.loadCases.Add(new() {
       ID = 0,
       Name = paramDefaults.DefaultLoadCaseName,
       Type = 0,
       });

       model.loadCombs.Add(new loadComb() {
       ID = 0,
       Name = paramDefaults.DefaultCombitationText,
       Case = 0,
       Factor = 1d,
       Type = 0,
       });
    }




public static void setUpUndo(){
    Undo.undoSetUp(model);
}


}

public static class paramDefaults
{
	//This is used for param table
    //Default fields
    //Dont modify here for Localization
    public const string units = "Units";
    public const string analysed = "Analyzed";
    public const string selfWeight = "SelfWeight Case";
    public const string ActiveCase = "ActiveLoad Case";
    public const string divideIntoSections = "Frame Divisions";
    public const string pDeltaIters = "Pdelta Iters";
    public const string pDeltaLoadComb = "Pdelta Load Comb";
    public const string DefaultLoadCaseName = "Dead";
    public const string DefaultText = "Default";
    public const string DefaultCombitationText = "Comb1";
    public const string BarAutoMesh = "Bar Auto Mesh";
    public const string AreaAutoMeh = "Area Auto Mesh";

}