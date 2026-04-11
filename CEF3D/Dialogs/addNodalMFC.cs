using System;
using System.Text.RegularExpressions;
namespace CEF;
public  class addNodalMFC:Form
{

    #region UI

    private void InitializeComponent()
    {
        No = new Button();
        ok = new Button();
        label1 = new Label();
        TbListaNodos = new TextBox();
        RestType = new ComboBox();
        groupBox2 = new GroupBox();
        label2 = new Label();
        cbRestr = new ComboBox();
        label3 = new Label();
        BtDelete = new Button();
        btNewR = new Button();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // No
        // 
        No.DialogResult = DialogResult.Cancel;
        No.Location = new Point(341, 359);
        No.Margin = new Padding(4, 3, 4, 3);
        No.Size = new Size(88, 31);
        No.TabIndex = 7;
        No.Text = I18n.DlgCancelText;
        No.UseVisualStyleBackColor = true;
        No.Click += No_Click;
        // 
        // ok
        // 
        ok.Location = new Point(245, 359);
        ok.Margin = new Padding(4, 3, 4, 3);
        ok.Size = new Size(88, 31);
        ok.TabIndex = 6;
        ok.Text = I18n.DlgOKText;
        ok.UseVisualStyleBackColor = true;
        ok.Click += ok_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(9, 153);
        label1.Margin = new Padding(4, 0, 4, 0);
        label1.Size = new Size(42, 15);
        label1.TabIndex = 24;
        label1.Text = I18n.Nodes;
        // 
        // TbListaNodos
        // 
        TbListaNodos.Location = new Point(8, 171);
        TbListaNodos.Margin = new Padding(4, 3, 4, 3);
        TbListaNodos.Multiline = true;
        TbListaNodos.Size = new Size(364, 95);
        TbListaNodos.TabIndex = 23;
        // 
        // RestType
        // 
        RestType.FormattingEnabled = true;
        RestType.Location = new Point(9, 99);
        RestType.Margin = new Padding(4, 3, 4, 3);
        RestType.Size = new Size(237, 23);
        RestType.TabIndex = 19;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(label1);
        groupBox2.Controls.Add(label2);
        groupBox2.Controls.Add(TbListaNodos);
        groupBox2.Controls.Add(cbRestr);
        groupBox2.Controls.Add(label3);
        groupBox2.Controls.Add(RestType);
        groupBox2.Location = new Point(21, 49);
        groupBox2.Margin = new Padding(4, 3, 4, 3);
        groupBox2.Padding = new Padding(4, 3, 4, 3);
        groupBox2.Size = new Size(408, 295);
        groupBox2.TabIndex = 25;
        groupBox2.TabStop = false;
        groupBox2.Text = "";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(9, 24);
        label2.Margin = new Padding(4, 0, 4, 0);
        label2.Size = new Size(65, 15);
        label2.TabIndex = 22;
        label2.Text = I18n.Name;
        // 
        // cbRestr
        // 
        cbRestr.FormattingEnabled = true;
        cbRestr.Location = new Point(8, 42);
        cbRestr.Margin = new Padding(4, 3, 4, 3);
        cbRestr.Size = new Size(237, 23);
        cbRestr.TabIndex = 21;
        cbRestr.SelectedIndexChanged += cbRestr_SelectedIndexChanged;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(9, 80);
        label3.Margin = new Padding(4, 0, 4, 0);
        label3.Size = new Size(31, 15);
        label3.TabIndex = 20;
        label3.Text = I18n.Type;
        // 
        // BtDelete
        // 
        BtDelete.Location = new Point(40, 359);
        BtDelete.Margin = new Padding(4, 3, 4, 3);
        BtDelete.Size = new Size(88, 31);
        BtDelete.TabIndex = 26;
        BtDelete.Text = I18n.Delete;
        BtDelete.UseVisualStyleBackColor = true;
        BtDelete.Click += BtDelete_Click;
        // 
        // btNewR
        // 
        btNewR.Location = new Point(136, 359);
        btNewR.Margin = new Padding(4, 3, 4, 3);
        btNewR.Size = new Size(88, 31);
        btNewR.TabIndex = 27;
        btNewR.Text = I18n.New;
        btNewR.UseVisualStyleBackColor = true;
        btNewR.Click += btNewR_Click;
        // 
        // AsMFC
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(470, 415);
        Controls.Add(btNewR);
        Controls.Add(BtDelete);
        Controls.Add(groupBox2);
        Controls.Add(No);
        Controls.Add(ok);
        Margin = new Padding(4, 3, 4, 3);
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Text =I18n.KinematicConstraints;
        Load += AsMFC_Load;
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ResumeLayout(false);
    }
    internal Button No;
    internal Button ok;
    internal ComboBox RestType;
    internal Label Label6;
    internal TextBox TbListaNodos;
    internal Label label1;
    internal TextBox Mz;
    internal GroupBox groupBox2;
    internal Label label3;
    internal Label label2;
    internal ComboBox cbRestr;
    internal Button BtDelete;
    internal Button btNewR;
    #endregion

