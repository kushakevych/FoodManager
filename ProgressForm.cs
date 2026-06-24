using System;
using System.Threading;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class ProgressForm : Form
    {
        public CancellationTokenSource Cancellation { get; } = new CancellationTokenSource();

        public ProgressForm()
        {
            InitializeComponent();
        }

        public void ReportProgress(int current, int total, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ReportProgress(current, total, message)));
                return;
            }
            if (total > 0)
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = total;
                progressBar.Value = Math.Min(Math.Max(0, current), total);
                lblCount.Text = $"{current} / {total}";
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                lblCount.Text = "";
            }
            lblMessage.Text = message ?? "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
            Cancellation.Cancel();
            lblMessage.Text = "Скасування...";
        }
    }
}