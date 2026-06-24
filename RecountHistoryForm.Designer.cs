namespace FoodManager
{
    partial class RecountHistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvSessions;
        private System.Windows.Forms.DataGridView dgvItems;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvSessions = new System.Windows.Forms.DataGridView();
            dgvItems = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvSessions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvItems).BeginInit();
            SuspendLayout();
            //
            // dgvSessions
            //
            dgvSessions.Location = new System.Drawing.Point(12, 12);
            dgvSessions.Size = new System.Drawing.Size(520, 220);
            dgvSessions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvSessions.RowHeadersVisible = false;
            dgvSessions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvSessions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Id", Visible = false },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Дата/Час", Width = 180 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Примітка", Width = 300 }
            });
            dgvSessions.SelectionChanged += new System.EventHandler(this.dgvSessions_SelectionChanged);
            //
            // dgvItems
            //
            dgvItems.Location = new System.Drawing.Point(12, 240);
            dgvItems.Size = new System.Drawing.Size(760, 300);
            dgvItems.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvItems.RowHeadersVisible = false;
            dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "ProductId", Visible = false },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Продукт", Width = 300 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Було", Width = 120 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Стало", Width = 120 },
                new System.Windows.Forms.DataGridViewTextBoxColumn() { HeaderText = "Delta", Width = 80 }
            });
            //
            // RecountHistoryForm
            //
            ClientSize = new System.Drawing.Size(784, 552);
            Controls.Add(dgvSessions);
            Controls.Add(dgvItems);
            Name = "RecountHistoryForm";
            Text = "Історія переобліків (сесії)";
            ((System.ComponentModel.ISupportInitialize)dgvSessions).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvItems).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}