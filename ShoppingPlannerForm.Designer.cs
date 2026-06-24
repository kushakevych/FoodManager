namespace FoodManager
{
    partial class ShoppingPlannerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.DataGridView dgvRecipes;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExport;

        // new checkboxes
        private System.Windows.Forms.CheckBox chkConsiderInventoryForDirectBuys;
        private System.Windows.Forms.CheckBox chkRoundToPack;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvProducts = new DataGridView();
            dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            selectedStoreCol = new DataGridViewComboBoxColumn();
            dgvRecipes = new DataGridView();
            dataGridViewCheckBoxColumn2 = new DataGridViewCheckBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            btnCompute = new Button();
            txtResult = new TextBox();
            btnClose = new Button();
            btnExport = new Button();
            chkConsiderInventoryForDirectBuys = new CheckBox();
            chkRoundToPack = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).BeginInit();
            SuspendLayout();
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Columns.AddRange(new DataGridViewColumn[] { dataGridViewCheckBoxColumn1, dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, selectedStoreCol });
            dgvProducts.Location = new Point(12, 12);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.RowHeadersWidth = 62;
            dgvProducts.Size = new Size(1362, 503);
            dgvProducts.TabIndex = 0;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            dataGridViewCheckBoxColumn1.HeaderText = "Купити";
            dataGridViewCheckBoxColumn1.MinimumWidth = 20;
            dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            dataGridViewCheckBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Id";
            dataGridViewTextBoxColumn1.MinimumWidth = 8;
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.Visible = false;
            dataGridViewTextBoxColumn1.Width = 80;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Найменування";
            dataGridViewTextBoxColumn2.MinimumWidth = 8;
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 220;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "г/шт";
            dataGridViewTextBoxColumn3.MinimumWidth = 8;
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.Width = 80;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Кількість";
            dataGridViewTextBoxColumn4.MinimumWidth = 8;
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.Width = 150;
            // 
            // selectedStoreCol
            // 
            selectedStoreCol.FlatStyle = FlatStyle.Flat;
            selectedStoreCol.HeaderText = "Магазин (вибрати)";
            selectedStoreCol.MinimumWidth = 8;
            selectedStoreCol.Name = "selectedStoreCol";
            selectedStoreCol.Width = 160;
            // 
            // dgvRecipes
            // 
            dgvRecipes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecipes.Columns.AddRange(new DataGridViewColumn[] { dataGridViewCheckBoxColumn2, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7 });
            dgvRecipes.Location = new Point(12, 521);
            dgvRecipes.Name = "dgvRecipes";
            dgvRecipes.RowHeadersWidth = 62;
            dgvRecipes.Size = new Size(1362, 246);
            dgvRecipes.TabIndex = 1;
            // 
            // dataGridViewCheckBoxColumn2
            // 
            dataGridViewCheckBoxColumn2.HeaderText = "Використати";
            dataGridViewCheckBoxColumn2.MinimumWidth = 20;
            dataGridViewCheckBoxColumn2.Name = "dataGridViewCheckBoxColumn2";
            dataGridViewCheckBoxColumn2.Width = 150;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Id";
            dataGridViewTextBoxColumn5.MinimumWidth = 8;
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.Visible = false;
            dataGridViewTextBoxColumn5.Width = 150;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.HeaderText = "Страва";
            dataGridViewTextBoxColumn6.MinimumWidth = 8;
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.Width = 150;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.HeaderText = "Порцій (шт)";
            dataGridViewTextBoxColumn7.MinimumWidth = 8;
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.Width = 120;
            // 
            // btnCompute
            // 
            btnCompute.Location = new Point(1380, 12);
            btnCompute.Name = "btnCompute";
            btnCompute.Size = new Size(247, 40);
            btnCompute.TabIndex = 2;
            btnCompute.Text = "Розрахувати";
            btnCompute.Click += btnCompute_Click;
            // 
            // txtResult
            // 
            txtResult.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtResult.Location = new Point(1382, 131);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.Size = new Size(504, 594);
            txtResult.TabIndex = 4;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(1748, 731);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(140, 36);
            btnClose.TabIndex = 5;
            btnClose.Text = "Закрити";
            btnClose.Click += btnClose_Click;
            // 
            // btnExport
            // 
            btnExport.Location = new Point(1633, 12);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(253, 40);
            btnExport.TabIndex = 3;
            btnExport.Text = "Експорт XLSX";
            btnExport.Click += btnExport_Click;
            // 
            // chkConsiderInventoryForDirectBuys
            // 
            chkConsiderInventoryForDirectBuys.Location = new Point(1380, 62);
            chkConsiderInventoryForDirectBuys.Name = "chkConsiderInventoryForDirectBuys";
            chkConsiderInventoryForDirectBuys.Size = new Size(488, 35);
            chkConsiderInventoryForDirectBuys.TabIndex = 5;
            chkConsiderInventoryForDirectBuys.Text = "Використовувати наявний облік для ручних покупок";
            // 
            // chkRoundToPack
            // 
            chkRoundToPack.Location = new Point(1380, 93);
            chkRoundToPack.Name = "chkRoundToPack";
            chkRoundToPack.Size = new Size(473, 32);
            chkRoundToPack.TabIndex = 6;
            chkRoundToPack.Text = "Округляти до фасовки при експорті";
            // 
            // ShoppingPlannerForm
            // 
            ClientSize = new Size(1900, 779);
            Controls.Add(chkRoundToPack);
            Controls.Add(chkConsiderInventoryForDirectBuys);
            Controls.Add(dgvProducts);
            Controls.Add(dgvRecipes);
            Controls.Add(btnCompute);
            Controls.Add(btnExport);
            Controls.Add(txtResult);
            Controls.Add(btnClose);
            Name = "ShoppingPlannerForm";
            Text = "Планувальник закупок";
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewComboBoxColumn selectedStoreCol;
    }
}