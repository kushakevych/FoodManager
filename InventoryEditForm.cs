using System;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class InventoryEditForm : Form
    {
        public enum Mode { Add, Reduce }

        public int ProductId { get; }
        public string ProductName { get; }
        public string Unit { get; }
        public Mode OperationMode { get; }

        public int Quantity => (int)numQuantity.Value;

        public InventoryEditForm(int productId, string productName, string unit, Mode mode)
        {
            InitializeComponent();
            ProductId = productId;
            ProductName = productName;
            Unit = string.IsNullOrEmpty(unit) ? "g" : unit;
            OperationMode = mode;

            lblProduct.Text = productName;
            lblUnit.Text = Unit == "pcs" ? "шт" : "г";
            numQuantity.Minimum = 0;
            numQuantity.Maximum = 1000000;
            numQuantity.Value = 1;

            btnOK.Text = mode == Mode.Add ? "Додати" : "Списати";
            Text = mode == Mode.Add ? "Додати в облік" : "Списати з обліку";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Quantity <= 0)
            {
                MessageBox.Show("Кількість має бути більшою за 0.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}