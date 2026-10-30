using System.IO.Compression;
using CEF.Dictionary;

namespace CEF.BarrElement;
internal class barrElement
{
    internal Bar mBar { get; set; }
    private static Dictionary<int,Dictionary<int,List<barDistrLoad>>> barDistrLoadsIndx = [];
    private static Dictionary<int,Dictionary<int,List<barPunctLoad>>> barPunctoadsIndx = [];
    private static Dictionary<int,Dictionary<int,List<nodalLoad>>> nodalLoadsIndx = [];
    private static bool flagDics = false;
    internal static void setUpLoadIndexes(ModelDB model)
    {
        if (flagDics) return;
        foreach (var c in model.barDistrLoads)
        {
            if (!barDistrLoadsIndx.TryGetValue(c.BarID, out var byLoadCase))
                barDistrLoadsIndx[c.BarID] = byLoadCase = new Dictionary<int, List<barDistrLoad>>();
            if (!byLoadCase.TryGetValue(c.Case, out var list))
                byLoadCase[c.Case] = list = new List<barDistrLoad>();
            list.Add(c);
        }
        foreach (var c in model.barPunctLoads)
        {
            if (!barPunctoadsIndx.TryGetValue(c.BarID, out var byLoadCase))
                barPunctoadsIndx[c.BarID] = byLoadCase = new Dictionary<int, List<barPunctLoad>>();
            if (!byLoadCase.TryGetValue(c.Case, out var list))
                byLoadCase[c.Case] = list = new List<barPunctLoad>();
            list.Add(c);
        }
        foreach (var c in model.nodalLoads)
        {
            if (!nodalLoadsIndx.TryGetValue(c.NodeID, out var byLoadCase))
                nodalLoadsIndx[c.NodeID] = byLoadCase = new Dictionary<int, List<nodalLoad>>();
            if (!byLoadCase.TryGetValue(c.Case, out var list))
                byLoadCase[c.Case] = list = new List<nodalLoad>();
            list.Add(c);
        }
        flagDics = true;
    }
   static double[,] T1gTM = new double[3, 3];
    internal static void getTransMatrix(Bar barra,  double[,] trans, ModelDB m_mBD)
    {        
        int i, j, i2, indxI, indxJ;       
        var mNodes = m_mBD.nodes;
        Array.Clear(T1gTM, 0, T1gTM.Length);
        Array.Clear(trans, 0, trans.Length); 
        indxI = mNodes.FindIndexByID(barra.startNode);
        indxJ = mNodes.FindIndexByID(barra.endNode);
        if ((indxI == -1) || (indxJ == -1))
            throw new Exception($"Error en la asignacion de los indices de los nudos: \n barra {barra.ID} - \n NodI {barra.startNode} y NodJ{barra.endNode} ");
        var nI = mNodes[indxI];
        var nJ = mNodes[indxJ];

        space3d.getLineRotMtrx([nI.X,nI.Y,nI.Z,nJ.X,nJ.Y,nJ.Z], T1gTM);
   
        for (i2 = 0; i2 < 4; i2++)
            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                    trans[i + 3 * i2, j + 3 * i2] = T1gTM[i, j];
    }
    static double[,] T = new double[12, 12];
    static double[,] strans = new double[3, 3];
    internal static void getKedMatrix(Bar mBar,double[,] ke, ModelDB m_mBD)
    {
        
        int i, j, indI, indJ;
        double deltax, deltay, deltaz, L;
        var mNodes = m_mBD.nodes;
        indI = mNodes.FindIndexByID(mBar.startNode);
        indJ = mNodes.FindIndexByID(mBar.endNode);
        if ((indI == -1) || (indJ == -1))
            throw new Exception($"Error en la asignacion de los indices de los nudos: \n barra {mBar.ID} - \n NodI {mBar.startNode} y NodJ{mBar.endNode} ");
        var nI = mNodes[indI];
        var nJ = mNodes[indJ];
        deltax = nJ.X - nI.X;
        deltay = nJ.Y - nI.Y;
        deltaz = nJ.Z - nI.Z;
        L = Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        var SectionIdx = m_mBD.sections.FindIndexByID(mBar.SectionID);
        var Section = m_mBD.sections[SectionIdx];
        var materialIdx = m_mBD.materials.FindIndexByID(Section.MaterialID);
        var Material = m_mBD.materials[materialIdx];
        double dy ;
        double dz ;
        dy = mBar.offsetY switch
        {
            1 => -Section.b / 2,
            2 => Section.b / 2,
            _ => 0.0,
        };
        dz = mBar.offsetZ switch
        {
            1 => -Section.h / 2,
            2 => Section.h / 2,
            _ => 0.0,
        };
        double Iyy_t = Section.Iyy + Section.Ag * dz * dz;
        double Izz_t = Section.Izz + Section.Ag * dy * dy;
        double EA_L = Material.E * Section.Ag / L;

        double EIzX12_L3 = 12 * Material.E * Izz_t / (L * L * L);
        double EIyX12_L3 = 12 * Material.E * Iyy_t / (L * L * L);

        double EIyX4_L = 4 * Material.E * Iyy_t/L;
        double EIzX4_L = 4 * Material.E * Izz_t/L;

        double EIzX6_L2 = 6 * Material.E * Izz_t / (L * L);
        double EIyX6_L2 = 6 * Material.E * Iyy_t / (L * L);

        double GJ_L = Material.G * Section.Tor / L;

        Array.Clear(ke, 0, ke.Length);

        ke[0, 0] = EA_L;
        ke[6, 0] = -EA_L;
        ke[6, 6] = EA_L;
        
        ke[1, 1] = EIzX12_L3;
        ke[5, 1] = EIzX6_L2;
        ke[5, 5] = EIzX4_L;
        ke[7, 1] = -EIzX12_L3;
        ke[7, 5] = -EIzX6_L2;
        ke[7, 7] = EIzX12_L3;
        ke[11, 1] = EIzX6_L2;
        ke[11, 5] = EIzX4_L / 2;
        ke[11, 7] = -EIzX6_L2;
        ke[11, 11] = EIzX4_L;

        ke[2, 2] = EIyX12_L3;
        ke[4, 2] = EIyX6_L2;
        ke[4, 4] = EIyX4_L;
        ke[8, 2] = -EIyX12_L3;
        ke[8, 4] = -EIyX6_L2;
        ke[8, 8] = EIyX12_L3;
        ke[10, 2] = EIyX6_L2;
        ke[10, 4] = EIyX4_L / 2;
        ke[10, 8] = -EIyX6_L2;
        ke[10, 10] = EIyX4_L;

        ke[3, 3] = GJ_L;
        ke[9, 3] = -GJ_L;
        ke[9, 9] = GJ_L;

        double EA_L_dy = EA_L * dy;
        double EA_L_dz = EA_L * dz;

        ke[5, 0] = -EA_L_dy;
        ke[ 11,0] = EA_L_dy;
        ke[4,0] = EA_L_dz;
        ke[ 10,0] = -EA_L_dz;
        ke[ 6,5] = EA_L_dy;
        ke[11,6] = -EA_L_dy;
        ke[6,4] = -EA_L_dz;
        ke[10,6] = EA_L_dz;
        /*
        ----|--------------|-----------------|-------|---------
            0  | Fx (i)       | 0b100000000000  | 0x800 | 2048
            1  | Fy (i)       | 0b010000000000  | 0x400 | 1024
            2  | Fz (i)       | 0b001000000000  | 0x200 | 512
            3  | Mx (i)       | 0b000100000000  | 0x100 | 256
            4  | My (i)       | 0b000010000000  | 0x080 | 128
            5  | Mz (i)       | 0b000001000000  | 0x040 | 64
            6  | Fx (j)       | 0b000000100000  | 0x020 | 32
            7  | Fy (j)       | 0b000000010000  | 0x010 | 16
            8  | Fz (j)       | 0b000000001000  | 0x008 | 8
            9  | Mx (j)       | 0b000000000100  | 0x004 | 4
            10 | My (j)       | 0b000000000010  | 0x002 | 2
            11 | Mz (j)       | 0b000000000001  | 0x001 | 1
        Ej: rel = 0b000001000010 (decimal 66) → frees Mz(i) y My(j)
        */
        if (mBar.releases != 0)
        {
            var rel = mBar.releases;
            var gdlxel = 12;
            for (int gdl = 0; gdl < gdlxel; gdl++)
            {
                int mask = 1 << (gdlxel - gdl - 1);
                if ((rel & mask) != 0)
                {
                    for (j = 0; j < 12; j++)
                    {
                        ke[gdl, j] = 0;
                        ke[j, gdl] = 0;
                    }
                    ke[gdl, gdl] = 0.0;
                }
            }
        }
        for (i = 0; i < 12; i++)
            for (j = i + 1; j < 12; j++)
                ke[i, j] = ke[j, i];

        if (mBar.Rotation != 0.0)
        {
            var ang = mBar.Rotation * Math.PI / 180;
            strans[0, 0] = 1.0;
            strans[0, 1] = 0.0;
            strans[0, 2] = 0.0;
            strans[1, 0] = 0.0;
            strans[1, 1] = Math.Cos(ang);
            strans[1, 2] = Math.Sin(ang);
            strans[2, 0] = 0.0;
            strans[2, 1] = -Math.Sin(ang);
            strans[2, 2] = Math.Cos(ang);
            Array.Clear(T, 0, T.Length);
            for (int b = 0; b < 4; b++)
                for (i = 0; i < 3; i++)
                    for (j = 0; j < 3; j++)
                        T[3 * b + i, 3 * b + j] = strans[i, j];
            ke = MatrixMath.MT_mult(T, 
                MatrixMath.M_mult(ke, T));   
        }
        }
    static double[] _a = new double[elemDim];
    static double[] _b = new double[elemDim];
    static double [] ha = new double[6];
    static double [] hb = new double[6];
    internal static void getLoadVector(Bar mBar, ref double[] loadVect, int currLoadCaseId, ModelDB m_mBD)
    {
        
        const int elemDim = 12;
        Array.Clear( trans, 0, trans.Length);
        Array.Clear( elemLocLoadVect ,0,elemDim);
        Array.Clear( _a  ,0,elemDim);
        Array.Clear( _b  ,0,elemDim);
        var nodes = m_mBD.nodes;
        for (var i = 0; i < elemDim; i++)
            loadVect[i] = 0.0;
        var nI = nodes.FindIndexByID(mBar.startNode);
        var nJ = nodes.FindIndexByID(mBar.endNode);
        if (nI == -1 || nJ == -1)
            throw new Exception($" Error ConformarVectorCarga: en la asignacion del indice de la carga lineal \n barra {mBar.ID}");
        var nodeI = nodes[nI];
        var nodej = nodes[nJ];
        var deltax = nodej.X - nodeI.X;
        var deltay = nodej.Y - nodeI.Y;
        var deltaz = nodej.Z - nodeI.Z;
        var L = Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        barrElement.getTransMatrix(mBar,  trans,m_mBD);
        if (barDistrLoadsIndx.TryGetValue(mBar.ID, out var byCase) &&
    byCase.TryGetValue(currLoadCaseId, out var cargas))
        {
            foreach (var load in cargas)
            {
                var a = load.a * L;
                var b = load.b * L;
                var c = b / 2;
                a += c;
                b = L - a;
                var Fx = load.Fx;
                var Fy = load.Fy;
                var Fz = load.Fz;
                var al2 = Math.Pow(a / L, 2);
                var bl2 = Math.Pow(b / L, 2);
                var cl2 = Math.Pow(c / L, 2);
                if (load.Type == 0)
                {
                    Array.Clear(ha, 0, 6);
                    Array.Clear(hb, 0, 6);
                    ha[0] = Fx;
                    ha[1] = Fy;
                    ha[2] = Fz;
                    for (var k = 0; k < 6; k++)
                        for (var j = 0; j < 6; j++)
                            hb[k] += trans[k, j] * ha[j];
                    Fx = hb[0];
                    Fy = hb[1];
                    Fz = hb[2];
                }
                elemLocLoadVect[0] = Fx * (2 * c * (1 - 3 * al2 - cl2 + (2 * a / L) * (al2 + cl2)));//fx
                elemLocLoadVect[1] = Fy * (2 * c * (1 - 3 * al2 - cl2 + (2 * a / L) * (al2 + cl2)));//fy
                elemLocLoadVect[2] = Fz * (2 * c * (1 - 3 * al2 - cl2 + (2 * a / L) * (al2 + cl2)));//fz
                elemLocLoadVect[3] = 0.0;//mx
                elemLocLoadVect[4] = -Fz * (2 * c * (a * bl2 - cl2 * (3 * b - L) / 3));//my
                elemLocLoadVect[5] = Fy * (2 * c * (a * bl2 - cl2 * (3 * b - L) / 3));//mz

                elemLocLoadVect[6] = Fx * (2 * c * (3 * al2 + cl2 - (2 * a / L) * (al2 + cl2)));//fx
                elemLocLoadVect[7] = Fy * (2 * c * (3 * al2 + cl2 - (2 * a / L) * (al2 + cl2)));//fy
                elemLocLoadVect[8] = Fz * (2 * c * (3 * al2 + cl2 - (2 * a / L) * (al2 + cl2)));//fz
                elemLocLoadVect[9] = 0.0;//mx
                elemLocLoadVect[10] = Fz * (2 * c * (b * al2 - cl2 * (3 * a - L) / 3));//my
                elemLocLoadVect[11] = -Fy * (2 * c * (b * al2 - cl2 * (3 * a - L) / 3));//mz
                if (mBar.releases != 0)
                {
                    var rel = mBar.releases;
                    var gdlxel = 12;
                    for (int gdl = 0; gdl < gdlxel; gdl++)
                    {
                        int mask = 1 << (gdlxel - gdl - 1);
                        if ((rel & mask) != 0)
                        {
                            elemLocLoadVect[gdl] = 0;
                        }
                    }
                }
                for (var k = 0; k < elemDim; k++)
                    for (var j = 0; j < elemDim; j++)
                        loadVect[k] += trans[k, j] * elemLocLoadVect[j];
            } }
        if (barPunctoadsIndx .TryGetValue(mBar.ID, out var porCaso2) &&
    porCaso2.TryGetValue(currLoadCaseId, out var loads))
        {
            foreach (var carga in loads)
            {
        var a = carga.a * L;
            var b = L - a;
            var Fx = carga.Fx;
            var Fy = carga.Fy;
            var Fz = carga.Fz;
            var Mx = carga.Mx;
            var My = carga.My;
            var Mz = carga.Mz;
            var al2 = Math.Pow(a / L, 2);
            var bl2 = Math.Pow(b / L, 2);
            var l3 = Math.Pow(L, 3);
            if (carga.Type == 0)
            {
                Array.Clear(ha, 0, 6);
                Array.Clear(hb, 0, 6);
                ha[0] = Fx;
                ha[1] = Fy;
                ha[2] = Fz;
                ha[3] = Mx;
                ha[4] = My;
                ha[5] = Mz;
                for (var k = 0; k < 6; k++)
                    for (var j = 0; j < 6; j++)
                        hb[k] += trans[k, j] * ha[j];
                Fx = ha[0];
                Fy = ha[1];
                Fz = ha[2];
                Mx = ha[3];
                My = ha[4];
                Mz = ha[5];
            }
            elemLocLoadVect[0] = Fx * (bl2) * (3 - 2 * b / L);
            elemLocLoadVect[1] = Fy * (bl2) * (3 - 2 * b / L) - Mz * 6 * a * b / (l3);
            elemLocLoadVect[2] = Fz * (bl2) * (3 - 2 * b / L) + My * 6 * a * b / (l3);
            elemLocLoadVect[3] = Mx * b / L;
            elemLocLoadVect[4] = -Fz * a * b * b / (L * L) + My * b / L * (2 - 3 * b / L);
            elemLocLoadVect[5] = Fy * a * b * b / (L * L) - Mz * b / L * (2 - 3 * b / L);
            elemLocLoadVect[6] = Fx * (al2) * (3 - 2 * a / L);
            elemLocLoadVect[7] = Fy * (al2) * (3 - 2 * a / L) + Mz * 6 * a * b / (l3);
            elemLocLoadVect[8] = Fz * (al2) * (3 - 2 * a / L) - My * 6 * a * b / (l3);
            elemLocLoadVect[9] = Mx * a / L;
            elemLocLoadVect[10] = Fz * a * a * b / (L * L) + My * a / L * (2 - 3 * a / L);
            elemLocLoadVect[11] = -Fy * a * a * b / (L * L) - Mz * a / L * (2 - 3 * a / L);
            if (mBar.releases != 0)
            {
                var rel = mBar.releases;
                var gdlxel = 12;
                for (int gdl = 0; gdl < gdlxel; gdl++)
                {
                    int mask = 1 << (gdlxel - gdl - 1);
                    if ((rel & mask) != 0)
                    {
                        elemLocLoadVect[gdl] = 0;
                    }
                }
            }
            for (var k = 0; k < elemDim; k++)
                for (var j = 0; j < elemDim; j++)
                    loadVect[k] += trans[k, j] * elemLocLoadVect[j];
        }
    }
    }
    const int elemDim = 12;
    static double[,] trans = new double[elemDim, elemDim];
    static double[] elemLocLoadVect = new double[elemDim];
    static double[] globalDispVect = new double[elemDim];
    static double[] localDispVect = new double[elemDim];
    static double[] loadVect = new double[elemDim];
    static double[] localLoadVect = new double[elemDim];
    static double[] nodalForces = new double[elemDim];
    static double[,] elemKMtrx = new double[elemDim, elemDim];
    internal static void getInternalPdForcesVector(Bar barra, int casoCargaActual, 
        in double[] p, 
        out double Ax1, out double Ax2, out double L, out int idxI, out int idxJ, ModelDB m_mBD)
    {
        
        const int DofsPerNode = 6;
        Array.Clear( trans,0  , trans.Length);
        Array.Clear( elemLocLoadVect, 0, elemDim);
        Array.Clear( globalDispVect, 0, elemDim);
        Array.Clear( localDispVect, 0, elemDim);
        Array.Clear( loadVect, 0, elemDim);
        Array.Clear( localLoadVect, 0, elemDim);
        Array.Clear( nodalForces, 0, elemDim);
        Array.Clear( elemKMtrx, 0, elemKMtrx.Length);
        var nodos = m_mBD.nodes;
        var dezp = m_mBD.nodalDisplacements;
        var reaccBaras = m_mBD.barInternForces;
        var idxnodaI = nodos.FindIndexByID(barra.startNode);
        var idxnodaJ = nodos.FindIndexByID(barra.endNode);
        if (idxnodaI == -1 || idxnodaJ == -1)
            throw new Exception($" Error ConformarVectorCarga: en la asignacion del indice de la carga lineal \n barra {barra.ID}");
        idxI = idxnodaI* DofsPerNode;
        idxJ = idxnodaJ* DofsPerNode;
        var nodeI = nodos[idxnodaI];
        var nodej = nodos[idxnodaJ];
        var deltax = nodej.X - nodeI.X;
        var deltay = nodej.Y - nodeI.Y;
        var deltaz = nodej.Z - nodeI.Z;
         L = Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        barrElement.getTransMatrix(barra,  trans,m_mBD);
        barrElement.getKedMatrix(barra,  elemKMtrx,m_mBD);
        globalDispVect[0] = p[DofsPerNode * idxnodaI + 0];
        globalDispVect[1] = p[DofsPerNode * idxnodaI + 1];
        globalDispVect[2] = p[DofsPerNode * idxnodaI + 2];
        globalDispVect[3] = p[DofsPerNode * idxnodaI + 3];
        globalDispVect[4] = p[DofsPerNode * idxnodaI + 4];
        globalDispVect[5] = p[DofsPerNode * idxnodaI + 5];
        globalDispVect[6] = p[DofsPerNode * idxnodaJ + 0];
        globalDispVect[7] = p[DofsPerNode * idxnodaJ + 1];
        globalDispVect[8] = p[DofsPerNode * idxnodaJ + 2];
        globalDispVect[9] = p[DofsPerNode * idxnodaJ + 3];
        globalDispVect[10] = p[DofsPerNode * idxnodaJ + 4];
        globalDispVect[11] = p[DofsPerNode * idxnodaJ + 5];
        for (var j = 0; j < elemDim; j++)
        {
            localDispVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localDispVect[j] += trans[k, j] * globalDispVect[k];
        }
        barrElement.getLoadVector(barra, ref loadVect, casoCargaActual,m_mBD);
        for (var j = 0; j < elemDim; j++)
        {
            localLoadVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localLoadVect[j] += trans[k, j] * loadVect[k];
        }
        for (int j = 0; j < elemDim; j++)
        {
            nodalForces[j] = 0.0;
            for (int k = 0; k < elemDim; k++)
            {
                nodalForces[j] += elemKMtrx[j, k] * localDispVect[k];
            }
        }
        for (int j = 0; j < elemDim; j++)
        {
            nodalForces[j] -= localLoadVect[j];
        }
        Ax1 = nodalForces[0]; Ax2 =  nodalForces[6];
    }
    internal static void getInternalForcesVector(Bar mBar, int casoCargaActual, ModelDB m_mDB)
    {        
        Array.Clear(trans, 0, trans.Length);
        Array.Clear(elemLocLoadVect, 0, elemDim);
        Array.Clear(globalDispVect, 0, elemDim);
        Array.Clear(localDispVect, 0, elemDim);
        Array.Clear(loadVect, 0, elemDim);
        Array.Clear(localLoadVect, 0, elemDim);
        Array.Clear(nodalForces, 0, elemDim);
        Array.Clear(elemKMtrx, 0, elemKMtrx.Length);
        var mNodes = m_mDB.nodes;
        var dezp = m_mDB.nodalDisplacements;
        var biForces = m_mDB.barInternForces;
        var indxI = mNodes.FindIndexByID(mBar.startNode);
        var indxJ = mNodes.FindIndexByID(mBar.endNode);
        if (indxI == -1 || indxJ == -1)
            throw new Exception($" Error ConformarVectorCarga: en la asignacion del indice de la carga lineal \n barra {mBar.ID}");

        barrElement.getTransMatrix(mBar, trans,m_mDB);
        barrElement.getKedMatrix(mBar, elemKMtrx,m_mDB);
        var checkI = FindIndexByID(mNodes[indxI].ID,casoCargaActual,m_mDB.nodalDisplacements);
        var checkJ = FindIndexByID(mNodes[indxJ].ID,casoCargaActual,m_mDB.nodalDisplacements);

        if (checkI == -1 || checkJ == -1)
            throw new Exception($"ID nudos no tiene Dezplazamiento ERROR en creación de Base de datos\n: Elemento {mBar.ID}: idxI={indxI} idxJ={indxJ}");
        if (dezp[checkI].Case != casoCargaActual || dezp[checkJ].Case  != casoCargaActual)
            throw new Exception($"Falla en casoCargaActual en Base de datos\n: Elemento {mBar.ID}: idxI={indxI} idxJ={indxJ} casoBuscado: {casoCargaActual} casoEncontrado: {dezp[checkI].Case}/{dezp[checkJ].Case}");
        globalDispVect[0] =  dezp[checkI].dx;
        globalDispVect[1] =  dezp[checkI].dy;
        globalDispVect[2] =  dezp[checkI].dz;
        globalDispVect[3] =  dezp[checkI].rx;
        globalDispVect[4] =  dezp[checkI].ry;
        globalDispVect[5] =  dezp[checkI].rz;
        globalDispVect[6] =  dezp[checkJ].dx;
        globalDispVect[7] =  dezp[checkJ].dy;
        globalDispVect[8] =  dezp[checkJ].dz;
        globalDispVect[9] =  dezp[checkJ].rx;
        globalDispVect[10] = dezp[checkJ].ry;
        globalDispVect[11] = dezp[checkJ].rz;
        for (var j = 0; j < elemDim; j++)
        {
            localDispVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localDispVect[j] += trans[k, j] * globalDispVect[k];
        }
        barrElement.getLoadVector(mBar, ref loadVect, casoCargaActual,m_mDB);
        for (var j = 0; j < elemDim; j++)
        {
            localLoadVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localLoadVect[j] += trans[k, j] * loadVect[k];
        }
        for (int j = 0; j < elemDim; j++)
        {
            nodalForces[j] = 0.0;
            for (int k = 0; k < elemDim; k++)
            {
                nodalForces[j] += elemKMtrx[j, k] * localDispVect[k];
            }
        }
        for (int j = 0; j < elemDim; j++)
        {
            nodalForces[j] -= localLoadVect[j];
        }
        var fuerzaBarra = new barInternForce();
        fuerzaBarra.ID = m_mDB.barInternForces.NextID();
        fuerzaBarra.BarID = mBar.ID;
        fuerzaBarra.Case = casoCargaActual;
        fuerzaBarra.Distance = dezp[indxI].Case;//****
       
        fuerzaBarra.Fxi =nodalForces[0];
        fuerzaBarra.Fyi =nodalForces[1];
        fuerzaBarra.Fzi =nodalForces[2];
        fuerzaBarra.Mxi =nodalForces[3];
        fuerzaBarra.Myi =nodalForces[4];
        fuerzaBarra.Mzi =nodalForces[5];
        fuerzaBarra.Fxj =nodalForces[6];
        fuerzaBarra.Fyj =nodalForces[7];
        fuerzaBarra.Fzj =nodalForces[8];
        fuerzaBarra.Mxj =nodalForces[9];
        fuerzaBarra.Myj =nodalForces[10];
        fuerzaBarra.Mzj =nodalForces[11];
        biForces.Add(fuerzaBarra);
    }


static Dictionary<(int,int),int> nodeCaseIndex=[];

static int FindIndexByID(int nodeId, int Case, DtaTable<nodalDisplacement> disp )
{        
    var d = disp;
    var count = d.Count;
   if(count>nodeCaseIndex.Count)
   {
        nodeCaseIndex = [];
    for(int j=0;j<count;j++){
        var r = d[j];
        nodeCaseIndex.Add((r.NodeID,r.Case),j);
    }
   }

   if(nodeCaseIndex.TryGetValue((nodeId, Case), out int  ind)) return ind;    
           
    return -1;
}


}