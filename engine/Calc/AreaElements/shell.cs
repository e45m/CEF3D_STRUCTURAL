using System.IO.Compression;

using CEF.Dictionary;

namespace CEF.AreaElement;
//Mindlin-Reissner-Q4-MITC4
internal static class Shell
{

    const int elemDim = 24;      
        
internal static void getStiffnesMtrx_old(Area area, double[,] ke, ModelDB m_mBD)
{
    var mNodes = m_mBD.nodes;

    var nI = mNodes.FindByID(area.Node1);
    var nJ = mNodes.FindByID(area.Node2);
    var nK = mNodes.FindByID(area.Node3);
    var nL = mNodes.FindByID(area.Node4); 

    
    var Section = m_mBD.areaSections.FindByID(area.SectionID);
    var Material = m_mBD.materials.FindByID(Section.MaterialID);

    // Dimensiones aproximadas del rectángulo
    double a = (space3d.calcDist(nI, nJ) + space3d.calcDist(nK, nL)) / 2.0;
    double b = (space3d.calcDist(nI, nL) + space3d.calcDist(nJ, nK)) / 2.0;

    double E = Material.E;
    double nu = Material.v;
    double G = Material.G;
    double t = Section.Thickness;

    double c = E * t / (1.0 - nu * nu);
    double Db = E * t * t * t / (12.0 * (1.0 - nu * nu));
    double ks = (5.0 / 6.0) * G * t;

    double nu1 = (1.0 - nu) / 2.0;

    // GDL locales -> posiciones en ke
    int[] x =
    [
         0,  1,  2,  3,  4,  5,
         6,  7,  8,  9, 10, 11,
        12, 13, 14, 15, 16, 17,
        18, 19, 20, 21, 22, 23
    ];

    // Ensamble simétrico
    void K(int i, int j, double val)
    {
        ke[x[i], x[j]] += val;

        if (i != j)
            ke[x[j], x[i]] += val;
    }

    // Índices locales
    int[] ux = { 0, 6, 12, 18 };
    int[] uy = { 1, 7, 13, 19 };
    int[] w  = { 2, 8, 14, 20 };
    int[] rx = { 3, 9, 15, 21 };
    int[] ry = { 4, 10, 16, 22 };
    int[] rz = { 5, 11, 17, 23 };

    double[] xi  = { -1, 1, 1, -1 };
    double[] eta = { -1, -1, 1, 1 };

// 1. MEMBRANA
// =================================================

// Bloques U-U y V-V (simétricos)
for (int i = 0; i < 4; i++)
{
    for (int j = i; j < 4; j++)
    {
        double A = xi[i] * xi[j] *
                   (1.0 + eta[i] * eta[j] / 3.0);

        double B = eta[i] * eta[j] *
                   (1.0 + xi[i] * xi[j] / 3.0);

        double Kuu = c * (
            b / (4.0 * a) * A +
            nu1 * a / (4.0 * b) * B);

        double Kvv = c * (
            a / (4.0 * b) * B +
            nu1 * b / (4.0 * a) * A);

        K(ux[i], ux[j], Kuu);
        K(uy[i], uy[j], Kvv);
    }
}

// Bloque U-V completo (4x4)
for (int i = 0; i < 4; i++)
{
    for (int j = 0; j < 4; j++)
    {
        double Kuv = c / 4.0 * (
            nu * xi[i] * eta[j] +
            nu1 * xi[j] * eta[i]);

        K(ux[i], uy[j], Kuv);
    }
}



    // =================================================
    // 2. FLEXIÓN
    // =================================================

    for (int i = 0; i < 4; i++)
    {
        for (int j = i; j < 4; j++)
        {
            double A = xi[i] * xi[j] *
                       (1.0 + eta[i] * eta[j] / 3.0);

            double B = eta[i] * eta[j] *
                       (1.0 + xi[i] * xi[j] / 3.0);

            double Kxx = Db * (
                a / (4.0 * b) * B +
                nu1 * b / (4.0 * a) * A);

            double Kyy = Db * (
                b / (4.0 * a) * A +
                nu1 * a / (4.0 * b) * B);

            double Kxy = -Db / 4.0 * (
                nu * xi[j] * eta[i] +
                nu1 * xi[i] * eta[j]);

            K(rx[i], rx[j], Kxx);
            K(ry[i], ry[j], Kyy);
            K(rx[i], ry[j], Kxy);
        }
    }

    // =================================================
    // 3. CORTANTE MITC4
    // =================================================

    // Orden: [w1, rx1, ry1, w2, rx2, ry2,
    //         w3, rx3, ry3, w4, rx4, ry4]

    double[,] alpha = new double[4, 12];

    // Alpha A
    alpha[0, 6]  =  1.0 / a;  // w3
    alpha[0, 9]  = -1.0 / a;  // w4
    alpha[0, 8]  =  0.5;      // ry3
    alpha[0, 11] =  0.5;      // ry4

    // Alpha C
    alpha[1, 0] = -1.0 / a;   // w1
    alpha[1, 3] =  1.0 / a;   // w2
    alpha[1, 2] =  0.5;       // ry1
    alpha[1, 5] =  0.5;       // ry2

    // Delta D
    alpha[2, 3] = -1.0 / b;   // w2
    alpha[2, 6] =  1.0 / b;   // w3
    alpha[2, 4] = -0.5;       // rx2
    alpha[2, 7] = -0.5;       // rx3

    // Delta B
    alpha[3, 0]  = -1.0 / b;  // w1
    alpha[3, 9]  =  1.0 / b;  // w4
    alpha[3, 1]  = -0.5;      // rx1
    alpha[3, 10] = -0.5;      // rx4

    int[] dof =
    {
         2,  3,  4,
         8,  9, 10,
        14, 15, 16,
        20, 21, 22
    };

    double factor = ks * a * b /6.0;

    for (int p = 0; p < 12; p++)
    {
        for (int q = p; q < 12; q++)
        {
            double sum = 0.0;

            for (int m = 0; m < 4; m++)
            {
                for (int n = 0; n < 4; n++)
                {
                    double weight = (m == n) ? 2.0 : 0.0;

                    if ((m == 0 && n == 1) ||
                        (m == 1 && n == 0) ||
                        (m == 2 && n == 3) ||
                        (m == 3 && n == 2))
                    {
                        weight += 1.0;
                    }

                    sum += weight * alpha[m, p] * alpha[n, q];
                }
            }

            K(dof[p], dof[q], factor * sum);
          
        }
    }

    // =================================================
    // 4. PERFORACIÓN
    // =================================================

    double alphaDrill = 1e-3;

    for (int i = 0; i < 4; i++)
    {
        K(rz[i], rz[i], alphaDrill * G * t * a * b);
    }
}



