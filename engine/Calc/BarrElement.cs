using System.Diagnostics;
using System.IO.Compression;
using CEF.Dictionary;

namespace CEF.BarrElement;
internal static class barrElement
{

    const int elemDim = 12;
    

    private static Dictionary<int,Dictionary<int,List<barDistrLoad>>> barDistrLoadsIndx = [];
    private static Dictionary<int,Dictionary<int,List<barPunctLoad>>> barPunctoadsIndx = [];
  
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

        flagDics = true;
    }
   
    internal static void getTransMatrix(Bar barra,  double[,] trans, ModelDB m_mBD)
    {        
        int i, j, i2;       
        var mNodes = m_mBD.nodes;
        double[,] T1gTM = new double[3, 3];

        Array.Clear(trans, 0, trans.Length); 
        var nI = mNodes.FindByID(barra.startNode);
        var nJ = mNodes.FindByID(barra.endNode);   

        space3d.getLineRotMtrxLocToGlob([nI.X,nI.Y,nI.Z,nJ.X,nJ.Y,nJ.Z], T1gTM);
   
        for (i2 = 0; i2 < 4; i2++)
            for (i = 0; i < 3; i++)
                for (j = 0; j < 3; j++)
                    trans[i + 3 * i2, j + 3 * i2] = T1gTM[i, j];
    }
    
    
    internal static void getKedMatrix(Bar mBar,double[,] ke, ModelDB m_mBD)
    {
        Debug.Assert(ke.Length==144);
        
        int i, j;
        double deltax, deltay, deltaz, L;
        double[,] T = new double[12, 12];
        double[,] strans = new double[3, 3];

        var mNodes = m_mBD.nodes;
        var nI = mNodes.FindByID(mBar.startNode);
        var nJ = mNodes.FindByID(mBar.endNode);   
        deltax = nJ.X - nI.X;
        deltay = nJ.Y - nI.Y;
        deltaz = nJ.Z - nI.Z;
        L = Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        var Section =m_mBD.sections.FindByID(mBar.SectionID);
        var Material = m_mBD.materials.FindByID(Section.MaterialID);
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
        ke[11,0] = EA_L_dy;
        ke[4,0] = EA_L_dz;
        ke[10,0] = -EA_L_dz;
        ke[6,5] = EA_L_dy;
        ke[11,6] = -EA_L_dy;
        ke[6,4] = -EA_L_dz;
        ke[10,6] = EA_L_dz;
        
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
            
            //TxSxTt : From local standart to global rotated
            Array.Copy(
              MatrixMath.M_multT( 
                MatrixMath.M_mult(T,ke),T),ke, 144); //len = 12x12=144
   
        }


    }

    internal static void getLoadVector(Bar mBar, ref double[] loadVect, int currLoadCaseId, ModelDB m_mBD)
    {
        double[,] trans = new double[elemDim, elemDim];
        double[] elemLocLoadVect = new double[elemDim];
        
         double[] _a = new double[elemDim];
         double[] _b = new double[elemDim];
         double [] ha = new double[6];
         double [] hb = new double[6];

        Array.Clear( trans, 0, trans.Length);
        Array.Clear( elemLocLoadVect ,0,elemDim);
        Array.Clear( _a  ,0,elemDim);
        Array.Clear( _b  ,0,elemDim);
        var nodes = m_mBD.nodes;
        for (var i = 0; i < elemDim; i++)
            loadVect[i] = 0.0;
        var nodeI = nodes.FindByID(mBar.startNode);
        var nodeJ = nodes.FindByID(mBar.endNode);

        var deltax = nodeJ.X - nodeI.X;
        var deltay = nodeJ.Y - nodeI.Y;
        var deltaz = nodeJ.Z - nodeI.Z;
        var L = Math.Sqrt(deltax * deltax + deltay * deltay + deltaz * deltaz);
        barrElement.getTransMatrix(mBar,  trans,m_mBD);
        if (barDistrLoadsIndx.TryGetValue(mBar.ID, out var byCase) &&
    byCase.TryGetValue(currLoadCaseId, out var cargas))
{
    // 3-point Gauss rule on [-1, 1]
    double[] gaussPoints = { -Math.Sqrt(0.6), 0.0, Math.Sqrt(0.6) };
    double[] gaussWeights = { 5.0 / 9.0, 8.0 / 9.0, 5.0 / 9.0 };

    foreach (var load in cargas)
    {
        // Load limits as fractions of L from start node (a < b)
        var xa = load.l1 * L;
        var xb = load.l2 * L;

        // Load intensities at both ends of the loaded segment
        // TODO: replace with load.Fx1, load.Fx2, etc. when the rest of the project is adjusted
        double[] f1 = { load.Fx, load.Fy, load.Fz };
        double[] f2 = { load.Fx, load.Fy, load.Fz };

        if (load.Type == 0) // 0 = Global -> convert to local (Tt * F)
        {
            double[] f1Loc = new double[3];
            double[] f2Loc = new double[3];
            for (var k = 0; k < 3; k++)
                for (var j = 0; j < 3; j++)
                {
                    f1Loc[k] += trans[j, k] * f1[j];
                    f2Loc[k] += trans[j, k] * f2[j];
                }
            f1 = f1Loc;
            f2 = f2Loc;
        }

        var halfLength = (xb - xa) / 2.0;
        var midPoint = (xb + xa) / 2.0;

        Array.Clear(elemLocLoadVect, 0, elemDim);

        for (var g = 0; g < 3; g++)
        {
            var x = midPoint + halfLength * gaussPoints[g];
            var xi = x / L;
            var w = gaussWeights[g] * halfLength;

            // Linear interpolation of the load at x
            var t = (x - xa) / (xb - xa);
            var qx = f1[0] + (f2[0] - f1[0]) * t;
            var qy = f1[1] + (f2[1] - f1[1]) * t;
            var qz = f1[2] + (f2[2] - f1[2]) * t;

            // Axial shape functions
            var n1Axial = 1 - xi;
            var n2Axial = xi;

            // Hermite shape functions (transverse displacement and rotation)
            var n1 = 1 - 3 * xi * xi + 2 * xi * xi * xi;
            var n2 = L * (xi - 2 * xi * xi + xi * xi * xi);
            var n3 = 3 * xi * xi - 2 * xi * xi * xi;
            var n4 = L * (-xi * xi + xi * xi * xi);

            // Start node
            elemLocLoadVect[0] += w * qx * n1Axial;  // fx
            elemLocLoadVect[1] += w * qy * n1;       // fy
            elemLocLoadVect[2] += w * qz * n1;       // fz
            elemLocLoadVect[4] += -w * qz * n2;      // my
            elemLocLoadVect[5] += w * qy * n2;       // mz

            // End node
            elemLocLoadVect[6] += w * qx * n2Axial;  // fx
            elemLocLoadVect[7] += w * qy * n3;       // fy
            elemLocLoadVect[8] += w * qz * n3;       // fz
            elemLocLoadVect[10] += -w * qz * n4;     // my
            elemLocLoadVect[11] += w * qy * n4;      // mz
        }

        if (mBar.releases != 0)
        {
            var rel = mBar.releases;
            var gdlxel = 12;
            for (int gdl = 0; gdl < gdlxel; gdl++)
            {
                int mask = 1 << (gdlxel - gdl - 1);
                if ((rel & mask) != 0)
                    elemLocLoadVect[gdl] = 0;
            }
        }

        // To global nodal loads
        for (var k = 0; k < elemDim; k++)
            for (var j = 0; j < elemDim; j++)
                loadVect[k] += trans[k, j] * elemLocLoadVect[j]; // T * F
    }
}
       if (barPunctoadsIndx.TryGetValue(mBar.ID, out var porCaso2) &&
    porCaso2.TryGetValue(currLoadCaseId, out var loads))
{
    foreach (var load in loads)
    {
        var a = load.l1 * L;
        var b = L - a;
        var Fx = load.Fx;
        var Fy = load.Fy;
        var Fz = load.Fz;
        var Mx = load.Mx;
        var My = load.My;
        var Mz = load.Mz;
        var al2 = Math.Pow(a / L, 2);
        var bl2 = Math.Pow(b / L, 2);
        var l3 = Math.Pow(L, 3);

        if (load.Type == 0) // 0 = Global -> convert to local (Tt * F)
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
                    hb[k] += trans[j, k] * ha[j];
            Fx = hb[0];
            Fy = hb[1];
            Fz = hb[2];
            Mx = hb[3];
            My = hb[4];
            Mz = hb[5];
        }

        // Start node
        elemLocLoadVect[0] = Fx * bl2 * (3 - 2 * b / L);
        elemLocLoadVect[1] = Fy * bl2 * (3 - 2 * b / L) - Mz * 6 * a * b / l3;
        elemLocLoadVect[2] = Fz * bl2 * (3 - 2 * b / L) + My * 6 * a * b / l3;
        elemLocLoadVect[3] = Mx * b / L;
        elemLocLoadVect[4] = -Fz * a * b * b / (L * L) + My * b / L * (2 - 3 * b / L);
        elemLocLoadVect[5] = Fy * a * b * b / (L * L) - Mz * b / L * (2 - 3 * b / L);

        // End node
        elemLocLoadVect[6] = Fx * al2 * (3 - 2 * a / L);
        elemLocLoadVect[7] = Fy * al2 * (3 - 2 * a / L) + Mz * 6 * a * b / l3;
        elemLocLoadVect[8] = Fz * al2 * (3 - 2 * a / L) - My * 6 * a * b / l3;
        elemLocLoadVect[9] = Mx * a / L;
        elemLocLoadVect[10] = Fz * a * a * b / (L * L) - My * a / L * (3 * a / L - 2) * -1;
        elemLocLoadVect[11] = -Fy * a * a * b / (L * L) - Mz * a / L * (2 - 3 * a / L);

        if (mBar.releases != 0)
        {
            var rel = mBar.releases;
            var gdlxel = 12;
            for (int gdl = 0; gdl < gdlxel; gdl++)
            {
                int mask = 1 << (gdlxel - gdl - 1);
                if ((rel & mask) != 0)
                    elemLocLoadVect[gdl] = 0;
            }
        }

        // To global nodal loads
        for (var k = 0; k < elemDim; k++)
            for (var j = 0; j < elemDim; j++)
                loadVect[k] += trans[k, j] * elemLocLoadVect[j]; // T * F
    }
}
}
    
    

    internal static void getInternalPdForcesVector(Bar barra, int casoCargaActual, 
        in double[] p, 
        out double Ax1, out double Ax2, out double L, out int idxI, out int idxJ, ModelDB m_mBD)
    {
        
        const int DofsPerNode = 6;
        double[,] trans = new double[elemDim, elemDim];
        double[] elemLocLoadVect = new double[elemDim];
        double[] globalDispVect = new double[elemDim];
        double[] localDispVect = new double[elemDim];
        double[] loadVect = new double[elemDim];
        double[] localLoadVect = new double[elemDim];
        double[] nodalForces = new double[elemDim];
        double[,] elemKMtrx = new double[elemDim, elemDim];

        Array.Clear( trans,0  , trans.Length);
        Array.Clear( elemLocLoadVect, 0, elemDim);
        Array.Clear( globalDispVect, 0, elemDim);
        Array.Clear( localDispVect, 0, elemDim);
        Array.Clear( loadVect, 0, elemDim);
        Array.Clear( localLoadVect, 0, elemDim);
        Array.Clear( nodalForces, 0, elemDim);
        Array.Clear( elemKMtrx, 0, elemKMtrx.Length);
        var nodos = m_mBD.nodes;  
        var idxnodaI = nodos.FindIndexByID(barra.startNode);
        var idxnodaJ = nodos.FindIndexByID(barra.endNode);

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
                localDispVect[j] += trans[k, j] * globalDispVect[k];//glob to loc Ttxd
        }
        barrElement.getLoadVector(barra, ref loadVect, casoCargaActual,m_mBD);
        for (var j = 0; j < elemDim; j++)
        {
            localLoadVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localLoadVect[j] += trans[k, j] * loadVect[k];//glob to loc Ttxd
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

        double[,] trans = new double[elemDim, elemDim];
        double[] elemLocLoadVect = new double[elemDim];
        double[] globalDispVect = new double[elemDim];
        double[] localDispVect = new double[elemDim];
        double[] loadVect = new double[elemDim];
        double[] localLoadVect = new double[elemDim];
        double[] nodalForces = new double[elemDim];
        double[,] elemKMtrx = new double[elemDim, elemDim];

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

        barrElement.getTransMatrix(mBar, trans,m_mDB);
        barrElement.getKedMatrix(mBar, elemKMtrx,m_mDB);
        var checkI = FindIndexByID(mNodes[indxI].ID,casoCargaActual,m_mDB.nodalDisplacements);
        var checkJ = FindIndexByID(mNodes[indxJ].ID,casoCargaActual,m_mDB.nodalDisplacements);

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