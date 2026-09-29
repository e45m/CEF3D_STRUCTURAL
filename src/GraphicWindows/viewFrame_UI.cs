using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
namespace CEF;

public partial class viewFrame
{
    internal PictureBox Canvas;
    internal ToolTip ballonTip;

    public Form Container;
   
    internal int vfHeight, vfWidth, vfTtop, vfLeft;

    private void initVfControls()
    {
       
        Canvas = new PictureBox();

        if (Canvas.Container != null)
        {
            ballonTip = new(Canvas.Container);
        }
        else
        {
            ballonTip = new();
        }
        Canvas.Anchor = AnchorStyles.None; //AnchorStyles.Top|AnchorStyles.Left;
        Canvas.ErrorImage = Properties.Resources.Ejes;
        Canvas.InitialImage = null;
        Canvas.Margin = new Padding(4, 3, 4, 3);
        Canvas.TabIndex = 1;
        Canvas.TabStop = false;
        Canvas.Paint += CanvasPaint;
        Canvas.MouseDown += canvasMouseDown;
        Canvas.MouseMove += CanvasMouseMove;
        Canvas.MouseUp += canvasMouseUp;
        Canvas.Resize += canvasResize;
        
        Container.AllowDrop = true;
        Container.AutoScaleDimensions = new SizeF(7F, 15F);
        Container.AutoScaleMode = AutoScaleMode.Font;       
        Container.Controls.Add(Canvas);
        //Container.DoubleBuffered;
        Container.Margin = new Padding(4, 3, 4, 3);
        Container.MinimizeBox = true;
        Container.MaximizeBox = true;
        Container.ShowIcon = true;
        Container.ShowInTaskbar = true;
        Container.Load += modelViewLoad;
        Container.KeyDown += canvasKeyDown;
        Container.KeyPress += canvasKeyPress;
        Container.KeyUp += canvasKeyUp;
        Container.MouseWheel += canvasMouseWheel;
        Container.Move += canvasMove;
        Container.Resize += containerResize;
        
        
    }

}