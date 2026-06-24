using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FoodManager
{
    public partial class AddRecipeForm : Form
    {
        public Recipe Recipe { get; private set; } = new Recipe();

        // internal storage for ingredients while editing
        private Dictionary<string, IngredientAmount> _ingredients = new Dictionary<string, IngredientAmount>();

        public AddRecipeForm() : this(null) { }

        public AddRecipeForm(Recipe existing)
        {
            InitializeComponent();
            cmbStatus.Items.AddRange(new string[] { "сніданок", "обід", "вечеря", "перекус" });

            // populate ingredient dropdown from products
            try
            {
                var products = DataAccess.GetProducts().Select(p => p.Name).ToArray();
                cmbIngredient.Items.AddRange(products);
                cmbIngredient.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbIngredient.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
            catch (Exception) { }

            // unit choices for ingredient amount (user may override for unknown product)
            cmbIngUnit.Items.Clear();
            cmbIngUnit.Items.AddRange(new string[] { "g", "pcs" });
            cmbIngUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIngUnit.SelectedItem = "g";

            if (existing != null)
            {
                Recipe = existing;
                txtName.Text = existing.Name;
                cmbStatus.Text = existing.Status;
                _ingredients = new Dictionary<string, IngredientAmount>(existing.Ingredients);
                UpdateIngredientsListDisplay();
            }

            cmbIngredient.TextChanged += CmbIngredient_TextChanged;
            cmbIngredient.SelectedIndexChanged += CmbIngredient_TextChanged;
        }

        private void CmbIngredient_TextChanged(object? sender, EventArgs e)
        {
            var name = cmbIngredient.Text?.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var prod = DataAccess.GetProductByName(name);
            if (prod != null)
            {
                // If product tracked in pcs => force unit to pcs and disable changing to g
                if (prod.InventoryUnit == "pcs")
                {
                    cmbIngUnit.SelectedItem = "pcs";
                    cmbIngUnit.Enabled = false;
                    lblUnitHint.Text = $"Одиниця продукту: шт (вага: {(prod.WeightPerUnitG?.ToString() ?? "не вказана")} г/шт)";
                }
                else
                {
                    cmbIngUnit.SelectedItem = "g";
                    cmbIngUnit.Enabled = true;
                    lblUnitHint.Text = $"Одиниця продукту: г";
                }
            }
            else
            {
                // unknown product - allow both units
                cmbIngUnit.Enabled = true;
                lblUnitHint.Text = "Продукт не знайдено у каталозі";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Recipe.Name = txtName.Text.Trim();
            Recipe.Status = cmbStatus.Text;
            // copy current ingredients
            Recipe.Ingredients = new Dictionary<string, IngredientAmount>(_ingredients);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnAddIngredient_Click(object sender, EventArgs e)
        {
            var name = cmbIngredient.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Оберіть або введіть назву інгредієнта.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var qty = Convert.ToInt32(numQuantity.Value);
            if (qty <= 0)
            {
                MessageBox.Show("Кількість має бути більшою за 0.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var unit = cmbIngUnit.SelectedItem?.ToString() ?? "g";

            // enforce rule: if product exists and is tracked in pcs, ingredient must be pcs
            var prod = DataAccess.GetProductByName(name);
            if (prod != null && prod.InventoryUnit == "pcs" && unit != "pcs")
            {
                MessageBox.Show($"Продукт '{name}' в обліку ведеться в штуках. Для рецепта інгредієнт повинен бути в штуках (шт). Якщо потрібно, змініть одиницю обліку продукту або вкажіть кількість в штуках.", "Невірна одиниця", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_ingredients.ContainsKey(name))
            {
                // if same ingredient and same unit -> sum quantities; if different unit -> keep separate entry by name (overwrites)
                var existing = _ingredients[name];
                if (existing.Unit == unit)
                {
                    existing.Quantity += qty;
                    _ingredients[name] = existing;
                }
                else
                {
                    // different unit for same-named ingredient -> replace and warn
                    _ingredients[name] = new IngredientAmount { Quantity = qty, Unit = unit };
                }
            }
            else
            {
                _ingredients[name] = new IngredientAmount { Quantity = qty, Unit = unit };
            }

            UpdateIngredientsListDisplay();
            numQuantity.Value = 0;
        }

        private void btnRemoveIngredient_Click(object sender, EventArgs e)
        {
            if (lstIngredients.SelectedItem == null) return;
            var txt = lstIngredients.SelectedItem.ToString();
            var idx = txt.IndexOf(':');
            if (idx > 0)
            {
                var name = txt.Substring(0, idx).Trim();
                if (_ingredients.ContainsKey(name))
                {
                    _ingredients.Remove(name);
                    UpdateIngredientsListDisplay();
                }
            }
        }

        private void lstIngredients_DoubleClick(object sender, EventArgs e)
        {
            btnRemoveIngredient_Click(sender, e);
        }

        private void UpdateIngredientsListDisplay()
        {
            lstIngredients.Items.Clear();
            foreach (var kv in _ingredients)
            {
                lstIngredients.Items.Add($"{kv.Key}: {kv.Value.Quantity} {kv.Value.Unit}");
            }
        }
    }
}