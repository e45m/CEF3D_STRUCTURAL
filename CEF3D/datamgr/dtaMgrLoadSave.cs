using System.Text;
namespace CEF;
internal static class DataMagrIO
{
    internal static ModelDB mdb ;
    internal static string ruta;
   



    public static void SaveModelFile()
    {
        using var sw = new StreamWriter(ruta, false, Encoding.UTF8);
        sw.Write(SaveToStream());
    }


    public static string  SaveToStream()
    {
         var sw = new StringBuilder();
        sw.AppendLine("CEF3D_File_ver=0");
        void W(string table, IEnumerable<string> rows)
        {
            sw.AppendLine(table);
            foreach (var r in rows)
                sw.AppendLine(r);
        }
        W("programParams",
            mdb.programParams.Select(x =>
                $"Name={x.Name}\tValue={x.Value}"));
        W("materials",
            mdb.materials.Select(x =>
                $"ID={x.ID}\tName={x.Name}\tType={x.Type}\tv={x.v}\tE={x.E}\tG={x.G}\tM={x.M}\tW={x.W}\tfc={x.fc}\tfy={x.fy}\tfu={x.fu}"));
        W("sections",
            mdb.sections.Select(x =>
                $"ID={x.ID}\tName={x.Name}\tType={x.Type}\th={x.h}\tb={x.b}\ttw={x.tw}\ttf={x.tf}\tAg={x.Ag}\tIyy={x.Iyy}\tIzz={x.Izz}\tIyz={x.Iyz}\tTor={x.Tor}\tAcyy={x.Acyy}\tAczz={x.Aczz}\tSyy={x.Syy}\tSzz={x.Szz}\tPyy={x.Pyy}\tPzz={x.Pzz}\tryy={x.ryy}\trzz={x.rzz}"));
        W("gridCoordsX", mdb.gridCoordsX.Select(x => $"ID={x.ID}\tCoord={x.Coord}\tcoordSys={x.coordSys}"));
        W("gridCoordsY", mdb.gridCoordsY.Select(x => $"ID={x.ID}\tCoord={x.Coord}\tcoordSys={x.coordSys}"));
        W("gridCoordsZ", mdb.gridCoordsZ.Select(x => $"ID={x.ID}\tCoord={x.Coord}\tcoordSys={x.coordSys}"));
        W("nodes",
            mdb.nodes.Select(x =>
                $"ID={x.ID}\tlabel={x.label}\tX={x.X}\tY={x.Y}\tZ={x.Z}"));
        W("bars",
            mdb.bars.Select(x =>
                $"ID={x.ID}\tlabel={x.label}\tstartNode={x.startNode}\tendNode={x.endNode}\tMaterialID={x.MaterialID}\tMaterialName={x.MaterialName}\tSectionID={x.SectionID}\tSectionName={x.SectionName}\tunused={x.unused}\tRotation={x.Rotation}\treleases={x.releases}\toffsetY={x.offsetY}\toffsetZ={x.offsetZ}"));
        W("nodalRestraints",
            mdb.nodalRestraints.Select(x =>
                $"ID={x.ID}\tux={x.ux}\tuy={x.uy}\tuz={x.uz}\trx={x.rx}\try={x.ry}\trz={x.rz}"));
        W("nodalConstraints",
            mdb.nodalConstraints.Select(x =>
                $"ID={x.ID}\tlabel={x.label}\tType={x.Type}\tnodesIDList={x.nodesIDList}\tmasterNodeID={x.masterNodeID}"));
        W("loadCases",
            mdb.loadCases.Select(x =>
                $"ID={x.ID}\tName={x.Name}\tType={x.Type}"));
        W("loadCombs",
            mdb.loadCombs.Select(x =>
                $"ID={x.ID}\tName={x.Name}\tType={x.Type}\tCase={x.Case}\tFactor={x.Factor}"));
        W("nodalLoads",
            mdb.nodalLoads.Select(x =>
                $"ID={x.ID}\tNodeID={x.NodeID}\tCase={x.Case}\tFx={x.Fx}\tFy={x.Fy}\tFz={x.Fz}\tMx={x.Mx}\tMy={x.My}\tMz={x.Mz}"));
        W("barDistrLoads",
            mdb.barDistrLoads.Select(x =>
                $"ID={x.ID}\ta={x.a}\tb={x.b}\tBarID={x.BarID}\tCase={x.Case}\tFx={x.Fx}\tFy={x.Fy}\tFz={x.Fz}\tMx={x.Mx}\tMy={x.My}\tMz={x.Mz}"));
        W("barPunctLoads",
            mdb.barPunctLoads.Select(x =>
                $"ID={x.ID}\ta={x.a}\tBarID={x.BarID}\tCase={x.Case}\tFx={x.Fx}\tFy={x.Fy}\tFz={x.Fz}\tMx={x.Mx}\tMy={x.My}\tMz={x.Mz}"));
        W("barThermalLoads",
            mdb.barThermalLoads.Select(x =>
                $"ID={x.ID}\tbarID={x.barID}\tCase={x.Case}\textTheperature={x.extTheperature}\tintThemperature={x.intThemperature}\tb={x.b}\th={x.h}"));
        W("nodalDisplacements",
            mdb.nodalDisplacements.Select(x =>
                $"ID={x.ID}\tCase={x.Case}\tdx={x.dx}\tdy={x.dy}\tdz={x.dz}\trx={x.rx}\try={x.ry}\trz={x.rz}"));
        W("nodalReactions",
            mdb.nodalReactions.Select(x =>
                $"ID={x.ID}\tCase={x.Case}\tFx={x.Fx}\tFy={x.Fy}\tFz={x.Fz}\tMx={x.Mx}\tMy={x.My}\tMz={x.Mz}"));
        W("barInternForces",
            mdb.barInternForces.Select(x =>
                $"ID_Barra={x.BarID}\tCase={x.Case}\tSubdivision={x.Subdivision}\tDistance={x.Distance}\tFxi={x.Fxi}\tFyi={x.Fyi}\tFzi={x.Fzi}\tMxi={x.Mxi}\tMyi={x.Myi}\tMzi={x.Mzi}\tFxj={x.Fxj}\tFyj={x.Fyj}\tFzj={x.Fzj}\tMxj={x.Mxj}\tMyj={x.Myj}\tMzj={x.Mzj}"));

        return sw.ToString();
    }
    public static void LoadModelFile( bool reload = true)
    {
        if (reload)
            mdb.InitThisClassProperies(); 
        parseFile();
    }


