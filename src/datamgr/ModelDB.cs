using CEF.Dictionary;
using System.Text;
namespace CEF;
public class ModelDB
{
    public ModelDB()
    {
        InitThisClassProperies();
    }
    public void InitThisClassProperies()
    {
        gridCoordsX = [];
        gridCoordsY = [];
        gridCoordsZ = [];
        barThermalLoads = [];
        programParams = [];
        loadCombs = [];
        loadCases = [];
        nodalDisplacements = [];
        nodalReactions = [];
        materials = [];
        sections = [];
        nodalRestraints = [];
        nodes = [];
        bars = [];
        barDistrLoads = [];
        barPunctLoads = [];
        nodalLoads = [];
        barInternForces = [];
        nodalConstraints = [];

    }
    public DtaTable<programParam> programParams;    
    public DtaTable<material> materials ;       
    public DtaTable<section> sections ;       
    public DtaTable<coordinate> gridCoordsX ;       
    public DtaTable<coordinate> gridCoordsY ;       
    public DtaTable<coordinate> gridCoordsZ ;       
    public DtaTable<node> nodes ;       
    public DtaTable<bar> bars ;       
    public DtaTable<nodalRestraint> nodalRestraints ;       
    public DtaTable<nodalConstraint> nodalConstraints ;       
    public DtaTable<loadCase> loadCases ;       
    public DtaTable<loadComb> loadCombs ;       
    public DtaTable<nodalLoad> nodalLoads ;       
    public DtaTable<barDistrLoad> barDistrLoads ;       
    public DtaTable<barPunctLoad> barPunctLoads ;       
    public DtaTable<barThermalLoad> barThermalLoads ;       
    public DtaTable<nodalDisplacement> nodalDisplacements ;       
    public DtaTable<nodalReaction> nodalReactions ;       
    public DtaTable<barInternForce> barInternForces ;       
   
    internal int getParamValue(string varName)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == varName)
                if(int.TryParse(reg.Value,out int v))
                return v;
        return -1; 
    }
    internal int setParamValue(string Name, int value)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == Name)
            {
                reg.Value = $"{value}";
                return 0;
            }
        return -1; 
    }
    internal string getParamText(string varName)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == varName)
                return reg.Value;
        return ""; 
    }
    internal int setParamText(string Name, string value)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == Name)
            {
                reg.Value = value;
                return 0;
            }
        return -1; 
    }

    internal void SetSuperpositionCombosResults() {
      throw new NotImplementedException();
        foreach (var cc in loadCombs)
        {
            var nc = new loadCase()
            {
                ID = loadCases.Max(c => c.ID) + 1,
                Name = $"comb_{cc.Name}",
                Type = cc.Type
            };
        }
    }


    internal void SortNodesbyCoordinates()
    {
        nodes.Items = nodes.OrderBy(n => (n.Z, n.Y, n.X)).ToList();
        nodes.FastFillMap();
    }

    public void LoadModelFile(string ruta, bool reload = true)
    {
        DataMagrIO.mdb = this;
        DataMagrIO.ruta = ruta;
        DataMagrIO.LoadModelFile();
    }
    public void LoadDumpFile(string fileDump)
    {
        DataMagrIO.mdb = this;
        DataMagrIO.LoadStringFile(fileDump);
    }
    public void SaveModelFile(string ruta, bool reload = true)
    {
        DataMagrIO.mdb = this;
        DataMagrIO.ruta = ruta;
        DataMagrIO.SaveModelFile( );
    }
    public string SaveToStream()
    {
        DataMagrIO.mdb = this;    
        return  DataMagrIO.SaveToStream();

    }
    public void ExportarDXF(string ruta)
    {
        var sb = new StringBuilder();
        sb.AppendLine("0");
        sb.AppendLine("SECTION");
        sb.AppendLine("2");
        sb.AppendLine("ENTITIES");
        if (this.nodes != null)
        {
            foreach (var n in this.nodes.Items)
            {
                sb.AppendLine("0");
                sb.AppendLine("POINT");
                sb.AppendLine("8");      
                sb.AppendLine("NODOS");
                sb.AppendLine("10"); sb.AppendLine(n.X.ToString("0.########"));
                sb.AppendLine("20"); sb.AppendLine(n.Y.ToString("0.########"));
                sb.AppendLine("30"); sb.AppendLine(n.Z.ToString("0.########"));
            }
        }
        if (this.bars != null && this.nodes != null)
        {
            foreach (var b in this.bars.Items)
            {
                var ni = this.nodes.Items.FirstOrDefault(x => x.ID == b.startNode,null);
                var nj = this.nodes.Items.FirstOrDefault(x => x.ID == b.endNode,null);
                if (ni == null || nj == null) continue;
                sb.AppendLine("0");
                sb.AppendLine("LINE");
                sb.AppendLine("8");
                sb.AppendLine("BARRAS");
                sb.AppendLine("10"); sb.AppendLine(ni.X.ToString("0.########"));
                sb.AppendLine("20"); sb.AppendLine(ni.Y.ToString("0.########"));
                sb.AppendLine("30"); sb.AppendLine(ni.Z.ToString("0.########"));
                sb.AppendLine("11"); sb.AppendLine(nj.X.ToString("0.########"));
                sb.AppendLine("21"); sb.AppendLine(nj.Y.ToString("0.########"));
                sb.AppendLine("31"); sb.AppendLine(nj.Z.ToString("0.########"));
            }
        }
        sb.AppendLine("0");
        sb.AppendLine("ENDSEC");
        sb.AppendLine("0");
        sb.AppendLine("EOF");
        File.WriteAllText(ruta, sb.ToString(), Encoding.ASCII);
    }
}


