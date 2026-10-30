using System;

namespace CEF;

public class setContournRestr : Form
{
    #region UI
    internal Button No;
    internal Button ok;
    internal GroupBox GroupBox1;
    internal GroupBox GroupBox3;
    internal GroupBox GroupBox2;
    internal CheckBox GiroY;
    internal CheckBox GiroZ;
    internal CheckBox GiroX;
    internal Label Label1;
    internal CheckBox DespY;
    internal CheckBox DespZ;
    internal CheckBox DespX;
    internal Button BttEmp;
    internal Button BttLib;
    internal Button BttBis;
    internal Button BttPatin;

    private void InitializeComponent()
    {
        No = dlgStyle.button(I18n.DlgCancelText, 0, 0, 88);
        ok = dlgStyle.button(I18n.DlgOKText, 0, 0, 88, true);

        GroupBox1 = dlgStyle.group(I18n.nodalRestraints, 112, 12, 278, 184);
        GroupBox3 = dlgStyle.group(I18n.Displacements, 16, 30, 116, 132);
        GroupBox2 = dlgStyle.group(I18n.Rotation, 143, 30, 116, 132);

        Label1 = new Label
        {
            Text = I18n.Restrain,
            AutoSize = true,
            BorderStyle = BorderStyle.FixedSingle,
            Left = 109,
            Top = 12
        };

        DespX = new CheckBox { Text = "X", AutoSize = true, Checked = true, Left = 34, Top = 25 };
        DespY = new CheckBox { Text = "Y", AutoSize = true, Checked = true, Left = 34, Top = 57 };
        DespZ = new CheckBox { Text = "Z", AutoSize = true, Checked = true, Left = 34, Top = 89 };

        GiroX = new CheckBox { Text = "X", AutoSize = true, Checked = true, Left = 38, Top = 25 };
        GiroY = new CheckBox { Text = "Y", AutoSize = true, Checked = true, Left = 38, Top = 57 };
        GiroZ = new CheckBox { Text = "Z", AutoSize = true, Checked = true, Left = 38, Top = 89 };

        BttEmp = dlgStyle.button(I18n.Fixed, 12, 40, 88);
        BttLib = dlgStyle.button(I18n.Free, 12, 80, 88);
        BttBis = dlgStyle.button(I18n.Hinge, 12, 120, 88);
        BttPatin = dlgStyle.button(I18n.Roller, 12, 160, 88);

        BttEmp.Click += BttEmp_Click;
        BttLib.Click += BttLib_Click;
        BttBis.Click += BttBis_Click;
        BttPatin.Click += BttPatin_Click;

        No.DialogResult = DialogResult.Cancel;
        No.Click += No_Click;
        ok.Click += ok_Click;

        GroupBox3.Controls.AddRange(new Control[] { DespX, DespY, DespZ });
        GroupBox2.Controls.AddRange(new Control[] { GiroX, GiroY, GiroZ });
        GroupBox1.Controls.AddRange(new Control[] { Label1, GroupBox3, GroupBox2 });

        var actionBar = dlgStyle.actionBar();
        actionBar.Dock = DockStyle.Bottom;
        actionBar.Height = 52;

        ok.Left = 194;
        ok.Top = 11;
        No.Left = 290;
        No.Top = 11;
        actionBar.Controls.AddRange(new Control[] { ok, No });

        Controls.AddRange(new Control[]
        {
            BttEmp, BttLib, BttBis, BttPatin, GroupBox1, actionBar
        });

        dlgStyle.applyForm(this);
        Text = I18n.BoundaryConditions;
        ClientSize = new Size(402, 258);
        AcceptButton = ok;
        CancelButton = No;
        ShowIcon = false;

    }

    #endregion

    public setContournRestr()
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

        int i;
        int Pos;
        int Nnudos = Selection.nodesMaxIndx;
        var Sel = Selection.getNodesColection();

        for (i = 0; i <= Nnudos; i++)
        {
            int id = App.model.nodes[Sel[i]].ID;
            Pos = App.model.nodalRestraints.FindIndexByID(id);//This restriction uses the same node ID
            if (Pos != -1)
            {
                App.model.nodalRestraints[Pos].ux = DespX.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].uy = DespY.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].uz = DespZ.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].rx = GiroX.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].ry = GiroY.Checked ? 1 : 0;
                App.model.nodalRestraints[Pos].rz = GiroZ.Checked ? 1 : 0;
            }
        }
        Close();
    }

    private void BttEmp_Click(object sender, EventArgs e)
    {
        DespX.Checked = true;
        DespY.Checked = true;
        DespZ.Checked = true;
        GiroX.Checked = true;
        GiroY.Checked = true;
        GiroZ.Checked = true;
    }

    private void BttLib_Click(object sender, EventArgs e)
    {
        DespX.Checked = false;
        DespY.Checked = false;
        DespZ.Checked = false;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }

    private void BttBis_Click(object sender, EventArgs e)
    {
        DespX.Checked = true;
        DespY.Checked = true;
        DespZ.Checked = true;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }

    private void BttPatin_Click(object sender, EventArgs e)
    {
        DespX.Checked = false;
        DespY.Checked = false;
        DespZ.Checked = true;
        GiroX.Checked = false;
        GiroY.Checked = false;
        GiroZ.Checked = false;
    }
}
