using System.Text.RegularExpressions;
using static CEF.ModelDB;

namespace CEF;

    internal class Mis
    {

        public static double stringToDouble(in string text, in double defaultVal = 0d)
        {           
            if (double.TryParse(text, System.Globalization.CultureInfo.InvariantCulture,out double value))
            return value;

            return defaultVal;
        }


        public static double calcDist(in Node a,in Node b) =>
            Math. Sqrt(Math.Pow(b.X - a.X, 2d) + Math.Pow(b.Y - a.Y, 2d) + Math.Pow(b.Z - a.Z, 2d));

        

// Generic input box: one label + text box per prompt, long prompts wrap.
// Returns the answers in order, or null if cancelled/closed.
public static string[] InputBoxN(string title, string[] prompts, string[] defaults = null)
{
    if (prompts == null || prompts.Length == 0) return null;
 
    const int W = 340;
 
    using var f = new Form();
    dlgStyle.applyForm(f);
    f.ClientSize = new Size(W, 100);   // final height is set below
    f.Text = title;
 
    var boxes = new TextBox[prompts.Length];
    int y = 14;
 
    for (int i = 0; i < prompts.Length; i++)
    {
        var sz = TextRenderer.MeasureText(prompts[i], f.Font, new Size(W - 40, 0), TextFormatFlags.WordBreak);
        f.Controls.Add(new Label { Left = 20, Top = y, Width = W - 40, Height = sz.Height, Text = prompts[i] });
        y += sz.Height + 4;
 
        boxes[i] = new TextBox
        {
            Left = 20,
            Top = y,
            Width = W - 40,
            Text = defaults != null && i < defaults.Length ? defaults[i] : ""
        };
        f.Controls.Add(boxes[i]);
        y += boxes[i].Height + 12;
    }
 
    var ok = dlgStyle.button(I18n.DlgOKText, W - 185, y, 85, true);
    var cancel = dlgStyle.button(I18n.DlgCancelText, W - 95, y, 75);
    ok.DialogResult = DialogResult.OK;
    cancel.DialogResult = DialogResult.Cancel;
    f.Controls.AddRange(new Control[] { ok, cancel });
    f.AcceptButton = ok;
    f.CancelButton = cancel;
 
    f.ClientSize = new Size(W, y + 28 + 14);
    f.Shown += (_, _) => { boxes[0].Focus(); boxes[0].SelectAll(); };
 
    return f.ShowDialog(Form.ActiveForm) == DialogResult.OK
        ? boxes.Select(b => b.Text).ToArray()
        : null;
}
 
// Same behavior as before: no pre-filled text; defAnswer is returned if cancelled
public static string InputBox(string titulo, string mensaje, string defAnswer = "")
{
    var r = InputBoxN(titulo, [mensaje]);
    return r == null ? defAnswer : r[0];
}
 
// Same behavior as before: both boxes pre-filled with defAnswer; (defAnswer, defAnswer) if cancelled
public static (string Answer1, string Answer2) InputBox2(string titulo, string mensaje1, string mensaje2, string defAnswer = "")
{
    var r = InputBoxN(titulo, [mensaje1, mensaje2], [defAnswer, defAnswer]);
    return r == null ? (defAnswer, defAnswer) : (r[0], r[1]);
}
 
// For 3 or more fields call InputBoxN directly:
//   var a = Mis.InputBoxN(title, [msg1, msg2, msg3]);   // null if cancelled
//   if (a == null) return;  ->  a[0], a[1], a[2]
 
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
    

            private static readonly char[] intListSeparators = { ',', ';', ' ', '\t', '\r', '\n' };

            // Shared core. strict = true: stops at the first token that is not a non-negative integer.
            // strict = false: skips bad tokens. Duplicates are dropped (first occurrence wins); null/empty text gives an empty array.
            private static bool intListCore(string text, bool strict, out int[] ids, out string badToken)
            {
                var list = new List<int>();
                var seen = new HashSet<int>();
                badToken = "";

                if (!string.IsNullOrEmpty(text))
                {
                    foreach (var tok in text.Split(intListSeparators, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!int.TryParse(tok, out int v) || v < 0)
                        {
                            if (!strict) continue;

                            badToken = tok;
                            ids = Array.Empty<int>();
                            return false;
                        }
                        if (seen.Add(v)) list.Add(v);
                    }
                }

                ids = list.ToArray();
                return true;
            }

            // Strict parse for user input: false + first offending token if anything is not a valid ID
            public static bool tryParseIntList(string text, out int[] ids, out string badToken)
            {
                return intListCore(text, true, out ids, out badToken);
            }

            // Lenient parse for stored data / calculation: never null, bad tokens skipped
            public static int[] parseIntList(string text)
            {
                intListCore(text, false, out int[] ids, out _);
                return ids;
            }

            // Canonical text form ("30,100,255"); null gives ""
            public static string joinIntList(IEnumerable<int> ids)
            {
                return ids == null ? "" : string.Join(",", ids);
            }
    }

    
