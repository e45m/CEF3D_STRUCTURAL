using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static CEF.viewFrame;
namespace CEF;
public class modelView : Cef3DWindowBase
{
    int mvHeight, mvWidth;
    int viewFramesCount;
    public viewFrame ViewFrame { get => actVF; }
    private viewFrame actVF;
    private viewFrame[] ViewFrames;
    private Form[] Containers;

    public modelView()
    {
        if (App.modelViewInstances == 0)
            App.mainWindow.Visible = false;
        App.modelViewInstances++;
        mvHeight = App.mainWindow.Height;
        mvWidth = App.mainWindow.Width;
        viewFramesCount = 1;//max 4
        ViewFrames = new viewFrame[viewFramesCount];
        Containers = new Form[viewFramesCount];
        for (int i = 0; i < viewFramesCount; i++)
        {
            Containers[i] = new()
            {
                TopLevel = false,                
                FormBorderStyle = FormBorderStyle.None,               
                Tag = i,
            };
            setLocation(Containers[i]);
            Containers[i].Show();
            ViewFrames[i] = new viewFrame(Containers[i], App.mainWindow, 
                                            Containers[i].Height, Containers[i].Width);
            actVF = ViewFrames[0];
            ViewFrames[0].activeVF = VFTyes.Single;
        }
        Controls.AddRange(Containers);
        Containers[0].Select();   
        setComboBoxes();   

        this.Resize += ModelView_Resize;
        this.FormClosing += ModelView_FormClosing;        
    }

    internal void  setStylesToAllVF(UISettings ms)
    {
   foreach (viewFrame vf in ViewFrames) 
                
      vf.ms = ms.Clone();

    }
    internal void setActiveVF(viewFrame VF)
    {

        if(viewFramesCount==1)
        { 
            actVF.activeVF = VFTyes.Single;
            return;
        }

        if (viewFramesCount>1&& actVF != VF){
            actVF.activeVF = VFTyes.NoActive;
            actVF = VF;       
            actVF.activeVF = VFTyes.Active;
            reloadViewFramesCanvas();
            actVF.Container.Select();
        }
}

    internal void reloadViewFramesCanvas()
    {
        foreach (var vf in ViewFrames)
            vf.reloadCanvas();
    }

    public void setViewframeCount(int n)
    {
        if (n == viewFramesCount) return;
        var oldvfc = viewFramesCount;
        viewFramesCount = n;
        var newViewFrames = new viewFrame[viewFramesCount];
        var newContainers = new Form[viewFramesCount];
        foreach (var Cont in Containers)
            Controls.Remove(Cont);
        if (n > oldvfc)
        {
            for (int i = 0; i < oldvfc; i++)
            {
                newViewFrames[i] = ViewFrames[i];
                newContainers[i] = Containers[i];
            }
            for (int i = oldvfc; i < viewFramesCount; i++)
            {
                newContainers[i] = new()
                {

                    TopLevel = false,                   
                    FormBorderStyle = FormBorderStyle.None,
                    Tag = i,
                };
                setLocation(newContainers[i]);
                newContainers[i].Show();
                newViewFrames[i] = new viewFrame(newContainers[i], App.mainWindow,
                                            newContainers[i].Height, newContainers[i].Width);
            }
        }
        else
        {   var l = ViewFrames.Length;
            for (int i = 0; i < n; i++)
            {
            int t = -1;
            for (int i2 = 0; i2 < l; i2++)
            {
                if ((int)ViewFrames[i2].Container.Tag == i)
                {
                    t = i2;
                    break;
                }


            }

                newViewFrames[i] = ViewFrames[t];
                newContainers[i] = Containers[t];

            }


        }
        ViewFrames = newViewFrames;
        Containers = newContainers;
        Controls.AddRange(Containers);
        mvHeight = Height;
        mvWidth = Width;
        for (int i = 0; i < viewFramesCount; i++)
        {
            setLocation(Containers[i]);
            ViewFrames[i].canvasRefreh();
        }
        Containers[0].Select();
    }
    private void ModelView_FormClosing(object sender, FormClosingEventArgs e)
    {
        App.modelViewInstances--;
        if (App.modelViewInstances == 0)
            App.mainWindow.Visible = true;
    }
    private void ModelView_Resize(object sender, EventArgs e)
    {
        mvHeight = Height;
        mvWidth = Width;

        for (int i = 0; i < viewFramesCount; i++)
        {
            

            setLocation(Containers[i]);
            ViewFrames[i].canvasRefreh();
        }
    }

        void setLocation(Form container){

        int tag =(int) container.Tag;
        int wfx = viewFramesCount < 2 ? 1 : 2;
        int wfy = viewFramesCount < 3 ? 1 : 2;
        const int clip = 95;
        container.Location = new Point(
                    30 + (tag % 2 == 0 ? 0 : mvWidth / wfx),
                    30 + (tag < 2 ? 0 :( mvHeight  - clip)/2));
        container.Width = mvWidth / wfx;
        //container.Height = mvHeight / wfy;
        container.Height = tag == 1 && viewFramesCount==3  ? mvHeight-clip : (mvHeight  - clip)/wfy;
    }

public void vfMove(bool up=true)
    {
        var currTag = (int)actVF.Container.Tag;
        var step = up ? 1 : -1;
        var newTag = currTag + step;

        if (newTag < viewFramesCount && newTag >= 0)
        {
            int t = -1;
            for (int i = 0; i < viewFramesCount; i++)
            {
                if ((int)ViewFrames[i].Container.Tag == newTag)
                {
                    t = i;
                    break;
                }


            }
            ViewFrames[t].Container.Tag = currTag;
            actVF.Container.Tag = newTag;

            for (int i = 0; i < viewFramesCount; i++)
            {
                setLocation(Containers[i]);
                 ViewFrames[i].canvasRefreh();
            }

        }
    }

}