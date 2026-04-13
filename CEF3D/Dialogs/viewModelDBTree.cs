
using System.Reflection;

namespace CEF;
public class viewModelDBTree : Form
{
    private TreeView treeView1;
    private int currIndex = 0;
    public viewModelDBTree()
    {
        treeView1 = new TreeView();
        treeView1.Dock = DockStyle.Fill;
        treeView1.Location = new Point(0, 0);
        treeView1.Margin = new Padding(4, 3, 4, 3);
        treeView1.Size = new Size(350, 600);
        treeView1.TabIndex = 4;
        treeView1.AfterLabelEdit += tree_AfterLabelEdit;
        treeView1.NodeMouseDoubleClick += treeView1_NodeMouseDoubleClick;
        treeView1.KeyDown += VisorBD_tree_KeyDown;
        // 
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(350, 600);
        Controls.Add(treeView1);
        Margin = new Padding(4, 3, 4, 3);
        Text = I18n.TreeView;
        Load += VisorBD_Load;
        KeyDown += VisorBD_tree_KeyDown;


    }
    void LlenarTreeView(TreeView tree, object baseObj)
    {
        tree.Nodes.Clear();
        TreeNode root = new TreeNode(baseObj.GetType().Name);
        tree.Nodes.Add(root);
        foreach (FieldInfo dtaTableObj in baseObj.GetType().GetFields())
        {
            var tableName = dtaTableObj.Name;
            if (I18n.translateTableNames.ContainsKey(tableName))
                tableName = I18n.translateTableNames[tableName];
            TreeNode nodeTable = new TreeNode(tableName);
            root.Nodes.Add(nodeTable);
            var dtaTable = dtaTableObj.GetValue(baseObj);
            if (dtaTable != null)
            {
                var regsInTableCount = dtaTable.GetType().GetProperty("Count");
                var regsInTableObj = dtaTable.GetType().GetProperty("Items");
                if (regsInTableObj != null)
                {
                    var regsInTableData = regsInTableObj.GetValue(dtaTable) as IEnumerable<object>;
                    if (regsInTableData != null)
                    {
                        int jj = 0;
                        foreach (var rowObj in regsInTableData)
                        {
                            TreeNode nodeDtableRow = null;
                            bool flag = true;
                            foreach (FieldInfo dataCellObj in rowObj.GetType().GetFields())
                            {
                                if (flag)
                                {
                                    flag = false;
                                    var nodeName = $"{(dataCellObj.GetValue(rowObj)?.ToString() ?? "")}";
                                    nodeDtableRow = new TreeNode(nodeName);
                                    nodeTable.Nodes.Add(nodeDtableRow);
                                    nodeDtableRow.Tag = rowObj;
                                }
                                else
                                {
                                    object valor = dataCellObj.GetValue(rowObj)?.ToString() ?? "";


                                    if (!I18n.translateTableNames.TryGetValue(dataCellObj.Name ?? "", out string fName))
                                        fName = dataCellObj.Name ?? "";


                                   parseMetadata.getEnumFieldDisplayValue(typeof(loadCase), Enum.GetNames<LoadCaseType>(), "Type", dataCellObj, rowObj, ref valor);
                                   parseMetadata.getEnumFieldDisplayValue(typeof(section), Enum.GetNames<sectType>(), "Type", dataCellObj, rowObj, ref valor);
                                   parseMetadata.getEnumFieldDisplayValue(typeof(material), Enum.GetNames<materialType>(), "Type", dataCellObj, rowObj, ref valor);
                                   parseMetadata.getEnumFieldDisplayValue(typeof(nodalConstraint), Enum.GetNames<MFCType>(), "Type", dataCellObj, rowObj, ref valor);
                                   parseMetadata.useUserUnits(ref valor, dataCellObj);

                                    TreeNode nodoValor = new($"{fName}:{valor?.ToString() ?? "null"}");
                                    nodoValor.Tag = (rowObj, dataCellObj);
                                    nodeDtableRow.Nodes.Add(nodoValor);
                                }
                            }
                        }
                    }

                }
            }
        }
        root.Expand();
    }
    private void VisorBD_Load(object sender, System.EventArgs e)
    {
        LlenarTreeView(treeView1, App.model);
    }
    void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
    }
    private void VisorBD_tree_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F5)
        {
            LlenarTreeView(treeView1, App.model);
            this.Refresh();
        }

        if (e.KeyCode == Keys.W && e.Modifiers.HasFlag(Keys.Control)) Close();


    }
    private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
    {

        if (e.Node.Tag is ValueTuple<object, FieldInfo> tf)
        {

            var (obj, fi) = tf;
            if ("ID" == fi.Name) return;
            if (typeof(loadCase) == obj.GetType() && "Type" == fi.Name) return;//TODO: Function to use the string enum to user UI
            if (typeof(section) == obj.GetType() && "Type" == fi.Name) return;
            if (typeof(material) == obj.GetType() && "Type" == fi.Name) return;
            if (typeof(nodalConstraint) == obj.GetType() && "Type" == fi.Name) return;
            object newText = Mis.InputBox("Tree view", "Ingres el valor");
            if (newText.ToString() == "") return;
            parseMetadata.useDefautlDBUnits(ref newText, fi);
            object convertido = Convert.ChangeType(newText, fi.FieldType);
            fi.SetValue(obj, convertido);
            var st = e.Node.Text.Split(":");
            e.Node.Text = $"{st[0]}:{newText}";
        }
    }
}
    public static class parseMetadata 
    {
    public static bool getEnumFieldDisplayValue(Type tableType, string[] enumNames, string colName, FieldInfo row, object rowObject, ref object valor)
    {
        if (rowObject.GetType() != tableType) return false;

        if (row.Name == colName)
        {
            int? val = (int?)row.GetValue(rowObject);

            if (val != null)
            {
                valor = enumNames[(int)val];
                return true;
            }
        }

        return false;
    }

    public static bool useUserUnits(ref object valor, FieldInfo dataCellObj)
    {

        var v = valor;
        if (v == null) return false;
        var vc = v.ToString();

        if (Mdta.forceFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.forceToUserUnits(vc);
            return true;

        }
        if (Mdta.momentFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.momentToUserUnits(vc);
            return true;

        }
        if (Mdta.pressFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.pressToUserUnits(vc);
            return true;


        }
        if (Mdta.lenFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.lenToUserUnits(vc);
            return true;


        }
        if (Mdta.areaFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.areaToUserUnits(vc);
            return true;


        }
        if (Mdta.len3Fields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.len3ToUserUnits(vc);
            return true;

        }
        if (Mdta.len4Fields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.len4ToUserUnits(vc);
            return true;

        }

        return false;
    }

    public static bool useDefautlDBUnits(ref object valor, FieldInfo dataCellObj)
    {

        var v = valor;
        if (v == null) return false;
        var vc = v.ToString();

        if (Mdta.forceFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.forceFromUserUnits(vc);
            return true;

        }
        if (Mdta.momentFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.momentFromUserUnits(vc);
            return true;

        }
        if (Mdta.pressFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.pressFromUserUnits(vc);
            return true;


        }
        if (Mdta.lenFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.lenFromUserUnits(vc);
            return true;


        }
        if (Mdta.areaFields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.areaFromUserUnits(vc);
            return true;


        }
        if (Mdta.len3Fields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.len3FromUserUnits(vc);
            return true;

        }
        if (Mdta.len4Fields.Contains(dataCellObj.Name ?? ""))
        {
            valor = uc.len4FromUserUnits(vc);
            return true;

        }

        return false;
    }
}