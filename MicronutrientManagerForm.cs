using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using FoodManager.Common;
using FoodManager.Services.Database;

namespace FoodManager
{
    public partial class MicronutrientManagerForm : Form
    {
        private readonly IDatabaseService _db;
        private List<Product> _products = new();
        private List<Recipe> _recipes = new();
        private Product? _currentProduct;

        public MicronutrientManagerForm(IDatabaseService? db = null, int? initialProductId = null, int? initialRecipeId = null)
        {
            InitializeComponent();

            _db = db ?? DatabaseService.Default;

            InitializeGrids();
            LoadData();

            cmbProduct.SelectedIndexChanged += CmbProduct_SelectedIndexChanged;
            cmbRecipe.SelectedIndexChanged += CmbRecipe_SelectedIndexChanged;
            numServings.ValueChanged += NumServings_ValueChanged;
            btnSaveProductNutrients.Click += BtnSaveProductNutrients_Click;
            btnClose.Click += (s, e) => Close();

            if (initialProductId.HasValue)
            {
                for (int i = 0; i < cmbProduct.Items.Count; i++)
                {
                    if (cmbProduct.Items[i] is Product p && p.Id == initialProductId.Value)
                    {
                        cmbProduct.SelectedIndex = i;
                        tabControl.SelectedTab = tabProductAnalysis;
                        break;
                    }
                }
            }

            if (initialRecipeId.HasValue)
            {
                for (int i = 0; i < cmbRecipe.Items.Count; i++)
                {
                    if (cmbRecipe.Items[i] is Recipe r && r.Id == initialRecipeId.Value)
                    {
                        cmbRecipe.SelectedIndex = i;
                        tabControl.SelectedTab = tabRecipeAnalysis;
                        break;
                    }
                }
            }
        }

        private void InitializeGrids()
        {
            // Product Nutrients Grid
            dgvProductNutrients.Columns.Clear();
            dgvProductNutrients.Columns.Add("keyCol", "Key");
            dgvProductNutrients.Columns["keyCol"].Visible = false;

            dgvProductNutrients.Columns.Add("catCol", "Категорія");
            dgvProductNutrients.Columns["catCol"].ReadOnly = true;
            dgvProductNutrients.Columns["catCol"].Width = 140;

            dgvProductNutrients.Columns.Add("nameCol", "Мікроелемент / Вітамін");
            dgvProductNutrients.Columns["nameCol"].ReadOnly = true;
            dgvProductNutrients.Columns["nameCol"].Width = 240;

            dgvProductNutrients.Columns.Add("valCol", "Вміст");
            dgvProductNutrients.Columns["valCol"].Width = 120;

            dgvProductNutrients.Columns.Add("unitCol", "Одиниця");
            dgvProductNutrients.Columns["unitCol"].ReadOnly = true;
            dgvProductNutrients.Columns["unitCol"].Width = 100;

            dgvProductNutrients.Columns.Add("rdaCol", "Добова норма (RDA)");
            dgvProductNutrients.Columns["rdaCol"].ReadOnly = true;
            dgvProductNutrients.Columns["rdaCol"].Width = 160;

            dgvProductNutrients.Columns.Add("pctCol", "% добової норми");
            dgvProductNutrients.Columns["pctCol"].ReadOnly = true;
            dgvProductNutrients.Columns["pctCol"].Width = 150;

            // Recipe Nutrients Grid
            dgvRecipeNutrients.Columns.Clear();
            dgvRecipeNutrients.Columns.Add("catCol", "Категорія");
            dgvRecipeNutrients.Columns["catCol"].Width = 140;

            dgvRecipeNutrients.Columns.Add("nameCol", "Мікроелемент / Вітамін");
            dgvRecipeNutrients.Columns["nameCol"].Width = 240;

            dgvRecipeNutrients.Columns.Add("valCol", "У порціях");
            dgvRecipeNutrients.Columns["valCol"].Width = 130;

            dgvRecipeNutrients.Columns.Add("unitCol", "Одиниця");
            dgvRecipeNutrients.Columns["unitCol"].Width = 100;

            dgvRecipeNutrients.Columns.Add("rdaCol", "Добова норма");
            dgvRecipeNutrients.Columns["rdaCol"].Width = 140;

            dgvRecipeNutrients.Columns.Add("pctCol", "% добової норми");
            dgvRecipeNutrients.Columns["pctCol"].Width = 150;

            // Reference Grid
            dgvReference.Columns.Clear();
            dgvReference.Columns.Add("catCol", "Категорія");
            dgvReference.Columns["catCol"].Width = 150;

            dgvReference.Columns.Add("nameCol", "Найменування");
            dgvReference.Columns["nameCol"].Width = 260;

            dgvReference.Columns.Add("rdaCol", "Рекомендована добова норма (ВООЗ / МОЗ)");
            dgvReference.Columns["rdaCol"].Width = 320;

            dgvReference.Columns.Add("unitCol", "Одиниця виміру");
            dgvReference.Columns["unitCol"].Width = 140;

            // Populate Reference Data
            foreach (var item in MicronutrientStandards.Standards)
            {
                dgvReference.Rows.Add(item.Category, item.Name, item.DailyRda, item.Unit);
            }
        }

        private void LoadData()
        {
            _products = _db.GetProducts().OrderBy(p => p.Name).ToList();
            _recipes = _db.GetRecipes().OrderBy(r => r.Name).ToList();

            cmbProduct.DisplayMember = "Name";
            cmbProduct.DataSource = _products;

            cmbRecipe.DisplayMember = "Name";
            cmbRecipe.DataSource = _recipes;

            if (_products.Count > 0)
            {
                cmbProduct.SelectedIndex = 0;
            }

            if (_recipes.Count > 0)
            {
                cmbRecipe.SelectedIndex = 0;
            }
        }

