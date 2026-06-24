using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;

namespace FoodManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            DataAccess.EnsureDatabase();
            LoadProducts();
            LoadRecipes();
            LoadInventoryView();
        }

        private void LoadProducts()
        {
            dgvProducts.Rows.Clear();
            foreach (var p in DataAccess.GetProducts())
            {
                // Build normalized prices string (normalized to product.PackDefaultSize)
                var pricesText = "";
                if (p.Prices != null && p.Prices.Count > 0)
                {
                    var parts = new System.Collections.Generic.List<string>();
                    foreach (var kv in p.Prices)
                    {
                        var store = kv.Key;
                        var rawPrice = kv.Value;
                        var norm = p.NormalizePriceToDefault(store, rawPrice);
                        if (norm.HasValue)
                        {
                            parts.Add($"{store}:{Math.Round(norm.Value, 2).ToString(CultureInfo.InvariantCulture)}");
                        }
                        else
                        {
                            // fallback to showing original price with pack if available
                            var storePack = p.GetPackSizeForStore(store);
                            parts.Add($"{store}:{rawPrice.ToString(CultureInfo.InvariantCulture)} ({storePack})");
                        }
                    }
                    pricesText = string.Join(";", parts);
                }
                var barcodes = ""; // hidden now
                dgvProducts.Rows.Add(p.Id, p.Name, p.WeightPerUnitG ?? 0, pricesText, barcodes, p.InventoryUnit);
            }
            dgvProducts.Sort(dgvProducts.Columns["pNameCol"],ListSortDirection.Ascending);

        }

        private void LoadRecipes()
        {
            dgvRecipes.Rows.Clear();
            foreach (var r in DataAccess.GetRecipes())
            {
                var ingreds = r.Ingredients != null && r.Ingredients.Count > 0
                    ? string.Join(";", r.Ingredients.Select(kv => $"{kv.Key}:{kv.Value.Quantity}{kv.Value.Unit}"))
                    : "";

                var totals = r.CalculateTotals();
                var kcalText = $"{totals.Kcal} kcal";
                var costText = totals.Cost > 0 ? $"{totals.Cost} грн" : "";

                dgvRecipes.Rows.Add(r.Id, r.Name, r.Status, ingreds, kcalText, costText);
            }
            dgvRecipes.Sort(dgvRecipes.Columns["rNameCol"], ListSortDirection.Ascending);
        }

        private void btnWeeklyPlan_Click(object sender, EventArgs e)
        {
            using var f = new WeeklyPlannerForm();
            f.ShowDialog();
        }

        private void LoadInventoryView()
        {
            dgvInventory.Rows.Clear();
            foreach (var p in DataAccess.GetProducts())
            {
                var (qty, unit) = DataAccess.GetInventoryQuantityForProduct(p.Name);
                var disp = unit == "pcs" ? $"{qty} шт" : $"{qty} г";
                dgvInventory.Rows.Add(p.Id, p.Name, disp);
            }
            dgvInventory.Sort(dgvInventory.Columns["iNameCol"], ListSortDirection.Ascending);
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            using var f = new AddProductForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                DataAccess.InsertProduct(f.Product);
                LoadProducts();
                LoadInventoryView();
            }
        }

        private void btnEditProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0 && dgvProducts.CurrentRow == null) return;
            var row = dgvProducts.SelectedRows.Count > 0 ? dgvProducts.SelectedRows[0] : dgvProducts.CurrentRow;
            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = DataAccess.GetProducts().FirstOrDefault(p => p.Id == id);
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

        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0 && dgvProducts.CurrentRow == null) return;
            var row = dgvProducts.SelectedRows.Count > 0 ? dgvProducts.SelectedRows[0] : dgvProducts.CurrentRow;
            var id = Convert.ToInt32(row.Cells[0].Value);
            var prodName = Convert.ToString(row.Cells[1].Value);
            if (MessageBox.Show($"Видалити продукт '{prodName}'?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            DataAccess.DeleteProduct(id);
            LoadProducts();
            LoadInventoryView();
        }

        private void btnAddRecipe_Click(object sender, EventArgs e)
        {
            using var f = new AddRecipeForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                DataAccess.InsertRecipe(f.Recipe);
                LoadRecipes();
            }
        }

        private void btnEditRecipe_Click(object sender, EventArgs e)
        {
            if (dgvRecipes.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvRecipes.SelectedRows[0].Cells[0].Value);
            var rec = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == id);
            if (rec == null) return;
            using var f = new AddRecipeForm(rec);
            if (f.ShowDialog() == DialogResult.OK)
            {
                f.Recipe.Id = id;
                DataAccess.UpdateRecipe(f.Recipe);
                LoadRecipes();
            }
        }

        private void btnDeleteRecipe_Click(object sender, EventArgs e)
        {
            if (dgvRecipes.SelectedRows.Count == 0) return;
            var id = Convert.ToInt32(dgvRecipes.SelectedRows[0].Cells[0].Value);
            var recName = Convert.ToString(dgvRecipes.SelectedRows[0].Cells[1].Value);
            if (MessageBox.Show($"Видалити страву '{recName}'?", "Підтвердження", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            DataAccess.DeleteRecipe(id);
            LoadRecipes();
        }

        private void btnAddInventory_Click(object sender, EventArgs e)
        {
            DataGridViewRow? row = null;
            if (dgvInventory.SelectedRows.Count > 0)
                row = dgvInventory.SelectedRows[0];
            else if (dgvInventory.CurrentRow != null)
                row = dgvInventory.CurrentRow;
            else if (dgvProducts.SelectedRows.Count > 0)
                row = dgvProducts.SelectedRows[0];
            else if (dgvProducts.CurrentRow != null)
                row = dgvProducts.CurrentRow;

            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = DataAccess.GetProducts().FirstOrDefault(p => p.Id == id);
            if (prod == null) return;

            using var f = new InventoryEditForm(prod.Id, prod.Name, prod.InventoryUnit, InventoryEditForm.Mode.Add);
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.Quantity > 0)
                {
                    DataAccess.AddInventory(id, f.Quantity, prod.InventoryUnit);
                    LoadInventoryView();
                    LoadProducts();
                }
            }
        }

        private void btnReduceInventory_Click(object sender, EventArgs e)
        {
            DataGridViewRow? row = null;
            if (dgvInventory.SelectedRows.Count > 0)
                row = dgvInventory.SelectedRows[0];
            else if (dgvInventory.CurrentRow != null)
                row = dgvInventory.CurrentRow;
            else if (dgvProducts.SelectedRows.Count > 0)
                row = dgvProducts.SelectedRows[0];
            else if (dgvProducts.CurrentRow != null)
                row = dgvProducts.CurrentRow;

            if (row == null) return;

            var id = Convert.ToInt32(row.Cells[0].Value);
            var prod = DataAccess.GetProducts().FirstOrDefault(p => p.Id == id);
            if (prod == null) return;

            using var f = new InventoryEditForm(prod.Id, prod.Name, prod.InventoryUnit, InventoryEditForm.Mode.Reduce);
            if (f.ShowDialog() == DialogResult.OK)
            {
                if (f.Quantity > 0)
                {
                    DataAccess.ReduceInventory(id, f.Quantity, prod.InventoryUnit);
                    LoadInventoryView();
                    LoadProducts();
                }
            }
        }

        private void btnGenerateShopping_Click(object sender, EventArgs e)
        {
            using var f = new ShoppingPlannerForm();
            f.ShowDialog();
            LoadInventoryView();
        }

        private void btnRecountAll_Click(object sender, EventArgs e)
        {
            using var f = new RecountAllForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                LoadInventoryView();
                LoadProducts();
            }
        }

        private void btnRecountHistory_Click(object sender, EventArgs e)
        {
            using var f = new RecountHistoryForm();
            f.ShowDialog();
        }

        private void btnWeeklyPlan_Click_1(object sender, EventArgs e)
        {
            using var f = new WeeklyPlannerForm();
            f.ShowDialog();
        }

        private async void btnUpdatePrices_Click(object sender, EventArgs e)
        {
            btnUpdatePrices.Enabled = false;
            var products = DataAccess.GetProducts().ToList();
            if (products.Count == 0)
            {
                MessageBox.Show("Немає продуктів для обробки.", "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnUpdatePrices.Enabled = true;
                return;
            }

            // Create progress form and show on UI thread.
            var progressForm = new ProgressForm();
            progressForm.Show(this);
            var cts = progressForm.Cancellation;

            // Initialize Playwright (used by some store parsers) once with desired parallelism
            int parallelism = 2; // tuneable: 2..6 depending on machine
            try
            {
                await PriceUpdaters.EnsurePlaywrightAsync(parallelism);
            }
            catch
            {
                // ignore init errors here; will surface later
            }

            int total = products.Count;
            int processed = 0;
            int updated = 0;
            int errors = 0;

            try
            {
                await Parallel.ForEachAsync(products, new ParallelOptions { MaxDegreeOfParallelism = parallelism, CancellationToken = cts.Token }, async (p, ct) =>
                {
                    // Report start (best-effort)
                    try
                    {
                        progressForm.ReportProgress(Volatile.Read(ref processed), total, $"Обробка: {p.Name}");
                    }
                    catch { /* ignore UI race */ }

                    try
                    {
                        var changed = await PriceUpdaterDebug.UpdateProductWithLogging(p, ct).ConfigureAwait(false);

                        if (changed)
                        {
                            // persist change
                            DataAccess.UpdateProduct(p);
                            Interlocked.Increment(ref updated);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // propagate cancellation to stop the loop
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
            catch (OperationCanceledException)
            {
                // canceled by user - ignore
            }
            finally
            {
                // Dispose shared Playwright resources (background) - awaitable
                try { await PriceUpdaters.DisposePlaywrightAsync().ConfigureAwait(false); } catch { }

                // Ensure UI updates and closing of progressForm happen on UI thread.
                try
                {
                    // update final text and close on UI thread
                    if (!progressForm.IsDisposed)
                    {
                        this.Invoke(new Action(() =>
                        {
                            try { progressForm.ReportProgress(processed, total, "Завершено."); } catch { }
                        }));

                        // small pause for UX but keep it non-blocking on UI: use Task.Delay and marshal close to UI again
                        await Task.Delay(200).ConfigureAwait(false);

                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                try { progressForm.Close(); } catch { }
                                try { progressForm.Dispose(); } catch { }

                                // All other UI updates must also happen on UI thread
                                try
                                {
                                    btnUpdatePrices.Enabled = true;
                                    LoadProducts();
                                    MessageBox.Show($"Оновлено: {updated}. Помилок: {errors}.", "Актуалізація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                catch { }
                            }));
                        }
                        catch
                        {
                            // ignore invoke errors
                        }
                    }
                    else
                    {
                        // progressForm disposed already; still need to re-enable UI
                        this.Invoke(new Action(() =>
                        {
                            try
                            {
                                btnUpdatePrices.Enabled = true;
                                LoadProducts();
                                MessageBox.Show($"Оновлено: {updated}. Помилок: {errors}.", "Актуалізація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch { }
                        }));
                    }
                }
                catch
                {
                    // ignore any UI thread invocation issues
                }
            }
        }
    }
}