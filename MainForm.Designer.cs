namespace FoodManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Top Banner & Stats
        private System.Windows.Forms.Panel panelTopBanner;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Label lblStatProducts;
        private System.Windows.Forms.Label lblStatRecipes;
        private System.Windows.Forms.Label lblStatInventory;
        private System.Windows.Forms.Label lblStatLowStock;

        // Main Tab Control
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabProducts;
        private System.Windows.Forms.TabPage tabRecipes;
        private System.Windows.Forms.TabPage tabInventory;

        // Products Tab
        private System.Windows.Forms.Panel panelProductFilter;
        private System.Windows.Forms.Label lblSearchProduct;
        private System.Windows.Forms.TextBox txtSearchProduct;
        private System.Windows.Forms.Button btnClearProductSearch;
        private System.Windows.Forms.Button btnMicronutrients;
        private System.Windows.Forms.Button btnBackupDb;
        private System.Windows.Forms.Button btnRestoreDb;
        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.DataGridViewTextBoxColumn pIdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pWeightCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pPricesCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pBarcodesCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pUnitCol;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnEditProduct;
        private System.Windows.Forms.Button btnDeleteProduct;
        private System.Windows.Forms.Button btnUpdatePrices;

        // Recipes Tab
        private System.Windows.Forms.Panel panelRecipeFilter;
        private System.Windows.Forms.Label lblSearchRecipe;
        private System.Windows.Forms.TextBox txtSearchRecipe;
        private System.Windows.Forms.Label lblFilterCategory;
        private System.Windows.Forms.ComboBox cmbFilterRecipeCategory;
        private System.Windows.Forms.Button btnRecipeMicronutrients;
        private System.Windows.Forms.DataGridView dgvRecipes;
        private System.Windows.Forms.DataGridViewTextBoxColumn rIdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn rNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn rStatusCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn rIngrCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn rKcalCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn rCostCol;
        private System.Windows.Forms.Button btnAddRecipe;
        private System.Windows.Forms.Button btnEditRecipe;
        private System.Windows.Forms.Button btnDeleteRecipe;
        private System.Windows.Forms.Button btnWeeklyPlan;

        // Inventory Tab
        private System.Windows.Forms.Panel panelInventoryFilter;
        private System.Windows.Forms.Label lblSearchInventory;
        private System.Windows.Forms.TextBox txtSearchInventory;
        private System.Windows.Forms.CheckBox chkOnlyLowStock;
        private System.Windows.Forms.Button btnRestockLow;
        private System.Windows.Forms.Button btnQuickTelegram;
        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.DataGridViewTextBoxColumn iIdCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn iNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn iQtyCol;
        private System.Windows.Forms.Button btnAddInventory;
        private System.Windows.Forms.Button btnReduceInventory;
        private System.Windows.Forms.Button btnGenerateShopping;
        private System.Windows.Forms.Button btnRecountAll;
        private System.Windows.Forms.Button btnRecountHistory;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dgvStyle = new System.Windows.Forms.DataGridViewCellStyle();
            panelTopBanner = new System.Windows.Forms.Panel();
            lblAppTitle = new System.Windows.Forms.Label();
            lblStatProducts = new System.Windows.Forms.Label();
            lblStatRecipes = new System.Windows.Forms.Label();
            lblStatInventory = new System.Windows.Forms.Label();
            lblStatLowStock = new System.Windows.Forms.Label();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabProducts = new System.Windows.Forms.TabPage();
            panelProductFilter = new System.Windows.Forms.Panel();
            lblSearchProduct = new System.Windows.Forms.Label();
            txtSearchProduct = new System.Windows.Forms.TextBox();
            btnClearProductSearch = new System.Windows.Forms.Button();
            btnMicronutrients = new System.Windows.Forms.Button();
            btnBackupDb = new System.Windows.Forms.Button();
            btnRestoreDb = new System.Windows.Forms.Button();
            dgvProducts = new System.Windows.Forms.DataGridView();
            pIdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pWeightCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pPricesCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pBarcodesCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            pUnitCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnAddProduct = new System.Windows.Forms.Button();
            btnEditProduct = new System.Windows.Forms.Button();
            btnDeleteProduct = new System.Windows.Forms.Button();
            btnUpdatePrices = new System.Windows.Forms.Button();
            tabRecipes = new System.Windows.Forms.TabPage();
            panelRecipeFilter = new System.Windows.Forms.Panel();
            lblSearchRecipe = new System.Windows.Forms.Label();
            txtSearchRecipe = new System.Windows.Forms.TextBox();
            lblFilterCategory = new System.Windows.Forms.Label();
            cmbFilterRecipeCategory = new System.Windows.Forms.ComboBox();
            btnRecipeMicronutrients = new System.Windows.Forms.Button();
            dgvRecipes = new System.Windows.Forms.DataGridView();
            rIdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            rNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            rStatusCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            rIngrCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            rKcalCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            rCostCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnAddRecipe = new System.Windows.Forms.Button();
            btnEditRecipe = new System.Windows.Forms.Button();
            btnDeleteRecipe = new System.Windows.Forms.Button();
            btnWeeklyPlan = new System.Windows.Forms.Button();
            tabInventory = new System.Windows.Forms.TabPage();
            panelInventoryFilter = new System.Windows.Forms.Panel();
            lblSearchInventory = new System.Windows.Forms.Label();
            txtSearchInventory = new System.Windows.Forms.TextBox();
            chkOnlyLowStock = new System.Windows.Forms.CheckBox();
            btnRestockLow = new System.Windows.Forms.Button();
            btnQuickTelegram = new System.Windows.Forms.Button();
            dgvInventory = new System.Windows.Forms.DataGridView();
            iIdCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            iNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            iQtyCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnAddInventory = new System.Windows.Forms.Button();
            btnReduceInventory = new System.Windows.Forms.Button();
            btnGenerateShopping = new System.Windows.Forms.Button();
            btnRecountAll = new System.Windows.Forms.Button();
            btnRecountHistory = new System.Windows.Forms.Button();
            panelTopBanner.SuspendLayout();
            tabControl1.SuspendLayout();
            tabProducts.SuspendLayout();
            panelProductFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            tabRecipes.SuspendLayout();
            panelRecipeFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).BeginInit();
            tabInventory.SuspendLayout();
            panelInventoryFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).BeginInit();
            SuspendLayout();
            // 
            // panelTopBanner
            // 
            panelTopBanner.BackColor = System.Drawing.Color.FromArgb(243, 246, 250);
            panelTopBanner.Controls.Add(lblAppTitle);
            panelTopBanner.Controls.Add(lblStatProducts);
            panelTopBanner.Controls.Add(lblStatRecipes);
            panelTopBanner.Controls.Add(lblStatInventory);
            panelTopBanner.Controls.Add(lblStatLowStock);
            panelTopBanner.Dock = System.Windows.Forms.DockStyle.Top;
            panelTopBanner.Location = new System.Drawing.Point(0, 0);
            panelTopBanner.Name = "panelTopBanner";
            panelTopBanner.Size = new System.Drawing.Size(1080, 56);
            panelTopBanner.TabIndex = 0;
            // 
            // lblAppTitle
            // 
            lblAppTitle.AutoSize = true;
            lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblAppTitle.ForeColor = System.Drawing.Color.FromArgb(24, 43, 73);
            lblAppTitle.Location = new System.Drawing.Point(12, 14);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new System.Drawing.Size(195, 28);
            lblAppTitle.TabIndex = 0;
            lblAppTitle.Text = "🥥 Food Manager";
            // 
            // lblStatProducts
            // 
            lblStatProducts.AutoSize = true;
            lblStatProducts.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatProducts.ForeColor = System.Drawing.Color.FromArgb(50, 70, 90);
            lblStatProducts.Location = new System.Drawing.Point(235, 19);
            lblStatProducts.Name = "lblStatProducts";
            lblStatProducts.Size = new System.Drawing.Size(112, 20);
            lblStatProducts.TabIndex = 1;
            lblStatProducts.Text = "🍏 Продуктів: 0";
            // 
            // lblStatRecipes
            // 
            lblStatRecipes.AutoSize = true;
            lblStatRecipes.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatRecipes.ForeColor = System.Drawing.Color.FromArgb(50, 70, 90);
            lblStatRecipes.Location = new System.Drawing.Point(380, 19);
            lblStatRecipes.Name = "lblStatRecipes";
            lblStatRecipes.Size = new System.Drawing.Size(81, 20);
            lblStatRecipes.TabIndex = 2;
            lblStatRecipes.Text = "🍲 Страв: 0";
            // 
            // lblStatInventory
            // 
            lblStatInventory.AutoSize = true;
            lblStatInventory.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatInventory.ForeColor = System.Drawing.Color.FromArgb(50, 70, 90);
            lblStatInventory.Location = new System.Drawing.Point(500, 19);
            lblStatInventory.Name = "lblStatInventory";
            lblStatInventory.Size = new System.Drawing.Size(100, 20);
            lblStatInventory.TabIndex = 3;
            lblStatInventory.Text = "📦 На складі: 0";
            // 
            // lblStatLowStock
            // 
            lblStatLowStock.AutoSize = true;
            lblStatLowStock.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblStatLowStock.ForeColor = System.Drawing.Color.FromArgb(180, 40, 40);
            lblStatLowStock.Location = new System.Drawing.Point(650, 19);
            lblStatLowStock.Name = "lblStatLowStock";
            lblStatLowStock.Size = new System.Drawing.Size(130, 20);
            lblStatLowStock.TabIndex = 4;
            lblStatLowStock.Text = "⚠️ Закінчуються: 0";
            // 
            // tabControl1
            // 
            tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl1.Controls.Add(tabProducts);
            tabControl1.Controls.Add(tabRecipes);
            tabControl1.Controls.Add(tabInventory);
            tabControl1.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tabControl1.Location = new System.Drawing.Point(0, 58);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1080, 602);
            tabControl1.TabIndex = 1;
            // 
            // tabProducts
            // 
            tabProducts.Controls.Add(panelProductFilter);
            tabProducts.Controls.Add(dgvProducts);
            tabProducts.Controls.Add(btnAddProduct);
            tabProducts.Controls.Add(btnEditProduct);
            tabProducts.Controls.Add(btnDeleteProduct);
            tabProducts.Controls.Add(btnUpdatePrices);
            tabProducts.Location = new System.Drawing.Point(4, 30);
            tabProducts.Name = "tabProducts";
            tabProducts.Size = new System.Drawing.Size(1072, 568);
            tabProducts.TabIndex = 0;
            tabProducts.Text = "Продукти";
            tabProducts.UseVisualStyleBackColor = true;
            // 
            // panelProductFilter
            // 
            panelProductFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelProductFilter.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            panelProductFilter.Controls.Add(lblSearchProduct);
            panelProductFilter.Controls.Add(txtSearchProduct);
            panelProductFilter.Controls.Add(btnClearProductSearch);
            panelProductFilter.Controls.Add(btnMicronutrients);
            panelProductFilter.Controls.Add(btnBackupDb);
            panelProductFilter.Controls.Add(btnRestoreDb);
            panelProductFilter.Location = new System.Drawing.Point(8, 6);
            panelProductFilter.Name = "panelProductFilter";
            panelProductFilter.Size = new System.Drawing.Size(1056, 44);
            panelProductFilter.TabIndex = 0;
            // 
            // lblSearchProduct
            // 
            lblSearchProduct.AutoSize = true;
            lblSearchProduct.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSearchProduct.Location = new System.Drawing.Point(8, 12);
            lblSearchProduct.Name = "lblSearchProduct";
            lblSearchProduct.Size = new System.Drawing.Size(74, 20);
            lblSearchProduct.TabIndex = 0;
            lblSearchProduct.Text = "🔍 Пошук:";
            // 
            // txtSearchProduct
            // 
            txtSearchProduct.Location = new System.Drawing.Point(88, 8);
            txtSearchProduct.Name = "txtSearchProduct";
            txtSearchProduct.PlaceholderText = "Введіть назву або штрих-код...";
            txtSearchProduct.Size = new System.Drawing.Size(260, 29);
            txtSearchProduct.TabIndex = 1;
            // 
            // btnClearProductSearch
            // 
            btnClearProductSearch.Location = new System.Drawing.Point(354, 8);
            btnClearProductSearch.Name = "btnClearProductSearch";
            btnClearProductSearch.Size = new System.Drawing.Size(32, 29);
            btnClearProductSearch.TabIndex = 2;
            btnClearProductSearch.Text = "✕";
            btnClearProductSearch.UseVisualStyleBackColor = true;
            // 
            // btnMicronutrients
            // 
            btnMicronutrients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnMicronutrients.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            btnMicronutrients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnMicronutrients.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnMicronutrients.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            btnMicronutrients.Location = new System.Drawing.Point(590, 6);
            btnMicronutrients.Name = "btnMicronutrients";
            btnMicronutrients.Size = new System.Drawing.Size(230, 32);
            btnMicronutrients.TabIndex = 3;
            btnMicronutrients.Text = "🥗 Вітаміни та мікроелементи";
            btnMicronutrients.UseVisualStyleBackColor = false;
            // 
            // btnBackupDb
            // 
            btnBackupDb.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnBackupDb.Location = new System.Drawing.Point(828, 6);
            btnBackupDb.Name = "btnBackupDb";
            btnBackupDb.Size = new System.Drawing.Size(108, 32);
            btnBackupDb.TabIndex = 4;
            btnBackupDb.Text = "💾 Бекап БД";
            btnBackupDb.UseVisualStyleBackColor = true;
            // 
            // btnRestoreDb
            // 
            btnRestoreDb.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRestoreDb.Location = new System.Drawing.Point(942, 6);
            btnRestoreDb.Name = "btnRestoreDb";
            btnRestoreDb.Size = new System.Drawing.Size(108, 32);
            btnRestoreDb.TabIndex = 5;
            btnRestoreDb.Text = "📂 Відновити";
            btnRestoreDb.UseVisualStyleBackColor = true;
            // 
            // dgvProducts
            // 
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvProducts.BackgroundColor = System.Drawing.Color.White;
            dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProducts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { pIdCol, pNameCol, pWeightCol, pPricesCol, pBarcodesCol, pUnitCol });
            dgvProducts.Location = new System.Drawing.Point(8, 56);
            dgvProducts.Name = "dgvProducts";
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.RowHeadersWidth = 50;
            dgvProducts.RowTemplate.Height = 28;
            dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.Size = new System.Drawing.Size(1056, 452);
            dgvProducts.TabIndex = 1;
            // 
            // pIdCol
            // 
            pIdCol.HeaderText = "ID";
            pIdCol.Name = "pIdCol";
            pIdCol.ReadOnly = true;
            pIdCol.Visible = false;
            pIdCol.Width = 50;
            // 
            // pNameCol
            // 
            pNameCol.HeaderText = "Найменування";
            pNameCol.Name = "pNameCol";
            pNameCol.ReadOnly = true;
            pNameCol.Width = 260;
            // 
            // pWeightCol
            // 
            pWeightCol.HeaderText = "Фасовка (г/шт)";
            pWeightCol.Name = "pWeightCol";
            pWeightCol.ReadOnly = true;
            pWeightCol.Width = 140;
            // 
            // pPricesCol
            // 
            pPricesCol.HeaderText = "Ціни у магазинах";
            pPricesCol.Name = "pPricesCol";
            pPricesCol.ReadOnly = true;
            pPricesCol.Width = 400;
            // 
            // pBarcodesCol
            // 
            pBarcodesCol.HeaderText = "Штрих-коди";
            pBarcodesCol.Name = "pBarcodesCol";
            pBarcodesCol.ReadOnly = true;
            pBarcodesCol.Visible = false;
            pBarcodesCol.Width = 120;
            // 
            // pUnitCol
            // 
            pUnitCol.HeaderText = "Одиниця";
            pUnitCol.Name = "pUnitCol";
            pUnitCol.ReadOnly = true;
            pUnitCol.Width = 100;
            // 
            // btnAddProduct
            // 
            btnAddProduct.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAddProduct.BackColor = System.Drawing.Color.FromArgb(240, 244, 248);
            btnAddProduct.Location = new System.Drawing.Point(8, 518);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.Size = new System.Drawing.Size(130, 38);
            btnAddProduct.TabIndex = 2;
            btnAddProduct.Text = "➕ Додати";
            btnAddProduct.UseVisualStyleBackColor = true;
            // 
            // btnEditProduct
            // 
            btnEditProduct.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnEditProduct.Location = new System.Drawing.Point(146, 518);
            btnEditProduct.Name = "btnEditProduct";
            btnEditProduct.Size = new System.Drawing.Size(130, 38);
            btnEditProduct.TabIndex = 3;
            btnEditProduct.Text = "✏️ Редагувати";
            btnEditProduct.UseVisualStyleBackColor = true;
            // 
            // btnDeleteProduct
            // 
            btnDeleteProduct.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnDeleteProduct.Location = new System.Drawing.Point(284, 518);
            btnDeleteProduct.Name = "btnDeleteProduct";
            btnDeleteProduct.Size = new System.Drawing.Size(130, 38);
            btnDeleteProduct.TabIndex = 4;
            btnDeleteProduct.Text = "🗑️ Видалити";
            btnDeleteProduct.UseVisualStyleBackColor = true;
            // 
            // btnUpdatePrices
            // 
            btnUpdatePrices.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnUpdatePrices.BackColor = System.Drawing.Color.FromArgb(227, 242, 253);
            btnUpdatePrices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnUpdatePrices.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnUpdatePrices.ForeColor = System.Drawing.Color.FromArgb(13, 71, 161);
            btnUpdatePrices.Location = new System.Drawing.Point(842, 518);
            btnUpdatePrices.Name = "btnUpdatePrices";
            btnUpdatePrices.Size = new System.Drawing.Size(222, 38);
            btnUpdatePrices.TabIndex = 5;
            btnUpdatePrices.Text = "🔄 Оновити ціни";
            btnUpdatePrices.UseVisualStyleBackColor = false;
            // 
            // tabRecipes
            // 
            tabRecipes.Controls.Add(panelRecipeFilter);
            tabRecipes.Controls.Add(dgvRecipes);
            tabRecipes.Controls.Add(btnAddRecipe);
            tabRecipes.Controls.Add(btnEditRecipe);
            tabRecipes.Controls.Add(btnDeleteRecipe);
            tabRecipes.Controls.Add(btnWeeklyPlan);
            tabRecipes.Location = new System.Drawing.Point(4, 30);
            tabRecipes.Name = "tabRecipes";
            tabRecipes.Size = new System.Drawing.Size(1072, 568);
            tabRecipes.TabIndex = 1;
            tabRecipes.Text = "Страви";
            tabRecipes.UseVisualStyleBackColor = true;
            // 
            // panelRecipeFilter
            // 
            panelRecipeFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelRecipeFilter.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            panelRecipeFilter.Controls.Add(lblSearchRecipe);
            panelRecipeFilter.Controls.Add(txtSearchRecipe);
            panelRecipeFilter.Controls.Add(lblFilterCategory);
            panelRecipeFilter.Controls.Add(cmbFilterRecipeCategory);
            panelRecipeFilter.Controls.Add(btnRecipeMicronutrients);
            panelRecipeFilter.Location = new System.Drawing.Point(8, 6);
            panelRecipeFilter.Name = "panelRecipeFilter";
            panelRecipeFilter.Size = new System.Drawing.Size(1056, 44);
            panelRecipeFilter.TabIndex = 0;
            // 
            // lblSearchRecipe
            // 
            lblSearchRecipe.AutoSize = true;
            lblSearchRecipe.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSearchRecipe.Location = new System.Drawing.Point(8, 12);
            lblSearchRecipe.Name = "lblSearchRecipe";
            lblSearchRecipe.Size = new System.Drawing.Size(74, 20);
            lblSearchRecipe.TabIndex = 0;
            lblSearchRecipe.Text = "🔍 Пошук:";
            // 
            // txtSearchRecipe
            // 
            txtSearchRecipe.Location = new System.Drawing.Point(88, 8);
            txtSearchRecipe.Name = "txtSearchRecipe";
            txtSearchRecipe.PlaceholderText = "Введіть назву страви...";
            txtSearchRecipe.Size = new System.Drawing.Size(260, 29);
            txtSearchRecipe.TabIndex = 1;
            // 
            // lblFilterCategory
            // 
            lblFilterCategory.AutoSize = true;
            lblFilterCategory.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblFilterCategory.Location = new System.Drawing.Point(365, 12);
            lblFilterCategory.Name = "lblFilterCategory";
            lblFilterCategory.Size = new System.Drawing.Size(81, 20);
            lblFilterCategory.TabIndex = 2;
            lblFilterCategory.Text = "Категорія:";
            // 
            // cmbFilterRecipeCategory
            // 
            cmbFilterRecipeCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbFilterRecipeCategory.FormattingEnabled = true;
            cmbFilterRecipeCategory.Location = new System.Drawing.Point(452, 8);
            cmbFilterRecipeCategory.Name = "cmbFilterRecipeCategory";
            cmbFilterRecipeCategory.Size = new System.Drawing.Size(180, 29);
            cmbFilterRecipeCategory.TabIndex = 3;
            // 
            // btnRecipeMicronutrients
            // 
            btnRecipeMicronutrients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRecipeMicronutrients.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            btnRecipeMicronutrients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRecipeMicronutrients.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnRecipeMicronutrients.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            btnRecipeMicronutrients.Location = new System.Drawing.Point(820, 6);
            btnRecipeMicronutrients.Name = "btnRecipeMicronutrients";
            btnRecipeMicronutrients.Size = new System.Drawing.Size(230, 32);
            btnRecipeMicronutrients.TabIndex = 4;
            btnRecipeMicronutrients.Text = "🥗 Аналіз мікроелементів";
            btnRecipeMicronutrients.UseVisualStyleBackColor = false;
            // 
            // dgvRecipes
            // 
            dgvRecipes.AllowUserToAddRows = false;
            dgvRecipes.AllowUserToDeleteRows = false;
            dgvRecipes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvRecipes.BackgroundColor = System.Drawing.Color.White;
            dgvRecipes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecipes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { rIdCol, rNameCol, rStatusCol, rIngrCol, rKcalCol, rCostCol });
            dgvRecipes.Location = new System.Drawing.Point(8, 56);
            dgvRecipes.Name = "dgvRecipes";
            dgvRecipes.ReadOnly = true;
            dgvRecipes.RowHeadersVisible = false;
            dgvRecipes.RowHeadersWidth = 50;
            dgvRecipes.RowTemplate.Height = 28;
            dgvRecipes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvRecipes.Size = new System.Drawing.Size(1056, 452);
            dgvRecipes.TabIndex = 1;
            // 
            // rIdCol
            // 
            rIdCol.HeaderText = "ID";
            rIdCol.Name = "rIdCol";
            rIdCol.ReadOnly = true;
            rIdCol.Visible = false;
            rIdCol.Width = 50;
            // 
            // rNameCol
            // 
            rNameCol.HeaderText = "Найменування";
            rNameCol.Name = "rNameCol";
            rNameCol.ReadOnly = true;
            rNameCol.Width = 240;
            // 
            // rStatusCol
            // 
            rStatusCol.HeaderText = "Категорія";
            rStatusCol.Name = "rStatusCol";
            rStatusCol.ReadOnly = true;
            rStatusCol.Width = 140;
            // 
            // rIngrCol
            // 
            rIngrCol.HeaderText = "Інгредієнти";
            rIngrCol.Name = "rIngrCol";
            rIngrCol.ReadOnly = true;
            rIngrCol.Width = 340;
            // 
            // rKcalCol
            // 
            rKcalCol.HeaderText = "Калорійність";
            rKcalCol.Name = "rKcalCol";
            rKcalCol.ReadOnly = true;
            rKcalCol.Width = 140;
            // 
            // rCostCol
            // 
            rCostCol.HeaderText = "Вартість";
            rCostCol.Name = "rCostCol";
            rCostCol.ReadOnly = true;
            rCostCol.Width = 130;
            // 
            // btnAddRecipe
            // 
            btnAddRecipe.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAddRecipe.Location = new System.Drawing.Point(8, 518);
            btnAddRecipe.Name = "btnAddRecipe";
            btnAddRecipe.Size = new System.Drawing.Size(140, 38);
            btnAddRecipe.TabIndex = 2;
            btnAddRecipe.Text = "➕ Додати страву";
            btnAddRecipe.UseVisualStyleBackColor = true;
            // 
            // btnEditRecipe
            // 
            btnEditRecipe.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnEditRecipe.Location = new System.Drawing.Point(156, 518);
            btnEditRecipe.Name = "btnEditRecipe";
            btnEditRecipe.Size = new System.Drawing.Size(140, 38);
            btnEditRecipe.TabIndex = 3;
            btnEditRecipe.Text = "✏️ Редагувати";
            btnEditRecipe.UseVisualStyleBackColor = true;
            // 
            // btnDeleteRecipe
            // 
            btnDeleteRecipe.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnDeleteRecipe.Location = new System.Drawing.Point(304, 518);
            btnDeleteRecipe.Name = "btnDeleteRecipe";
            btnDeleteRecipe.Size = new System.Drawing.Size(140, 38);
            btnDeleteRecipe.TabIndex = 4;
            btnDeleteRecipe.Text = "🗑️ Видалити";
            btnDeleteRecipe.UseVisualStyleBackColor = true;
            // 
            // btnWeeklyPlan
            // 
            btnWeeklyPlan.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnWeeklyPlan.BackColor = System.Drawing.Color.FromArgb(238, 242, 255);
            btnWeeklyPlan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnWeeklyPlan.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnWeeklyPlan.ForeColor = System.Drawing.Color.FromArgb(49, 46, 129);
            btnWeeklyPlan.Location = new System.Drawing.Point(842, 518);
            btnWeeklyPlan.Name = "btnWeeklyPlan";
            btnWeeklyPlan.Size = new System.Drawing.Size(222, 38);
            btnWeeklyPlan.TabIndex = 5;
            btnWeeklyPlan.Text = "📅 Тижневий план";
            btnWeeklyPlan.UseVisualStyleBackColor = false;
            // 
            // tabInventory
            // 
            tabInventory.Controls.Add(panelInventoryFilter);
            tabInventory.Controls.Add(dgvInventory);
            tabInventory.Controls.Add(btnAddInventory);
            tabInventory.Controls.Add(btnReduceInventory);
            tabInventory.Controls.Add(btnGenerateShopping);
            tabInventory.Controls.Add(btnRecountAll);
            tabInventory.Controls.Add(btnRecountHistory);
            tabInventory.Location = new System.Drawing.Point(4, 30);
            tabInventory.Name = "tabInventory";
            tabInventory.Size = new System.Drawing.Size(1072, 568);
            tabInventory.TabIndex = 2;
            tabInventory.Text = "Облік";
            tabInventory.UseVisualStyleBackColor = true;
            // 
            // panelInventoryFilter
            // 
            panelInventoryFilter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelInventoryFilter.BackColor = System.Drawing.Color.FromArgb(250, 250, 252);
            panelInventoryFilter.Controls.Add(lblSearchInventory);
            panelInventoryFilter.Controls.Add(txtSearchInventory);
            panelInventoryFilter.Controls.Add(chkOnlyLowStock);
            panelInventoryFilter.Controls.Add(btnRestockLow);
            panelInventoryFilter.Controls.Add(btnQuickTelegram);
            panelInventoryFilter.Location = new System.Drawing.Point(8, 6);
            panelInventoryFilter.Name = "panelInventoryFilter";
            panelInventoryFilter.Size = new System.Drawing.Size(1056, 44);
            panelInventoryFilter.TabIndex = 0;
            // 
            // lblSearchInventory
            // 
            lblSearchInventory.AutoSize = true;
            lblSearchInventory.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSearchInventory.Location = new System.Drawing.Point(8, 12);
            lblSearchInventory.Name = "lblSearchInventory";
            lblSearchInventory.Size = new System.Drawing.Size(74, 20);
            lblSearchInventory.TabIndex = 0;
            lblSearchInventory.Text = "🔍 Пошук:";
            // 
            // txtSearchInventory
            // 
            txtSearchInventory.Location = new System.Drawing.Point(88, 8);
            txtSearchInventory.Name = "txtSearchInventory";
            txtSearchInventory.PlaceholderText = "Введіть назву продукту...";
            txtSearchInventory.Size = new System.Drawing.Size(260, 29);
            txtSearchInventory.TabIndex = 1;
            // 
            // chkOnlyLowStock
            // 
            chkOnlyLowStock.AutoSize = true;
            chkOnlyLowStock.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            chkOnlyLowStock.ForeColor = System.Drawing.Color.FromArgb(198, 40, 40);
            chkOnlyLowStock.Location = new System.Drawing.Point(365, 11);
            chkOnlyLowStock.Name = "chkOnlyLowStock";
            chkOnlyLowStock.Size = new System.Drawing.Size(242, 24);
            chkOnlyLowStock.TabIndex = 2;
            chkOnlyLowStock.Text = "⚠️ Тільки дефіцитні позиції";
            chkOnlyLowStock.UseVisualStyleBackColor = true;
            // 
            // btnRestockLow
            // 
            btnRestockLow.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnRestockLow.BackColor = System.Drawing.Color.FromArgb(255, 243, 224);
            btnRestockLow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnRestockLow.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnRestockLow.ForeColor = System.Drawing.Color.FromArgb(230, 81, 0);
            btnRestockLow.Location = new System.Drawing.Point(630, 6);
            btnRestockLow.Name = "btnRestockLow";
            btnRestockLow.Size = new System.Drawing.Size(220, 32);
            btnRestockLow.TabIndex = 3;
            btnRestockLow.Text = "🛒 Закупити закінчені";
            btnRestockLow.UseVisualStyleBackColor = false;
            // 
            // btnQuickTelegram
            // 
            btnQuickTelegram.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnQuickTelegram.BackColor = System.Drawing.Color.FromArgb(240, 244, 248);
            btnQuickTelegram.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnQuickTelegram.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnQuickTelegram.Location = new System.Drawing.Point(858, 6);
            btnQuickTelegram.Name = "btnQuickTelegram";
            btnQuickTelegram.Size = new System.Drawing.Size(192, 32);
            btnQuickTelegram.TabIndex = 4;
            btnQuickTelegram.Text = "📋 Список в Telegram";
            btnQuickTelegram.UseVisualStyleBackColor = false;
            // 
            // dgvInventory
            // 
            dgvInventory.AllowUserToAddRows = false;
            dgvInventory.AllowUserToDeleteRows = false;
            dgvInventory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvInventory.BackgroundColor = System.Drawing.Color.White;
            dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInventory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { iIdCol, iNameCol, iQtyCol });
            dgvInventory.Location = new System.Drawing.Point(8, 56);
            dgvInventory.Name = "dgvInventory";
            dgvInventory.ReadOnly = true;
            dgvInventory.RowHeadersVisible = false;
            dgvInventory.RowHeadersWidth = 50;
            dgvInventory.RowTemplate.Height = 28;
            dgvInventory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvInventory.Size = new System.Drawing.Size(1056, 452);
            dgvInventory.TabIndex = 1;
            // 
            // iIdCol
            // 
            iIdCol.HeaderText = "ID";
            iIdCol.Name = "iIdCol";
            iIdCol.ReadOnly = true;
            iIdCol.Visible = false;
            iIdCol.Width = 50;
            // 
            // iNameCol
            // 
            iNameCol.HeaderText = "Найменування";
            iNameCol.Name = "iNameCol";
            iNameCol.ReadOnly = true;
            iNameCol.Width = 360;
            // 
            // iQtyCol
            // 
            iQtyCol.HeaderText = "Поточна кількість на складі";
            iQtyCol.Name = "iQtyCol";
            iQtyCol.ReadOnly = true;
            iQtyCol.Width = 260;
            // 
            // btnAddInventory
            // 
            btnAddInventory.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnAddInventory.Location = new System.Drawing.Point(8, 518);
            btnAddInventory.Name = "btnAddInventory";
            btnAddInventory.Size = new System.Drawing.Size(140, 38);
            btnAddInventory.TabIndex = 2;
            btnAddInventory.Text = "➕ Оприбуткувати";
            btnAddInventory.UseVisualStyleBackColor = true;
            // 
            // btnReduceInventory
            // 
            btnReduceInventory.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnReduceInventory.Location = new System.Drawing.Point(156, 518);
            btnReduceInventory.Name = "btnReduceInventory";
            btnReduceInventory.Size = new System.Drawing.Size(140, 38);
            btnReduceInventory.TabIndex = 3;
            btnReduceInventory.Text = "➖ Списати";
            btnReduceInventory.UseVisualStyleBackColor = true;
            // 
            // btnGenerateShopping
            // 
            btnGenerateShopping.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            btnGenerateShopping.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            btnGenerateShopping.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnGenerateShopping.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnGenerateShopping.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            btnGenerateShopping.Location = new System.Drawing.Point(304, 518);
            btnGenerateShopping.Name = "btnGenerateShopping";
            btnGenerateShopping.Size = new System.Drawing.Size(200, 38);
            btnGenerateShopping.TabIndex = 4;
            btnGenerateShopping.Text = "🛒 Планувальник закупок";
            btnGenerateShopping.UseVisualStyleBackColor = false;
            // 
            // btnRecountAll
            // 
            btnRecountAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRecountAll.Location = new System.Drawing.Point(744, 518);
            btnRecountAll.Name = "btnRecountAll";
            btnRecountAll.Size = new System.Drawing.Size(150, 38);
            btnRecountAll.TabIndex = 5;
            btnRecountAll.Text = "📋 Переоблік";
            btnRecountAll.UseVisualStyleBackColor = true;
            // 
            // btnRecountHistory
            // 
            btnRecountHistory.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnRecountHistory.Location = new System.Drawing.Point(902, 518);
            btnRecountHistory.Name = "btnRecountHistory";
            btnRecountHistory.Size = new System.Drawing.Size(162, 38);
            btnRecountHistory.TabIndex = 6;
            btnRecountHistory.Text = "📜 Історія переобліків";
            btnRecountHistory.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(1080, 660);
            Controls.Add(tabControl1);
            Controls.Add(panelTopBanner);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MinimumSize = new System.Drawing.Size(960, 600);
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Food Manager — Облік продуктів, страви та закупівлі";
            panelTopBanner.ResumeLayout(false);
            panelTopBanner.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabProducts.ResumeLayout(false);
            panelProductFilter.ResumeLayout(false);
            panelProductFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            tabRecipes.ResumeLayout(false);
            panelRecipeFilter.ResumeLayout(false);
            panelRecipeFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRecipes).EndInit();
            tabInventory.ResumeLayout(false);
            panelInventoryFilter.ResumeLayout(false);
            panelInventoryFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInventory).EndInit();
            ResumeLayout(false);
        }
    }
}