    static void parseFile()
    {
        if (!File.Exists(ruta))
            throw new FileNotFoundException("Archivo no encontrado", ruta);
        string[] lines = File.ReadAllLines(ruta);
        parseStringLines(lines);

    }

   public static void LoadStringFile(string fileDump)
    {
        if (string.IsNullOrEmpty(fileDump)) return;
        if (fileDump=="") return;
        mdb.InitThisClassProperies(); 
        string[] lines = fileDump.Split("\r\n").ToArray();
        parseStringLines(lines);

    }


    static void parseStringLines(string[] lines)
    {

        var Tabla = "";
        foreach(var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.Contains("CEF3D_File_ver=0"))
            {
                continue;
            }
            if (!line.Contains("="))
            {
                Tabla = line;
                continue;
            }
            switch (Tabla)
            {
                case "programParams":
                    programParams(line);
                    break;
                case "materials":
                    materials(line);
                    break;
                case "sections":
                    sections(line);
                    break;
                case "gridCoordsX":
                    gridCoordsX(line);
                    break;
                case "gridCoordsY":
                    gridCoordsY(line);
                    break;
                case "gridCoordsZ":
                    gridCoordsZ(line);
                    break;
                case "nodes":
                    nodes(line);
                    break;
                case "bars":
                    bars(line);
                    break;
                case "nodalRestraints":
                    nodalRestraints(line);
                    break;
                case "nodalConstraints":
                    nodalConstraints(line);
                    break;
                case "loadCases":
                    loadCases(line);
                    break;
                case "loadCombs":
                    loadCombs(line);
                    break;
                case "nodalLoads":
                    nodalLoads(line);
                    break;
                case "barDistrLoads":
                    barDistrLoads(line);
                    break;
                case "barPunctLoads":
                    barPunctLoads(line);
                    break;
                case "barThermalLoads":
                    barThermalLoads(line);
                    break;
                case "nodalDisplacements":
                    nodalDisplacements(line);
                    break;
                case "nodalReactions":
                    nodalReactions(line);
                    break;
                case "barInternForces":
                    barInternForces(line);
                    break;
            }
        }
    }
    static void programParams(string linea)
    {
        var reg = new programParam();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "Name":
                    reg.Name = value;
                    break;
                case "Value":
                    reg.Value = value;
                      break;
               }
        }
        mdb.programParams.Add(reg);
    }
    static void materials(string linea)
    {
       var reg = new material();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Name":
                    reg.Name = value;
                    break;
                case "Type":
                    reg.Type = int.Parse(value);
                    break;
                case "v":
                    reg.v = double.Parse(value);
                    break;
                case "E":
                    reg.E = double.Parse(value);
                    break;
                case "G":
                    reg.G = double.Parse(value);
                    break;
                case "M":
                    reg.M = double.Parse(value);
                    break;
                case "W":
                    reg.W = double.Parse(value);
                    break;
                case "fc":
                    reg.fc = double.Parse(value);
                    break;
                case "fy":
                    reg.fy = double.Parse(value);
                    break;
                case "fu":
                    reg.fu = double.Parse(value);
                    break;
            }
        }
        mdb.materials.Add(reg);
    }
    static void sections(string linea)
    {
        var reg = new section();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : "";
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Name":
                    reg.Name = value;
                    break;
                case "Type":
                    reg.Type = int.Parse(value);
                    break;
                case "h":
                    reg.h = double.Parse(value);
                    break;
                case "tw":
                    reg.tw = double.Parse(value);
                    break;
                case "tf":
                    reg.tf = double.Parse(value);
                    break;
                case "Ag":
                    reg.Ag = double.Parse(value);
                    break;
                case "Iyy":
                    reg.Iyy = double.Parse(value);
                    break;
                case "Izz":
                    reg.Izz = double.Parse(value);
                    break;
                case "Iyz":
                    reg.Iyz = double.Parse(value);
                    break;
                case "b":
                    reg.b = double.Parse(value);
                    break;
                case "G":
                    reg.tw = double.Parse(value);
                    break;
                case "M":
                    reg.tf = double.Parse(value);
                    break;
                case "W":
                    reg.Ag = double.Parse(value);
                    break;
                case "fc":
                    reg.Iyy = double.Parse(value);
                    break;
                case "Fy":
                    reg.Izz = double.Parse(value);
                    break;
                case "Fu":
                    reg.Iyz = double.Parse(value);
                    break;
                case "Tor":
                    reg.Tor = double.Parse(value);
                    break;
                case "Acyy":
                    reg.Acyy = double.Parse(value);
                    break;
                case "Aczz":
                    reg.Aczz = double.Parse(value);
                    break;
                case "Syy":
                    reg.Syy = double.Parse(value);
                    break;
                case "Szz":
                    reg.Szz = double.Parse(value);
                    break;
                case "Pyy":
                    reg.Pyy = double.Parse(value);
                    break;
                case "Pzz":
                    reg.Pzz = double.Parse(value);
                    break;
                case "ryy":
                    reg.ryy = double.Parse(value);
                    break;
                case "rzz":
                    reg.rzz = double.Parse(value);
                    break;
            }
        }
        mdb.sections.Add(reg);
    }
    static void gridCoordsX(string linea)
    {
         var reg = new coordinate();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Coord":
                    reg.Coord = double.Parse(value);
                    break;
                case "coordSys":
                    reg.coordSys = value;
                    break;              
            }
        }  
            mdb.gridCoordsX.Add(reg);
    }
    static void gridCoordsY(string linea)
    {
        var reg = new coordinate();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Coord":
                    reg.Coord = double.Parse(value);
                    break;
                case "coordSys":
                    reg.coordSys = value;
                    break;
            }
        }
            mdb.gridCoordsY.Add(reg);
    }
    static void gridCoordsZ(string linea)
    {
        var reg = new coordinate();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Coord":
                    reg.Coord = double.Parse(value);
                    break;
                case "coordSys":
                    reg.coordSys = value;
                    break;
            }
        }
            mdb.gridCoordsZ.Add(reg);
    }
    static void nodes(string linea)
    {
        var reg = new node();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "label":
                    reg.label = value;
                    break;
                case "X":
                    reg.X = double.Parse(value);
                    break;
                case "Y":
                    reg.Y = double.Parse(value);
                    break;
                case "Z":
                    reg.Z = double.Parse(value);
                    break;
            }
        }
            mdb.nodes.Add(reg);
    }
    static void bars(string linea)
    {
        var reg = new bar();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "label":
                    reg.label = value;
                    break;
                case "startNode":
                    reg.startNode = int.Parse(value);
                    break;
                case "endNode":
                    reg.endNode = int.Parse(value);
                    break;
                case "MaterialID":
                    reg.MaterialID = int.Parse(value);
                    break;
                case "MaterialName":
                    reg.MaterialName = value;
                    break;
                case "sectionID":
                    reg.SectionID = int.Parse(value);
                    break;
                case "sectionName":
                    reg.SectionName = value;
                    break;
                case "unused":
                    reg.unused = int.Parse(value);
                    break;
                case "Rotation":
                    reg.Rotation = double.Parse(value);
                    break;
                case "releases":
                    reg.releases = int.Parse(value);
                    break;
                case "offsetY":
                    reg.offsetY = int.Parse(value);
                    break;
                case "offsetZ":
                    reg.offsetZ = int.Parse(value);
                    break;
            }
        }
            mdb.bars.Add(reg);
    }
    static void nodalRestraints(string linea)
    {
        var reg = new nodalRestraint();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "ux":
                    reg.ux = int.Parse(value);
                    break;
                case "uy":
                    reg.uy = int.Parse(value);
                    break;
                case "uz":
                    reg.uz = int.Parse(value);
                    break;
                case "rx":
                    reg.rx = int.Parse(value);
                    break;
                case "ry":
                    reg.ry = int.Parse(value);
                    break;
                case "rz":
                    reg.rz = int.Parse(value);
                    break;
            }
        }
            mdb.nodalRestraints.Add(reg);
    }
    static void nodalConstraints(string linea)
    {
        var reg = new nodalConstraint();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "label":
                    reg.label = value;
                    break;
                case "type":
                    reg.Type = int.Parse(value);
                    break;
                case "nodesIDList":
                    reg.nodesIDList = value;
                    break;
                case "masterNodeID":
                    reg.masterNodeID = int.Parse(value);
                    break;
                 }
        }
            mdb.nodalConstraints.Add(reg);
    }
    static void loadCases(string linea)
    {
        var reg = new loadCase();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Name":
                    reg.Name = value;
                    break;
                case "Type":
                    reg.Type = int.Parse(value);
                    break;
            }
        }
            mdb.loadCases.Add(reg);
    }
    static void loadCombs(string linea)
    {
        var reg = new loadComb();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Name":
                    reg.Name = value;
                    break;
                case "Type":
                    reg.Type = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Factor":
                    reg.Factor = double.Parse(value);
                    break;
            }
        }
            mdb.loadCombs.Add(reg);
    }
    static void nodalLoads(string linea)
    {
        var reg = new nodalLoad();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "nodeID":
                    reg.NodeID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Fx":
                    reg.Fx = double.Parse(value);
                    break;
                case "Fy":
                    reg.Fy = double.Parse(value);
                    break;
                case "Fz":
                    reg.Fz = double.Parse(value);
                    break;
                case "Mx":
                    reg.Mx = double.Parse(value);
                    break;
                case "My":
                    reg.My = double.Parse(value);
                    break;
                case "Mz":
                    reg.Mz = double.Parse(value);
                    break;
            }
        }
            mdb.nodalLoads.Add(reg);
    }
    static void barDistrLoads(string linea)
    {
        var reg = new barDistrLoad();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "a":
                    reg.a = double.Parse(value);
                    break;
                case "b":
                    reg.b = double.Parse(value);
                    break;
                case "BarID":
                    reg.BarID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Fx":
                    reg.Fx = double.Parse(value);
                    break;
                case "Fy":
                    reg.Fy = double.Parse(value);
                    break;
                case "Fz":
                    reg.Fz = double.Parse(value);
                    break;
                case "Mx":
                    reg.Mx = double.Parse(value);
                    break;
                case "My":
                    reg.My = double.Parse(value);
                    break;
                case "Mz":
                    reg.Mz = double.Parse(value);
                    break;
            }
        }
            mdb.barDistrLoads.Add(reg);
    }
    static void barPunctLoads(string linea)
    {
        var reg = new barPunctLoad();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "a":
                    reg.a = int.Parse(value);
                    break;
                case "BarID":
                    reg.BarID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Fx":
                    reg.Fx = double.Parse(value);
                    break;
                case "Fy":
                    reg.Fy = double.Parse(value);
                    break;
                case "Fz":
                    reg.Fz = double.Parse(value);
                    break;
                case "Mx":
                    reg.Mx = double.Parse(value);
                    break;
                case "My":
                    reg.My = double.Parse(value);
                    break;
                case "Mz":
                    reg.Mz = double.Parse(value);
                    break;
            }
        }
            mdb.barPunctLoads.Add(reg);
    }
    static void barThermalLoads(string linea)
    {
        var reg = new barThermalLoad();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "barID":
                    reg.barID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "TemExterior":
                    reg.extTheperature = double.Parse(value);
                    break;
                case "TemInterior":
                    reg.intThemperature = double.Parse(value);
                    break;
                case "b":
                    reg.b = double.Parse(value);
                    break;
                case "h":
                    reg.h = double.Parse(value);
                    break;
            }
        }
            mdb.barThermalLoads.Add(reg);
    }
    static void nodalDisplacements(string linea)
    {
        var reg = new nodalDisplacement();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "dx":
                    reg.dx = double.Parse(value);
                    break;
                case "dy":
                    reg.dy = double.Parse(value);
                    break;
                case "dz":
                    reg.dz = double.Parse(value);
                    break;
                case "rx":
                    reg.rx = double.Parse(value);
                    break;
                case "ry":
                    reg.ry = double.Parse(value);
                    break;
                case "rz":
                    reg.rz = double.Parse(value);
                    break;
            }
        }
            mdb.nodalDisplacements.Add(reg);
    }
    static void nodalReactions(string linea)
    {
        var reg = new nodalReaction();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "ID":
                    reg.ID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Fx":
                    reg.Fx = double.Parse(value);
                    break;
                case "Fy":
                    reg.Fy = double.Parse(value);
                    break;
                case "Fz":
                    reg.Fz = double.Parse(value);
                    break;
                case "Mx":
                    reg.Mx = double.Parse(value);
                    break;
                case "My":
                    reg.My = double.Parse(value);
                    break;
                case "Mz":
                    reg.Mz = double.Parse(value);
                    break;
            }
        }
            mdb.nodalReactions.Add(reg);
    }
    static void barInternForces(string linea)
    {
        var reg = new barInternForce();
        var l = linea.Split("\t");
        foreach (var x in l)
        {
            if (string.IsNullOrWhiteSpace(x)) continue;
            var l1 = x.Split("=");
            string varName = l1[0];
            string value = l1.Length > 1 ? l1[1] : ""; 
            switch (varName)
            {
                case "barID":
                    reg.BarID = int.Parse(value);
                    break;
                case "Case":
                    reg.Case = int.Parse(value);
                    break;
                case "Subdivision":
                    reg.Subdivision = int.Parse(value);
                    break;
                case "Distance":
                    reg.Distance = double.Parse(value);
                    break;
                case "Fxi":
                    reg.Fxi = double.Parse(value);
                    break;
                case "Fyi":
                    reg.Fyi = double.Parse(value);
                    break;
                case "Fzi":
                    reg.Fzi = double.Parse(value);
                    break;
                case "Mxi":
                    reg.Mxi = double.Parse(value);
                    break;
                case "Myi":
                    reg.Myi = double.Parse(value);
                    break;
                case "Mzi":
                    reg.Mzi = double.Parse(value);
                    break;
                case "Fxj":
                    reg.Fxj = double.Parse(value);
                    break;
                case "Fyj":
                    reg.Fyj = double.Parse(value);
                    break;
                case "Fzj":
                    reg.Fzj = double.Parse(value);
                    break;
                case "Mxj":
                    reg.Mxj = double.Parse(value);
                    break;
                case "Myj":
                    reg.Myj = double.Parse(value);
                    break;
                case "Mzj":
                    reg.Mzj = double.Parse(value);
                    break;
            }
        }
            mdb.barInternForces.Add(reg);
    }
}