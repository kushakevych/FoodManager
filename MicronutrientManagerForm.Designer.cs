namespace FoodManager
{
    partial class MicronutrientManagerForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabProductAnalysis;
        private System.Windows.Forms.TabPage tabRecipeAnalysis;
        private System.Windows.Forms.TabPage tabRdaReference;

        // Product Analysis controls
        private System.Windows.Forms.ComboBox cmbProduct;
        private System.Windows.Forms.Label lblSelectProduct;
        private System.Windows.Forms.DataGridView dgvProductNutrients;
        private System.Windows.Forms.Button btnSaveProductNutrients;
        private System.Windows.Forms.Label lblProductUnitHint;

        // Recipe Analysis controls
        private System.Windows.Forms.ComboBox cmbRecipe;
        private System.Windows.Forms.Label lblSelectRecipe;
        private System.Windows.Forms.NumericUpDown numServings;
        private System.Windows.Forms.Label lblServings;
        private System.Windows.Forms.DataGridView dgvRecipeNutrients;
        private System.Windows.Forms.Label lblRecipeSummary;

        // Reference controls
        private System.Windows.Forms.DataGridView dgvReference;

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

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
            panelTop = new System.Windows.Forms.Panel();
            lblTitle = new System.Windows.Forms.Label();
            lblSubtitle = new System.Windows.Forms.Label();
            tabControl = new System.Windows.Forms.TabControl();
            tabProductAnalysis = new System.Windows.Forms.TabPage();
            lblSelectProduct = new System.Windows.Forms.Label();
            cmbProduct = new System.Windows.Forms.ComboBox();
            lblProductUnitHint = new System.Windows.Forms.Label();
            dgvProductNutrients = new System.Windows.Forms.DataGridView();
            btnSaveProductNutrients = new System.Windows.Forms.Button();
            tabRecipeAnalysis = new System.Windows.Forms.TabPage();
            lblSelectRecipe = new System.Windows.Forms.Label();
            cmbRecipe = new System.Windows.Forms.ComboBox();
            lblServings = new System.Windows.Forms.Label();
            numServings = new System.Windows.Forms.NumericUpDown();
            lblRecipeSummary = new System.Windows.Forms.Label();
            dgvRecipeNutrients = new System.Windows.Forms.DataGridView();
            tabRdaReference = new System.Windows.Forms.TabPage();
            dgvReference = new System.Windows.Forms.DataGridView();
            btnClose = new System.Windows.Forms.Button();
            panelTop.SuspendLayout();
            tabControl.SuspendLayout();
            tabProductAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductNutrients).BeginInit();
            tabRecipeAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numServings).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecipeNutrients).BeginInit();
            tabRdaReference.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvReference).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = System.Drawing.Color.FromArgb(238, 246, 238);
            panelTop.Controls.Add(lblTitle);
            panelTop.Controls.Add(lblSubtitle);
            panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            panelTop.Location = new System.Drawing.Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new System.Drawing.Size(940, 70);
            panelTop.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(27, 94, 32);
            lblTitle.Location = new System.Drawing.Point(16, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(380, 31);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "🥗 Вітаміни та мікроелементи";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(85, 85, 85);
            lblSubtitle.Location = new System.Drawing.Point(20, 42);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new System.Drawing.Size(560, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Детальний аналіз вмісту вітамінів і мінералів та відповідності добовим нормам (RDA)";
            // 
            // tabControl
            // 
            tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tabControl.Controls.Add(tabProductAnalysis);
            tabControl.Controls.Add(tabRecipeAnalysis);
            tabControl.Controls.Add(tabRdaReference);
            tabControl.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            tabControl.Location = new System.Drawing.Point(12, 78);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(916, 520);
            tabControl.TabIndex = 1;
            // 
            // tabProductAnalysis
            // 
            tabProductAnalysis.Controls.Add(lblSelectProduct);
            tabProductAnalysis.Controls.Add(cmbProduct);
            tabProductAnalysis.Controls.Add(lblProductUnitHint);
            tabProductAnalysis.Controls.Add(dgvProductNutrients);
            tabProductAnalysis.Controls.Add(btnSaveProductNutrients);
            tabProductAnalysis.Location = new System.Drawing.Point(4, 30);
            tabProductAnalysis.Name = "tabProductAnalysis";
            tabProductAnalysis.Padding = new System.Windows.Forms.Padding(12);
            tabProductAnalysis.Size = new System.Drawing.Size(908, 486);
            tabProductAnalysis.TabIndex = 0;
            tabProductAnalysis.Text = "Мікроелементи продукту";
            tabProductAnalysis.UseVisualStyleBackColor = true;
            // 
            // lblSelectProduct
            // 
            lblSelectProduct.AutoSize = true;
            lblSelectProduct.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSelectProduct.Location = new System.Drawing.Point(12, 14);
            lblSelectProduct.Name = "lblSelectProduct";
            lblSelectProduct.Size = new System.Drawing.Size(130, 21);
            lblSelectProduct.TabIndex = 0;
            lblSelectProduct.Text = "Оберіть продукт:";
            // 
            // cmbProduct
            // 
            cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbProduct.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbProduct.FormattingEnabled = true;
            cmbProduct.Location = new System.Drawing.Point(150, 11);
            cmbProduct.Name = "cmbProduct";
            cmbProduct.Size = new System.Drawing.Size(280, 29);
            cmbProduct.TabIndex = 1;
            // 
            // lblProductUnitHint
            // 
            lblProductUnitHint.AutoSize = true;
            lblProductUnitHint.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            lblProductUnitHint.Location = new System.Drawing.Point(445, 15);
            lblProductUnitHint.Name = "lblProductUnitHint";
            lblProductUnitHint.Size = new System.Drawing.Size(210, 21);
            lblProductUnitHint.TabIndex = 2;
            lblProductUnitHint.Text = "Вміст вказано на 100 грамів";
            // 
            // dgvProductNutrients
            // 
            dgvProductNutrients.AllowUserToAddRows = false;
            dgvProductNutrients.AllowUserToDeleteRows = false;
            dgvProductNutrients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvProductNutrients.BackgroundColor = System.Drawing.Color.White;
            dgvProductNutrients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductNutrients.Location = new System.Drawing.Point(12, 50);
            dgvProductNutrients.Name = "dgvProductNutrients";
            dgvProductNutrients.RowHeadersVisible = false;
            dgvProductNutrients.RowHeadersWidth = 51;
            dgvProductNutrients.RowTemplate.Height = 29;
            dgvProductNutrients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvProductNutrients.Size = new System.Drawing.Size(884, 380);
            dgvProductNutrients.TabIndex = 3;
            // 
            // btnSaveProductNutrients
            // 
            btnSaveProductNutrients.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnSaveProductNutrients.BackColor = System.Drawing.Color.FromArgb(46, 125, 50);
            btnSaveProductNutrients.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSaveProductNutrients.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSaveProductNutrients.ForeColor = System.Drawing.Color.White;
            btnSaveProductNutrients.Location = new System.Drawing.Point(676, 438);
            btnSaveProductNutrients.Name = "btnSaveProductNutrients";
            btnSaveProductNutrients.Size = new System.Drawing.Size(220, 38);
            btnSaveProductNutrients.TabIndex = 4;
            btnSaveProductNutrients.Text = "💾 Зберегти зміни продукту";
            btnSaveProductNutrients.UseVisualStyleBackColor = false;
            // 
            // tabRecipeAnalysis
            // 
            tabRecipeAnalysis.Controls.Add(lblSelectRecipe);
            tabRecipeAnalysis.Controls.Add(cmbRecipe);
            tabRecipeAnalysis.Controls.Add(lblServings);
            tabRecipeAnalysis.Controls.Add(numServings);
            tabRecipeAnalysis.Controls.Add(lblRecipeSummary);
            tabRecipeAnalysis.Controls.Add(dgvRecipeNutrients);
            tabRecipeAnalysis.Location = new System.Drawing.Point(4, 30);
            tabRecipeAnalysis.Name = "tabRecipeAnalysis";
            tabRecipeAnalysis.Padding = new System.Windows.Forms.Padding(12);
            tabRecipeAnalysis.Size = new System.Drawing.Size(908, 486);
            tabRecipeAnalysis.TabIndex = 1;
            tabRecipeAnalysis.Text = "Аналіз страви / рецепта";
            tabRecipeAnalysis.UseVisualStyleBackColor = true;
            // 
            // lblSelectRecipe
            // 
            lblSelectRecipe.AutoSize = true;
            lblSelectRecipe.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSelectRecipe.Location = new System.Drawing.Point(12, 14);
            lblSelectRecipe.Name = "lblSelectRecipe";
            lblSelectRecipe.Size = new System.Drawing.Size(120, 21);
            lblSelectRecipe.TabIndex = 0;
            lblSelectRecipe.Text = "Оберіть страву:";
            // 
            // cmbRecipe
            // 
            cmbRecipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbRecipe.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            cmbRecipe.FormattingEnabled = true;
            cmbRecipe.Location = new System.Drawing.Point(140, 11);
            cmbRecipe.Name = "cmbRecipe";
            cmbRecipe.Size = new System.Drawing.Size(260, 29);
            cmbRecipe.TabIndex = 1;
            // 
            // lblServings
            // 
            lblServings.AutoSize = true;
            lblServings.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblServings.Location = new System.Drawing.Point(420, 14);
            lblServings.Name = "lblServings";
            lblServings.Size = new System.Drawing.Size(68, 21);
            lblServings.TabIndex = 2;
            lblServings.Text = "Порцій:";
            // 
            // numServings
            // 
            numServings.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            numServings.Location = new System.Drawing.Point(495, 12);
            numServings.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            numServings.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numServings.Name = "numServings";
            numServings.Size = new System.Drawing.Size(70, 29);
            numServings.TabIndex = 3;
            numServings.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblRecipeSummary
            // 
            lblRecipeSummary.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblRecipeSummary.AutoSize = true;
            lblRecipeSummary.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblRecipeSummary.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
            lblRecipeSummary.Location = new System.Drawing.Point(12, 448);
            lblRecipeSummary.Name = "lblRecipeSummary";
            lblRecipeSummary.Size = new System.Drawing.Size(350, 21);
            lblRecipeSummary.TabIndex = 5;
            lblRecipeSummary.Text = "Підсумок: 100% добової норми покрито для: -";
            // 
            // dgvRecipeNutrients
            // 
            dgvRecipeNutrients.AllowUserToAddRows = false;
            dgvRecipeNutrients.AllowUserToDeleteRows = false;
            dgvRecipeNutrients.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvRecipeNutrients.BackgroundColor = System.Drawing.Color.White;
            dgvRecipeNutrients.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecipeNutrients.Location = new System.Drawing.Point(12, 50);
            dgvRecipeNutrients.Name = "dgvRecipeNutrients";
            dgvRecipeNutrients.ReadOnly = true;
            dgvRecipeNutrients.RowHeadersVisible = false;
            dgvRecipeNutrients.RowHeadersWidth = 51;
            dgvRecipeNutrients.RowTemplate.Height = 29;
            dgvRecipeNutrients.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvRecipeNutrients.Size = new System.Drawing.Size(884, 385);
            dgvRecipeNutrients.TabIndex = 4;
            // 
            // tabRdaReference
            // 
            tabRdaReference.Controls.Add(dgvReference);
            tabRdaReference.Location = new System.Drawing.Point(4, 30);
            tabRdaReference.Name = "tabRdaReference";
            tabRdaReference.Padding = new System.Windows.Forms.Padding(12);
            tabRdaReference.Size = new System.Drawing.Size(908, 486);
            tabRdaReference.TabIndex = 2;
            tabRdaReference.Text = "Довідник добових норм (RDA)";
            tabRdaReference.UseVisualStyleBackColor = true;
            // 
            // dgvReference
            // 
            dgvReference.AllowUserToAddRows = false;
            dgvReference.AllowUserToDeleteRows = false;
            dgvReference.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            dgvReference.BackgroundColor = System.Drawing.Color.White;
            dgvReference.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReference.Location = new System.Drawing.Point(12, 12);
            dgvReference.Name = "dgvReference";
            dgvReference.ReadOnly = true;
            dgvReference.RowHeadersVisible = false;
            dgvReference.RowHeadersWidth = 51;
            dgvReference.RowTemplate.Height = 29;
            dgvReference.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgvReference.Size = new System.Drawing.Size(884, 460);
            dgvReference.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnClose.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnClose.Location = new System.Drawing.Point(808, 606);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(120, 36);
            btnClose.TabIndex = 2;
            btnClose.Text = "Закрити";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // MicronutrientManagerForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(940, 650);
            Controls.Add(btnClose);
            Controls.Add(tabControl);
            Controls.Add(panelTop);
            Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            MinimumSize = new System.Drawing.Size(800, 500);
            Name = "MicronutrientManagerForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Вітаміни та мікроелементи — Food Manager";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            tabControl.ResumeLayout(false);
            tabProductAnalysis.ResumeLayout(false);
            tabProductAnalysis.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvProductNutrients).EndInit();
            tabRecipeAnalysis.ResumeLayout(false);
            tabRecipeAnalysis.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numServings).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecipeNutrients).EndInit();
            tabRdaReference.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvReference).EndInit();
            ResumeLayout(false);
        }
    }
}
