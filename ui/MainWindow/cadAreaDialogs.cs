namespace CEF;

// Input dialogs for area operations (style from dlgStyle)
internal static class cadAreaDialogs
{
    // Divisions along u (node1 -> node2) and v (node1 -> node4). Returns false if cancelled.
    public static bool askDivideArea(Form owner, out int nx, out int ny, int defaultDiv = 2)
    {
        const int margin = 16, rowH = 32, lblW = 200, tbW = 60, btnW = 90, btnH = 28;
        int width = margin * 2 + lblW + tbW;
        int btnY = margin + rowH * 2 + 4;

        using var f = new Form { Text = I18n.DivideAreasXY, ClientSize = new Size(width, btnY + btnH + margin) };
        dlgStyle.applyForm(f);

        var tbX = dlgStyle.editableField(f, I18n.AreaDivisionsX, margin, margin, lblW, tbW);
        var tbY = dlgStyle.editableField(f, I18n.AreaDivisionsY, margin, margin + rowH, lblW, tbW);
        tbX.Text = $"{defaultDiv}";
        tbY.Text = $"{defaultDiv}";

        var ok = dlgStyle.button(I18n.DlgOKText, width - margin - btnW * 2 - 8, btnY, btnW, true);
        var cancel = dlgStyle.button(I18n.DlgCancelText, width - margin - btnW, btnY, btnW);
        ok.DialogResult = DialogResult.OK;
        cancel.DialogResult = DialogResult.Cancel;
        f.AcceptButton = ok;
        f.CancelButton = cancel;
        f.Controls.Add(ok);
        f.Controls.Add(cancel);

        // Keep the dialog open while the input is invalid
        int vx = 1, vy = 1;
        f.FormClosing += (s, ev) =>
        {
            if (f.DialogResult != DialogResult.OK) return;
            if (int.TryParse(tbX.Text, out vx) && int.TryParse(tbY.Text, out vy) && vx >= 1 && vy >= 1) return;
            MessageBox.Show(I18n.CheckInput);
            ev.Cancel = true;
        };

        bool accepted = f.ShowDialog(owner) == DialogResult.OK;
        nx = vx;
        ny = vy;
        return accepted;
    }
}
