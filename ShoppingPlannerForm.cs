// ShoppingPlannerForm.cs — apply rounding on store change, keep original quantities, commit cell edits
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace FoodManager
{
    public partial class ShoppingPlannerForm : Form
    {
        private List<PlanEntry> _lastPlan = new List<PlanEntry>();
        private List<string> _lastWarnings = new List<string>();

        private Dictionary<string, (int qty, string unit)>? _initialBuys;

        // cache for event handlers
        private Dictionary<string, Product> _lastProductsByName = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
        private List<string> _lastStores = new List<string>();

        public ShoppingPlannerForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadRecipes();

            // commit combo changes immediately and handle selection changes
            dgvProducts.CurrentCellDirtyStateChanged += DgvProducts_CurrentCellDirtyStateChanged;
            dgvProducts.CellValueChanged += DgvProducts_CellValueChanged;
        }

        private void DgvProducts_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvProducts.IsCurrentCellDirty)
            {
                // commit so CellValueChanged fires for combobox immediately when user selects value
                dgvProducts.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvProducts_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = dgvProducts.Columns[e.ColumnIndex];
            if (col.Name != "selectedStoreCol") return;

            // nothing to do if plan/products not initialized
            if (_lastProductsByName == null || _lastProductsByName.Count == 0) return;

            // restore original quantities before any rounding to avoid cumulative rounding
            foreach (var pe in _lastPlan)
            {
                pe.TotalQty = pe.OriginalQty;
                pe.Packs = null;
                // optionally clear notes related to previous rounding (keep other notes)
                // pe.Note = RemoveRoundingNote(pe.Note); // optional helper if you want to remove old rounding notes
            }

            // If rounding option enabled, re-apply rounding based on currently selected stores
            if (chkRoundToPack.Checked)
            {
                _lastWarnings.Clear(); // optional: refresh warnings
                ApplyRoundingToPlanBasedOnSelectedStore(_lastProductsByName);
            }
            else
            {
                // when rounding disabled ensure no packs info remains
                foreach (var pe in _lastPlan) pe.Packs = null;
            }

            // Refresh UI: re-render prices (colors depend on entry.TotalQty) and update plan qty column
            RenderStorePricesInGrid(_lastStores, _lastProductsByName);
            UpdatePlanQtyColumnDisplay();
        }

        private void ApplyPrefill()
        {
            if (_initialBuys == null) return;
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow) continue;
                var name = Convert.ToString(row.Cells[2].Value) ?? "";
                if (_initialBuys.TryGetValue(name, out var v))
                {
                    row.Cells[0].Value = true;
                    // If product.Unit differs from prefilling unit, try to convert using product info
                    var prod = DataAccess.GetProductByName(name);
                    var targetUnit = Convert.ToString(row.Cells[3].Value) ?? "g";
                    int qtyToSet = v.qty;
                    if (prod != null && !string.Equals(targetUnit, v.unit, StringComparison.OrdinalIgnoreCase))
                    {
                        // convert between g <-> pcs if possible
                        if (v.unit == "g" && targetUnit == "pcs" && prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                        {
                            qtyToSet = (int)Math.Ceiling((double)v.qty / prod.WeightPerUnitG.Value);
                        }
                        else if (v.unit == "pcs" && targetUnit == "g" && prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                        {
                            qtyToSet = v.qty * prod.WeightPerUnitG.Value;
                        }
                        // else leave as-is (may be inconsistent)
                    }
                    row.Cells[4].Value = qtyToSet;
                }
            }
        }

        public ShoppingPlannerForm(Dictionary<string, (int qty, string unit)>? prefill) : this()
        {
            _initialBuys = prefill;
            LoadProducts();
            LoadRecipes();

            // Apply prefill (if any) after loading products
            if (_initialBuys != null) ApplyPrefill();
        }

        private void LoadProducts()
        {
            dgvProducts.Rows.Clear();
            foreach (var p in DataAccess.GetProducts())
            {
                var unit = p.InventoryUnit ?? "g";
                // Columns: Buy checkbox, Id (hidden), Name, Unit, Qty, SelectedStore
                // Ensure we don't keep adding planQtyCol repeatedly — designer should ensure fixed columns.
                dgvProducts.Rows.Add(false, p.Id, p.Name, unit, 0, "");
            }
        }

        private void LoadRecipes()
        {
            dgvRecipes.Rows.Clear();
            foreach (var r in DataAccess.GetRecipes())
            {
                // Columns: Use checkbox, Id (hidden), Name, Servings
                dgvRecipes.Rows.Add(false, r.Id, r.Name, 1);
            }
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            _lastPlan.Clear();
            _lastWarnings.Clear();

            bool considerDirectAgainstInventory = chkConsiderInventoryForDirectBuys.Checked;
            bool roundingRequested = chkRoundToPack.Checked; // this will control rounding behavior in UI (option 1)

            // 1) collect direct product purchases (user-chosen).
            var directBuys = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow) continue;
                var toBuy = Convert.ToBoolean(row.Cells[0].Value);
                if (!toBuy) continue;
                var name = Convert.ToString(row.Cells[2].Value) ?? "";
                var unit = Convert.ToString(row.Cells[3].Value) ?? "g";
                if (!int.TryParse(Convert.ToString(row.Cells[4].Value), out var qty)) qty = 0;
                if (qty <= 0) continue;
                directBuys[name] = (qty, unit);
            }

            // 2) collect recipe-derived needs. Aggregate amounts in their ingredient unit (g or pcs)
            var recipeNeeds = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in dgvRecipes.Rows)
            {
                if (row.IsNewRow) continue;
                var sel = Convert.ToBoolean(row.Cells[0].Value);
                if (!sel) continue;
                var id = Convert.ToInt32(row.Cells[1].Value);
                if (!int.TryParse(Convert.ToString(row.Cells[3].Value), out var servings)) servings = 1;
                var recipe = DataAccess.GetRecipes().FirstOrDefault(r => r.Id == id);
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
                        // try to convert between pcs/g if product info available
                        var prod = DataAccess.GetProductByName(ingName);
                        if (prod != null && prod.WeightPerUnitG.HasValue && prod.WeightPerUnitG.Value > 0)
                        {
                            int curAsGrams = cur.unit == "pcs" ? cur.qty * prod.WeightPerUnitG.Value : cur.qty;
                            int addAsGrams = amount.Unit == "pcs" ? totalQty * prod.WeightPerUnitG.Value : totalQty;
                            recipeNeeds[ingName] = (curAsGrams + addAsGrams, "g");
                        }
                        else
                        {
                            // cannot convert reliably; keep existing unit and sum quantities (will warn later)
                            recipeNeeds[ingName] = (cur.qty + totalQty, cur.unit);
                        }
                    }
                }
            }

            var results = new List<string>();
            var warnings = new List<string>();

            // set of ingredients/products to process
            var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            allNames.UnionWith(directBuys.Keys);
            allNames.UnionWith(recipeNeeds.Keys);

            // prepare caches used later by event handler
            _lastProductsByName = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
            var storeSet = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var name in allNames)
            {
                var product = DataAccess.GetProductByName(name);
                if (product != null)
                {
                    _lastProductsByName[name] = product;
                    if (product.Prices != null)
                    {
                        foreach (var k in product.Prices.Keys) storeSet.Add(k);
                    }
                }

                string productUnit = product?.InventoryUnit; // may be null -> use ingredient/unit info

                // Direct buy values
                directBuys.TryGetValue(name, out var direct); // default (0,"")
                // Recipe need values
                recipeNeeds.TryGetValue(name, out var recipeNeed); // default (0,"")

                // Determine canonical unit to calculate in:
                // Prefer productUnit if known; otherwise use recipeNeed.unit if present; otherwise direct.unit; fallback to "g"
                string canonicalUnit = productUnit ?? (recipeNeed.unit ?? direct.unit ?? "g");

                // Compute inventory available for this product, converted to canonicalUnit when possible
                var (haveQty, haveUnit) = DataAccess.GetInventoryQuantityForProduct(name);
                int haveInCanonical = 0;
                bool inventoryConversionOk = true;
                if (haveQty == 0)
                {
                    haveInCanonical = 0;
                }
                else
                {
                    if (string.Equals(haveUnit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        haveInCanonical = haveQty;
                    }
                    else
                    {
                        // need to convert via weight_per_unit_g when possible
                        if (canonicalUnit == "g" && haveUnit == "pcs")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                haveInCanonical = haveQty * product.WeightPerUnitG.Value;
                            }
                            else
                            {
                                inventoryConversionOk = false;
                            }
                        }
                        else if (canonicalUnit == "pcs" && haveUnit == "g")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                // convert grams to whole pieces available (floor)
                                haveInCanonical = haveQty / product.WeightPerUnitG.Value;
                            }
                            else
                            {
                                inventoryConversionOk = false;
                            }
                        }
                        else
                        {
                            inventoryConversionOk = false;
                        }
                    }
                }

                // Compute recipe-required amount in canonical unit
                int recipeNeededInCanonical = 0;
                bool recipeConversionOk = true;
                if (recipeNeed.qty > 0)
                {
                    if (string.Equals(recipeNeed.unit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        recipeNeededInCanonical = recipeNeed.qty;
                    }
                    else
                    {
                        // convert recipe unit to canonical
                        if (recipeNeed.unit == "g" && canonicalUnit == "pcs")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                recipeNeededInCanonical = (int)Math.Ceiling((double)recipeNeed.qty / product.WeightPerUnitG.Value);
                            }
                            else
                            {
                                recipeConversionOk = false;
                                warnings.Add($"Не вдалося конвертувати потребу для '{name}': рецепт вказує {recipeNeed.qty} г, але продукт відстежується в штуках і відсутня вага г/шт.");
                            }
                        }
                        else if (recipeNeed.unit == "pcs" && canonicalUnit == "g")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                recipeNeededInCanonical = recipeNeed.qty * product.WeightPerUnitG.Value;
                            }
                            else
                            {
                                recipeConversionOk = false;
                                warnings.Add($"Не вдалося конвертувати потребу для '{name}': рецепт вказує {recipeNeed.qty} шт, але продукт відстежується в грамах і відсутня вага г/шт.");
                            }
                        }
                        else
                        {
                            recipeConversionOk = false;
                            warnings.Add($"Невідома комбінація одиниць для '{name}' (рецепт: {recipeNeed.unit}, очікувана: {canonicalUnit}).");
                        }
                    }
                }

                // Subtract inventory from recipe needs ONLY (per requirement), then optionally apply remaining inventory to direct buys
                int remainingForRecipe = recipeNeededInCanonical;
                int directInCanonical = 0;
                if (direct.qty > 0)
                {
                    if (string.IsNullOrEmpty(direct.unit) || string.Equals(direct.unit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                    {
                        directInCanonical = direct.qty;
                    }
                    else
                    {
                        if (direct.unit == "g" && canonicalUnit == "pcs")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                directInCanonical = (int)Math.Ceiling((double)direct.qty / product.WeightPerUnitG.Value);
                            }
                            else
                            {
                                warnings.Add($"Не вдалося конвертувати ручну покупку для '{name}' з г -> шт (відсутня вага г/шт).");
                            }
                        }
                        else if (direct.unit == "pcs" && canonicalUnit == "g")
                        {
                            if (product != null && product.WeightPerUnitG.HasValue && product.WeightPerUnitG.Value > 0)
                            {
                                directInCanonical = direct.qty * product.WeightPerUnitG.Value;
                            }
                            else
                            {
                                warnings.Add($"Не вдалося конвертувати ручну покупку для '{name}' з шт -> г (відсутня вага г/шт).");
                            }
                        }
                        else
                        {
                            warnings.Add($"Невідома конвертація одиниць для ручної покупки '{name}': {direct.unit} -> {canonicalUnit}");
                        }
                    }
                }

                // Apply inventory to recipe first
                if (recipeNeededInCanonical > 0)
                {
                    if (inventoryConversionOk)
                    {
                        remainingForRecipe = Math.Max(0, recipeNeededInCanonical - haveInCanonical);
                    }
                    else
                    {
                        warnings.Add($"Наявний облік для '{name}' не було враховано (unit={haveUnit}) — неможлива конвертація в '{canonicalUnit}'.");
                        remainingForRecipe = recipeNeededInCanonical;
                    }
                }

                int adjustedDirect = directInCanonical;

                if (considerDirectAgainstInventory)
                {
                    // Use leftover inventory (after allocating to recipe) to reduce direct buys.
                    int haveAfterRecipe = Math.Max(0, haveInCanonical - recipeNeededInCanonical);
                    if (haveAfterRecipe > 0 && directInCanonical > 0)
                    {
                        adjustedDirect = Math.Max(0, directInCanonical - haveAfterRecipe);
                    }
                }

                // Combine: direct buys (possibly adjusted) + recipe residual
                int totalToBuyInCanonical = adjustedDirect + remainingForRecipe;

                // Prepare plan entry and save original quantity (used for re-rounding on store change)
                var ent = new PlanEntry
                {
                    Name = name,
                    Unit = canonicalUnit,
                    OriginalQty = totalToBuyInCanonical,
                    TotalQty = totalToBuyInCanonical,
                    DirectQty = adjustedDirect,
                    RecipeQty = remainingForRecipe,
                    Note = ""
                };

                if (!recipeConversionOk)
                {
                    ent.Note += "Неможлива конвертація потреб зі страв; ";
                }
                if (!inventoryConversionOk)
                {
                    ent.Note += "Наявний облік не було віднято (неможлива конвертація); ";
                }
                if (considerDirectAgainstInventory && adjustedDirect != directInCanonical)
                {
                    ent.Note += $"Прямі покупки зменшені на залишок інвентарю ({directInCanonical - adjustedDirect} {canonicalUnit}); ";
                }

                // Only add to result if something to buy or if there was an unconvertable recipe need (so user can see)
                if (ent.TotalQty > 0 || (!string.IsNullOrEmpty(ent.Note) && recipeNeed.qty > 0))
                {
                    _lastPlan.Add(ent);
                    if (ent.TotalQty > 0)
                    {
                        if (ent.Unit == "pcs")
                            results.Add($"{ent.Name}: купити {ent.TotalQty} шт (прямі покупки {ent.DirectQty} шт + потреби зі страв {ent.RecipeQty} шт)");
                        else
                            results.Add($"{ent.Name}: купити {ent.TotalQty} г (прямі покупки {ent.DirectQty} г + потреби зі страв {ent.RecipeQty} г)");
                    }
                    if (!string.IsNullOrEmpty(ent.Note))
                    {
                        results.Add($"  УВАГА: {ent.Note.Trim()}");
                    }
                }
            }

            // Render store prices first to populate selectedStore defaults in grid
            _lastStores = storeSet.ToList();
            RenderStorePricesInGrid(_lastStores, _lastProductsByName);

            // If rounding requested (option 1), apply rounding to _lastPlan entries based on selected store from grid.
            if (roundingRequested)
            {
                ApplyRoundingToPlanBasedOnSelectedStore(_lastProductsByName);
                // re-render store prices so colors are computed against rounded quantities
                RenderStorePricesInGrid(_lastStores, _lastProductsByName);
            }

            // update plan quantity column in grid (create/update)
            UpdatePlanQtyColumnDisplay();

            // show results/warnings
            var outLines = new List<string>();
            if (results.Count == 0) outLines.Add("Нічого купувати за поточними виборами.");
            else
            {
                outLines.Add("Плановані закупки:");
                outLines.AddRange(results);
            }
            if (_lastWarnings.Count > 0) warnings.AddRange(_lastWarnings);
            if (warnings.Count > 0)
            {
                outLines.Add("");
                outLines.Add("УВАГИ / ПОВІДОМЛЕННЯ:");
                outLines.AddRange(warnings.Distinct());
            }
            txtResult.Lines = outLines.ToArray();
        }

        // Apply rounding to _lastPlan entries based on selected store chosen in dgvProducts
        // Restores OriginalQty before rounding to avoid repeated rounding
        private void ApplyRoundingToPlanBasedOnSelectedStore(Dictionary<string, Product> productsByName)
        {
            var nameToRow = new Dictionary<string, DataGridViewRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow r in dgvProducts.Rows)
            {
                if (r.IsNewRow) continue;
                var nm = Convert.ToString(r.Cells[2].Value) ?? "";
                if (!nameToRow.ContainsKey(nm)) nameToRow[nm] = r;
            }

            foreach (var entry in _lastPlan)
            {
                // restore original before any rounding
                entry.TotalQty = entry.OriginalQty;
                entry.Packs = null;
                // find selected store
                if (!nameToRow.TryGetValue(entry.Name, out var row)) continue;
                if (!dgvProducts.Columns.Contains("selectedStoreCol")) continue;
                var sel = Convert.ToString(row.Cells["selectedStoreCol"].Value) ?? "";
                if (string.IsNullOrWhiteSpace(sel)) continue;
                if (!productsByName.TryGetValue(entry.Name, out var prod)) prod = DataAccess.GetProductByName(entry.Name);
                if (prod == null) continue;

                var pack = prod.GetPackSizeForStore(sel);
                if (pack.HasValue && pack.Value > 0 && entry.TotalQty > 0)
                {
                    var packs = (int)Math.Ceiling((double)entry.TotalQty / pack.Value);
                    var newQty = packs * pack.Value;
                    if (newQty != entry.TotalQty)
                    {
                        entry.Note = (entry.Note ?? "") + $" Округлено до фасовки {pack.Value} ({packs} упак.).";
                        entry.Packs = packs;
                        _lastWarnings.Add($"{entry.Name}: округлено {entry.TotalQty} -> {newQty} ({packs} x {pack.Value} {entry.Unit}) для магазину {sel}");
                        entry.TotalQty = newQty;
                    }
                    else
                    {
                        entry.Packs = packs;
                    }
                }
                else
                {
                    entry.Packs = null;
                }
            }
        }

        // Ensure grid has a readonly "План (обсяг)" column that shows the computed (possibly rounded) quantity from _lastPlan.
        private void UpdatePlanQtyColumnDisplay()
        {
            // ensure column exists
            if (!dgvProducts.Columns.Contains("planQtyCol"))
            {
                // find index of selectedStoreCol to insert before it; otherwise append at end
                int insertIndex = dgvProducts.Columns.Contains("selectedStoreCol") ? dgvProducts.Columns["selectedStoreCol"].Index : dgvProducts.Columns.Count;
                var planCol = new DataGridViewTextBoxColumn()
                {
                    Name = "planQtyCol",
                    HeaderText = "План (обсяг)",
                    ReadOnly = true,
                    Width = 120
                };
                dgvProducts.Columns.Insert(insertIndex, planCol);
            }

            var nameToEntry = _lastPlan.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow r in dgvProducts.Rows)
            {
                if (r.IsNewRow) continue;
                var nm = Convert.ToString(r.Cells[2].Value) ?? "";
                if (nameToEntry.TryGetValue(nm, out var entry))
                {
                    r.Cells["planQtyCol"].Value = entry.TotalQty > 0 ? entry.TotalQty.ToString() : "";
                    // optionally show packs in tooltip
                    if (entry.Packs.HasValue)
                    {
                        r.Cells["planQtyCol"].ToolTipText = $"Упаковок: {entry.Packs.Value}";
                    }
                    else r.Cells["planQtyCol"].ToolTipText = "";
                }
                else
                {
                    r.Cells["planQtyCol"].Value = "";
                    r.Cells["planQtyCol"].ToolTipText = "";
                }
            }
        }

        // Renders per-store price columns into dgvProducts and colors best (min) as green and worst (max) as red.
        // Also populates the SelectedStore combobox column with available stores and sets a default (cheapest) per row.
        // Sets ToolTipText for the selectedStore cell with the store link (if available).
        private void RenderStorePricesInGrid(List<string> stores, Dictionary<string, Product> productsByName)
        {
            // Remove any previously added dynamic store columns (store_<name>)
            var existingStoreCols = dgvProducts.Columns.Cast<DataGridViewColumn>().Where(c => c.Name.StartsWith("store_")).ToList();
            foreach (var c in existingStoreCols) dgvProducts.Columns.Remove(c);

            // Insert new store columns after the first 5 static columns (Buy, Id, Name, Unit, Qty, SelectedStore exists at index 5)
            int insertIndex = Math.Min(5, dgvProducts.Columns.Count);
            foreach (var store in stores)
            {
                var col = new DataGridViewTextBoxColumn()
                {
                    Name = "store_" + store,
                    HeaderText = store,
                    ReadOnly = true,
                    Width = 120
                };
                dgvProducts.Columns.Insert(insertIndex++, col);
            }

            // Populate SelectedStore combobox column items (union of stores)
            var selectedStoreCol = dgvProducts.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == "selectedStoreCol") as DataGridViewComboBoxColumn;
            if (selectedStoreCol != null)
            {
                selectedStoreCol.Items.Clear();
                foreach (var s in stores) selectedStoreCol.Items.Add(s);
            }

            var nameToRow = new Dictionary<string, DataGridViewRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow r in dgvProducts.Rows)
            {
                if (r.IsNewRow) continue;
                var nm = Convert.ToString(r.Cells[2].Value) ?? "";
                if (!nameToRow.ContainsKey(nm)) nameToRow[nm] = r;
            }

            foreach (var entry in _lastPlan)
            {
                if (!nameToRow.TryGetValue(entry.Name, out var row)) continue;

                var costs = new System.Collections.Generic.List<(string store, decimal? cost, decimal? price)>();
                foreach (var store in stores)
                {
                    decimal? price = null;
                    decimal? cost = null;
                    if (productsByName.TryGetValue(entry.Name, out var prod))
                    {
                        if (prod.Prices != null && prod.Prices.TryGetValue(store, out var p))
                        {
                            price = p;
                            // Try to compute a sensible price-per-unit (ppu) with fallbacks.
                            decimal? ppu = prod.PricePerUnitFromStore(store, p);
                            if (!ppu.HasValue)
                            {
                                var storePack = prod.GetPackSizeForStore(store);
                                if (storePack.HasValue && storePack.Value > 0)
                                {
                                    ppu = p / (decimal)storePack.Value;
                                }
                            }
                            if (!ppu.HasValue && prod.InventoryUnit == "pcs")
                            {
                                ppu = p; // assume price is per piece
                            }

                            if (ppu.HasValue)
                            {
                                try
                                {
                                    cost = ppu.Value * entry.TotalQty; // entry.TotalQty may have been rounded already
                                }
                                catch { cost = null; }
                            }
                        }
                    }
                    costs.Add((store, cost, price));
                }

                var nonNullCosts = costs.Where(x => x.cost.HasValue).Select(x => x.cost!.Value).ToList();
                decimal? min = nonNullCosts.Count > 0 ? (decimal?)nonNullCosts.Min() : null;
                decimal? max = nonNullCosts.Count > 0 ? (decimal?)nonNullCosts.Max() : (decimal?)null;

                // Fill store price cells and coloring
                foreach (var st in costs)
                {
                    var colName = "store_" + st.store;
                    if (!dgvProducts.Columns.Contains(colName)) continue;
                    var cell = row.Cells[colName];
                    if (st.price.HasValue)
                        cell.Value = st.price.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        cell.Value = "";

                    if (st.cost.HasValue && min.HasValue && max.HasValue)
                    {
                        if (st.cost.Value == min.Value) cell.Style.BackColor = System.Drawing.Color.LightGreen;
                        else if (st.cost.Value == max.Value) cell.Style.BackColor = System.Drawing.Color.LightCoral;
                        else cell.Style.BackColor = System.Drawing.Color.White;
                    }
                    else
                    {
                        cell.Style.BackColor = System.Drawing.Color.White;
                    }
                }

                // Set default SelectedStore for this row to cheapest store (if any)
                if (selectedStoreCol != null)
                {
                    var best = costs.Where(x => x.cost.HasValue).OrderBy(x => x.cost!.Value).FirstOrDefault();
                    var bestStore = best.store;
                    if (!string.IsNullOrEmpty(bestStore))
                    {
                        try
                        {
                            // only set if empty (do not override manual selection)
                            if (string.IsNullOrEmpty(Convert.ToString(row.Cells["selectedStoreCol"].Value)))
                                row.Cells["selectedStoreCol"].Value = bestStore;

                            // Also set tooltip to link (if exists)
                            if (productsByName.TryGetValue(entry.Name, out var prod))
                            {
                                if (prod.Links != null && prod.Links.TryGetValue(bestStore, out var url) && !string.IsNullOrWhiteSpace(url))
                                {
                                    row.Cells["selectedStoreCol"].ToolTipText = url;
                                }
                                else
                                {
                                    row.Cells["selectedStoreCol"].ToolTipText = "";
                                }
                            }
                        }
                        catch { /* ignore if not settable */ }
                    }
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_lastPlan == null || _lastPlan.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту. Спочатку натисніть 'Розрахувати'.", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Before exporting, ensure user has option to pick stores in the grid.
            var productsByName = new Dictionary<string, Product>(StringComparer.OrdinalIgnoreCase);
            var storeSet = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pEntry in _lastPlan)
            {
                var prod = DataAccess.GetProductByName(pEntry.Name);
                if (prod != null)
                {
                    productsByName[pEntry.Name] = prod;
                    if (prod.Prices != null)
                        foreach (var k in prod.Prices.Keys) storeSet.Add(k);
                }
            }
            var stores = storeSet.ToList();

            // Build mapping name -> selected store (read from dgvProducts SelectedStore column)
            var nameToSelectedStore = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var nameToRow = new Dictionary<string, DataGridViewRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow r in dgvProducts.Rows)
            {
                if (r.IsNewRow) continue;
                var nm = Convert.ToString(r.Cells[2].Value) ?? "";
                if (!nameToRow.ContainsKey(nm)) nameToRow[nm] = r;
                if (dgvProducts.Columns.Contains("selectedStoreCol"))
                {
                    var val = Convert.ToString(r.Cells["selectedStoreCol"].Value) ?? "";
                    if (!string.IsNullOrEmpty(val)) nameToSelectedStore[nm] = val;
                }
            }

            // Check for any plan item without a selected store while multiple stores available for that product
            var missingSelections = new List<string>();
            foreach (var entry in _lastPlan)
            {
                if (!nameToSelectedStore.TryGetValue(entry.Name, out var sel) || string.IsNullOrEmpty(sel))
                {
                    if (productsByName.TryGetValue(entry.Name, out var prod) && prod.Prices != null && prod.Prices.Count > 0)
                    {
                        missingSelections.Add(entry.Name);
                    }
                }
            }

            if (missingSelections.Count > 0)
            {
                var listPreview = string.Join(Environment.NewLine, missingSelections.Select(x => "- " + x));
                var msg = "Деякі продукти мають доступні ціни в магазинах, але для них не вибраний магазин:\n" + listPreview +
                          "\n\nНатисніть 'Так' щоб продовжити і використовувати найвигідніший магазин автоматично, або 'Ні' щоб скасувати і вибрати магазини вручну у таблиці.";
                var res = MessageBox.Show(msg, "Вибір магазину", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.No) return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            sfd.FileName = "shopping_plan.xlsx";
            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                OfficeOpenXml.ExcelPackage.License.SetNonCommercialPersonal("Kostiantyn Yevdokymenko");
                using var package = new ExcelPackage(new FileInfo(sfd.FileName));
                var ws = package.Workbook.Worksheets.Add("ShoppingPlan");

                // Build store->column mapping while writing header
                var storeToCol = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                // Header
                int col = 1;
                ws.Cells[1, col++].Value = "Найменування";

                int storeStartCol = col;
                for (int i = 0; i < stores.Count; i++)
                {
                    var st = stores[i];
                    ws.Cells[1, col].Value = st;
                    storeToCol[st] = col;
                    col++;
                }

                int chosenStoreCol = col++;
                ws.Cells[1, chosenStoreCol].Value = "Обраний магазин";

                int volumeCol = col++;
                ws.Cells[1, volumeCol].Value = "Обсяг";

                bool rounding = chkRoundToPack.Checked;
                int packsCol = -1;
                if (rounding)
                {
                    packsCol = col++;
                    ws.Cells[1, packsCol].Value = "Кількість";
                }

                int costCol = col++;
                ws.Cells[1, costCol].Value = "Вартість";

                // header style
                using (var hdr = ws.Cells[1, 1, 1, Math.Max(5, col - 1)])
                {
                    hdr.Style.Font.Bold = true;
                    hdr.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    hdr.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(230, 230, 230));
                }

                int row = 2;

                // For each plan entry compute display values; rounding to pack by chosen store if requested
                foreach (var entry in _lastPlan)
                {
                    col = 1;
                    ws.Cells[row, col++].Value = entry.Name;

                    // write store price columns (display stored price)
                    productsByName.TryGetValue(entry.Name, out var prod);
                    for (int si = 0; si < stores.Count; si++)
                    {
                        var st = stores[si];
                        var priceCell = ws.Cells[row, storeToCol[st]];
                        if (prod != null && prod.Prices != null && prod.Prices.TryGetValue(st, out var p))
                        {
                            priceCell.Value = p;
                            priceCell.Style.Numberformat.Format = "0.##";
                        }
                        else
                        {
                            priceCell.Value = "";
                        }
                    }

                    // determine chosen store for this product (from grid), fallback to cheapest if none
                    string? chosenStore = null;
                    if (nameToSelectedStore.TryGetValue(entry.Name, out var ssel) && !string.IsNullOrWhiteSpace(ssel))
                    {
                        chosenStore = ssel;
                    }
                    else
                    {
                        // pick cheapest among available costs (use computed price-per-unit if possible)
                        string best = "";
                        decimal? bestVal = null;
                        foreach (var st in stores)
                        {
                            if (prod != null && prod.Prices != null && prod.Prices.TryGetValue(st, out var p))
                            {
                                var ppu = prod.PricePerUnitFromStore(st, p);
                                if (!ppu.HasValue)
                                {
                                    var storePack = prod.GetPackSizeForStore(st);
                                    if (storePack.HasValue && storePack.Value > 0)
                                        ppu = p / (decimal)storePack.Value;
                                }
                                if (!ppu.HasValue && prod.InventoryUnit == "pcs")
                                    ppu = p;
                                if (ppu.HasValue)
                                {
                                    var costEst = ppu.Value * entry.TotalQty;
                                    if (!bestVal.HasValue || costEst < bestVal.Value)
                                    {
                                        bestVal = costEst; best = st;
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(best)) chosenStore = best;
                    }

                    var chosenStoreCell = ws.Cells[row, chosenStoreCol];
                    chosenStoreCell.Value = chosenStore ?? "";

                    // add comment/hyperlink to chosenStoreCell if product link exists
                    if (!string.IsNullOrEmpty(chosenStore) && prod != null && prod.Links != null && prod.Links.TryGetValue(chosenStore, out var url) && !string.IsNullOrWhiteSpace(url))
                    {
                        try
                        {
                            var c = chosenStoreCell.AddComment(url, "app");
                            c.AutoFit = true;
                        }
                        catch { }
                        try
                        {
                            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                            {
                                chosenStoreCell.Hyperlink = new Uri(url);
                                chosenStoreCell.Style.Font.UnderLine = true;
                                chosenStoreCell.Style.Font.Color.SetColor(Color.Blue);
                            }
                        }
                        catch { }
                    }

                    // Determine quantity to export (already rounded in _lastPlan if roundingRequested & selected store had pack)
                    int exportQty = entry.TotalQty;

                    // write volume cell (Обсяг)
                    var volCell = ws.Cells[row, volumeCol];
                    volCell.Value = exportQty;
                    volCell.Style.Numberformat.Format = "0";

                    // If rounding mode and pack exists for chosenStore -> compute packs via formula and cost as price_per_pack * packs
                    bool usedFormulaForCost = false;
                    if (rounding && !string.IsNullOrEmpty(chosenStore) && prod != null)
                    {
                        var storePack = prod.GetPackSizeForStore(chosenStore);
                        if (storePack.HasValue && storePack.Value > 0)
                        {
                            if (packsCol > 0)
                            {
                                var volAddr = volCell.Address;
                                ws.Cells[row, packsCol].Formula = $"=ROUNDUP({volAddr}/{storePack.Value},0)";
                                ws.Cells[row, packsCol].Style.Numberformat.Format = "0";

                                var priceAddr = ws.Cells[row, storeToCol[chosenStore]].Address;
                                ws.Cells[row, costCol].Formula = $"=IF(AND(ISNUMBER({priceAddr}),{storePack.Value}>0),{priceAddr}*ROUNDUP({volAddr}/{storePack.Value},0),\"\")";
                                ws.Cells[row, costCol].Style.Numberformat.Format = "0.##";
                                usedFormulaForCost = true;
                            }
                        }
                    }

                    // other formula fallbacks (same as before)...
                    if (!usedFormulaForCost)
                    {
                        if (!string.IsNullOrEmpty(chosenStore) && prod != null && prod.Prices != null && prod.Prices.TryGetValue(chosenStore, out var chosenPrice))
                        {
                            var ppu = prod.PricePerUnitFromStore(chosenStore, chosenPrice);
                            if (!ppu.HasValue)
                            {
                                var storePack = prod.GetPackSizeForStore(chosenStore);
                                if (storePack.HasValue && storePack.Value > 0)
                                {
                                    ppu = chosenPrice / (decimal)storePack.Value;
                                }
                            }
                            if (ppu.HasValue)
                            {
                                var volAddr = ws.Cells[row, volumeCol].Address;
                                ws.Cells[row, costCol].Formula = $"=IF(ISNUMBER({volAddr}),{ppu.Value.ToString(CultureInfo.InvariantCulture)}*{volAddr},\"\")";
                                ws.Cells[row, costCol].Style.Numberformat.Format = "0.##";
                                usedFormulaForCost = true;
                            }
                            else
                            {
                                var ppuNumeric = prod.PricePerUnitFromStore(chosenStore, chosenPrice);
                                if (ppuNumeric.HasValue)
                                {
                                    var numericCost = ppuNumeric.Value * exportQty;
                                    ws.Cells[row, costCol].Value = Math.Round(numericCost, 2);
                                    ws.Cells[row, costCol].Style.Numberformat.Format = "0.##";
                                }
                                else
                                {
                                    ws.Cells[row, costCol].Value = "";
                                }
                            }
                        }
                        else
                        {
                            bool setFallback = false;
                            if (prod != null && prod.Prices != null)
                            {
                                foreach (var st in prod.Prices.Keys)
                                {
                                    var p = prod.Prices[st];
                                    var pack = prod.GetPackSizeForStore(st);
                                    var priceAddrFallback = ws.Cells[row, storeToCol[st]].Address;
                                    var volAddr = ws.Cells[row, volumeCol].Address;
                                    if (rounding && pack.HasValue && pack.Value > 0)
                                    {
                                        ws.Cells[row, costCol].Formula = $"=IF(AND(ISNUMBER({priceAddrFallback}),{pack.Value}>0),{priceAddrFallback}*ROUNDUP({volAddr}/{pack.Value},0),\"\")";
                                        ws.Cells[row, costCol].Style.Numberformat.Format = "0.##";
                                        setFallback = true;
                                        break;
                                    }
                                    else
                                    {
                                        var ppu = prod.PricePerUnitFromStore(st, p);
                                        if (!ppu.HasValue && pack.HasValue && pack.Value > 0)
                                            ppu = p / (decimal)pack.Value;
                                        if (ppu.HasValue)
                                        {
                                            ws.Cells[row, costCol].Formula = $"=IF(ISNUMBER({volAddr}),{ppu.Value.ToString(CultureInfo.InvariantCulture)}*{volAddr},\"\")";
                                            ws.Cells[row, costCol].Style.Numberformat.Format = "0.##";
                                            setFallback = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!setFallback) ws.Cells[row, costCol].Value = "";
                        }
                    }

                    row++;
                }

                // After all product rows write the summary row as a formula SUM over cost column
                int firstCostRow = 2;
                int lastCostRow = row - 1;
                int totalCols = costCol;
                ws.Cells[row, 1].Value = "Підсумок";
                if (stores.Count >= 0)
                {
                    int mergeTo = 1 + stores.Count; // merge over store columns as part of label
                    ws.Cells[row, 1, row, mergeTo].Merge = true;
                    ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }

                var totalCell = ws.Cells[row, costCol];
                var sumRange = $"{ws.Cells[firstCostRow, costCol].Address}:{ws.Cells[lastCostRow, costCol].Address}";
                totalCell.Formula = $"=SUM({sumRange})";
                totalCell.Style.Numberformat.Format = "0.##";

                // Style summary row
                using (var rng = ws.Cells[row, 1, row, totalCols])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 249, 196)); // light yellow
                }

                // Auto-fit columns
                ws.Cells[1, 1, row, Math.Max(5, totalCols)].AutoFitColumns();
                ws.Cells[1, 1, row, Math.Max(5, totalCols)].Style.Font.Name = "Calibri";

                // Save package
                package.Save();

                // Show any warnings accumulated during rounding
                if (_lastWarnings.Count > 0)
                {
                    var warnMsg = string.Join(Environment.NewLine, _lastWarnings.Distinct());
                    MessageBox.Show($"Excel збережено: {sfd.FileName}\n\nУВАГИ:\n{warnMsg}", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Excel збережено: {sfd.FileName}", "Експорт XLSX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні XLSX: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeCsv(string v) => $"\"{(v ?? "").Replace("\"", "\"\"")}\"";

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private class PlanEntry
        {
            public string Name { get; set; } = "";
            public string Unit { get; set; } = "g";
            public int OriginalQty { get; set; } = 0; // preserved original computed amount (before rounding)
            public int TotalQty { get; set; } = 0; // possibly rounded to pack
            public int DirectQty { get; set; } = 0;
            public int RecipeQty { get; set; } = 0;
            public int? Packs { get; set; } = null; // number of packs after rounding (if any)
            public string Note { get; set; } = "";
        }
    }
}