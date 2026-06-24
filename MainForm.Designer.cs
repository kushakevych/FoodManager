// MainForm.Designer.cs
namespace FoodManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProducts;
        private System.Windows.Forms.TabPage tabRecipes;
        private System.Windows.Forms.TabPage tabInventory;

        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnEditProduct;
        private System.Windows.Forms.Button btnDeleteProduct;

        private System.Windows.Forms.DataGridView dgvRecipes;
        private System.Windows.Forms.Button btnAddRecipe;
        private System.Windows.Forms.Button btnEditRecipe;
        private System.Windows.Forms.Button btnDeleteRecipe;

        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.Button btnAddInventory;
        private System.Windows.Forms.Button btnReduceInventory;
        private System.Windows.Forms.Button btnGenerateShopping;

        private System.Windows.Forms.Button btnRecountAll;
        private System.Windows.Forms.Button btnRecountHistory;
        private System.Windows.Forms.Button btnWeeklyPlan;
        private System.Windows.Forms.Button btnUpdatePrices;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            tabControl1 = new TabControl();
            tabProducts = new TabPage();
            dgvProducts = new DataGridView();
            pIdCol = new DataGridViewTextBoxColumn();
            pNameCol = new DataGridViewTextBoxColumn();
            pWeightCol = new DataGridViewTextBoxColumn();
            pPricesCol = new DataGridViewTextBoxColumn();
            pBarcodesCol = new DataGridViewTextBoxColumn();
            pUnitCol = new DataGridViewTextBoxColumn();
            btnAddProduct = new Button();
            btnEditProduct = new Button();
            btnDeleteProduct = new Button();
            tabRecipes = new TabPage();
            dgvRecipes = new DataGridView();
            btnAddRecipe = new Button();
            btnEditRecipe = new Button();
            btnDeleteRecipe = new Button();
            btnWeeklyPlan = new Button();
            tabInventory = new TabPage();
            dgvInventory = new DataGridView();
            btnAddInventory = new Button();
            btnReduceInventory = new Button();
            btnGenerateShopping = new Button();
            btnRecountAll = new Button();
            btnRecountHistory = new Button();
            btnUpdatePrices = new Button();
            rIdCol = new DataGridViewTextBoxColumn();
            rNameCol = new DataGridViewTextBoxColumn();
            rStatusCol = new DataGridViewTextBoxColumn();
            rIngrCol = new DataGridViewTextBoxColumn();
            rKcalCol = new DataGridViewTextBoxColumn();
            rCostCol = new DataGridViewTextBoxColumn();
            iIdCol = new DataGridViewTextBoxColumn();
            iNameCol = new DataGridViewTextBoxColumn();
            iQtyCol = new DataGridViewTextBoxColumn();
            tabControl1.SuspendLayout();
            tabProducts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            tabRecipes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).BeginInit();
            tabInventory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabProducts);
            tabControl1.Controls.Add(tabRecipes);
            tabControl1.Controls.Add(tabInventory);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(980, 640);
            tabControl1.TabIndex = 0;
            // 
            // tabProducts
            // 
            tabProducts.Controls.Add(dgvProducts);
            tabProducts.Controls.Add(btnAddProduct);
            tabProducts.Controls.Add(btnEditProduct);
            tabProducts.Controls.Add(btnDeleteProduct);
            tabProducts.Location = new Point(4, 34);
            tabProducts.Name = "tabProducts";
            tabProducts.Size = new Size(972, 602);
            tabProducts.TabIndex = 0;
            tabProducts.Text = "Продукти";
            tabProducts.UseVisualStyleBackColor = true;
            // 
            // dgvProducts
            // 
            dgvProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Columns.AddRange(new DataGridViewColumn[] { pIdCol, pNameCol, pWeightCol, pPricesCol, pBarcodesCol, pUnitCol });
            dgvProducts.Location = new Point(10, 10);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.RowHeadersWidth = 62;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Size = new Size(950, 520);
            dgvProducts.TabIndex = 0;
            // 
            // pIdCol
            // 
            pIdCol.HeaderText = "ID";
            pIdCol.MinimumWidth = 8;
            pIdCol.Name = "pIdCol";
            pIdCol.ReadOnly = true;
            pIdCol.Visible = false;
            pIdCol.Width = 50;
            // 
            // pNameCol
            // 
            pNameCol.HeaderText = "Найменування";
            pNameCol.MinimumWidth = 8;
            pNameCol.Name = "pNameCol";
            pNameCol.ReadOnly = true;
            pNameCol.SortMode = DataGridViewColumnSortMode.Programmatic;
            pNameCol.Width = 220;
            // 
            // pWeightCol
            // 
            pWeightCol.HeaderText = "Вага г/шт";
            pWeightCol.MinimumWidth = 8;
            pWeightCol.Name = "pWeightCol";
            pWeightCol.ReadOnly = true;
            pWeightCol.Width = 150;
            // 
            // pPricesCol
            // 
            pPricesCol.HeaderText = "Ціни";
            pPricesCol.MinimumWidth = 8;
            pPricesCol.Name = "pPricesCol";
            pPricesCol.ReadOnly = true;
            pPricesCol.Width = 220;
            // 
            // pBarcodesCol
            // 
            pBarcodesCol.HeaderText = "Штрих-коди";
            pBarcodesCol.MinimumWidth = 8;
            pBarcodesCol.Name = "pBarcodesCol";
            pBarcodesCol.ReadOnly = true;
            pBarcodesCol.Visible = false;
            pBarcodesCol.Width = 200;
            // 
            // pUnitCol
            // 
            pUnitCol.HeaderText = "Одиниця";
            pUnitCol.MinimumWidth = 8;
            pUnitCol.Name = "pUnitCol";
            pUnitCol.ReadOnly = true;
            pUnitCol.Width = 150;
            // 
            // btnAddProduct
            // 
            btnAddProduct.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddProduct.Location = new Point(10, 540);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.Size = new Size(120, 40);
            btnAddProduct.TabIndex = 1;
            btnAddProduct.Text = "Додати";
            btnAddProduct.Click += btnAddProduct_Click;
            // 
            // btnEditProduct
            // 
            btnEditProduct.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEditProduct.Location = new Point(140, 540);
            btnEditProduct.Name = "btnEditProduct";
            btnEditProduct.Size = new Size(120, 40);
            btnEditProduct.TabIndex = 2;
            btnEditProduct.Text = "Редагувати";
            btnEditProduct.Click += btnEditProduct_Click;
            // 
            // btnDeleteProduct
            // 
            btnDeleteProduct.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteProduct.Location = new Point(270, 540);
            btnDeleteProduct.Name = "btnDeleteProduct";
            btnDeleteProduct.Size = new Size(120, 40);
            btnDeleteProduct.TabIndex = 3;
            btnDeleteProduct.Text = "Видалити";
            btnDeleteProduct.Click += btnDeleteProduct_Click;
            // 
            // tabRecipes
            // 
            tabRecipes.Controls.Add(dgvRecipes);
            tabRecipes.Controls.Add(btnAddRecipe);
            tabRecipes.Controls.Add(btnEditRecipe);
            tabRecipes.Controls.Add(btnDeleteRecipe);
            tabRecipes.Controls.Add(btnWeeklyPlan);
            tabRecipes.Location = new Point(4, 34);
            tabRecipes.Name = "tabRecipes";
            tabRecipes.Size = new Size(972, 602);
            tabRecipes.TabIndex = 1;
            tabRecipes.Text = "Страви";
            tabRecipes.UseVisualStyleBackColor = true;
            // 
            // dgvRecipes
            // 
            dgvRecipes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRecipes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecipes.Columns.AddRange(new DataGridViewColumn[] { rIdCol, rNameCol, rStatusCol, rIngrCol, rKcalCol, rCostCol });
            dgvRecipes.Location = new Point(10, 10);
            dgvRecipes.Name = "dgvRecipes";
            dgvRecipes.ReadOnly = true;
            dgvRecipes.RowHeadersVisible = false;
            dgvRecipes.RowHeadersWidth = 62;
            dgvRecipes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRecipes.Size = new Size(950, 520);
            dgvRecipes.TabIndex = 0;
            // 
            // btnAddRecipe
            // 
            btnAddRecipe.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddRecipe.Location = new Point(10, 540);
            btnAddRecipe.Name = "btnAddRecipe";
            btnAddRecipe.Size = new Size(140, 40);
            btnAddRecipe.TabIndex = 1;
            btnAddRecipe.Text = "Додати";
            btnAddRecipe.Click += btnAddRecipe_Click;
            // 
            // btnEditRecipe
            // 
            btnEditRecipe.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEditRecipe.Location = new Point(160, 540);
            btnEditRecipe.Name = "btnEditRecipe";
            btnEditRecipe.Size = new Size(140, 40);
            btnEditRecipe.TabIndex = 2;
            btnEditRecipe.Text = "Редагувати";
            btnEditRecipe.Click += btnEditRecipe_Click;
            // 
            // btnDeleteRecipe
            // 
            btnDeleteRecipe.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteRecipe.Location = new Point(310, 540);
            btnDeleteRecipe.Name = "btnDeleteRecipe";
            btnDeleteRecipe.Size = new Size(140, 40);
            btnDeleteRecipe.TabIndex = 3;
            btnDeleteRecipe.Text = "Видалити";
            btnDeleteRecipe.Click += btnDeleteRecipe_Click;
            // 
            // btnWeeklyPlan
            // 
            btnWeeklyPlan.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnWeeklyPlan.Location = new Point(710, 540);
            btnWeeklyPlan.Name = "btnWeeklyPlan";
            btnWeeklyPlan.Size = new Size(250, 40);
            btnWeeklyPlan.TabIndex = 4;
            btnWeeklyPlan.Text = "План на тиждень";
            btnWeeklyPlan.Click += btnWeeklyPlan_Click;
            // 
            // tabInventory
            // 
            tabInventory.Controls.Add(dgvInventory);
            tabInventory.Controls.Add(btnAddInventory);
            tabInventory.Controls.Add(btnReduceInventory);
            tabInventory.Controls.Add(btnGenerateShopping);
            tabInventory.Controls.Add(btnRecountAll);
            tabInventory.Controls.Add(btnRecountHistory);
            tabInventory.Controls.Add(btnUpdatePrices);
            tabInventory.Location = new Point(4, 34);
            tabInventory.Name = "tabInventory";
            tabInventory.Size = new Size(972, 602);
            tabInventory.TabIndex = 2;
            tabInventory.Text = "Облік";
            tabInventory.UseVisualStyleBackColor = true;
            // 
            // dgvInventory
            // 
            dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvInventory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventory.Columns.AddRange(new DataGridViewColumn[] { iIdCol, iNameCol, iQtyCol });
            dgvInventory.Location = new Point(10, 10);
            dgvInventory.Name = "dgvInventory";
            dgvInventory.ReadOnly = true;
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.RowHeadersWidth = 62;
            dgvInventory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInventory.Size = new Size(688, 584);
            dgvInventory.TabIndex = 0;
            // 
            // btnAddInventory
            // 
            btnAddInventory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddInventory.Location = new Point(714, 10);
            btnAddInventory.Name = "btnAddInventory";
            btnAddInventory.Size = new Size(250, 40);
            btnAddInventory.TabIndex = 1;
            btnAddInventory.Text = "Додати в облік";
            btnAddInventory.Click += btnAddInventory_Click;
            // 
            // btnReduceInventory
            // 
            btnReduceInventory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReduceInventory.Location = new Point(714, 56);
            btnReduceInventory.Name = "btnReduceInventory";
            btnReduceInventory.Size = new Size(250, 40);
            btnReduceInventory.TabIndex = 2;
            btnReduceInventory.Text = "Списати";
            btnReduceInventory.Click += btnReduceInventory_Click;
            // 
            // btnGenerateShopping
            // 
            btnGenerateShopping.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGenerateShopping.Location = new Point(714, 554);
            btnGenerateShopping.Name = "btnGenerateShopping";
            btnGenerateShopping.Size = new Size(250, 40);
            btnGenerateShopping.TabIndex = 3;
            btnGenerateShopping.Text = "Запланувати закупки";
            btnGenerateShopping.Click += btnGenerateShopping_Click;
            // 
            // btnRecountAll
            // 
            btnRecountAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRecountAll.Location = new Point(714, 121);
            btnRecountAll.Name = "btnRecountAll";
            btnRecountAll.Size = new Size(250, 40);
            btnRecountAll.TabIndex = 4;
            btnRecountAll.Text = "Переоблік";
            btnRecountAll.Click += btnRecountAll_Click;
            // 
            // btnRecountHistory
            // 
            btnRecountHistory.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRecountHistory.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            btnRecountHistory.Location = new Point(714, 167);
            btnRecountHistory.Name = "btnRecountHistory";
            btnRecountHistory.Size = new Size(250, 40);
            btnRecountHistory.TabIndex = 5;
            btnRecountHistory.Text = "Історія переобліків";
            btnRecountHistory.Click += btnRecountHistory_Click;
            // 
            // btnUpdatePrices
            // 
            btnUpdatePrices.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnUpdatePrices.Location = new Point(714, 508);
            btnUpdatePrices.Name = "btnUpdatePrices";
            btnUpdatePrices.Size = new Size(250, 40);
            btnUpdatePrices.TabIndex = 6;
            btnUpdatePrices.Text = "Актуалізувати ціни";
            btnUpdatePrices.Click += btnUpdatePrices_Click;
            // 
            // rIdCol
            // 
            rIdCol.HeaderText = "ID";
            rIdCol.MinimumWidth = 8;
            rIdCol.Name = "rIdCol";
            rIdCol.ReadOnly = true;
            rIdCol.SortMode = DataGridViewColumnSortMode.Programmatic;
            rIdCol.Visible = false;
            rIdCol.Width = 50;
            // 
            // rNameCol
            // 
            rNameCol.HeaderText = "Найменування";
            rNameCol.MinimumWidth = 8;
            rNameCol.Name = "rNameCol";
            rNameCol.ReadOnly = true;
            rNameCol.Width = 220;
            // 
            // rStatusCol
            // 
            rStatusCol.HeaderText = "Статус";
            rStatusCol.MinimumWidth = 8;
            rStatusCol.Name = "rStatusCol";
            rStatusCol.ReadOnly = true;
            rStatusCol.Width = 150;
            // 
            // rIngrCol
            // 
            rIngrCol.HeaderText = "Інгредієнти";
            rIngrCol.MinimumWidth = 8;
            rIngrCol.Name = "rIngrCol";
            rIngrCol.ReadOnly = true;
            rIngrCol.Visible = false;
            rIngrCol.Width = 220;
            // 
            // rKcalCol
            // 
            rKcalCol.HeaderText = "ккал";
            rKcalCol.MinimumWidth = 8;
            rKcalCol.Name = "rKcalCol";
            rKcalCol.ReadOnly = true;
            rKcalCol.Width = 120;
            // 
            // rCostCol
            // 
            rCostCol.HeaderText = "₴";
            rCostCol.MinimumWidth = 8;
            rCostCol.Name = "rCostCol";
            rCostCol.ReadOnly = true;
            rCostCol.Width = 120;
            // 
            // iIdCol
            // 
            iIdCol.HeaderText = "ID";
            iIdCol.MinimumWidth = 8;
            iIdCol.Name = "iIdCol";
            iIdCol.ReadOnly = true;
            iIdCol.Visible = false;
            iIdCol.Width = 50;
            // 
            // iNameCol
            // 
            iNameCol.HeaderText = "Найменування";
            iNameCol.MinimumWidth = 8;
            iNameCol.Name = "iNameCol";
            iNameCol.ReadOnly = true;
            iNameCol.SortMode = DataGridViewColumnSortMode.Programmatic;
            iNameCol.Width = 220;
            // 
            // iQtyCol
            // 
            iQtyCol.HeaderText = "Кількість";
            iQtyCol.MinimumWidth = 8;
            iQtyCol.Name = "iQtyCol";
            iQtyCol.ReadOnly = true;
            iQtyCol.Width = 150;
            // 
            // MainForm
            // 
            ClientSize = new Size(980, 640);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "Food Manager";
            tabControl1.ResumeLayout(false);
            tabProducts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            tabRecipes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).EndInit();
            tabInventory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvInventory).EndInit();
            ResumeLayout(false);
        }

        private DataGridViewTextBoxColumn pIdCol;
        private DataGridViewTextBoxColumn pNameCol;
        private DataGridViewTextBoxColumn pWeightCol;
        private DataGridViewTextBoxColumn pPricesCol;
        private DataGridViewTextBoxColumn pBarcodesCol;
        private DataGridViewTextBoxColumn pUnitCol;
        private DataGridViewTextBoxColumn rIdCol;
        private DataGridViewTextBoxColumn rNameCol;
        private DataGridViewTextBoxColumn rStatusCol;
        private DataGridViewTextBoxColumn rIngrCol;
        private DataGridViewTextBoxColumn rKcalCol;
        private DataGridViewTextBoxColumn rCostCol;
        private DataGridViewTextBoxColumn iIdCol;
        private DataGridViewTextBoxColumn iNameCol;
        private DataGridViewTextBoxColumn iQtyCol;
    }
}