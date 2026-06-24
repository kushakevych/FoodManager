using System;
using System.Windows.Forms;

namespace FoodManager
{
    public static class InputBox
    {
        public static string Show(string prompt, string title)
        {
            using var form = new Form();
            form.Text = title;
            var lbl = new Label(){Text=prompt, Left=10, Top=10, Width=400};
            var txt = new TextBox(){Left=10, Top=40, Width=360};
            var ok = new Button(){Text="OK", Left=10, Top=80, DialogResult=DialogResult.OK};
            var cancel = new Button(){Text="Відмінити", Left=100, Top=80, DialogResult=DialogResult.Cancel};
            form.Controls.AddRange(new System.Windows.Forms.Control[]{lbl,txt,ok,cancel});
            form.AcceptButton = ok;
            form.CancelButton = cancel;
            form.ClientSize = new System.Drawing.Size(400,130);
            if (form.ShowDialog()==DialogResult.OK) return txt.Text;
            return string.Empty;
        }
    }
}
