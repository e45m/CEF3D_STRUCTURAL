using CEF.Dictionary;
using System.Diagnostics.Metrics;
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
        areas= [];
        areaSections = [];
        areaInternalForces = [];

    }
    public DtaTable<programParam> programParams;    
    public DtaTable<Material> materials ;       
    public DtaTable<Section> sections ;       
    public DtaTable<Coordinate> gridCoordsX ;       
    public DtaTable<Coordinate> gridCoordsY ;       
    public DtaTable<Coordinate> gridCoordsZ ;       
    public DtaTable<Node> nodes ;       
    public DtaTable<Bar> bars ;       
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
    public DtaTable<Area> areas ;       
    public DtaTable<areaSection> areaSections ;       
    public DtaTable<areaInternalForce> areaInternalForces ;       
   
    public int getParamValue(string varName)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == varName)
                if(int.TryParse(reg.Value,out int v))
                return v;
        return -1; 
    }
    public int setParamValue(string Name, int value, bool create=false)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == Name)
            {
                reg.Value = $"{value}";
                return 0;
            }
        if(!create)
        return -1; 

        var newParam = new programParam() {
            Name= Name,
            Value = $"{value}",
        };
        programParams.Add(newParam);
        return 0;
    }
    public string getParamText(string varName)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == varName)
                return reg.Value;
        return ""; 
    }
    public int setParamText(string Name, string value, bool create=false)
    {
        foreach (var reg in this.programParams)
            if (reg.Name == Name)
            {
                reg.Value = value;
                return 0;
            }
       if(!create)
        return -1; 

        var newParam = new programParam() {
            Name= Name,
            Value = value,
        };
        programParams.Add(newParam);
        return 0;
    }

    //Using this implementation limits the modelo to 2MM NODES or 12MM GDL
    public static int calcCombinedIndex(int ID, int other){
        //if(ID>2000000 || other>1000) return -1;
        return  (ID << 10) | other;
    }
    public static int calcIDfromCombinedIndex(int hash)=> hash >> 10;
    public static int calcOtherfromCombinedIndex(int hash)=> hash & 1023;
       

    public void SortNodesbyCoordinates()
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





public class Coordinate: IHasID
{
    int IHasID.ID { get =>ID; }
    public int ID;
    public double Coord;
    public string coordSys;
    
}
public class barThermalLoad : IHasID
{
    int IHasID.ID { get => ID; }

    public int ID;
    public int BarID;
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
   // public string Formula;
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
    int IHasID.ID { get => ID; }
    public int ID;
    public int NodeID;//Change to nodeID
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
    int IHasID.ID { get => ID; } 
    public int ID;//change to nodeID
    public int NodeID;//change to nodeID
    public int Case;
    public double Fx;
    public double Fy;
    public double Fz;
    public double Mx;
    public double My;
    public double Mz;
}
public class Material : IHasID
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
public class Section : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string Name;
    public int Type;
    public string MaterialName;
    public int MaterialID;
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
public class Node : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string label;
    public double X;
    public double Y;
    public double Z;
	
	
}
public class Bar : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public string label;
    public int startNode;
    public int endNode;
    public int SectionID;
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
    public double l1;
    public double l2;
    public double Fx;
    public double Fy;
    public double Fz;

}
public class barPunctLoad : IHasID
{
    int IHasID.ID { get => ID; }
    
    public int ID;
    public int BarID;
    public int Case;
    public int Type;
    public double l1;
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
    int IHasID.ID { get => ID; }
    public int ID;
    public int BarID;
    public int Case;
    public double Pos;
    public double St;
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
    public Bar Bar;
    public Node P1;
    public Node P2;
    public double len;

}

public class areaSection : IHasID
{
   int IHasID.ID { get => ID; }
   public int ID;
   public string Name;
   public int Kind; //Kind of area thin, thick, shell, membrane, etc, multilayer..    
   public int MaterialID;
   public string MaterialName;
   public double Thickness;


}

public class Area : IHasID
{
   int IHasID.ID { get => ID; }
   public int ID;
   public string label;   
   public int SectionID;//area section contains all the kinds and posibilities this struct only defines general geon and info
   public string SectionName;
   public  int Node1, Node2, Node3, Node4; //N4 is allways the first Counter Clock wise is postive fase
   public int sign; //+1 is normail orientation -1 inverted axes   
}

public class areaLoad : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int AreaID;
    public int Case;
    public int Type;//Local or Global
    public double n1;//Force pararallel vector n1,n2,n3
    public double n2;
    public double n3;
    //Local axes 1
    public double Fn;//Force/unit 
    

}

public class areaInternalForce : IHasID
{
    int IHasID.ID { get => ID; }
    public int ID;
    public int AreaID;
    public int Case;
    
    public double Fx1, Fy1, Fz1, Mx1, My1, Mz1;
    public double Fx2, Fy2, Fz2, Mx2, My2, Mz2;
    public double Fx3, Fy3, Fz3, Mx3, My3, Mz3;
    public double Fx4, Fy4, Fz4, Mx4, My4, Mz4;

    //...???

}