    internal static void getLoadVector(Area area, ref double[] loadVect, int currLoadCaseId, ModelDB m_mBD)
    {
        //Load vector is always zeros for this element. 
        // double[] elemLocLoadVect = new double[elemDim];
        //  double[] loadVect = new double[elemDim];
        //  double[] localLoadVect = new double[elemDim];
        return;
  
    }
    


// ASSUMPTION (only line to change): name of the class that holds getStiffnesMtrx

    // Local stiffness matrix (24 x 24) of a rectangular area element.
    // The DOFs of each node are consecutive: [u v w rx ry rz] of Node1,
    // then Node2, Node3 and Node4 (no reordering is done here).
    // Nodes must be counter-clockwise: local x goes Node1 -> Node2 and
    // local y goes Node1 -> Node4.
    // ke is accumulated with +=, so it must be zero on entry.
    internal static void getStiffnesMtrx(Area area, double[,] ke, ModelDB m_mBD)
    {
        var mNodes = m_mBD.nodes;

        var nI = mNodes.FindByID(area.Node1);
        var nJ = mNodes.FindByID(area.Node2);
        var nK = mNodes.FindByID(area.Node3);
        var nL = mNodes.FindByID(area.Node4);

        var Section = m_mBD.areaSections.FindByID(area.SectionID);
        var Material = m_mBD.materials.FindByID(Section.MaterialID);

        // Element sides: mean of the two opposite sides (exact only for a rectangle)
        double a = (space3d.calcDist(nI, nJ) + space3d.calcDist(nK, nL)) / 2.0;
        double b = (space3d.calcDist(nI, nL) + space3d.calcDist(nJ, nK)) / 2.0;

        // G is not read from the material: it is computed from E and v inside the element
        Q4PlateStiffness.addStiffness(a, b, Material.E, Material.v, Section.Thickness, ke);
    }

   
   
