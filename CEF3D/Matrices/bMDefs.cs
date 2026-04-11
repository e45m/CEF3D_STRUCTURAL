using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
namespace CEF.matrixImpl;
using T = double;
internal partial class BandedMatrix
{
    struct Profile
    {
      public int start;
      public int end;
    }
    private int Rows;
    private int Columns;
    public int getRows() => Rows;
    public int getCols() => Columns;
    private T[][] rawMtrx;
    private Profile[] p; 
    private Profile[] vp; 
    private readonly int compactThreshold;
    int helper; 
}