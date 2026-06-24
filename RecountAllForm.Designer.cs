namespace FoodManager
{
    partial class RecountAllForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvRecount;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvRecount = new System.Windows.Forms.DataGridView();
            btnApply = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)dgvRecount).BeginInit();
            SuspendLayout();
            //
            // dgvRecount
            //
            dgvRecount.Location = new System.Drawing.Point(12, 12);
            dgvRecount.Size = new System.Drawing.Size(760, 420);
            dgvRecount.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRecount.RowHeadersVisible = false;
            dgvRecount.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvRecount.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Id", Visible = false },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Продукт", Width = 300 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Одиниця", Width = 80 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Поточний", Width = 120 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Фактичний (нова)", Width = 120 }
            });
            //
            // btnApply
            //
            btnApply.Location = new System.Drawing.Point(12, 444);
            btnApply.Size = new System.Drawing.Size(140, 36);
            btnApply.Text = "Застосувати переоблік";
            btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnCancel
            //
            btnCancel.Location = new System.Drawing.Point(168, 444);
            btnCancel.Size = new System.Drawing.Size(120, 36);
            btnCancel.Text = "Відмінити";
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // RecountAllForm
            //
            ClientSize = new System.Drawing.Size(784, 492);
            Controls.Add(dgvRecount);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
            Name = "RecountAllForm";
            Text = "Переоблік всього обліку";
            ((System.ComponentModel.ISupportInitialize)dgvRecount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}