    internal static void getInternalForcesVector(Area area, int casoCargaActual, ModelDB m_mDB)
    {             
        
         double[,] trans = new double[elemDim, elemDim];
         double[] globalDispVect = new double[elemDim];
         double[] localDispVect = new double[elemDim];
         double[] nodalForces = new double[elemDim];
         double[,] elemKMtrx = new double[elemDim, elemDim];             

        var mNodes = m_mDB.nodes;
        var dezp = m_mDB.nodalDisplacements;
        var aForces = m_mDB.areaInternalForces;

        var indxI = mNodes.FindIndexByID(area.Node1);
        var indxJ = mNodes.FindIndexByID(area.Node2);
        var indxK = mNodes.FindIndexByID(area.Node3);
        var indxL = mNodes.FindIndexByID(area.Node4);

        areaElement.getTransMatrix(area, trans,m_mDB);
        Shell.getStiffnesMtrx(area, elemKMtrx,m_mDB);

        var checkI = FindIndexByID(mNodes[indxI].ID,casoCargaActual,m_mDB.nodalDisplacements);
        var checkJ = FindIndexByID(mNodes[indxJ].ID,casoCargaActual,m_mDB.nodalDisplacements);
        var checkK = FindIndexByID(mNodes[indxK].ID,casoCargaActual,m_mDB.nodalDisplacements);
        var checkL = FindIndexByID(mNodes[indxL].ID,casoCargaActual,m_mDB.nodalDisplacements);

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

        globalDispVect[12] =  dezp[checkK].dx;
        globalDispVect[13] =  dezp[checkK].dy;
        globalDispVect[14] =  dezp[checkK].dz;
        globalDispVect[15] =  dezp[checkK].rx;
        globalDispVect[16] =  dezp[checkK].ry;
        globalDispVect[17] =  dezp[checkK].rz;

        globalDispVect[18] =  dezp[checkL].dx;
        globalDispVect[19] =  dezp[checkL].dy;
        globalDispVect[20] =  dezp[checkL].dz;
        globalDispVect[21] =  dezp[checkL].rx;
        globalDispVect[22] =  dezp[checkL].ry;
        globalDispVect[23] =  dezp[checkL].rz;

        for (var j = 0; j < elemDim; j++)
        {
            localDispVect[j] = 0;
            for (var k = 0; k < elemDim; k++)
                localDispVect[j] += trans[k, j] * globalDispVect[k];
        }
        //getLoadVector(area, ref loadVect, casoCargaActual,m_mDB);
        //Load vector is zeros always for this element
        // for (var j = 0; j < elemDim; j++)
        // {
        //     localLoadVect[j] = 0;
        //     for (var k = 0; k < elemDim; k++)
        //         localLoadVect[j] += trans[k, j] * loadVect[k];
        // }
        for (int j = 0; j < elemDim; j++)
        {
            nodalForces[j] = 0.0;
            for (int k = 0; k < elemDim; k++)
            {
                nodalForces[j] += elemKMtrx[j, k] * localDispVect[k];
            }
        }
        // for (int j = 0; j < elemDim; j++)
        // {
        //     nodalForces[j] -= localLoadVect[j];
        // }

        var accInternAreas = new areaInternalForce();
        accInternAreas.ID = m_mDB.areaInternalForces.NextID();
        accInternAreas.AreaID = area.ID;
        accInternAreas.Case = casoCargaActual;
        
        int x =0;       
        accInternAreas.Fx1 =nodalForces[x++];
        accInternAreas.Fy1 =nodalForces[x++];
        accInternAreas.Fz1 =nodalForces[x++];
        accInternAreas.Mx1 =nodalForces[x++];
        accInternAreas.My1 =nodalForces[x++];
        accInternAreas.Mz1 =nodalForces[x++];

        accInternAreas.Fx2 =nodalForces[x++];
        accInternAreas.Fy2 =nodalForces[x++];
        accInternAreas.Fz2 =nodalForces[x++];
        accInternAreas.Mx2 =nodalForces[x++];
        accInternAreas.My2 =nodalForces[x++];
        accInternAreas.Mz2 =nodalForces[x++];

        accInternAreas.Fx3 =nodalForces[x++];
        accInternAreas.Fy3 =nodalForces[x++];
        accInternAreas.Fz3 =nodalForces[x++];
        accInternAreas.Mx3 =nodalForces[x++];
        accInternAreas.My3 =nodalForces[x++];
        accInternAreas.Mz3 =nodalForces[x++];

        accInternAreas.Fx4 =nodalForces[x++];
        accInternAreas.Fy4 =nodalForces[x++];
        accInternAreas.Fz4 =nodalForces[x++];
        accInternAreas.Mx4 =nodalForces[x++];
        accInternAreas.My4 =nodalForces[x++];
        accInternAreas.Mz4 =nodalForces[x++];


        aForces.Add(accInternAreas);
    }

//Better to have copy in each kind

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



// Closed-form stiffness matrix of a rectangular 4-node Mindlin-Reissner
// plate-shell element (Q4), 6 DOF per node, in the element local system.
//
// Node order (counter-clockwise, local x along 1->2, local y along 1->4):
//
//     4 ------- 3
//     |         |
//     |         |  b
//     1 ------- 2
//          a
//
// DOF order per node: u, v, w, rx, ry, rz
// Position of a DOF inside ke = 6 * node + dofOffset   (node = 0..3)
//
// The matrix is the sum of four independent parts:
//   1. membrane  (u, v)
//   2. bending   (rx, ry)
//   3. shear     (w, rx, ry)  with MITC4 to avoid shear locking
//   4. drilling  (rz)         penalty tying each rz to the in-plane rotation
//
// Every block is filled with plain double loops over all node pairs (i, j),
// so no symmetry trick is used: both ke[row, col] and ke[col, row] are
// written explicitly.
internal static class Q4PlateStiffness
{
    // DOF offsets inside one node
    const int DofU = 0;
    const int DofV = 1;
    const int DofW = 2;
    const int DofRx = 3;
    const int DofRy = 4;
    const int DofRz = 5;

