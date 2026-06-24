namespace FoodManager
{
    partial class InventoryEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.Label lblProduct;
        internal System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblPrompt = new System.Windows.Forms.Label();
            lblProduct = new System.Windows.Forms.Label();
            numQuantity = new System.Windows.Forms.NumericUpDown();
            lblUnit = new System.Windows.Forms.Label();
            btnOK = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            SuspendLayout();
            // 
            // lblPrompt
            // 
            lblPrompt.Location = new System.Drawing.Point(12, 9);
            lblPrompt.Size = new System.Drawing.Size(360, 24);
            lblPrompt.Text = "Продукт:";
            // 
            // lblProduct
            // 
            lblProduct.Location = new System.Drawing.Point(12, 33);
            lblProduct.Size = new System.Drawing.Size(360, 24);
            lblProduct.Text = "ProductName";
            // 
            // numQuantity
            // 
            numQuantity.Location = new System.Drawing.Point(12, 70);
            numQuantity.Size = new System.Drawing.Size(140, 26);
            // 
            // lblUnit
            // 
            lblUnit.Location = new System.Drawing.Point(160, 72);
            lblUnit.Size = new System.Drawing.Size(60, 24);
            lblUnit.Text = "г";
            // 
            // btnOK
            // 
            btnOK.Location = new System.Drawing.Point(12, 110);
            btnOK.Size = new System.Drawing.Size(120, 36);
            btnOK.Text = "OK";
            btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(152, 110);
            btnCancel.Size = new System.Drawing.Size(120, 36);
            btnCancel.Text = "Відмінити";
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // InventoryEditForm
            // 
            ClientSize = new System.Drawing.Size(380, 160);
            Controls.Add(lblPrompt);
            Controls.Add(lblProduct);
            Controls.Add(numQuantity);
            Controls.Add(lblUnit);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Name = "InventoryEditForm";
            Text = "Редагувати облік";
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}