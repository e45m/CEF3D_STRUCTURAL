using System.Reflection;

namespace CEF;

public class viewModelDBTree : Form
{
    const string dummyKey = "dummy"; // placeholder child so the expand arrow shows; children are built on expand

    class TableTag { public IEnumerable<object> Rows; }
    class RowTag { public object Row; }
    class FieldTag { public object Row; public FieldInfo Field; }

    internal TreeView treeView1;

    public viewModelDBTree()
    {
        InitializeComponent();
    }

    void InitializeComponent()
    {
        treeView1 = new TreeView { Dock = DockStyle.Fill, ShowLines = true, HideSelection = false, ItemHeight = 20 };
        treeView1.BeforeExpand += tree_BeforeExpand;
        treeView1.NodeMouseDoubleClick += tree_NodeMouseDoubleClick;

        FormBorderStyle = FormBorderStyle.Sizable;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(380, 640);
        MinimumSize = new Size(260, 300);
        KeyPreview = true;
        Text = I18n.TreeView;
        Controls.Add(treeView1);
        KeyDown += onKeyDown;
        Load += (s, e) => fillTree();
    }

    void fillTree()
    {
        treeView1.BeginUpdate();
        treeView1.Nodes.Clear();
        TreeNode root = new TreeNode(Engine.getModel().GetType().Name);
        treeView1.Nodes.Add(root);
        foreach (FieldInfo tbl in Engine.getModel().GetType().GetFields())
        {
            TreeNode nodeTable = new TreeNode(parseMetadata.tableText(tbl.Name));
            root.Nodes.Add(nodeTable);
            object table = tbl.GetValue(Engine.getModel());
            if (table == null) continue;
            PropertyInfo itemsProp = table.GetType().GetProperty("Items");
            if (itemsProp == null || itemsProp.GetValue(table) is not IEnumerable<object> rows) continue;
            int n = rows is System.Collections.ICollection ic ? ic.Count : rows.Count();
            nodeTable.Text = $"{nodeTable.Text} ({n})";
            if (n == 0) continue;
            nodeTable.Tag = new TableTag { Rows = rows };
            nodeTable.Nodes.Add(dummyKey, "");
        }
        root.Expand();
        treeView1.EndUpdate();
    }

    void tree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
    {
        TreeNode node = e.Node;
        if (node.Nodes.Count != 1 || node.Nodes[0].Name != dummyKey) return;
        treeView1.BeginUpdate();
        node.Nodes.Clear();
        if (node.Tag is TableTag tt) fillRows(node, tt);
        else if (node.Tag is RowTag rt) fillFields(node, rt.Row);
        treeView1.EndUpdate();
    }

    static void fillRows(TreeNode parent, TableTag tt)
    {
        foreach (object row in tt.Rows)
        {
            FieldInfo[] fields = parseMetadata.getFields(row.GetType());
            string text = fields.Length > 0 ? fields[0].GetValue(row)?.ToString() ?? "" : "";
            TreeNode n = new TreeNode(text) { Tag = new RowTag { Row = row } };
            n.Nodes.Add(dummyKey, "");
            parent.Nodes.Add(n);
        }
    }

    static void fillFields(TreeNode parent, object row)
    {
        FieldInfo[] fields = parseMetadata.getFields(row.GetType());
        for (int i = 1; i < fields.Length; i++)
        {
            FieldInfo f = fields[i];
            parent.Nodes.Add(new TreeNode(fieldText(row, f)) { Tag = new FieldTag { Row = row, Field = f } });
        }
    }

    static string fieldText(object row, FieldInfo f)
    {
        return $"{parseMetadata.headerText(f)}: {parseMetadata.formatCell(row, f)}";
    }

    void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node.Tag is not FieldTag ft) return;
        if (parseMetadata.isReadOnly(ft.Row.GetType(), ft.Field)) return;
        string text = Mis.InputBox(I18n.TreeView, I18n.TableEnterValue)?.ToString();
        if (string.IsNullOrEmpty(text)) return;
        if (!parseMetadata.tryParseCell(ft.Row, ft.Field, text))
        {
            MessageBox.Show(I18n.TableInvalidValue);
            return;
        }
        e.Node.Text = fieldText(ft.Row, ft.Field);
    }

    void onKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5)
        {
            fillTree();
            e.Handled = true;
        }
        else if (e.Control && e.KeyCode == Keys.W) Close();
    }
}