public static class Mdta//meta data
{
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
    //---
    public static readonly IReadOnlySet<string> forceFields = new HashSet<string>() { "Fx", "Fy", "Fz",  "Fxi", "Fyi", "Fzi", "Fxj", "Fyj", "Fzj", "W" };
    public static readonly IReadOnlySet<string> momentFields = new HashSet<string>() {  "Mx", "My", "Mz", "Mxi", "Myi", "Mzi", "Mxj", "Myj", "Mzj" };
    public static readonly IReadOnlySet<string> lenFields = new HashSet<string>() { "a", "b", "dx", "dy", "dz", "Distance", "h", "tw", "tf", "ryy", "rzz", "ux", "uy", "uz", "X", "Y", "Z", "offsetY", "offsetZ", "Coord" };
    public static readonly IReadOnlySet<string> areaFields = new HashSet<string>() { "Ag", "Acyy", "Aczz" };
    public static readonly IReadOnlySet<string> pressFields = new HashSet<string>() { "E", "G", "fc", "fy", "fu" };
    public static readonly IReadOnlySet<string> len3Fields = new HashSet<string>() { "Szz", "Syy" };
    public static readonly IReadOnlySet<string> massFields = new HashSet<string>() { "M" };
    public static readonly IReadOnlySet<string> len4Fields = new HashSet<string>() { "Iyy", "Izz", "Iyz", "Tor", "Pyy", "Pzz" };
    public static readonly IReadOnlySet<string> themperatureFields = new HashSet<string>() { "extTheperature", "intThemperature" };

}



public class coordinate: IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public double Coord;
    public string coordSys;

    
}
public class barThermalLoad : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int barID;
    public int Case;
    public double extTheperature;
    public double intThemperature;
    public double b;
    public double h;
}
public class programParam : IHasID
{
    int IHasID.ID { get => -1; }
    public string Name;    
    public string Value;

}




public class loadComb : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int Case;
    public string Name;
    public int Type;
    public string Formula;
    public double Factor;
}
public class loadCase : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string Name;
    public int Type;
}
public class nodalDisplacement : IHasID
{
    int IHasID.ID { get => ((int)cefDictionary.makeKey(ID, Case)); }

    public int ID;
    public int Case;
    public double dx;
    public double dy;
    public double dz;
    public double rx;
    public double ry;
    public double rz;
}
public class nodalReaction : IHasID
{
    int IHasID.ID { get => ((int)(((long)ID << 24) | (uint)Case)); }
    public int ID;
    public int Case;
    public double Fx;
    public double Fy;
    public double Fz;
    public double Mx;
    public double My;
    public double Mz;
}
public class material : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string Name;
    public int Type;
    public double v;
    public double E;
    public double G;
    public double M;
    public double W;
    public double fc;
    public double fy;
    public double fu;
}
public class section : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string Name;
    public int Type;
    public double h;
    public double b;
    public double tw;
    public double tf;
    public double Ag;
    public double Iyy;
    public double Izz;
    public double Iyz;
    public double Tor;
    public double Acyy;
    public double Aczz;
    public double Syy;
    public double Szz;
    public double Pyy;
    public double Pzz;
    public double ryy;
    public double rzz;
}
public class nodalRestraint : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int ux;
    public int uy;
    public int uz;
    public int rx;
    public int ry;
    public int rz;
}
public class node : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string label;
    public double X;
    public double Y;
    public double Z;
}
public class bar : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string label;
    public int startNode;
    public int endNode;
    public int MaterialID;
    public int SectionID;
    public string MaterialName;
    public string SectionName;
    public int unused;
    public double Rotation;
    public int releases;
    public int offsetY;
    public int offsetZ;
}
public class barDistrLoad : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int BarID;
    public int Case;
    public int Type;
    public double a;
    public double b;
    public double Fx;
    public double Fy;
    public double Fz;
    public double Mx;
    public double My;
    public double Mz;
}
public class barPunctLoad : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int BarID;
    public int Case;
    public int Type;
    public double a;
    public double Fx;
    public double Fy;
    public double Fz;
    public double Mx;
    public double My;
    public double Mz;
}
public class nodalLoad : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int NodeID;
    public int Case;
    public double Fx;
    public double Fy;
    public double Fz;
    public double Mx;
    public double My;
    public double Mz;
}
public class barInternForce : IHasID
{
    int IHasID.ID { get => ((int)( ((long) BarID << 24) | (uint) Case));
 }

    public int BarID;
    public int Case;
    public int Subdivision;
    public double Distance;
    public double Fxi;
    public double Fyi;
    public double Fzi;
    public double Mxi;
    public double Myi;
    public double Mzi;
    public double Fxj;
    public double Fyj;
    public double Fzj;
    public double Mxj;
    public double Myj;
    public double Mzj;
}
public class nodalConstraint : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string label;
    public int Type;
    public string nodesIDList;
    public int masterNodeID;
}

public struct barGeom
{
    public bar Bar;
    public node P1;
    public node P2;
    public double len;

}