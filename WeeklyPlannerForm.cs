// WeeklyPlannerForm.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace FoodManager
{
    public partial class WeeklyPlannerForm : Form
    {
        public WeeklyPlannerForm()
        {
            InitializeComponent();
            LoadRecipesIntoCombo();
            // prefill days
            cmbDay.Items.AddRange(new string[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" });
            cmbDay.SelectedIndex = 0;

            // wire events for preview
            cmbRecipe.SelectedIndexChanged += CmbRecipe_SelectedIndexChanged;
            numServings.ValueChanged += NumServings_ValueChanged;
        }

        private void LoadRecipesIntoCombo()
        {
            cmbRecipe.Items.Clear();
            foreach (var r in DataAccess.GetRecipes())
            {
                cmbRecipe.Items.Add(new ComboItem { Id = r.Id, Name = r.Name });
            }
            if (cmbRecipe.Items.Count > 0) cmbRecipe.SelectedIndex = 0;

            UpdateSelectedRecipeInfo();
        }

        private void CmbRecipe_SelectedIndexChanged(object? sender, EventArgs e)
        {
            UpdateSelectedRecipeInfo();
        }

        private void NumServings_ValueChanged(object? sender, EventArgs e)
        {
            UpdateSelectedRecipeInfo();
        }

        private void UpdateSelectedRecipeInfo()
        {
            if (cmbRecipe.SelectedItem is not ComboItem ci) return;
            var recipe = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == ci.Id);
            if (recipe == null) return;

            // Calculate totals for a single "recipe unit"
            var single = recipe.CalculateTotals();

            // compute for selected servings
            var servings = (int)numServings.Value;
            var multi = new NutritionTotals
            {
                Kcal = single.Kcal * servings,
                Protein = single.Protein * servings,
                Carbs = single.Carbs * servings,
                Sugars = single.Sugars * servings,
                Fats = single.Fats * servings,
                Saturated = single.Saturated * servings,
                Cost = Math.Round(single.Cost * servings, 2)
            };
            multi.Round();

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
            var day = cmbDay.SelectedItem?.ToString() ?? "";
            var servings = (int)numServings.Value;
            dgvPlan.Rows.Add(day, ci.Id, ci.Name, servings);
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            // aggregate per day and week
            var dayTotals = new Dictionary<string, NutritionTotals>();
            var weekTotals = new NutritionTotals();

            foreach (DataGridViewRow row in dgvPlan.Rows)
            {
                if (row.IsNewRow) continue;
                var day = Convert.ToString(row.Cells[0].Value) ?? "";
                var recipeId = Convert.ToInt32(row.Cells[1].Value);
                var servings = Convert.ToInt32(row.Cells[3].Value);
                var recipe = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == recipeId);
                if (recipe == null) continue;
                var totals = recipe.CalculateTotals();
                // multiply by servings
                totals.Kcal *= servings;
                totals.Protein *= servings;
                totals.Carbs *= servings;
                totals.Sugars *= servings;
                totals.Fats *= servings;
                totals.Saturated *= servings;
                totals.Cost *= servings;

                if (!dayTotals.ContainsKey(day)) dayTotals[day] = new NutritionTotals();
                var d = dayTotals[day];
                d.Kcal += totals.Kcal;
                d.Protein += totals.Protein;
                d.Carbs += totals.Carbs;
                d.Sugars += totals.Sugars;
                d.Fats += totals.Fats;
                d.Saturated += totals.Saturated;
                d.Cost += totals.Cost;

                // week accum
                weekTotals.Kcal += totals.Kcal;
                weekTotals.Protein += totals.Protein;
                weekTotals.Carbs += totals.Carbs;
                weekTotals.Sugars += totals.Sugars;
                weekTotals.Fats += totals.Fats;
                weekTotals.Saturated += totals.Saturated;
                weekTotals.Cost += totals.Cost;
            }

            // build output
            var lines = new List<string>();
            foreach (var kv in dayTotals)
            {
                var t = kv.Value;
                t.Round();
                lines.Add($"{kv.Key}: Kcal={t.Kcal} | Білки={t.Protein}г | Вуглеводи={t.Carbs}г | Цукри={t.Sugars}г | Жири={t.Fats}г | Вартість={t.Cost} грн");
            }
            lines.Add("");
            weekTotals.Round();
            lines.Add($"Тиждень: Kcal={weekTotals.Kcal} | Білки={weekTotals.Protein}г | Вуглеводи={weekTotals.Carbs}г | Цукри={weekTotals.Sugars}г | Жири={weekTotals.Fats}г | Вартість={weekTotals.Cost} грн");

            txtResult.Lines = lines.ToArray();
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        // Export weekly plan: rows per day/recipe with nutrition (for servings), day subtotals and week total.
        // Column "Вартість (найвигідніша)" shows cheapest total cost for that recipe (for the given servings).
        private void btnExport_Click(object sender, EventArgs e)
        {
            // collect rows from dgvPlan grouped by day
            var planRows = new List<(string day, Recipe recipe, int servings)>();
            foreach (DataGridViewRow row in dgvPlan.Rows)
            {
                if (row.IsNewRow) continue;
                var day = Convert.ToString(row.Cells[0].Value) ?? "";
                var recipeId = Convert.ToInt32(row.Cells[1].Value);
                var servings = Convert.ToInt32(row.Cells[3].Value);
                var recipe = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == recipeId);
                if (recipe == null) continue;
                planRows.Add((day, recipe, servings));
            }

            if (planRows.Count == 0)
            {
                MessageBox.Show("Немає записів у плані для експорту.", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            sfd.FileName = "weekly_plan_export.xlsx";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var package = new ExcelPackage(new FileInfo(sfd.FileName));
                var ws = package.Workbook.Worksheets.Add("WeeklyPlan");

                // Header row
                int c = 1;
                ws.Cells[1, c++].Value = "День";
                ws.Cells[1, c++].Value = "Страва";
                ws.Cells[1, c++].Value = "Порцій";
                ws.Cells[1, c++].Value = "Калорії (ккал)";
                ws.Cells[1, c++].Value = "Білки (г)";
                ws.Cells[1, c++].Value = "Жири (г)";
                ws.Cells[1, c++].Value = "Насичені (г)";
                ws.Cells[1, c++].Value = "Вуглеводи (г)";
                ws.Cells[1, c++].Value = "Цукри (г)";
                ws.Cells[1, c++].Value = "Вартість";

                // style header
                using (var hdr = ws.Cells[1, 1, 1, 10])
                {
                    hdr.Style.Font.Bold = true;
                    hdr.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hdr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 230, 241));
                    hdr.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                var days = planRows.Select(p => p.day).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                decimal weekTotalCost = 0m;

                foreach (var day in days)
                {
                    var rowsForDay = planRows.Where(p => string.Equals(p.day, day, StringComparison.OrdinalIgnoreCase)).ToList();
                    double dayKcal = 0, dayProtein = 0, dayCarbs = 0, dayFats = 0, daySat = 0, daySugars = 0;
                    decimal dayCost = 0m;

                    foreach (var item in rowsForDay)
                    {
                        var recipe = item.recipe;
                        var servings = item.servings;
                        var totals = recipe.CalculateTotals(); // totals for 1 recipe-unit
                        // multiply by servings
                        totals.Kcal *= servings;
                        totals.Protein *= servings;
                        totals.Carbs *= servings;
                        totals.Sugars *= servings;
                        totals.Fats *= servings;
                        totals.Saturated *= servings;
                        totals.Cost *= servings;

                        // write row
                        c = 1;
                        ws.Cells[row, c++].Value = day;
                        ws.Cells[row, c++].Value = recipe.Name;
                        ws.Cells[row, c++].Value = servings;
                        ws.Cells[row, c++].Value = totals.Kcal;
                        ws.Cells[row, c++].Value = totals.Protein;
                        ws.Cells[row, c++].Value = totals.Fats;
                        ws.Cells[row, c++].Value = totals.Saturated;
                        ws.Cells[row, c++].Value = totals.Carbs;
                        ws.Cells[row, c++].Value = totals.Sugars;
                        ws.Cells[row, c++].Value = totals.Cost;

                        // numeric formats
                        ws.Cells[row, 4].Style.Numberformat.Format = "0.0"; // kcal
                        ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##"; // grams
                        ws.Cells[row, 10].Style.Numberformat.Format = "0.##"; // cost

                        dayKcal += totals.Kcal;
                        dayProtein += totals.Protein;
                        dayCarbs += totals.Carbs;
                        dayFats += totals.Fats;
                        daySat += totals.Saturated;
                        daySugars += totals.Sugars;
                        dayCost += totals.Cost;

                        row++;
                    }

                    // Day subtotal row
                    c = 1;
                    ws.Cells[row, c++].Value = $"{day} — Підсумок";
                    // merge name across recipe & servings columns for clarity
                    ws.Cells[row, 1, row, 3].Merge = true;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                    ws.Cells[row, 4].Value = Math.Round(dayKcal, 1);
                    ws.Cells[row, 5].Value = Math.Round(dayProtein, 2);
                    ws.Cells[row, 6].Value = Math.Round(dayFats, 2);
                    ws.Cells[row, 7].Value = Math.Round(daySat, 2);
                    ws.Cells[row, 8].Value = Math.Round(dayCarbs, 2);
                    ws.Cells[row, 9].Value = Math.Round(daySugars, 2);
                    ws.Cells[row, 10].Value = Math.Round(dayCost, 2);

                    // style subtotal row
                    using (var rng = ws.Cells[row, 1, row, 10])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(232, 242, 223)); // light greenish
                    }

                    // set number formats for subtotal
                    ws.Cells[row, 4].Style.Numberformat.Format = "0.0";
                    ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##";
                    ws.Cells[row, 10].Style.Numberformat.Format = "0.##";

                    weekTotalCost += dayCost;
                    row++;
                }

                // Final week total row
                // compute week totals by summing subtotal rows or recompute:
                double weekKcal = 0, weekProtein = 0, weekCarbs = 0, weekFats = 0, weekSat = 0, weekSugars = 0;
                decimal computedWeekCost = 0m;

                // recompute by iterating through planRows (robust)
                foreach (var item in planRows)
                {
                    var totals = item.recipe.CalculateTotals();
                    totals.Kcal *= item.servings;
                    totals.Protein *= item.servings;
                    totals.Carbs *= item.servings;
                    totals.Sugars *= item.servings;
                    totals.Fats *= item.servings;
                    totals.Saturated *= item.servings;
                    totals.Cost *= item.servings;

                    weekKcal += totals.Kcal;
                    weekProtein += totals.Protein;
                    weekCarbs += totals.Carbs;
                    weekFats += totals.Fats;
                    weekSat += totals.Saturated;
                    weekSugars += totals.Sugars;
                    computedWeekCost += totals.Cost;
                }

                // Write week total
                c = 1;
                ws.Cells[row, c++].Value = "Тиждень — Підсумок";
                ws.Cells[row, 1, row, 3].Merge = true;
                ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[row, 4].Value = Math.Round(weekKcal, 1);
                ws.Cells[row, 5].Value = Math.Round(weekProtein, 2);
                ws.Cells[row, 6].Value = Math.Round(weekFats, 2);
                ws.Cells[row, 7].Value = Math.Round(weekSat, 2);
                ws.Cells[row, 8].Value = Math.Round(weekCarbs, 2);
                ws.Cells[row, 9].Value = Math.Round(weekSugars, 2);
                ws.Cells[row, 10].Value = Math.Round(computedWeekCost, 2);

                using (var rng = ws.Cells[row, 1, row, 10])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 242, 204));
                }

                ws.Cells[row, 4].Style.Numberformat.Format = "0.0";
                ws.Cells[row, 5, row, 9].Style.Numberformat.Format = "0.##";
                ws.Cells[row, 10].Style.Numberformat.Format = "0.##";

                // Layout niceties
                ws.Cells[1, 1, row, 10].AutoFitColumns();
                ws.View.FreezePanes(2, 1);
                ws.Cells[1, 1, row, 10].Style.Font.Name = "Calibri";

                package.Save();

                MessageBox.Show($"Excel збережено: {sfd.FileName}", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні XLSX: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // aggregate ingredients helper (left as previously implemented)
        private Dictionary<string, (int qty, string unit)> AggregateIngredientsFromPlan()
        {
            var recipeNeeds = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row in dgvPlan.Rows)
            {
                if (row.IsNewRow) continue;
                var recipeId = Convert.ToInt32(row.Cells[1].Value);
                var servings = Convert.ToInt32(row.Cells[3].Value);
                var recipe = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == recipeId);
                if (recipe == null) continue;
                foreach (var kv in recipe.Ingredients)
                {
                    var ingName = kv.Key;
                    var amount = kv.Value; // IngredientAmount
                    var totalQty = amount.Quantity * servings;
                    if (!recipeNeeds.ContainsKey(ingName)) recipeNeeds[ingName] = (0, amount.Unit);
                    var cur = recipeNeeds[ingName];
                    if (cur.unit == amount.Unit)
                    {
                        recipeNeeds[ingName] = (cur.qty + totalQty, cur.unit);
                    }
                    else
                    {
                        // attempt conversion via product weight
                        var prod = DataAccess.GetProductByName(ingName);
                        if (prod != null && prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                        {
                            int curAsGrams = cur.unit == "pcs" ? cur.qty * prod.WeightPerUnitG.Value : cur.qty;
                            int addAsGrams = amount.Unit == "pcs" ? totalQty * prod.WeightPerUnitG.Value : totalQty;
                            recipeNeeds[ingName] = (curAsGrams + addAsGrams, "g");
                        }
                        else
                        {
                            // cannot convert reliably; sum quantities in original unit (keep existing unit)
                            recipeNeeds[ingName] = (cur.qty + totalQty, cur.unit);
                        }
                    }
                }
            }

            // Convert recipeNeeds to product.InventoryUnit when possible (so shopping planner grid uses product unit)
            var result = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            foreach (var kv in recipeNeeds)
            {
                var name = kv.Key;
                var qty = kv.Value.qty;
                var unit = kv.Value.unit;
                var prod = DataAccess.GetProductByName(name);
                if (prod != null && !string.IsNullOrEmpty(prod.InventoryUnit))
                {
                    var targetUnit = prod.InventoryUnit;
                    if (string.Equals(unit, targetUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        result[name] = (qty, unit);
                    }
                    else
                    {
                        // Need conversion
                        if (unit == "g" && targetUnit == "pcs")
                        {
                            if (prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                            {
                                int pcs = (int)Math.Ceiling((double)qty / prod.WeightPerUnitG.Value);
                                result[name] = (pcs, "pcs");
                            }
                            else
                            {
                                // fallback: keep grams if cannot convert
                                result[name] = (qty, "g");
                            }
                        }
                        else if (unit == "pcs" && targetUnit == "g")
                        {
                            if (prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                            {
                                int grams = qty * prod.WeightPerUnitG.Value;
                                result[name] = (grams, "g");
                            }
                            else
                            {
                                result[name] = (qty, "pcs");
                            }
                        }
                        else
                        {
                            // unknown combination — keep original
                            result[name] = (qty, unit);
                        }
                    }
                }
                else
                {
                    // product not found or no inventory unit specified — preserve aggregated unit
                    result[name] = (qty, unit);
                }
            }

            return result;
        }

        // button to open shopping planner prefilled (assumes ShoppingPlannerForm has constructor accepting prefill)
        private void btnOpenShopping_Click(object sender, EventArgs e)
        {
            var aggregated = AggregateIngredientsFromPlan();
            if (aggregated == null || aggregated.Count == 0)
            {
                MessageBox.Show("Немає інгредієнтів у плані для експорту в план закупок.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sp = new ShoppingPlannerForm(aggregated);
            sp.ShowDialog();
        }

        private class ComboItem
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public override string ToString() => Name;
        }
    }
}