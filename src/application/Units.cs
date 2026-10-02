namespace CEF;
public enum ModelUnits
{
    //Force
    kN = 0,
    N,
    kgf,
    Tonf,
    lbf,
    kip,//pound force
    //Mass
    kg,
    pound,
    kpound,
    //lenght
    m,
    cm,
    mm,
    inch,
    foot,
    //Temp
    C,
    F,
    K
}

internal static class Uc//unitsconverter
{
    public static string[] supportedUnits  = [
            "kN,kg,m,C", //Default and data base used
            "kN,kg,cm,C",
            "kN,kg,mm,C",
            "N,kg,m,C",
            "N,kg,cm,C",
            "N,kg,mm,C",
            "Tonf,kg,m,C",
            "Tonf,kg,cm,C",
            "Tonf,kg,mm,C",
            "lbf,pound,inch,F",
            "lbf,pound,ft,F",
            "kip,pound,inch,F",
            "kip,pound,ft,F"
        ];

    //Unites helper function.
    private static readonly Dictionary<ModelUnits, double> uFactor =
        new Dictionary<ModelUnits, double>
    {
    { ModelUnits.kN,      1.0 },//Default
    { ModelUnits.N,       0.001 },                     
    { ModelUnits.kgf,     0.009806652048217224 },      
    { ModelUnits.Tonf,    9.806652048217224 },         
    { ModelUnits.lbf,     0.0044482216152605 },        
    { ModelUnits.kip,     4.4482216152605 },           
    { ModelUnits.kg,      1.0 },//Default
    { ModelUnits.pound,   0.45359237 },                
    { ModelUnits.kpound,  453.59237 },                 
    { ModelUnits.m,       1.0 },//Default
    { ModelUnits.cm,      0.01 },                      
    { ModelUnits.mm,      0.001 },                     
    { ModelUnits.inch,    0.0254 },                    
    { ModelUnits.foot,    0.3048 },                    
    { ModelUnits.C,       1.0 },//Default
    { ModelUnits.F,       0.5555555555555556 },        
    { ModelUnits.K,       1.0 }
    };
    static double force;
    static double lenFact;// {  set; get; }
    static double mass;
    static double acc;
    static double time ;
    static double vel ;
    static double area ;
    static double volume ;
    static double temp ;

    public static ModelUnits activeForceU { get; private set; }
    public static ModelUnits activeMassU { get; private set; }
    public static ModelUnits activeLenU { get; private set; }
    public static ModelUnits activeTempU { get; private set; }

    public static void ChangeUc(string kNmC)
    {
        setUc(kNmC);

    }

    public static double forceToUserUnits(double v) => v / force;
    public static double forceFromUserUnits(double v) => v * force;
    public static double lenToUserUnits(double v) => v / lenFact;
    public static double lenFromUserUnits(double v) => v * lenFact;
    public static double massToUserUnits(double v) => v / mass;
    public static double massFromUserUnits(double v) => v * mass;
    public static double accToUserUnits(double v) => v / acc;
    public static double accFromUserUnits(double v) => v * acc;
    public static double timeToUserUnits(double v) => v / time;
    public static double timeFromUserUnits(double v) => v * time;
    public static double velToUserUnits(double v) => v / vel;
    public static double velFromUserUnits(double v) => v * vel;
    public static double areaToUserUnits(double v) => v / area;
    public static double areaFromUserUnits(double v) => v * area;
    public static double volToUserUnits(double v) => v / volume;
    public static double volFromUserUnits(double v) => v * volume;
    public static double tempToUserUnits(double v) => ConvertTemp(v, ModelUnits.C, activeTempU);
    public static double tempFromUserUnits(double v) => ConvertTemp(v, activeTempU,ModelUnits.C);
    public static double len4ToUserUnits(double v) => v / (area * area);
    public static double len4FromUserUnits(double v) => v * (area * area);
    public static double len3ToUserUnits(double v) => v / (lenFact * area);
    public static double len3FromUserUnits(double v) => v * (lenFact * area);
    public static double forcePerUnitAreaToUserUnits(double v) => v / (force / area);
    public static double pressFromUserUnits(double v) => v * (force/ area);
    public static double momentToUserUnits(double v) => v / (force *lenFact);
    public static double momentFromUserUnits(double v) => v * (force*lenFact);