    // Natural coordinates (xi, eta) of the 4 nodes, same order as the sketch above
    static readonly double[] nodeXi = { -1.0, 1.0, 1.0, -1.0 };
    static readonly double[] nodeEta = { -1.0, -1.0, 1.0, 1.0 };

    // Adds the element stiffness into ke (at least 24 x 24).
    // Values are accumulated with +=, so ke must be zero on entry.
    //   a, b   : element sides along local x and local y
    //   E, nu  : Young modulus and Poisson ratio
    //   t      : thickness
    internal static void addStiffness(double a, double b, double E, double nu, double t,
                                      double[,] ke,
                                      double shearCorrection = 5.0 / 6.0,
                                      double drillingFactor = 1.0e-3)
    {
        // G is computed from E and nu so the element is exactly isotropic
        double G = E / (2.0 * (1.0 + nu));

        double membraneStiff = E * t / (1.0 - nu * nu);                  // c
        double bendingStiff = E * t * t * t / (12.0 * (1.0 - nu * nu));  // Db
        double shearStiff = shearCorrection * G * t;                     // ks
        double nuShear = (1.0 - nu) / 2.0;                               // nu'

        double drillingStiff = drillingFactor * G * t * a * b;

        addMembrane(a, b, nu, nuShear, membraneStiff, ke);
        addBending(a, b, nu, nuShear, bendingStiff, ke);
        addShear(a, b, shearStiff, ke);
        addDrilling(a, b, drillingStiff, ke);
    }

    // -------------------------------------------------------------
    // 1. MEMBRANE (u, v)
    // -------------------------------------------------------------
    // For every node pair (i, j) two helper terms are used:
    //   p = b / (4a) * xi_i xi_j  * (1 + eta_i eta_j / 3)   (strain along x)
    //   q = a / (4b) * eta_i eta_j * (1 + xi_i xi_j / 3)    (strain along y)
    // They come from the exact 2x2 Gauss integration on a rectangle.
    private static void addMembrane(double a, double b, double nu, double nuShear,
                                    double c, double[,] ke)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                double sx = nodeXi[i] * nodeXi[j];
                double sy = nodeEta[i] * nodeEta[j];

                double p = b / (4.0 * a) * sx * (1.0 + sy / 3.0);
                double q = a / (4.0 * b) * sy * (1.0 + sx / 3.0);

                int rowU = 6 * i + DofU;
                int rowV = 6 * i + DofV;
                int colU = 6 * j + DofU;
                int colV = 6 * j + DofV;

                // u-u and v-v
                ke[rowU, colU] += c * (p + nuShear * q);
                ke[rowV, colV] += c * (q + nuShear * p);