        private void CmbProduct_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbProduct.SelectedItem is not Product p) return;
            _currentProduct = p;

            lblProductUnitHint.Text = StoreConstants.IsPieces(p.InventoryUnit)
                ? "Вміст вказано на 1 штуку"
                : "Вміст вказано на 100 грамів продукту";

            dgvProductNutrients.Rows.Clear();
            var micro = p.Micronutrients ?? new MicronutrientInfo();

            foreach (var std in MicronutrientStandards.Standards)
            {
                var val = MicronutrientStandards.GetValue(micro, std.Key);
                var pct = std.DailyRda > 0 ? (val / std.DailyRda) * 100.0 : 0;
                var pctText = $"{pct:F1}%";

                int rowIdx = dgvProductNutrients.Rows.Add(std.Key, std.Category, std.Name, val.ToString("0.##", CultureInfo.InvariantCulture), std.Unit, $"{std.DailyRda} {std.Unit}", pctText);

                ApplyPercentageColor(dgvProductNutrients.Rows[rowIdx].Cells["pctCol"], pct);
            }
        }

        private void BtnSaveProductNutrients_Click(object? sender, EventArgs e)
        {
            if (_currentProduct == null) return;

            var micro = _currentProduct.Micronutrients ?? new MicronutrientInfo();

            foreach (DataGridViewRow row in dgvProductNutrients.Rows)
            {
                if (row.IsNewRow) continue;
                var key = Convert.ToString(row.Cells["keyCol"].Value);
                if (string.IsNullOrEmpty(key)) continue;

                var valStr = Convert.ToString(row.Cells["valCol"].Value)?.Replace(',', '.');
                if (double.TryParse(valStr, NumberStyles.Number, CultureInfo.InvariantCulture, out var parsedVal))
                {
                    MicronutrientStandards.SetValue(micro, key, parsedVal);
                }
            }

            micro.Round();
            _currentProduct.Micronutrients = micro;

            try
            {
                _db.UpdateProduct(_currentProduct);
                MessageBox.Show($"Мікроелементи для '{_currentProduct.Name}' успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CmbProduct_SelectedIndexChanged(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbRecipe_SelectedIndexChanged(object? sender, EventArgs e) => UpdateRecipeAnalysis();

        private void NumServings_ValueChanged(object? sender, EventArgs e) => UpdateRecipeAnalysis();

        private void UpdateRecipeAnalysis()
        {
            if (cmbRecipe.SelectedItem is not Recipe r) return;

            var servings = (double)numServings.Value;
            dgvRecipeNutrients.Rows.Clear();

            var totalMicro = new MicronutrientInfo();
            var productsLookup = _products.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in r.Ingredients)
            {
                if (!productsLookup.TryGetValue(kvp.Key, out var prod) || prod.Micronutrients == null) continue;

                double factor = 0;
                if (StoreConstants.IsGrams(prod.InventoryUnit) && StoreConstants.IsGrams(kvp.Value.Unit))
                {
                    factor = (kvp.Value.Quantity / 100.0) * servings;
                }
                else if (StoreConstants.IsPieces(prod.InventoryUnit) && StoreConstants.IsPieces(kvp.Value.Unit))
                {
                    factor = kvp.Value.Quantity * servings;
                }

                if (factor > 0)
                {
                    totalMicro.AddScaled(prod.Micronutrients, factor);
                }
            }

            totalMicro.Round();
            int coveredStandards = 0;

            foreach (var std in MicronutrientStandards.Standards)
            {
                var val = MicronutrientStandards.GetValue(totalMicro, std.Key);
                var pct = std.DailyRda > 0 ? (val / std.DailyRda) * 100.0 : 0;
                var pctText = $"{pct:F1}%";

                if (pct >= 80) coveredStandards++;

                int rowIdx = dgvRecipeNutrients.Rows.Add(std.Category, std.Name, val.ToString("0.##", CultureInfo.InvariantCulture), std.Unit, $"{std.DailyRda} {std.Unit}", pctText);

                ApplyPercentageColor(dgvRecipeNutrients.Rows[rowIdx].Cells["pctCol"], pct);
            }

            lblRecipeSummary.Text = $"Підсумок: {coveredStandards} з {MicronutrientStandards.Standards.Count} мікроелементів покривають >80% добової норми.";
        }

        private static void ApplyPercentageColor(DataGridViewCell cell, double pct)
        {
            if (pct >= 80 && pct <= 130)
            {
                cell.Style.BackColor = Color.FromArgb(232, 245, 233); // Soft green
                cell.Style.ForeColor = Color.FromArgb(27, 94, 32);
                cell.Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
            else if (pct > 130)
            {
                cell.Style.BackColor = Color.FromArgb(227, 242, 253); // Soft blue (rich/plentiful)
                cell.Style.ForeColor = Color.FromArgb(13, 71, 161);
            }
            else if (pct < 40 && pct > 0)
            {
                cell.Style.BackColor = Color.FromArgb(255, 243, 224); // Soft orange (low)
                cell.Style.ForeColor = Color.FromArgb(230, 81, 0);
            }
            else if (pct == 0)
            {
                cell.Style.ForeColor = Color.FromArgb(150, 150, 150); // Gray (none)
            }
        }
    }
}