    public static double forceToUserUnits(string v) => forceToUserUnits(Mis.stringToDouble(v));
    public static double forceFromUserUnits(string v) => forceFromUserUnits(Mis.stringToDouble(v));
    public static double lenToUserUnits(string v) => lenToUserUnits(Mis.stringToDouble(v));
    public static double lenFromUserUnits(string v) => lenFromUserUnits(Mis.stringToDouble(v));
    public static double massToUserUnits(string v) => massToUserUnits(Mis.stringToDouble(v));
    public static double massFromUserUnits(string v) => massFromUserUnits(Mis.stringToDouble(v));
    public static double accToUserUnits(string v) => accToUserUnits(Mis.stringToDouble(v));
    public static double accFromUserUnits(string v) => accFromUserUnits(Mis.stringToDouble(v));
    public static double timeToUserUnits(string v) => timeToUserUnits(Mis.stringToDouble(v));
    public static double timeFromUserUnits(string v) => timeFromUserUnits(Mis.stringToDouble(v));
    public static double velToUserUnits(string v) => velToUserUnits(Mis.stringToDouble(v));
    public static double velFromUserUnits(string v) => velFromUserUnits(Mis.stringToDouble(v));
    public static double areaToUserUnits(string v) => areaToUserUnits(Mis.stringToDouble(v));
    public static double areaFromUserUnits(string v) => areaFromUserUnits(Mis.stringToDouble(v));
    public static double volToUserUnits(string v) => volToUserUnits(Mis.stringToDouble(v));
    public static double volFromUserUnits(string v) => volFromUserUnits(Mis.stringToDouble(v));
    public static double tempToUserUnits(string v) => tempToUserUnits(Mis.stringToDouble(v));
    public static double tempFromUserUnits(string v) => tempFromUserUnits(Mis.stringToDouble(v));
    public static double len4ToUserUnits(string v) => len4ToUserUnits(Mis.stringToDouble(v));
    public static double len4FromUserUnits(string v) => len4FromUserUnits(Mis.stringToDouble(v));
    public static double len3ToUserUnits(string v) => len3ToUserUnits(Mis.stringToDouble(v));
    public static double len3FromUserUnits(string v) => len3FromUserUnits(Mis.stringToDouble(v));
    public static double pressToUserUnits(string v) => forcePerUnitAreaToUserUnits(Mis.stringToDouble(v));
    public static double pressFromUserUnits(string v) => pressFromUserUnits(Mis.stringToDouble(v));
    public static double momentToUserUnits(string v) => momentToUserUnits(Mis.stringToDouble(v));
    public static double momentFromUserUnits(string v) => momentFromUserUnits(Mis.stringToDouble(v));


    public static void setUc()
    {
        force = uFactor[activeForceU];
        lenFact = uFactor[activeLenU];
        mass = uFactor[activeMassU]; ;
        temp = uFactor[activeTempU];
        time = 1;
        acc = lenFact;// /time^2
        vel = lenFact;
        area = lenFact * lenFact;
        volume = area * lenFact;
    }
    public static void setUc(string kNmC)
    {
        var input = kNmC.Split(",");
        ParseForce(input[0]);
        ParseMass(input[1]);
        ParseLength(input[2]);
        ParseTemp(input[3]);
        setUc();
    }
    private static void ParseForce(string s)
    {
        switch (s.Trim().ToLower())
        {
            case "kn": activeForceU = ModelUnits.kN; break;
            case "n": activeForceU = ModelUnits.N; break;
            case "kgf": activeForceU = ModelUnits.kgf; break;
            case "tonf": activeForceU = ModelUnits.Tonf; break;
            case "lbf": activeForceU = ModelUnits.lbf; break;
            case "kip":
            case "kpi": activeForceU = ModelUnits.kip; break;
        }
    }
    private static void ParseMass(string s)
    {
        switch (s.Trim().ToLower())
        {
            case "kg": activeMassU = ModelUnits.kg; break;
            case "lb":
            case "pound": activeMassU = ModelUnits.pound; break;
            case "klb":
            case "kpound": activeMassU = ModelUnits.kpound; break;
        }
    }
    private static void ParseLength(string s)
    {
        switch (s.Trim().ToLower())
        {
            case "m": activeLenU = ModelUnits.m; break;
            case "cm": activeLenU = ModelUnits.cm; break;
            case "mm": activeLenU = ModelUnits.mm; break;
            case "in":
            case "inch": activeLenU = ModelUnits.inch; break;
            case "ft":
            case "foot": activeLenU = ModelUnits.foot; break;
        }
    }
    private static void ParseTemp(string s)
    {
        switch (s.Trim().ToLower())
        {
            case "c": activeTempU = ModelUnits.C; break;
            case "f": activeTempU = ModelUnits.F; break;
            case "k": activeTempU = ModelUnits.K; break;
        }
    }
    private static double ConvertTemp(double value, ModelUnits uPrev, ModelUnits uNew)
    {
        // tabla factor + offset: de unidad → °C
        static (double f, double o) ToC(ModelUnits u) => u switch
        {
            ModelUnits.C => (1.0, 0.0),
            ModelUnits.F => (5.0 / 9.0, -32.0 * 5.0 / 9.0),
            ModelUnits.K => (1.0, -273.15),
            _ => (1.0, 0.0)
        };
        // tabla factor + offset: de °C → unidad
        static (double f, double o) FromC(ModelUnits u) => u switch
        {
            ModelUnits.C => (1.0, 0.0),
            ModelUnits.F => (9.0 / 5.0, 32.0),
            ModelUnits.K => (1.0, 273.15),
            _ => (1.0, 0.0)
        };
        // convertir valor previo → °C
        var t1 = ToC(uPrev);
        double c = value * t1.f + t1.o;
        // convertir °C → nueva unidad
        var t2 = FromC(uNew);
        return c * t2.f + t2.o;
    }
}