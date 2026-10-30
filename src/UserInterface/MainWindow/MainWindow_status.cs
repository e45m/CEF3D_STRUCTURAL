
using System.Security.Cryptography;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public partial class MainWindow
{
 internal void enableDisable(bool enable )
    {
     List<Form> listChild;
     listChild = OwnedForms.Where(x => x is modelView).ToList(); 
     foreach (var A in listChild)
        A.Enabled = enable;
 }

 //Todo: After cald, design, etc controls enadble/disable

 internal void enableDisable_calc(bool enable )
    {
     List<Form> listChild;
     listChild = OwnedForms.Where(x => x is modelView).ToList(); 
     foreach (var A in listChild)
        { 
         ((modelView)A).MainMenuCEF.Items.Find("Edit",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("Draw",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("Nodes",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("Bars",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("CoordSys",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("CreateCases",true)[0].Enabled=enable;
         ((modelView)A).MainMenuCEF.Items.Find("Results",true)[0].Enabled=!enable;
         
        }}
        
 }


