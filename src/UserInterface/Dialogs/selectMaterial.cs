using System;
using System.Drawing;
using System.Windows.Forms;

//TODO: Debug. why navigate fais when new material is adde an restarts the dlag solve it?

namespace CEF;

// Picks a material from App.model.materials. Use selectMaterial.Pick(...)
public class selectMaterial : Form
{
    private ListView lv;
    private Button btNew, btAccept, btCancel;

    public int SelectedID = -1;   // -1 = nothing selected

    // Returns the chosen material ID, or -1 if the dialog was cancelled
    public static int Pick(IWin32Window owner, int currentId = -1)
    {
        using var dlg = new selectMaterial(currentId);
        return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedID : -1;
    }

    public selectMaterial(int currentId = -1)
    {
        InitializeComponent();
        fillList(currentId);
    }

    private void InitializeComponent()
    {
        dlgStyle.applyForm(this, true);
        Text = I18n.SelectMaterial;
        ClientSize = new Size(580, 340);
        MinimumSize = Size;

        lv = new ListView
        {
            Left = 10,
            Top = 10,
            Width = 560,
            Height = 282,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            MultiSelect = false,
            HideSelection = false,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        lv.Columns.Add("ID", 50);
        lv.Columns.Add(I18n.Name, 170);
        lv.Columns.Add(I18n.Type, 90);
        lv.Columns.Add("E", 90);
        lv.Columns.Add("fc", 80);
        lv.Columns.Add("Fy", 80);
        lv.DoubleClick += (_, _) => Accept();

        btNew = dlgStyle.button(I18n.NewMaterial, 10, 302, 120);
        btNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btNew.Click += OnNewClick;

        btAccept = dlgStyle.button(I18n.DlgOKText, 380, 302, 90, true);
        btAccept.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btAccept.Click += (_, _) => Accept();

        btCancel = dlgStyle.button(I18n.DlgCancelText, 480, 302, 90);
        btCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btCancel.DialogResult = DialogResult.Cancel;

        AcceptButton = btAccept;
        CancelButton = btCancel;

        Controls.AddRange(new Control[] { lv, btNew, btAccept, btCancel });
    }

    private void fillList(int selectId)
    {
        lv.Items.Clear();

        for (int i = 0; i < App.model.materials.Count; i++)
        {
            var m = App.model.materials[i];

            var it = new ListViewItem(m.ID.ToString());
            it.SubItems.Add(m.Name);
            it.SubItems.Add(((materialType)m.Type).ToString());
            it.SubItems.Add(Uc.forcePerUnitAreaToUserUnits(m.E).ToString());
            it.SubItems.Add(Uc.forcePerUnitAreaToUserUnits(m.fc).ToString());
            it.SubItems.Add(Uc.forcePerUnitAreaToUserUnits(m.fy).ToString());
            it.Tag = m.ID;
            lv.Items.Add(it);

            if (m.ID == selectId)
            {
                it.Selected = true;
                it.EnsureVisible();
            }
        }
    }

    private void Accept()
    {
        if (lv.SelectedItems.Count == 0) return;

        SelectedID = (int)lv.SelectedItems[0].Tag;
        DialogResult = DialogResult.OK;
    }

    private void OnNewClick(object sender, EventArgs e)
    {
        int keep = lv.SelectedItems.Count > 0 ? (int)lv.SelectedItems[0].Tag : -1;

        using (var dlg = new addMaterialToModel())
            dlg.ShowDialog(this);

        fillList(keep);
    }
}