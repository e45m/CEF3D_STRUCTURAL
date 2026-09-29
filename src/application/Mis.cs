using System.Text.RegularExpressions;
using static CEF.ModelDB;

namespace CEF
{
    internal class Mis
    {

        public static double stringToDouble(in string text, in double defaultVal = 0d)
        {           
            if (double.TryParse(text, out double value))
            return value;

            return defaultVal;
        }


        public static double calcDist(in node a,in node b) =>
            Math. Sqrt(Math.Pow(b.X - a.X, 2d) + Math.Pow(b.Y - a.Y, 2d) + Math.Pow(b.Z - a.Z, 2d));

        

        public static string InputBox(string titulo, string mensaje, string defAnswer = "")
        {
            Form f = new Form();
            f.Width = 300;
            f.Height = 140;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Text = titulo;
            Label lbl = new Label() { Left = 10, Top = 10, Text = mensaje, Width = 260 };
            TextBox txt = new TextBox() { Left = 10, Top = 35, Width = 260 };
            Button ok = new Button() { Text = I18n.DlgOKText, Left = 200, Width = 70, Top = 65, DialogResult = DialogResult.OK };
            f.Controls.Add(lbl);
            f.Controls.Add(txt);
            f.Controls.Add(ok);
            f.AcceptButton = ok;
            return f.ShowDialog() == DialogResult.OK ? txt.Text : defAnswer;
        }
        public static (string Answer1, string Answer2) InputBox2(string titulo, string mensaje1, string mensaje2, string defAnswer = "")
        {
            Form f = new Form();
            f.Width = 300;
            f.Height = 200;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Text = titulo;
            Label lbl = new Label() { Left = 10, Top = 10, Text = mensaje1, Width = 260 };
            TextBox txt = new TextBox() { Left = 10, Top = 35, Width = 260 };
            Label lbl2 = new Label() { Left = 10, Top = 60, Text = mensaje2, Width = 260 };
            TextBox txt2 = new TextBox() { Left = 10, Top = 105, Width = 260 };
            Button ok = new Button() { Text = I18n.DlgOKText, Left = 200, Width = 70, Top = 125, DialogResult = DialogResult.OK };
            txt.Text = txt2.Text = defAnswer;
            f.Controls.Add(lbl);
            f.Controls.Add(txt);
            f.Controls.Add(lbl2);
            f.Controls.Add(txt2);
            f.Controls.Add(ok);
            f.AcceptButton = ok;
            return f.ShowDialog() == DialogResult.OK ? (txt.Text, txt2.Text) : (defAnswer, defAnswer);
        }
        internal static  bool parceStringToIntDoubArray(in string  input, out int[] integers, out double[] doubles)
        {

            bool r = false;

            var matches = Regex.Matches(input, @"\{([^}]+)\}");
            integers = null;
            doubles = null;
            if (matches.Count >= 2)
            {
                 integers = matches[0].Groups[1].Value
                    .Split(',')
                    .Select(int.Parse)
                    .ToArray();

                 doubles = matches[1].Groups[1].Value
                    .Split(',')
                    .Select(s => double.Parse(s, System.Globalization.CultureInfo.InvariantCulture))
                    .ToArray();

                r = true;
            }


            return r;
        }
    }
}