namespace FoodManager
{
    partial class ProgressForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblCount;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            Cancellation.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            progressBar = new ProgressBar();
            lblMessage = new Label();
            btnCancel = new Button();
            lblCount = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 34);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(560, 28);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 0;
            // 
            // lblMessage
            // 
            lblMessage.Location = new Point(12, 9);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(420, 22);
            lblMessage.TabIndex = 1;
            lblMessage.Text = "Початок...";
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(438, 68);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(134, 32);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Скасувати";
            btnCancel.Click += btnCancel_Click;
            // 
            // lblCount
            // 
            lblCount.Location = new Point(438, 9);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(134, 22);
            lblCount.TabIndex = 2;
            lblCount.TextAlign = ContentAlignment.TopRight;
            // 
            // ProgressForm
            // 
            ClientSize = new Size(584, 112);
            Controls.Add(progressBar);
            Controls.Add(lblMessage);
            Controls.Add(lblCount);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Актуалізація цін...";
            ResumeLayout(false);
        }
    }
}