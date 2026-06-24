using System;
using System.Linq;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class RecountHistoryForm : Form
    {
        public RecountHistoryForm()
        {
            InitializeComponent();
            LoadSessions();
        }

        private void LoadSessions()
        {
            dgvSessions.Rows.Clear();
            var sessions = DataAccess.GetRecountSessions();
            foreach (var s in sessions)
            {
                dgvSessions.Rows.Add(s.Id, s.CreatedAt.ToString("yyyy-MM-dd HH:mm"), s.Note);
            }
            if (dgvSessions.Rows.Count > 0)
            {
                dgvSessions.Rows[0].Selected = true;
                LoadItemsForSelectedSession();
            }
        }

        private void dgvSessions_SelectionChanged(object sender, EventArgs e)
        {
            LoadItemsForSelectedSession();
        }

        private void LoadItemsForSelectedSession()
        {
            dgvItems.Rows.Clear();
            if (dgvSessions.SelectedRows.Count == 0 && dgvSessions.CurrentRow == null) return;
            var row = dgvSessions.SelectedRows.Count > 0 ? dgvSessions.SelectedRows[0] : dgvSessions.CurrentRow;
            var rid = Convert.ToInt64(row.Cells[0].Value);
            var items = DataAccess.GetRecountItemsByRecountId(rid);
            foreach (var it in items)
            {
                var deltaText = it.Delta >= 0 ? $"+{it.Delta}" : it.Delta.ToString();
                dgvItems.Rows.Add(it.ProductId, it.ProductName, $"{it.OldQuantity} {it.Unit}", $"{it.NewQuantity} {it.Unit}", deltaText);
            }
        }
    }
}