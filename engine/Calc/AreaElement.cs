using System.IO.Compression;

using CEF.Dictionary;

namespace CEF.AreaElement;


internal class areaElement
{

public static int areaElementSize(areaKind Kind)
{
    return Kind switch{
        areaKind.Membrane=>8,
        areaKind.Shell=>24,
        _ =>0
    };

}

public static int[] areaElementDOF(areaKind Kind)
{
    //u,v,w,rx,ry,rz
    return Kind switch{
        areaKind.Membrane=>[1,1,0,0,0,0],
        areaKind.Shell=>[1,1,1,1,1,1],
        _ =>[0,0,0,0,0,0]
    };

}


  
/// <summary>
/// Full local -> global transformation matrix of an area element:
/// 3x3 plane rotation repeated along the diagonal
/// (18x18 for a triangle, 24x24 for a quad; size taken from trans).
/// </summary>
internal static void getTransMatrix(Area area, double[,] trans, ModelDB m_mBD)
{
    var mNodes = m_mBD.nodes;
    double[,] T1gTM = new double[3, 3];
   // Array.Clear(T1gTM, 0, T1gTM.Length);
    Array.Clear(trans, 0, trans.Length);

    var nI = mNodes.FindByID(area.Node1);
    var nJ = mNodes.FindByID(area.Node2);
    var nK = mNodes.FindByID(area.Node3);
    var nL = mNodes.FindByID(area.Node4);

 

    // Node 4 not found/unassigned => triangle
    
    double[] pts;
    if (nL == null)
    {
        pts = new[] { nI.X, nI.Y, nI.Z, nJ.X, nJ.Y, nJ.Z, nK.X, nK.Y, nK.Z };
    }
    else
    {
        //var nL = mNodes[idxL];
        pts = new[] { nI.X, nI.Y, nI.Z, nJ.X, nJ.Y, nJ.Z, nK.X, nK.Y, nK.Z, nL.X, nL.Y, nL.Z };
    }

    space3d.getPlaneRotMtrxLocToGlob(pts, T1gTM);

    int blockCount = trans.GetLength(0) / 3;
    for (int b = 0; b < blockCount; b++)
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                trans[i + 3 * b, j + 3 * b] = T1gTM[i, j];
}

    internal static void getKedMatrix(Area area,double[,] ke, ModelDB m_mBD)
    {
        
        var Section = m_mBD.areaSections.FindByID(area.SectionID);
       
       
         switch  (Section.Kind)

        {   
            case (int)areaKind.Membrane:  Membrane.getStiffnesMtrx(area, ke,  m_mBD); break;            
            case (int)areaKind.Shell:  Shell.getStiffnesMtrx(area, ke,  m_mBD); break;            
            

            //----
             case (int)areaKind.Hole:             
             case (int)areaKind.None:  Array.Clear(ke, 0, ke.Length); break;            
            
            //one case for each option
        };


    }


    internal static void getLoadVector(Area area, ref double[] loadVect, int currLoadCaseId, ModelDB m_mBD)
    {   
        
        var Section = m_mBD.areaSections.FindByID(area.SectionID);

       
       
         switch  (Section.Kind)
        {  
             case (int)areaKind.Membrane:  Membrane.getLoadVector(area, ref loadVect, currLoadCaseId, m_mBD); break;            
             case (int)areaKind.Shell:  Shell.getLoadVector(area, ref loadVect, currLoadCaseId, m_mBD); break;            
            

            //----
             case (int)areaKind.Hole:             
             case (int)areaKind.None:  return ; //break;            
            
            //one case for each option
        };       
  
    }    
   
    internal static void getInternalForcesVector(Area area, int casoCargaActualID, ModelDB m_mBD)
    {     
       
        var Section = m_mBD.areaSections.FindByID(area.SectionID);
      
       
         switch  (Section.Kind)
        {   
            case (int)areaKind.Membrane:  Membrane.getInternalForcesVector(area,  casoCargaActualID, m_mBD); break;            
            case (int)areaKind.Shell:  Shell.getInternalForcesVector(area,  casoCargaActualID, m_mBD); break;            
            

            //----
             case (int)areaKind.Hole:             
             case (int)areaKind.None:  return ;// break;            
            
            //one case for each option
        };


    }

}