                // u_i - v_j and its transpose v_j - u_i
                double kuv = c / 4.0 * (nu * nodeXi[i] * nodeEta[j]
                                      + nuShear * nodeXi[j] * nodeEta[i]);
                ke[rowU, colV] += kuv;
                ke[colV, rowU] += kuv;
            }
        }
    }

    // -------------------------------------------------------------
    // 2. BENDING (rx, ry)
    // -------------------------------------------------------------
    // Same helper terms p and q as in the membrane.
    // Curvatures: kx = d(ry)/dx,  ky = -d(rx)/dy,  kxy = d(ry)/dy - d(rx)/dx
    // NOTE: the rx-ry block is NOT symmetric in (i, j), so all 16 pairs
    // must be visited (this was the cause of the Fz unbalance).
    private static void addBending(double a, double b, double nu, double nuShear,
                                   double Db, double[,] ke)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                double sx = nodeXi[i] * nodeXi[j];
                double sy = nodeEta[i] * nodeEta[j];

                double p = b / (4.0 * a) * sx * (1.0 + sy / 3.0);
                double q = a / (4.0 * b) * sy * (1.0 + sx / 3.0);

                int rowRx = 6 * i + DofRx;
                int rowRy = 6 * i + DofRy;
                int colRx = 6 * j + DofRx;
                int colRy = 6 * j + DofRy;

                // rx-rx and ry-ry
                ke[rowRx, colRx] += Db * (q + nuShear * p);
                ke[rowRy, colRy] += Db * (p + nuShear * q);

                // rx_i - ry_j and its transpose ry_j - rx_i
                double kxy = -Db / 4.0 * (nu * nodeXi[j] * nodeEta[i]
                                        + nuShear * nodeXi[i] * nodeEta[j]);
                ke[rowRx, colRy] += kxy;
                ke[colRy, rowRx] += kxy;
            }
        }
    }

    // -------------------------------------------------------------
    // 3. SHEAR with MITC4 (w, rx, ry)
    // -------------------------------------------------------------
    // Shear strains: gxz = dw/dx + ry,   gyz = dw/dy - rx
    // MITC4 does not use the strains at the Gauss points. It measures them
    // at 4 tying points and interpolates them linearly:
    //
    //   A = (xi= 0, eta=+1)   gxz_A   -> vector gA
    //   C = (xi= 0, eta=-1)   gxz_C   -> vector gC
    //   D = (xi=+1, eta= 0)   gyz_D   -> vector hD
    //   B = (xi=-1, eta= 0)   gyz_B   -> vector hB
    //
    //   gxz = (1+eta)/2 * gxz_A + (1-eta)/2 * gxz_C
    //   gyz = (1+xi )/2 * gyz_D + (1-xi )/2 * gyz_B
    //
    // Each vector has 12 entries: 3 per node, ordered [w, rx, ry] of node 1..4.
    // Integrating the squares and products of the two interpolations over the
    // element gives:
    //
    //   ks * a * b / 6 * ( 2 * (gA gA' + gC gC' + hD hD' + hB hB')
    //                        + (gA gC' + gC gA' + hD hB' + hB hD') )
    //
    // The weights are stored in the 4x4 matrix 'tieWeight'.
    private static void addShear(double a, double b, double ks, double[,] ke)
    {
        // Local position of w, rx, ry inside the 3 entries of one node
        const int LocW = 0;
        const int LocRx = 1;
        const int LocRy = 2;

        // Rows: 0 = gA, 1 = gC, 2 = hD, 3 = hB.   Columns: 3 * node + LocXxx
        double[,] tie = new double[4, 12];

        // gA: gxz at A. Only nodes 3 and 4 (index 2 and 3) are on the line eta = +1
        tie[0, 3 * 2 + LocW] = 1.0 / a;
        tie[0, 3 * 3 + LocW] = -1.0 / a;
        tie[0, 3 * 2 + LocRy] = 0.5;
        tie[0, 3 * 3 + LocRy] = 0.5;

        // gC: gxz at C. Only nodes 1 and 2 (index 0 and 1) are on the line eta = -1
        tie[1, 3 * 0 + LocW] = -1.0 / a;
        tie[1, 3 * 1 + LocW] = 1.0 / a;
        tie[1, 3 * 0 + LocRy] = 0.5;
        tie[1, 3 * 1 + LocRy] = 0.5;

        // hD: gyz at D. Only nodes 2 and 3 (index 1 and 2) are on the line xi = +1
        tie[2, 3 * 1 + LocW] = -1.0 / b;
        tie[2, 3 * 2 + LocW] = 1.0 / b;
        tie[2, 3 * 1 + LocRx] = -0.5;
        tie[2, 3 * 2 + LocRx] = -0.5;

        // hB: gyz at B. Only nodes 1 and 4 (index 0 and 3) are on the line xi = -1
        tie[3, 3 * 0 + LocW] = -1.0 / b;
        tie[3, 3 * 3 + LocW] = 1.0 / b;
        tie[3, 3 * 0 + LocRx] = -0.5;
        tie[3, 3 * 3 + LocRx] = -0.5;

        // Weights: 2 on the diagonal, 1 between gA-gC and between hD-hB
        double[,] tieWeight =
        {
            { 2.0, 1.0, 0.0, 0.0 },
            { 1.0, 2.0, 0.0, 0.0 },
            { 0.0, 0.0, 2.0, 1.0 },
            { 0.0, 0.0, 1.0, 2.0 }
        };

        double factor = ks * a * b / 6.0;

        for (int p = 0; p < 12; p++)
        {
            for (int q = 0; q < 12; q++)
            {
                double sum = 0.0;
                for (int m = 0; m < 4; m++)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        sum += tieWeight[m, n] * tie[m, p] * tie[n, q];
                    }
                }

                ke[shearDofIndex(p), shearDofIndex(q)] += factor * sum;
            }
        }
    }

    // Converts the shear local index (0..11) to the position inside ke.
    // local = 3 * node + (0: w, 1: rx, 2: ry).
    // In ke, w, rx and ry are consecutive (offsets 2, 3, 4), so the
    // position is 6 * node + DofW + (local % 3).
    private static int shearDofIndex(int local)
    {
        int node = local / 3;
        int component = local % 3;
        return 6 * node + DofW + component;
    }

    // -------------------------------------------------------------
    // 4. DRILLING (rz)
    // -------------------------------------------------------------
    // The membrane does not use rz, so without this term ke is singular.
    // One penalty per node ties its rz to the mean in-plane rotation of the
    // element, omega, evaluated at the element center:
    //
    //   omega = 1/2 * (dv/dx - du/dy)
    //   energy = drillingStiff * sum over nodes i of (rz_i - omega)^2
    //
    // A rigid rotation about z (u = -th*y, v = th*x, rz = th in all nodes)
    // gives rz_i - omega = 0, so it costs no energy. A plain diagonal spring
    // on rz would not do this.
    // A single penalty on the mean of the 4 rz is NOT enough: it leaves
    // 3 rz patterns without stiffness (ke would have 9 zero eigenvalues).
    private static void addDrilling(double a, double b, double drillingStiff, double[,] ke)
    {
        // omega as a row vector over the 24 DOFs: omega = omega . d
        double[] omega = new double[24];
        for (int j = 0; j < 4; j++)
        {
            omega[6 * j + DofV] = nodeXi[j] / (4.0 * a);
            omega[6 * j + DofU] = -nodeEta[j] / (4.0 * b);
        }

        for (int node = 0; node < 4; node++)
        {
            // r = (rz_node - omega) as a row vector over the 24 DOFs
            double[] r = new double[24];
            for (int k = 0; k < 24; k++)
            {
                r[k] = -omega[k];
            }
            r[6 * node + DofRz] += 1.0;

            for (int row = 0; row < 24; row++)
            {
                for (int col = 0; col < 24; col++)
                {
                    ke[row, col] += drillingStiff * r[row] * r[col];
                }
            }
        }
    }

    // Reference check (a = b = 1, E = 1, nu = 0.3, t = 1, drillingFactor = 0):
    //   ke[u1, u1] =  0.49451     ke[u1, u3] = -0.24725 (node 3 is the opposite corner)
    //   ke[u1, u2] = -0.30220     ke[u1, u4] =  0.05495 (node 4 is the neighbor along y)
    //   ke[w1, w1] =  0.21368     ke[w1, w3] = -0.10684
    //   ke[rx1, rx1] = 0.06792    ke[rx1, ry1] = -0.01488
    // The 24 x 24 matrix is symmetric and has exactly 6 zero eigenvalues
    // (3 rigid translations, rigid rotations about x, y and z).
}

}