using System.IO.Compression;

using CEF.Dictionary;

namespace CEF.AreaElement;

internal static class Membrane
{

    const int elemDim = 24;
    internal static void getStiffnesMtrx(Area area,double[,] ke, ModelDB m_mBD)
    {
        int i, j ;       
        var mNodes = m_mBD.nodes;

        var nI = mNodes.FindByID(area.Node1);
        var nJ = mNodes.FindByID(area.Node2);
        var nK = mNodes.FindByID(area.Node3);
        var nL = mNodes.FindByID(area.Node4);       

        var Section = m_mBD.areaSections.FindByID(area.SectionID);      
        var Material = m_mBD.materials.FindByID(Section.MaterialID);       

        //Must be rectangular. so nI-nJ == nK-nL and nI-nL==nJ-nK
        //Using a gross aprox for cuasi trapesoidal
        var Lx = (space3d.calcDist(nI,nJ)+space3d.calcDist(nK,nL))/2;
        var Ly = (space3d.calcDist(nI,nL)+space3d.calcDist(nJ,nK))/2;

        var r  = Lx/Ly;
        var r2 = r*r;
        var u1 = 1-Material.v;
        var u2 = 1+Material.v;
        var u3 = 1-3*Material.v;
        var E  = Material.E*Section.Thickness/(24*u1*u2*r);

        //ke 24x24 initialized to 0
        int[] x = [0,1,6,7,12,13,18,19];   // gdl locales 0..7 -> posiciones globales

        // set local  K(i,j) to ke global, this is simetryc
        void K(int i, int j, double val) => ke[x[i],x[j]] = ke[x[j],x[i]] = val*E;

        // diagonals
        for (int k = 0; k < 8; k += 2)
        {
            K(k,   k,   8 + 4*u1*r2);
            K(k+1, k+1, 8*r2 + 4*u1);
        }

        // normal terms
        K(2,0,-8+2*u1*r2);   K(3,1, 4*r2-4*u1);
        K(4,0,-4-2*u1*r2);   K(4,2, 4-4*u1*r2);
        K(5,1,-4*r2-2*u1);   K(5,3,-8*r2+2*u1);
        K(6,0, 4-4*u1*r2);   K(6,2,-4-2*u1*r2);   K(6,4,-8+2*u1*r2);
        K(7,1, 2*u1-8*r2);   K(7,3,-4*r2-2*u1);   K(7,5, 4*r2-4*u1);

        // cross terms (ux-uy)
        K(1,0, 3*r*u2);  K(2,1, 3*r*u3);  K(3,0,-3*r*u3);  K(3,2,-3*r*u2);
        K(4,1,-3*r*u2);  K(4,3, 3*r*u3);  K(5,0,-3*r*u2);  K(5,2,-3*r*u3);  K(5,4, 3*r*u2);
        K(6,1,-3*r*u3);  K(6,3, 3*r*u2);  K(6,5, 3*r*u3);
        K(7,0, 3*r*u3);  K(7,2, 3*r*u2);  K(7,4,-3*r*u3);  K(7,6,-3*r*u2);

        for (i = 0; i < 8; i++)
            for (j = i + 1; j < 8; j++)                
                ke[x[i],x[j]] = ke[x[j],x[i]];
            
      
        }

   

    internal static void getLoadVector(Area area, ref double[] loadVect, int currLoadCaseId, ModelDB m_mBD)
    {
        //Load vector is always zeros for this element. 
        // double[] elemLocLoadVect = new double[elemDim];
        //  double[] loadVect = new double[elemDim];
        //  double[] localLoadVect = new double[elemDim];
        return;
  
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
        Membrane.getStiffnesMtrx(area, elemKMtrx,m_mDB);

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


}