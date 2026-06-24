namespace FoodManager
{
    partial class AddRecipeForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbStatus;

        private System.Windows.Forms.ComboBox cmbIngredient;
        private System.Windows.Forms.ComboBox cmbIngUnit;
        private System.Windows.Forms.Label lblUnitHint;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Button btnAddIngredient;
        private System.Windows.Forms.ListBox lstIngredients;
        private System.Windows.Forms.Button btnRemoveIngredient;

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtName = new TextBox();
            cmbStatus = new ComboBox();
            cmbIngredient = new ComboBox();
            cmbIngUnit = new ComboBox();
            lblUnitHint = new Label();
            numQuantity = new NumericUpDown();
            btnAddIngredient = new Button();
            lstIngredients = new ListBox();
            btnRemoveIngredient = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(12, 12);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Найменування страви";
            txtName.Size = new Size(502, 31);
            txtName.TabIndex = 0;
            // 
            // cmbStatus
            // 
            cmbStatus.AccessibleName = "";
            cmbStatus.Location = new Point(520, 12);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(200, 33);
            cmbStatus.TabIndex = 1;
            // 
            // cmbIngredient
            // 
            cmbIngredient.Location = new Point(12, 79);
            cmbIngredient.Name = "cmbIngredient";
            cmbIngredient.Size = new Size(376, 33);
            cmbIngredient.TabIndex = 2;
            // 
            // cmbIngUnit
            // 
            cmbIngUnit.Location = new Point(328, 315);
            cmbIngUnit.Name = "cmbIngUnit";
            cmbIngUnit.Size = new Size(60, 33);
            cmbIngUnit.TabIndex = 3;
            cmbIngUnit.Visible = false;
            // 
            // lblUnitHint
            // 
            lblUnitHint.Location = new Point(12, 117);
            lblUnitHint.Name = "lblUnitHint";
            lblUnitHint.Size = new Size(220, 26);
            lblUnitHint.TabIndex = 4;
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(394, 82);
            numQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(120, 31);
            numQuantity.TabIndex = 5;
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnAddIngredient
            // 
            btnAddIngredient.Location = new Point(520, 80);
            btnAddIngredient.Name = "btnAddIngredient";
            btnAddIngredient.Size = new Size(200, 33);
            btnAddIngredient.TabIndex = 6;
            btnAddIngredient.Text = "Додати інгредієнт";
            btnAddIngredient.Click += btnAddIngredient_Click;
            // 
            // lstIngredients
            // 
            lstIngredients.ItemHeight = 25;
            lstIngredients.Location = new Point(12, 146);
            lstIngredients.Name = "lstIngredients";
            lstIngredients.Size = new Size(502, 154);
            lstIngredients.TabIndex = 7;
            lstIngredients.DoubleClick += lstIngredients_DoubleClick;
            // 
            // btnRemoveIngredient
            // 
            btnRemoveIngredient.Location = new Point(520, 146);
            btnRemoveIngredient.Name = "btnRemoveIngredient";
            btnRemoveIngredient.Size = new Size(200, 36);
            btnRemoveIngredient.TabIndex = 8;
            btnRemoveIngredient.Text = "Видалити";
            btnRemoveIngredient.Click += btnRemoveIngredient_Click;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(609, 315);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(111, 36);
            btnOK.TabIndex = 9;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(12, 315);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(111, 36);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Відмінити";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 46);
            label1.Name = "label1";
            label1.Size = new Size(96, 25);
            label1.TabIndex = 11;
            label1.Text = "Продукти:";
            // 
            // AddRecipeForm
            // 
            ClientSize = new Size(732, 365);
            Controls.Add(label1);
            Controls.Add(txtName);
            Controls.Add(cmbStatus);
            Controls.Add(cmbIngredient);
            Controls.Add(cmbIngUnit);
            Controls.Add(lblUnitHint);
            Controls.Add(numQuantity);
            Controls.Add(btnAddIngredient);
            Controls.Add(lstIngredients);
            Controls.Add(btnRemoveIngredient);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Name = "AddRecipeForm";
            Text = "Додати/Редагувати страву";
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
    }
}