using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FoodManager.Common;

namespace FoodManager
{
    public partial class MainForm : Form
    {
        private List<Product> _allProducts = new();
        private List<Recipe> _allRecipes = new();
        private List<InventorySummary> _allInventory = new();

        public MainForm()
        {
            InitializeComponent();

            ApplyModernGridStyles(dgvProducts);
            ApplyModernGridStyles(dgvRecipes);
            ApplyModernGridStyles(dgvInventory);

            InitializeFilters();
            WireEvents();

            DataAccess.EnsureDatabase();

            LoadProducts();
            LoadRecipes();
            LoadInventoryView();
        }

        private static void ApplyModernGridStyles(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(243, 244, 246);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 41, 55);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.RowTemplate.Height = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 252);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            grid.GridColor = Color.FromArgb(229, 231, 235);
            grid.BorderStyle = BorderStyle.None;
        }

        private void InitializeFilters()
        {
            cmbFilterRecipeCategory.Items.Clear();
            cmbFilterRecipeCategory.Items.AddRange(new object[]
            {
                "Всі категорії", "сніданок", "обід", "вечеря", "перекус"
            });
            cmbFilterRecipeCategory.SelectedIndex = 0;
        }

        private void WireEvents()
        {
            // Products
            btnAddProduct.Click += btnAddProduct_Click;
            btnEditProduct.Click += btnEditProduct_Click;
            btnDeleteProduct.Click += btnDeleteProduct_Click;
            btnUpdatePrices.Click += btnUpdatePrices_Click;
            txtSearchProduct.TextChanged += (s, e) => ApplyProductFilter();
            btnClearProductSearch.Click += (s, e) => txtSearchProduct.Clear();
            btnMicronutrients.Click += BtnMicronutrients_Click;
            btnBackupDb.Click += BtnBackupDb_Click;
            btnRestoreDb.Click += BtnRestoreDb_Click;

            // Recipes
            btnAddRecipe.Click += btnAddRecipe_Click;
            btnEditRecipe.Click += btnEditRecipe_Click;
            btnDeleteRecipe.Click += btnDeleteRecipe_Click;
            btnWeeklyPlan.Click += btnWeeklyPlan_Click;
            txtSearchRecipe.TextChanged += (s, e) => ApplyRecipeFilter();
            cmbFilterRecipeCategory.SelectedIndexChanged += (s, e) => ApplyRecipeFilter();
            btnRecipeMicronutrients.Click += BtnRecipeMicronutrients_Click;

            // Inventory
            btnAddInventory.Click += btnAddInventory_Click;
            btnReduceInventory.Click += btnReduceInventory_Click;
            btnGenerateShopping.Click += btnGenerateShopping_Click;
            btnRecountAll.Click += btnRecountAll_Click;
            btnRecountHistory.Click += btnRecountHistory_Click;
            txtSearchInventory.TextChanged += (s, e) => ApplyInventoryFilter();
            chkOnlyLowStock.CheckedChanged += (s, e) => ApplyInventoryFilter();
            btnRestockLow.Click += BtnRestockLow_Click;
            btnQuickTelegram.Click += BtnQuickTelegram_Click;
        }

        private void UpdateStats()
        {
            lblStatProducts.Text = $"🍏 Продуктів: {_allProducts.Count}";
            lblStatRecipes.Text = $"🍲 Страв: {_allRecipes.Count}";

            int lowStockCount = _allInventory.Count(i => i.Quantity <= 200);
            lblStatInventory.Text = $"📦 Позицій на складі: {_allInventory.Count}";
            lblStatLowStock.Text = $"⚠️ Закінчуються: {lowStockCount}";
            lblStatLowStock.ForeColor = lowStockCount > 0 ? Color.FromArgb(198, 40, 40) : Color.FromArgb(46, 125, 50);
        }

        #region Products Tab

        private void LoadProducts()
        {
            _allProducts = DataAccess.GetProducts().ToList();
            ApplyProductFilter();
            UpdateStats();
        }

        private void ApplyProductFilter()
        {
            dgvProducts.Rows.Clear();
            var query = txtSearchProduct.Text.Trim();

            var filtered = _allProducts.AsEnumerable();
            if (!string.IsNullOrEmpty(query))
            {
                filtered = filtered.Where(p =>
                    p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    p.Barcodes.Any(b => b.Contains(query, StringComparison.OrdinalIgnoreCase)));
            }

            foreach (var p in filtered)
            {
                var pricesText = string.Empty;
                if (p.Prices != null && p.Prices.Count > 0)
                {
                    var parts = new List<string>();
                    foreach (var kv in p.Prices)
                    {
                        var norm = p.NormalizePriceToDefault(kv.Key, kv.Value);
                        if (norm.HasValue)
                        {
                            parts.Add($"{kv.Key}: {Math.Round(norm.Value, 2).ToString(CultureInfo.InvariantCulture)}");
                        }
                        else
                        {
                            var storePack = p.GetPackSizeForStore(kv.Key);
                            parts.Add($"{kv.Key}: {kv.Value.ToString(CultureInfo.InvariantCulture)} ({storePack})");
                        }
                    }
                    pricesText = string.Join("; ", parts);
                }

                var barcodes = p.Barcodes != null ? string.Join(", ", p.Barcodes) : string.Empty;
                dgvProducts.Rows.Add(p.Id, p.Name, p.WeightPerUnitG ?? 0, pricesText, barcodes, p.InventoryUnit);
            }

            if (dgvProducts.Columns.Contains("pNameCol") && dgvProducts.Rows.Count > 0)
            {
                dgvProducts.Sort(dgvProducts.Columns["pNameCol"], ListSortDirection.Ascending);
            }
        }

        private void btnAddProduct_Click(object? sender, EventArgs e)
        {
            using var f = new AddProductForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                DataAccess.InsertProduct(f.Product);
                LoadProducts();
                LoadInventoryView();
            }
        }

        private void btnEditProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0 && dgvProducts.CurrentRow == null) return;
            var row = dgvProducts.SelectedRows.Count > 0 ? dgvProducts.SelectedRows[0] : dgvProducts.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = _allProducts.FirstOrDefault(p => p.Id == id);
            if (prod == null) return;

            using var f = new AddProductForm(prod);
            if (f.ShowDialog() == DialogResult.OK)
            {
                f.Product.Id = id;
                DataAccess.UpdateProduct(f.Product);
                LoadProducts();
                LoadInventoryView();
            }
        }

        private void btnDeleteProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0 && dgvProducts.CurrentRow == null) return;
            var row = dgvProducts.SelectedRows.Count > 0 ? dgvProducts.SelectedRows[0] : dgvProducts.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prodName = Convert.ToString(row.Cells[1].Value);
            if (MessageBox.Show($"Видалити продукт '{prodName}'?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            DataAccess.DeleteProduct(id);
            LoadProducts();
            LoadInventoryView();
        }

        private void BtnMicronutrients_Click(object? sender, EventArgs e)
        {
            int? selectedProductId = null;
            if (dgvProducts.CurrentRow != null)
            {
                selectedProductId = Convert.ToInt32(dgvProducts.CurrentRow.Cells[0].Value);
            }

            using var f = new MicronutrientManagerForm(initialProductId: selectedProductId);
            f.ShowDialog();
            LoadProducts();
        }

        private void BtnBackupDb_Click(object? sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                FileName = $"food_backup_{DateTime.Now:yyyyMMdd_HHmm}.db"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataAccess.BackupDatabase(sfd.FileName);
                    MessageBox.Show($"Резервну копію бази даних успішно створено:\n{sfd.FileName}", "Резервне копіювання", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка створення резервної копії: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnRestoreDb_Click(object? sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (MessageBox.Show("Відновлення бази даних замінить поточні дані. Продовжити?", "Підтвердження відновлення", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DataAccess.RestoreDatabase(ofd.FileName);
                        LoadProducts();
                        LoadRecipes();
                        LoadInventoryView();
                        MessageBox.Show("Базу даних успішно відновлено!", "Відновлено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка відновлення: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Recipes Tab

        private void LoadRecipes()
        {
            _allRecipes = DataAccess.GetRecipes().ToList();
            ApplyRecipeFilter();
            UpdateStats();
        }

        private void ApplyRecipeFilter()
        {
            dgvRecipes.Rows.Clear();
            var productsLookup = _allProducts.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var query = txtSearchRecipe.Text.Trim();
            var selectedCategory = cmbFilterRecipeCategory.SelectedItem?.ToString();

            var filtered = _allRecipes.AsEnumerable();
            if (!string.IsNullOrEmpty(query))
            {
                filtered = filtered.Where(r => r.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                               r.Ingredients.Keys.Any(k => k.Contains(query, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(selectedCategory) && selectedCategory != "Всі категорії")
            {
                filtered = filtered.Where(r => string.Equals(r.Status, selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var r in filtered)
            {
                var ingreds = r.Ingredients != null && r.Ingredients.Count > 0
                    ? string.Join("; ", r.Ingredients.Select(kv => $"{kv.Key}:{kv.Value.Quantity}{kv.Value.Unit}"))
                    : string.Empty;

                var totals = r.CalculateTotals(productsLookup);
                var kcalText = $"{totals.Kcal} ккал";
                var costText = totals.Cost > 0 ? $"{totals.Cost:0.00} грн" : string.Empty;

                dgvRecipes.Rows.Add(r.Id, r.Name, r.Status, ingreds, kcalText, costText);
            }

            if (dgvRecipes.Columns.Contains("rNameCol") && dgvRecipes.Rows.Count > 0)
            {
                dgvRecipes.Sort(dgvRecipes.Columns["rNameCol"], ListSortDirection.Ascending);
            }
        }

        private void btnAddRecipe_Click(object? sender, EventArgs e)
        {
            using var f = new AddRecipeForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                DataAccess.InsertRecipe(f.Recipe);
                LoadRecipes();
            }
        }

        private void btnEditRecipe_Click(object? sender, EventArgs e)
        {
            if (dgvRecipes.SelectedRows.Count == 0 && dgvRecipes.CurrentRow == null) return;
            var row = dgvRecipes.SelectedRows.Count > 0 ? dgvRecipes.SelectedRows[0] : dgvRecipes.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var rec = _allRecipes.FirstOrDefault(r => r.Id == id);
            if (rec == null) return;

            using var f = new AddRecipeForm(rec);
            if (f.ShowDialog() == DialogResult.OK)
            {
                f.Recipe.Id = id;
                DataAccess.UpdateRecipe(f.Recipe);
                LoadRecipes();
            }
        }

        private void btnDeleteRecipe_Click(object? sender, EventArgs e)
        {
            if (dgvRecipes.SelectedRows.Count == 0 && dgvRecipes.CurrentRow == null) return;
            var row = dgvRecipes.SelectedRows.Count > 0 ? dgvRecipes.SelectedRows[0] : dgvRecipes.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var recName = Convert.ToString(row.Cells[1].Value);
            if (MessageBox.Show($"Видалити страву '{recName}'?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            DataAccess.DeleteRecipe(id);
            LoadRecipes();
        }

        private void btnWeeklyPlan_Click(object? sender, EventArgs e)
        {
            using var f = new WeeklyPlannerForm();
            f.ShowDialog();
        }

        private void BtnRecipeMicronutrients_Click(object? sender, EventArgs e)
        {
            int? selectedRecipeId = null;
            if (dgvRecipes.CurrentRow != null)
            {
                selectedRecipeId = Convert.ToInt32(dgvRecipes.CurrentRow.Cells[0].Value);
            }

            using var f = new MicronutrientManagerForm(initialRecipeId: selectedRecipeId);
            f.ShowDialog();
        }

        #endregion

        #region Inventory Tab

        private void LoadInventoryView()
        {
            _allInventory = DataAccess.GetAllInventorySummaries().ToList();
            ApplyInventoryFilter();
            UpdateStats();
        }

        private void ApplyInventoryFilter()
        {
            dgvInventory.Rows.Clear();
            var query = txtSearchInventory.Text.Trim();
            var onlyLow = chkOnlyLowStock.Checked;

            var filtered = _allInventory.AsEnumerable();
            if (!string.IsNullOrEmpty(query))
            {
                filtered = filtered.Where(i => i.ProductName.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            if (onlyLow)
            {
                filtered = filtered.Where(i => i.Quantity <= 200);
            }

            foreach (var item in filtered)
            {
                string displayText = item.DisplayText;
                if (item.Quantity <= 0)
                {
                    displayText = $"{item.Quantity} {StoreConstants.GetDisplayUnit(item.Unit)} (Вичерпано!)";
                }
                else if (item.Quantity <= 200)
                {
                    displayText = $"{item.Quantity} {StoreConstants.GetDisplayUnit(item.Unit)} (Закінчується)";
                }

                int rowIdx = dgvInventory.Rows.Add(item.ProductId, item.ProductName, displayText);
                var row = dgvInventory.Rows[rowIdx];

                // Visual row tinting for low/zero stock
                if (item.Quantity <= 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                }
                else if (item.Quantity <= 200)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 249, 195);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(161, 98, 7);
                }
            }

            if (dgvInventory.Columns.Contains("iNameCol") && dgvInventory.Rows.Count > 0)
            {
                dgvInventory.Sort(dgvInventory.Columns["iNameCol"], ListSortDirection.Ascending);
            }
        }

        private void btnAddInventory_Click(object? sender, EventArgs e)
        {
            var row = dgvInventory.CurrentRow ?? dgvProducts.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = _allProducts.FirstOrDefault(p => p.Id == id);
            if (prod == null) return;

            using var f = new InventoryEditForm(prod.Id, prod.Name, prod.InventoryUnit, InventoryEditForm.Mode.Add);
            if (f.ShowDialog() == DialogResult.OK && f.Quantity > 0)
            {
                DataAccess.AddInventory(id, f.Quantity, prod.InventoryUnit);
                LoadInventoryView();
            }
        }

        private void btnReduceInventory_Click(object? sender, EventArgs e)
        {
            var row = dgvInventory.CurrentRow ?? dgvProducts.CurrentRow;
            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = _allProducts.FirstOrDefault(p => p.Id == id);
            if (prod == null) return;

            using var f = new InventoryEditForm(prod.Id, prod.Name, prod.InventoryUnit, InventoryEditForm.Mode.Reduce);
            if (f.ShowDialog() == DialogResult.OK && f.Quantity > 0)
            {
                DataAccess.ReduceInventory(id, f.Quantity, prod.InventoryUnit);
                LoadInventoryView();
            }
        }

        private void btnGenerateShopping_Click(object? sender, EventArgs e)
        {
            using var f = new ShoppingPlannerForm();
            f.ShowDialog();
            LoadInventoryView();
        }

        private void BtnRestockLow_Click(object? sender, EventArgs e)
        {
            var lowItems = _allInventory.Where(i => i.Quantity <= 200).ToList();
            if (lowItems.Count == 0)
            {
                MessageBox.Show("Усі продукти на складі у достатній кількості (> 200г / шт)!", "Інвентаризація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var prefill = lowItems.ToDictionary(
                i => i.ProductName,
                i => (Math.Max(1000, i.Quantity <= 0 ? 1000 : 500), i.Unit),
                StringComparer.OrdinalIgnoreCase);

            using var f = new ShoppingPlannerForm(prefill);
            f.ShowDialog();
            LoadInventoryView();
        }

        private void BtnQuickTelegram_Click(object? sender, EventArgs e)
        {
            var lowItems = _allInventory.Where(i => i.Quantity <= 200).ToList();
            if (lowItems.Count == 0)
            {
                MessageBox.Show("Усі продукти на складі в наявності!", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("⚠️ ПОТРІБНО ЗАКУПИТИ (Закінчились на складі):");
            sb.AppendLine($"📅 {DateTime.Now:dd.MM.yyyy HH:mm}");
            sb.AppendLine("──────────────────────");
            foreach (var item in lowItems)
            {
                var status = item.Quantity <= 0 ? "❗ ВИЧЕРПАНО" : $"залишок: {item.DisplayText}";
                sb.AppendLine($"▫️ {item.ProductName} ({status})");
            }
            sb.AppendLine("──────────────────────");
            sb.AppendLine($"Всього позицій для поповнення: {lowItems.Count}");

            try
            {
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Список дефіцитних товарів скопійовано у буфер обміну для відправки в Telegram / Viber!", "Скопійовано", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка роботи з буфером: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRecountAll_Click(object? sender, EventArgs e)
        {
            using var f = new RecountAllForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadInventoryView();
            }
        }

        private void btnRecountHistory_Click(object? sender, EventArgs e)
        {
            using var f = new RecountHistoryForm();
            f.ShowDialog();
        }

        #endregion

        #region Price Updating

        private async void btnUpdatePrices_Click(object? sender, EventArgs e)
        {
            btnUpdatePrices.Enabled = false;
            var products = DataAccess.GetProducts().ToList();
            if (products.Count == 0)
            {
                MessageBox.Show("Немає продуктів для обробки.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdatePrices.Enabled = true;
                return;
            }

            var progressForm = new ProgressForm();
            progressForm.Show(this);
            var cts = progressForm.Cancellation;

            int parallelism = 2;
            try
            {
                await PriceUpdaters.EnsurePlaywrightAsync(parallelism);
            }
            catch { }

            int total = products.Count;
            int processed = 0;
            int updated = 0;
            int errors = 0;

            try
            {
                await Parallel.ForEachAsync(products, new ParallelOptions { MaxDegreeOfParallelism = parallelism, CancellationToken = cts.Token }, async (p, ct) =>
                {
                    try
                    {
                        progressForm.ReportProgress(Volatile.Read(ref processed), total, $"Обробка: {p.Name}");
                    }
                    catch { }

                    try
                    {
                        var changed = await PriceUpdaterDebug.UpdateProductWithLogging(p, ct).ConfigureAwait(false);
                        if (changed)
                        {
                            DataAccess.UpdateProduct(p);
                            Interlocked.Increment(ref updated);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        Interlocked.Increment(ref errors);
                    }
                    finally
                    {
                        Interlocked.Increment(ref processed);
                        try
                        {
                            progressForm.ReportProgress(processed, total, $"Оброблено: {p.Name} ({processed}/{total})");
                        }
                        catch { }
                    }
                });
            }
            catch (OperationCanceledException) { }
            finally
            {
                try { await PriceUpdaters.DisposePlaywrightAsync().ConfigureAwait(false); } catch { }

                Invoke(new Action(() =>
                {
                    try
                    {
                        if (!progressForm.IsDisposed)
                        {
                            progressForm.Close();
                            progressForm.Dispose();
                        }
                    }
                    catch { }

                    btnUpdatePrices.Enabled = true;
                    LoadProducts();
                    MessageBox.Show($"Оновлення цін завершено.\n\nОновлено товарів: {updated}\nПомилок: {errors}", "Актуалізація цін", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
        }

        #endregion
    }
}
