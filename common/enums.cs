//Todo dicts de strings para UI

#if !__CEFENUMS__
#define __CEFENUMS__

public enum areaKind
{
Membrane = 0,//u,v
Shell,//full Minlin shell requires implement numeric integration...
Hole,//A hole in other area shape if has no intersect is like none, this will be used for the mesh functions ..
None, //Just undifined empty is omited here
}

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
   Combination,//Load cases from load combinations
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
    CProfile,
    Custom, //Section desinger
    other

}


public enum materialType
{
    Steel ,
    Concrete ,
    Aluminium ,
    Wood ,
    Bambu ,
    Tendon, 
    Rebar,
}
//Multi freedom Restriction/constraints type
[Flags]
public enum MFCType
{
    diaphragm = 0,//Z    
    plate,//Z      
    body,
    weld,
    //diaphragGen,
    //plateGen,
    beam,
    //rod,
    equal
}


    [Flags]
public enum CalcState
{
    None = 0,
    Primal = 1,
    Dual=2,
    Master=4,
    Slave = 8,
    Inactive=16,
    OK=32,
    Error=64,
    Pivot=128,
    Absent=256,
    Negative=512,
    MFCConstr=1024,
};

[Flags]
public enum BarActions
{
    None = 0,
    N = 1,          // axial
    Vy = 2,         // cortante en y local
    Vz = 4,         // cortante en z local
    Mx = 8,         // torsión
    My = 16,        // momento alrededor de y local
    Mz = 32,        // momento alrededor de z local
    Shear = Vy | Vz,
    Bending = My | Mz,
    All = N | Vy | Vz | Mx | My | Mz
}

[Flags]
public enum AreaAction{
    None = 0,
    Mx,
    My,
    Mz,
    Fx,
    Fy,
    Fz,
}


    public enum ViewFrameView
    {
        ThreeDimensional,
        XY,
        XZ,
        YZ,
    }

    #endif