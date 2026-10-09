using CEF.matrixImpl;
using System.Windows.Forms;
using static CEF.modelView;
using static System.Math;
namespace CEF;
public partial class MainWindow
{
 public void reloadAllCanvasWindows()
    {       
        List<Form> listChild;
        listChild = OwnedForms.Where(x => x is modelView).ToList(); 
        foreach (var currentA in listChild)
            ((modelView)currentA).reloadViewFramesCanvas();
    }

 public void CloseAllWindows()
    {
        List<Form> listChild;
        listChild = OwnedForms.ToList(); 
        foreach (var currentA in listChild)
          currentA.Close();
    }
 internal void addNewWindow(object sender, EventArgs e)
  {
      var vas = new modelView(); 
      vas.Text = vas.Text + " [" + App.modelViewInstances + "]" + App.activeFileName; 
          vas.Show(this); 
  }
 internal void abuotDlgShow(object sender, EventArgs e)
    {
        var vbd = new LicenceNotice();
        vbd.Show(this);
    }


    internal void setViewframes(object sender, EventArgs e)
    {

        var mitem = ((ToolStripMenuItem)sender);
        var s =int.Parse(mitem?.Text??"0");

        var f =(modelView) ((mitem)).OwnerItem.OwnerItem.Owner.FindForm();
 

        if (f is modelView )
        {
            f.setViewframeCount(s);
            f.Refresh();
            f.ViewFrame.resetCanvasGeometry();
        }
    }

    internal void setXYView(object sender, EventArgs e)
  {
        if (ActiveForm is modelView mv)
        {
            mv.Text = "XY" + " [" + App.modelViewInstances + "]" + App.activeFileName;
            mv.ViewFrame.setPlane (viewFrame.View.XY);
            mv.ViewFrame.resetCanvasGeometry();
            mv.ViewFrame.canvasRefreh();

        }
    }
  internal void setXZView(object sender, EventArgs e)
  {
        if (ActiveForm is modelView mv)
        {
            mv.Text = "XZ" + " [" + App.modelViewInstances + "]" + App.activeFileName;
            mv.ViewFrame.setPlane(viewFrame.View.XZ);
            mv.ViewFrame.resetCanvasGeometry();
            mv.ViewFrame.canvasRefreh();

        }

    }
  internal void setYZView(object sender, EventArgs e)
  {
        if (ActiveForm is modelView mv)
        {
            mv.Text = "YZ" + " [" + App.modelViewInstances + "]" + App.activeFileName;
            mv.ViewFrame.setPlane(viewFrame.View.YZ);
            mv.ViewFrame.resetCanvasGeometry();
            mv.ViewFrame.canvasRefreh();

        }
    }
  internal void set3DView(object sender, EventArgs e)
  {
       if (ActiveForm is modelView mv)
        {
            mv.Text = "3D" + " [" + App.modelViewInstances + "]" + App.activeFileName;
            mv.ViewFrame.setPlane(viewFrame.View.ThreeDimensional);
            mv.ViewFrame.resetCanvasGeometry();
            mv.ViewFrame.canvasRefreh();

        }

    }
  internal void movePlaneUp(object sender, EventArgs e)
  {
    if (ActiveForm is modelView mv)
    {
        
        mv.ViewFrame.incLevel();
        levelText.Text = mv.ViewFrame.getLevel().ToString();
        mv.ViewFrame.canvasRefreh();
    }
  }
  internal void movePlaneDowm(object sender, EventArgs e)
  {
    if (ActiveForm is modelView mv)
    {
            mv.ViewFrame.incLevel(-1);
            levelText.Text = mv.ViewFrame.getLevel().ToString();
             mv.ViewFrame.canvasRefreh();
    }
  }

    internal void snapBtnClick(object sender, EventArgs e)
    {


        var s = (ToolStripButton)sender;
        
        void setColor(bool flag ){
              if (flag)
              s.BackColor = Color.LightBlue;
            if (!flag)
               s.BackColor = Color.Gray;
        }

        var text = s.Text;

        if (ActiveForm is modelView mv)
        {
        switch (text)
        {
                case I18n.SnapEnd://"End":
                    viewFrame.snapToEnds = !viewFrame.snapToEnds;
                    setColor(viewFrame.snapToEnds);
                break;
                case I18n.SnapCenter:// "Cen":
                    viewFrame.snapToCenter = !viewFrame.snapToCenter;
                    setColor(viewFrame.snapToCenter);
                    break;
                case I18n.SnapNear:// "Nea":
                    viewFrame.snapToNear = !viewFrame.snapToNear;
                    setColor(viewFrame.snapToNear);
                    break;
                case I18n.SnapPerpendicular://"Per":
                    viewFrame.snapToPerp = !viewFrame.snapToPerp;
                    setColor(viewFrame.snapToPerp);
                    break;
                case I18n.SnapPoint:// "Pnt"://Point
                    viewFrame.snapToPoints = !viewFrame.snapToPoints;
                    setColor(viewFrame.snapToPoints);
                    break;

        }

            mv.ViewFrame.incLevel(-1);
            levelText.Text = mv.ViewFrame.getLevel().ToString();
            mv.ViewFrame.canvasRefreh();
        }
    }
    internal void setModelViewOptions(object sender, EventArgs e)
    {
        var mv = getActiveModelView();
        var pe = new SetPropWindow();
        pe.setProperty(mv.ms);
        pe.ShowDialog(this);
        if (mv.ms.applyToNewWindows)
            App.visualModelStyles = mv.ms.Clone();
        if (mv.ms.applyToAllWindows)
        {
            modelView A; 
            List<Form> listChild;
            listChild = OwnedForms.Where(x => x is modelView).ToList(); 
            foreach (var currentA in listChild)
            {
                A = (modelView)currentA;
                if (A.Text != mv.Container.Text)
                    A.ViewFrame.ms = mv.ms.Clone(); ;
            }
        }
    }
    internal void mvResteView(object sender, EventArgs e)
    {
       if (ActiveForm is modelView mv)
            {
                mv.ViewFrame.resetView();
                mv.ViewFrame.projection.setM();
                mv.ViewFrame.reloadCanvas();
            }
    }
    internal void nodeDisplTable(object sender, EventArgs e)
    {
        var tablename = nameof(App.model.nodalDisplacements);
        var vbd = new viewModelDBTable([tablename], false);
        vbd.Show(this);
    }
    internal void nodeReactTable(object sender, EventArgs e)
    {
        var tablename = nameof(App.model.nodalReactions);
        var vbd = new viewModelDBTable([tablename], false);
        vbd.Show(this);
    }
    internal void barInternalActionsTable(object sender, EventArgs e)
    {
        var tablename = nameof(App.model.barInternForces);
        var vbd = new viewModelDBTable([tablename], false);
        vbd.Show(this);
    }
    private viewFrame getActiveModelView()
    {
        viewFrame mv = null;
            if (ActiveForm is modelView)
            {
                mv =( (modelView)ActiveForm).ViewFrame;
          
            }
        return mv;
    }
}