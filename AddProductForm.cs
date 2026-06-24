using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class AddProductForm : Form
    {
        public Product Product { get; private set; } = new Product();

        private int _editingIndex = -1;

        public AddProductForm() : this(null) { }

        public AddProductForm(Product existing)
        {
            InitializeComponent();

            cmbUnit.Items.Clear();
            cmbUnit.Items.AddRange(new string[] { "g", "pcs" });
            cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;

            if (existing != null)
            {
                Product = existing;
                txtName.Text = existing.Name;
                // txtWeight is used as default pack (in the same unit as InventoryUnit)
                txtWeight.Text = existing.WeightPerUnitG?.ToString() ?? "";

                txtProt.Text = existing.Nutrition.Protein_100g.ToString(CultureInfo.InvariantCulture);
                txtFats.Text = existing.Nutrition.Fats_100g.ToString(CultureInfo.InvariantCulture);
                txtCarbs.Text = existing.Nutrition.Carbs_100g.ToString(CultureInfo.InvariantCulture);
                txtSugars.Text = existing.Nutrition.Sugars_100g.ToString(CultureInfo.InvariantCulture);
                txtSat.Text = existing.Nutrition.Saturated_100g.ToString(CultureInfo.InvariantCulture);
                txtKcal.Text = existing.Nutrition.Kcal_100g.ToString(CultureInfo.InvariantCulture);
                txtFiber.Text = existing.Nutrition.Fiber_100g.ToString(CultureInfo.InvariantCulture);

                existing.Prices.TryGetValue("АТБ", out var atb); txtPriceATB.Text = atb > 0 ? atb.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("Сільпо", out var silpo); txtPriceSilpo.Text = silpo > 0 ? silpo.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("Метро", out var metro); txtPriceMetro.Text = metro > 0 ? metro.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("Столичний ринок", out var stol); txtPriceStolich.Text = stol > 0 ? stol.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("Ашан", out var ashan); txtPriceAshan.Text = ashan > 0 ? ashan.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("Велика Кишеня", out var vk); txtPriceVK.Text = vk > 0 ? vk.ToString(CultureInfo.InvariantCulture) : "";
                existing.Prices.TryGetValue("NOVUS", out var nov); txtPriceNovus.Text = nov > 0 ? nov.ToString(CultureInfo.InvariantCulture) : "";

                existing.Links.TryGetValue("АТБ", out var l1); txtLinkATB.Text = l1 ?? "";
                existing.Links.TryGetValue("Сільпо", out var l2); txtLinkSilpo.Text = l2 ?? "";
                existing.Links.TryGetValue("Метро", out var l3); txtLinkMetro.Text = l3 ?? "";
                existing.Links.TryGetValue("Столичний ринок", out var l4); txtLinkStolich.Text = l4 ?? "";
                existing.Links.TryGetValue("Ашан", out var l5); txtLinkAshan.Text = l5 ?? "";
                existing.Links.TryGetValue("Велика Кишеня", out var l6); txtLinkVK.Text = l6 ?? "";
                existing.Links.TryGetValue("NOVUS", out var l7); txtLinkNovus.Text = l7 ?? "";

                lstManualSources.Items.Clear();
                if (existing.ManualPriceSources != null)
                {
                    foreach (var s in existing.ManualPriceSources)
                    {
                        var display = s;
                        if (existing.Prices != null && existing.Prices.TryGetValue(s, out var p)) display = $"{s}:{p.ToString(CultureInfo.InvariantCulture)}";
                        if (existing.PackSizes != null && existing.PackSizes.TryGetValue(s, out var pack)) display = display + $":{pack}";
                        lstManualSources.Items.Add(display);
                    }
                }

                if (existing.PackSizes != null)
                {
                    if (existing.PackSizes.TryGetValue("АТБ", out var pa)) txtPackATB.Text = pa > 0 ? pa.ToString() : "";
                    if (existing.PackSizes.TryGetValue("Сільпо", out var ps)) txtPackSilpo.Text = ps > 0 ? ps.ToString() : "";
                    if (existing.PackSizes.TryGetValue("Метро", out var pm)) txtPackMetro.Text = pm > 0 ? pm.ToString() : "";
                    if (existing.PackSizes.TryGetValue("Ашан", out var pah)) txtPackAshan.Text = pah > 0 ? pah.ToString() : "";
                    if (existing.PackSizes.TryGetValue("Столичний ринок", out var pst)) txtPackStolich.Text = pst > 0 ? pst.ToString() : "";
                    if (existing.PackSizes.TryGetValue("Велика Кишеня", out var pvk)) txtPackVK.Text = pvk > 0 ? pvk.ToString() : "";
                    if (existing.PackSizes.TryGetValue("NOVUS", out var pnov)) txtPackNovus.Text = pnov > 0 ? pnov.ToString() : "";
                }

                cmbUnit.SelectedItem = existing.InventoryUnit ?? "g";
            }
            else
            {
                cmbUnit.SelectedItem = "g";
            }
        }

        private void btnAddCustom_Click(object sender, EventArgs e)
        {
            var name = txtCustomSourceName.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введіть назву джерела.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price = 0;
            if (!string.IsNullOrWhiteSpace(txtCustomPrice.Text))
            {
                if (!decimal.TryParse(txtCustomPrice.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out price) || price <= 0)
                {
                    MessageBox.Show("Невірна ціна.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int pack = 0;
            if (!string.IsNullOrWhiteSpace(txtCustomPack.Text))
            {
                if (!int.TryParse(txtCustomPack.Text.Trim(), out pack) || pack <= 0)
                {
                    MessageBox.Show("Невірна фасовка (введіть ціле число в тих же одиницях, що й продукт).", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Build item: Name[:price][:pack]
            string itemText;
            if (price > 0 && pack > 0) itemText = $"{name}:{price.ToString(CultureInfo.InvariantCulture)}:{pack}";
            else if (price > 0) itemText = $"{name}:{price.ToString(CultureInfo.InvariantCulture)}";
            else if (pack > 0) itemText = $"{name}::{pack}"; // empty price placeholder
            else itemText = name;

            if (_editingIndex >= 0 && _editingIndex < lstManualSources.Items.Count)
            {
                lstManualSources.Items[_editingIndex] = itemText;
                _editingIndex = -1;
                btnAddCustom.Text = "Додати";
            }
            else
            {
                lstManualSources.Items.Add(itemText);
            }

            txtCustomSourceName.Text = "";
            txtCustomPrice.Text = "";
            txtCustomPack.Text = "";
        }

        private void btnEditCustom_Click(object sender, EventArgs e)
        {
            if (lstManualSources.SelectedItem == null) return;
            var idx = lstManualSources.SelectedIndex;
            var s = Convert.ToString(lstManualSources.SelectedItem) ?? "";
            var parts = s.Split(new[] { ':' }, 3);
            txtCustomSourceName.Text = parts[0].Trim();
            txtCustomPrice.Text = parts.Length > 1 ? parts[1].Trim() : "";
            txtCustomPack.Text = parts.Length > 2 ? parts[2].Trim() : "";

            _editingIndex = idx;
            btnAddCustom.Text = "Зберегти";
        }

        private void lstManualSources_DoubleClick(object sender, EventArgs e) => btnEditCustom_Click(sender, e);

        private void btnRemoveCustom_Click(object sender, EventArgs e)
        {
            if (lstManualSources.SelectedItem != null)
            {
                var idx = lstManualSources.SelectedIndex;
                lstManualSources.Items.RemoveAt(idx);
                if (_editingIndex == idx)
                {
                    _editingIndex = -1;
                    btnAddCustom.Text = "Додати";
                    txtCustomSourceName.Text = "";
                    txtCustomPrice.Text = "";
                    txtCustomPack.Text = "";
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Product.Name = txtName.Text.Trim();

            // txtWeight is used as default pack value (in same unit as InventoryUnit)
            if (int.TryParse(txtWeight.Text, out var w)) Product.WeightPerUnitG = w;
            else Product.WeightPerUnitG = null;

            Product.Nutrition = new NutritionInfo
            {
                Carbs_100g = ParseDouble(txtCarbs.Text),
                Sugars_100g = ParseDouble(txtSugars.Text),
                Fats_100g = ParseDouble(txtFats.Text),
                Saturated_100g = ParseDouble(txtSat.Text),
                Protein_100g = ParseDouble(txtProt.Text),
                Kcal_100g = ParseDouble(txtKcal.Text),
                Fiber_100g = ParseDouble(txtFiber.Text)
            };

            // per-store prices
            var prices = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            if (decimal.TryParse(txtPriceATB.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d1) && d1 > 0) prices["АТБ"] = d1;
            if (decimal.TryParse(txtPriceSilpo.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2) && d2 > 0) prices["Сільпо"] = d2;
            if (decimal.TryParse(txtPriceStolich.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d3) && d3 > 0) prices["Столичний ринок"] = d3;
            if (decimal.TryParse(txtPriceMetro.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d4) && d4 > 0) prices["Метро"] = d4;
            if (decimal.TryParse(txtPriceVK.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d5) && d5 > 0) prices["Велика Кишеня"] = d5;
            if (decimal.TryParse(txtPriceAshan.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d6) && d6 > 0) prices["Ашан"] = d6;
            if (decimal.TryParse(txtPriceNovus.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d7) && d7 > 0) prices["NOVUS"] = d7;
            Product.Prices = prices;

            // links
            var links = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(txtLinkATB.Text)) links["АТБ"] = txtLinkATB.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkSilpo.Text)) links["Сільпо"] = txtLinkSilpo.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkStolich.Text)) links["Столичний ринок"] = txtLinkStolich.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkMetro.Text)) links["Метро"] = txtLinkMetro.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkVK.Text)) links["Велика Кишеня"] = txtLinkVK.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkAshan.Text)) links["Ашан"] = txtLinkAshan.Text.Trim();
            if (!string.IsNullOrWhiteSpace(txtLinkNovus.Text)) links["NOVUS"] = txtLinkNovus.Text.Trim();
            Product.Links = links;

            // per-store pack sizes
            var pack = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (int.TryParse(txtPackATB.Text, out var pa) && pa > 0) pack["АТБ"] = pa;
            if (int.TryParse(txtPackSilpo.Text, out var ps) && ps > 0) pack["Сільпо"] = ps;
            if (int.TryParse(txtPackMetro.Text, out var pm) && pm > 0) pack["Метро"] = pm;
            if (int.TryParse(txtPackAshan.Text, out var pah) && pah > 0) pack["Ашан"] = pah;
            if (int.TryParse(txtPackStolich.Text, out var pst) && pst > 0) pack["Столичний ринок"] = pst;
            if (int.TryParse(txtPackVK.Text, out var pvk) && pvk > 0) pack["Велика Кишеня"] = pvk;
            if (int.TryParse(txtPackNovus.Text, out var pnov) && pnov > 0) pack["NOVUS"] = pnov;
            Product.PackSizes = pack;

            // manual sources parsing: Name[:price][:pack]
            var manualSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var itm in lstManualSources.Items)
            {
                var s = Convert.ToString(itm) ?? "";
                if (string.IsNullOrWhiteSpace(s)) continue;
                var parts = s.Split(new[] { ':' }, 3);
                var nm = parts[0].Trim();
                if (string.IsNullOrEmpty(nm)) continue;
                manualSet.Add(nm);

                if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
                {
                    if (decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var pv) && pv > 0)
                    {
                        Product.Prices[nm] = pv;
                    }
                }

                if (parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
                {
                    if (int.TryParse(parts[2].Trim(), out var packVal) && packVal > 0)
                    {
                        Product.PackSizes[nm] = packVal;
                    }
                }
            }
            Product.ManualPriceSources = manualSet;

            Product.InventoryUnit = cmbUnit.SelectedItem?.ToString() ?? "g";

            DialogResult = DialogResult.OK;
            Close();
        }

        private double ParseDouble(string s)
        {
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;
            return 0;
        }
    }
}