    Dictionary<MFCType, string> tipoMFC;
    public addNodalMFC()
    {
        InitializeComponent();
    }
    private void AsMFC_Load(object sender, EventArgs e)
    {
        dataLoad();
        int Nnudos = Selection.nodesMaxIndx;
        var Sel = Selection.getNodesColection();
        if(Sel!=null){
        var ids = Sel.Select(n => App.model.nodes[n].ID).ToArray();
            TbListaNodos.Text = string.Join(",", ids);//Allways , not localizable
        }
    }
    private void dataLoad()
    {
        tipoMFC = new Dictionary<MFCType, string>()
{
    { MFCType.diaphragm,   I18n.DiaphragmZ },
    { MFCType.diaphragmXX, I18n.DiaphragmXX },
    { MFCType.diaphragmYY, I18n.DiaphragmYY },
    { MFCType.plate,   I18n.PlateZ },
    { MFCType.plateXX, I18n.PlateXX },
    { MFCType.plateYY, I18n.PlateYY },
    { MFCType.body, I18n.Body },
    { MFCType.weld, I18n.Weld },
    { MFCType.beamX, I18n.BeamX },
    { MFCType.beamY, I18n.BeamY },
    { MFCType.beamZ, I18n.BeamZ},
    { MFCType.rodX, I18n.RodX },
    { MFCType.rodY,  I18n.RodY },
    { MFCType.rodZ,  I18n.RodZ },
    { MFCType.equal,  I18n.Equal }
};
        RestType.Items.Clear();
        cbRestr.Items.Clear();
        foreach (var it in tipoMFC)
            RestType.Items.Add(it.Value);
        RestType.SelectedIndex = 0;
        var Rest = App.model.nodalConstraints;
        foreach (var r in Rest)
            cbRestr.Items.Add(r.label);
        if (cbRestr.Items.Count>0) {
            cbRestr.SelectedIndex = 0;
            TbListaNodos.Text = Rest[cbRestr.SelectedIndex].nodesIDList;
        }
    }
    private void ok_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        var idx = cbRestr.SelectedIndex;
        if (idx == -1) return;
        var list = TbListaNodos.Text.Replace(@"\s+", ""); ;
        bool ok = Regex.IsMatch(list, @"^\d+(,\d+)*$");
        if (ok)
        {
            App.model.nodalConstraints[idx].nodesIDList = list;
            Close();  
        }
        else
        {
            MessageBox.Show(I18n.CheckInput);
        }
    }
    private void No_Click(object sender, EventArgs e)
    {
        Close();
    }
    private void BtDelete_Click(object sender, EventArgs e)
    {
        App.takeUndoSnapShot();

        var idx = cbRestr.SelectedIndex;
        App.model.nodalConstraints.RemoveAt(idx);
        cbRestr.Items.RemoveAt(idx);
        dataLoad();
    }
    private void btNewR_Click(object sender, EventArgs e)
    {
        var name = Mis.InputBox($"{I18n.New} {RestType.SelectedItem}", $"{ I18n.Name}: ");
        var i = App.model.nodalConstraints.Count > 0? App.model.nodalConstraints.Max(x => x.ID) + 1:0;
        var t = RestType.SelectedIndex;
        var r = new nodalConstraint() { ID = i, label = name, Type = t };
        App.model.nodalConstraints.Add(r);
        cbRestr.Items.Add(name);
        dataLoad();
    }
    private void cbRestr_SelectedIndexChanged(object sender, EventArgs e)
    {
        var r = cbRestr.SelectedIndex;
        var t = App.model.nodalConstraints[r].Type;
        RestType.SelectedIndex = t;
        TbListaNodos.Text = App.model.nodalConstraints[r].nodesIDList;

        RestType.Refresh();
    }
}