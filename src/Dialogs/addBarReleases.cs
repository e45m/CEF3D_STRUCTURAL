using System;
using System.Windows.Forms;
using static System.Math;
using static CEF.ModelDB;
namespace CEF;
public  class addBarReleases:Form
{
    
    internal CheckBox [] cbRels;

    private void InitializeComponent()
    { 
        cbRels = new CheckBox[12];
        var cbNames =new string[] { "u", "v", "w", "r1", "r2", "r3", "u", "v", "w", "r1", "r2", "r3" };

        int x, y;
        for(int i=0; i < 12; i++)
        {
            x = 40+(i<6?0:100);
            y = 50+i*30- (i < 6 ? 0 :6*30);
            cbRels[i] = new() { AutoSize = true, Location = new Point(x, y), Margin = new Padding(4, 3, 4, 3), Size = new Size(33, 19), TabIndex = 5, Text = cbNames[i], UseVisualStyleBackColor = true };           
        }

      var  label1 = new Label() { Text = I18n.startNode,AutoSize = true, Location = new Point( 40, 20), Size = new Size(44, 15), TabIndex = 8,};
      var  label2 = new Label() { Text = I18n.endNode,AutoSize = true, Location = new Point(140, 20), Size = new Size(44, 15), TabIndex = 9,};

      var  bttOk = new Button() { Location = new Point(40, 250), Margin = new Padding(4, 3, 4, 3), Size = new Size(70, 30),
            TabIndex = 3, Text = I18n.DlgOKText, UseVisualStyleBackColor = true,  DialogResult = DialogResult.OK};
        bttOk.Click += ok_Click;

      var  bttNo = new Button() {Location = new Point(120, 250), Margin = new Padding(4, 3, 4, 3),  Size = new Size(70, 30), 
            TabIndex = 4, Text = I18n.DlgCancelText, UseVisualStyleBackColor = true, DialogResult = DialogResult.Cancel};
        bttNo.Click += No_Click;

        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AcceptButton = bttOk;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(200, 300);

        Controls.AddRange(new Control[] {label1,label2, bttNo, bttOk });
        Controls.AddRange(cbRels);

        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowIcon = false;
        ShowInTaskbar = false;
        Text = I18n.ReleaseDegreeOfFreedom;
      
    } 

    public double MaxQ;
    public addBarReleases()
    {
        InitializeComponent();
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();   
    }
    private void ok_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        int N = Selection.barMaxIndex;
        int  indB;        
        var Sel = Selection.getBarsColection();
        var value = 0b0;
        for (int i = 0; i < 12; i++)
            if (cbRels[i].Checked)
                value += (1 << (11 - i));

        for (int i = 0; i <=N; i++)
            {
                indB = Sel[i];
                Bar barra = App.model.bars[indB];
                 barra.releases = value;
            }
            if (this.ParentForm is MainWindow cedm)
                    cedm.reloadAllCanvasWindows();
        Close();
    }

}

        //for (int i = 0; i< 12; i++)
        //    if (cbRels[i].Checked)
        //        value += (1 << (11 - i));
        //Equiv to: 
        //value += cbRels[0].Checked ? 0b100000000000 : 0b0;
        //value += cbRels[1].Checked ? 0b010000000000 : 0b0;
        //value += cbRels[2].Checked ? 0b001000000000 : 0b0;
        //value += cbRels[3].Checked ? 0b000100000000 : 0b0;
        //value += cbRels[4].Checked ? 0b000010000000 : 0b0;
        //value += cbRels[5].Checked ? 0b000001000000 : 0b0;
        //value += cbRels[6].Checked ? 0b000000100000 : 0b0;
        //value += cbRels[7].Checked ? 0b000000010000 : 0b0;
        //value += cbRels[8].Checked ? 0b000000001000 : 0b0;
        //value += cbRels[9].Checked ? 0b000000000100 : 0b0;
        //value += cbRels[10].Checked? 0b000000000010 : 0b0;
        //value += cbRels[11].Checked? 0b000000000001 : 0b0;