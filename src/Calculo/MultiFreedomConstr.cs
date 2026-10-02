using CEF.matrixImpl;
using CEF.LUSolver;
using System.Data;
namespace CEF.MFC;
internal class Mfc
{
    const int GDLxNUDO = 6;
    internal struct Restriction
    {
        internal int type; 
        internal int masterID;
        internal double masterX;
        internal double masterY;
        internal double masterZ;
        internal List<int> nodeIDs;
    }
    internal static CalcState createMFCMatrix( ModelDB mDB, ref matrixImpl.SparseMatrix T)
    {
        var m_mDB = mDB;
        var restricciones = App.model.nodalConstraints;
        if (restricciones.Count == 0)
            return CalcState.OK;
        var nodes = m_mDB.nodes;
        List<Restriction> constr = [];
        double x_ = 0, y_ = 0, z_ = 0;
        int count_ = 0, rest_count_ = 0;
        var id = -1;
        foreach (var Restr in restricciones)
        {
            Restriction c = new();
            c.nodeIDs = [];
            c.type = Restr.Type;
            x_ = 0; y_ = 0; z_ = 0; count_ = 0;
            var node_listIDs = Restr.nodesIDList.Replace(" ","")
                .Split(",",StringSplitOptions.RemoveEmptyEntries).
                ToArray().
                Select(int.Parse).ToArray();
            foreach (var item in node_listIDs)
            {
                var nodeInx = nodes.FindIndexByID(item);
                if (nodeInx == -1) throw new Exception();
                c.nodeIDs.Add(item);
                x_ += nodes[nodeInx].X;
                y_ += nodes[nodeInx].Y;
                z_ += nodes[nodeInx].Z;
                count_++;
            }
            c.masterID = nodes.Max(n=>n.ID) +1+ rest_count_; 
            c.masterX = x_ / count_;
            c.masterY = y_ / count_;
            c.masterZ = z_ / count_;
            rest_count_++;
            var tol = 0.5d;
            var n = m_mDB.nodes.FirstOrDefault(p1 =>
            {
                var x = Math.Abs(c.masterX - p1.X) < tol;
                var y = Math.Abs(c.masterY - p1.Y) < tol;
                var z = Math.Abs(c.masterZ - p1.Z) < tol;
                return x && y && z;
            },/*Default*/new node() { ID = -1 });
            if (n is node mn)
                id = mn.ID;
            if (id == -1)
            {
                var mnode = new node()
                {
                    ID = c.masterID,
                    label = $"{Restr.label}*",
                    X = c.masterX,
                    Y = c.masterY,
                    Z = c.masterZ
                };
                m_mDB.nodes.Add(mnode);
                m_mDB.nodalRestraints.Add(new() { ID = c.masterID });
                id = c.masterID;
            }
            else
            {
                m_mDB.nodes.First(n => n.ID == id).label += "*";
            }
            Restr.masterNodeID = id;
            c.masterID = id;
            constr.Add(c);
        }
        mDB.SortNodesbyCoordinates();
        var Trows = m_mDB.nodes.Count; 
        var Tcols = Trows;
        Trows *= GDLxNUDO;
        Tcols *= GDLxNUDO;
        T = new SparseMatrix(Trows, Tcols); 
        T.makeI();
        setT_MFC(ref  T,  m_mDB,  constr);
        return CalcState.OK;
 }
    internal static CalcState setT_MFC(ref  SparseMatrix T,  ModelDB m_mDB,  List<Restriction> constr)
    {
        var nodes = m_mDB.nodes;
            foreach (var mfc in constr)
            {
                var masterID = mfc.masterID;
                var masterIndex = nodes.FindIndexByID(masterID);
                var masterGdlStart = masterIndex * GDLxNUDO;
                double xmaster = mfc.masterX;
                double ymaster = mfc.masterY;
                double zmaster = mfc.masterZ;
                foreach (int slaveID in mfc.nodeIDs)
                {
                   if (slaveID == masterID) continue;
                    var indexNode = nodes.FindIndexByID(slaveID);
                    var slaveGdlStart = indexNode*GDLxNUDO;
                    var node = nodes[indexNode];
                    double dx =  node.X -xmaster ;
                    double dy =  node.Y -ymaster ;
                    double dz =  node.Z -zmaster ;
                switch ((MFCType)mfc.type)
                        {
                            case MFCType.diaphragm:
                                {
                                    T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                                    T[slaveGdlStart + 1, slaveGdlStart + 1] = 0.0;
                                    T[slaveGdlStart + 2, slaveGdlStart + 2] = 1.0;
                                    T[slaveGdlStart + 3, slaveGdlStart + 3] = 1.0;
                                    T[slaveGdlStart + 4, slaveGdlStart + 4] = 1.0;
                                    T[slaveGdlStart + 5, slaveGdlStart + 5] = 0.0;
                                    T[slaveGdlStart + 0, masterGdlStart + 5] = -dy;
                                    T[slaveGdlStart + 1, masterGdlStart + 5] = dx;
                                    T[slaveGdlStart + 5, masterGdlStart + 5] = 1.0;
                                    T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                                    T[slaveGdlStart + 1, masterGdlStart + 1] = 1.0;
                                break;
                                }
                    case MFCType.diaphragmXX: 
                        {
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 4, slaveGdlStart + 4] = 1.0;
                            T[slaveGdlStart + 5, slaveGdlStart + 5] = 1.0;
                            T[slaveGdlStart + 1, slaveGdlStart + 1] = 0.0;
                            T[slaveGdlStart + 2, slaveGdlStart + 2] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 1, masterGdlStart + 1] = 1.0; 
                            T[slaveGdlStart + 2, masterGdlStart + 2] = 1.0; 
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0; 
                            T[slaveGdlStart + 1, masterGdlStart + 3] = -dz; 
                            T[slaveGdlStart + 2, masterGdlStart + 3] = dy;  
                            break;
                        }
                    case MFCType.diaphragmYY: 
                        {
                            T[slaveGdlStart + 1, slaveGdlStart + 1] = 1.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 1.0;
                            T[slaveGdlStart + 5, slaveGdlStart + 5] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 2, slaveGdlStart + 2] = 0.0;
                            T[slaveGdlStart + 4, slaveGdlStart + 4] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0; 
                            T[slaveGdlStart + 2, masterGdlStart + 2] = 1.0; 
                            T[slaveGdlStart + 4, masterGdlStart + 4] = 1.0; 
                            T[slaveGdlStart + 0, masterGdlStart + 4] = dz;  
                            T[slaveGdlStart + 2, masterGdlStart + 4] = -dx; 
                            break;
                        }
                    case MFCType.plate: 
                        {
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 1, slaveGdlStart + 1] = 1.0;
                            T[slaveGdlStart + 5, slaveGdlStart + 5] = 1.0;
                            T[slaveGdlStart + 2, slaveGdlStart + 2] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 4, slaveGdlStart + 4] = 0.0;
                            T[slaveGdlStart + 2, masterGdlStart + 2] = 1.0; 
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0; 
                            T[slaveGdlStart + 4, masterGdlStart + 4] = 1.0; 
                            T[slaveGdlStart + 2, masterGdlStart + 3] = dy;  
                            T[slaveGdlStart + 2, masterGdlStart + 4] = -dx; 
                            break;
                        }
                    case MFCType.plateXX: 
                        {
                            T[slaveGdlStart + 1, slaveGdlStart + 1] = 1.0;
                            T[slaveGdlStart + 2, slaveGdlStart + 2] = 1.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 4, slaveGdlStart + 4] = 0.0;
                            T[slaveGdlStart + 5, slaveGdlStart + 5] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 4, masterGdlStart + 4] = 1.0;
                            T[slaveGdlStart + 5, masterGdlStart + 5] = 1.0;
                            T[slaveGdlStart + 0, masterGdlStart + 4] = dz;
                            T[slaveGdlStart + 0, masterGdlStart + 5] = -dy;
                            break;
                        }
                    case MFCType.plateYY: 
                        {
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 2, slaveGdlStart + 2] = 1.0;
                            T[slaveGdlStart + 4, slaveGdlStart + 4] = 1.0;
                            T[slaveGdlStart + 1, slaveGdlStart + 1] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 5, slaveGdlStart + 5] = 0.0;
                            T[slaveGdlStart + 1, masterGdlStart + 1] = 1.0;
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0;
                            T[slaveGdlStart + 5, masterGdlStart + 5] = 1.0;
                            T[slaveGdlStart + 1, masterGdlStart + 3] = -dz;
                            T[slaveGdlStart + 1, masterGdlStart + 5] = dx;
                            break;
                        }
                    case MFCType.body:
                    case MFCType.weld: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 0, masterGdlStart + 4] = dz;
                            T[slaveGdlStart + 0, masterGdlStart + 5] = -dy;
                            T[slaveGdlStart + 1, masterGdlStart + 1] = 1.0;
                            T[slaveGdlStart + 1, masterGdlStart + 3] = -dz;
                            T[slaveGdlStart + 1, masterGdlStart + 5] = dx;
                            T[slaveGdlStart + 2, masterGdlStart + 2] = 1.0;
                            T[slaveGdlStart + 2, masterGdlStart + 3] = dy;
                            T[slaveGdlStart + 2, masterGdlStart + 4] = -dx;
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0;
                            T[slaveGdlStart + 4, masterGdlStart + 4] = 1.0;
                            T[slaveGdlStart + 5, masterGdlStart + 5] = 1.0;
                            break;
                        }
                    case MFCType.beamX: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0;
                            break;
                        }
                    case MFCType.rodX: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            break;
                        }
                    case MFCType.beamY: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0;
                            break;
                        }
                    case MFCType.rodY: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            break;
                        }
                    case MFCType.beamZ: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 3, slaveGdlStart + 3] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            T[slaveGdlStart + 3, masterGdlStart + 3] = 1.0;
                            break;
                        }
                    case MFCType.rodZ: 
                        {
                            for (int i = 0; i < 6; i++) T[slaveGdlStart + i, slaveGdlStart + i] = 1.0;
                            T[slaveGdlStart + 0, slaveGdlStart + 0] = 0.0;
                            T[slaveGdlStart + 0, masterGdlStart + 0] = 1.0;
                            break;
                        }
                    case MFCType.equal: 
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                T[slaveGdlStart + i, slaveGdlStart + i] = 0.0;
                                T[slaveGdlStart + i, masterGdlStart + i] = 1.0;
                            }
                            break;
                        }
                    default:
                        throw new Exception();
                        }
                }
        }
        return CalcState.OK;
    }
}