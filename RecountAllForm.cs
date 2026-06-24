using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class RecountAllForm : Form
    {
        public RecountAllForm()
        {
            InitializeComponent();
            LoadGrid();
        }

        private void LoadGrid()
        {
            dgvRecount.Rows.Clear();
            foreach (var p in DataAccess.GetProducts())
            {
                var (qty, unit) = DataAccess.GetInventoryQuantityForProductById(p.Id);
                // Columns: Id(hidden), Name, Unit, CurrentQty, NewQty
                dgvRecount.Rows.Add(p.Id, p.Name, unit, qty, qty);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            // Collect changed rows
            var changed = new List<(int productId, int oldQty, int newQty, string unit)>();
            foreach (DataGridViewRow row in dgvRecount.Rows)
            {
                if (row.IsNewRow) continue;
                var pid = Convert.ToInt32(row.Cells[0].Value);
                var oldQty = Convert.ToInt32(row.Cells[3].Value);
                var newQtyParsed = 0;
                if (!int.TryParse(Convert.ToString(row.Cells[4].Value), out newQtyParsed))
                {
                    MessageBox.Show($"Невірне число для продукту '{row.Cells[1].Value}'", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var unit = Convert.ToString(row.Cells[2].Value) ?? "g";
                if (newQtyParsed != oldQty)
                    changed.Add((pid, oldQty, newQtyParsed, unit));
            }

            if (changed.Count == 0)
            {
                MessageBox.Show("Немає змін для застосування.", "Переоблік", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Ask for optional note
            var note = InputBox.Show("Примітка (необов'язково):", "Примітка до переобліку");

            try
            {
                DataAccess.PerformBulkRecount(changed.ToArray(), note);
                MessageBox.Show($"Переоблік застосовано: {changed.Count} змін(и).", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при застосуванні переобліку: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}