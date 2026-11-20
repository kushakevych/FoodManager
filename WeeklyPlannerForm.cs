using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FoodManager.Services.Database;
using FoodManager.Services.Export;
using FoodManager.Services.Planning;

namespace FoodManager
{
    public partial class WeeklyPlannerForm : Form
    {
        private readonly IWeeklyPlannerService _plannerService;
        private readonly IExcelExportService _exportService;
        private readonly IDatabaseService _db;

        public WeeklyPlannerForm(
            IWeeklyPlannerService? plannerService = null,
            IExcelExportService? exportService = null,
            IDatabaseService? db = null)
        {
            InitializeComponent();

            _db = db ?? DatabaseService.Default;
            _plannerService = plannerService ?? new WeeklyPlannerService(_db);
            _exportService = exportService ?? new ExcelExportService(_db);

            LoadRecipesIntoCombo();

            cmbDay.Items.AddRange(new object[]
            {
                "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя"
            });
            cmbDay.SelectedIndex = 0;

            cmbRecipe.SelectedIndexChanged += CmbRecipe_SelectedIndexChanged;
            numServings.ValueChanged += NumServings_ValueChanged;
        }

        private void LoadRecipesIntoCombo()
        {
            cmbRecipe.Items.Clear();
            foreach (var r in _db.GetRecipes())
            {
                cmbRecipe.Items.Add(new ComboItem { Id = r.Id, Name = r.Name });
            }
            if (cmbRecipe.Items.Count > 0) cmbRecipe.SelectedIndex = 0;

            UpdateSelectedRecipeInfo();
        }

        private void CmbRecipe_SelectedIndexChanged(object? sender, EventArgs e) => UpdateSelectedRecipeInfo();

        private void NumServings_ValueChanged(object? sender, EventArgs e) => UpdateSelectedRecipeInfo();

        private void UpdateSelectedRecipeInfo()
        {
            if (cmbRecipe.SelectedItem is not ComboItem ci) return;

            var servings = (int)numServings.Value;
            var multi = _plannerService.ComputeSingleRecipeServings(ci.Id, servings);

            textBox1.Text = $"{multi.Protein}";
            textBox2.Text = $"{multi.Fats}";
            textBox3.Text = $"{multi.Carbs}";
            textBox4.Text = $"{multi.Kcal}";
            textBox5.Text = $"{multi.Saturated}";
            textBox6.Text = $"{multi.Sugars}";
            textBox7.Text = $"{multi.Cost}";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cmbRecipe.SelectedItem is not ComboItem ci) return;
            var day = cmbDay.SelectedItem?.ToString() ?? string.Empty;
            var servings = (int)numServings.Value;
            dgvPlan.Rows.Add(day, ci.Id, ci.Name, servings);
        }

        private List<WeeklyPlanItem> CollectPlanItems()
        {
            var list = new List<WeeklyPlanItem>();
            foreach (DataGridViewRow row in dgvPlan.Rows)
            {
                if (row.IsNewRow) continue;
                var day = Convert.ToString(row.Cells[0].Value) ?? string.Empty;
                var recipeId = Convert.ToInt32(row.Cells[1].Value);
                var recipeName = Convert.ToString(row.Cells[2].Value) ?? string.Empty;
                var servings = Convert.ToInt32(row.Cells[3].Value);
                if (servings > 0)
                {
                    list.Add(new WeeklyPlanItem(day, recipeId, recipeName, servings));
                }
            }
            return list;
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            var planItems = CollectPlanItems();
            var result = _plannerService.ComputeWeeklyTotals(planItems);
            txtResult.Lines = result.FormattedSummary.ToArray();
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void btnExport_Click(object sender, EventArgs e)
        {
            var planItems = CollectPlanItems();
            if (planItems.Count == 0)
            {
                MessageBox.Show("Немає записів у плані для експорту.", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FileName = "weekly_plan_export.xlsx"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                _exportService.ExportWeeklyPlan(sfd.FileName, planItems);
                MessageBox.Show($"Excel збережено: {sfd.FileName}", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні XLSX: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenShopping_Click(object sender, EventArgs e)
        {
            var planItems = CollectPlanItems();
            var prefill = _plannerService.GenerateShoppingListPrefill(planItems);

            if (prefill.Count == 0)
            {
                MessageBox.Show("Немає інгредієнтів у плані для експорту в план закупок.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sp = new ShoppingPlannerForm(prefill);
            sp.ShowDialog();
        }

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public override string ToString() => Name;
        }
    }
}
