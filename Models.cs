using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace FoodManager
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // ¬ј∆Ќќ: semantic change Ч поле 'weight_per_unit_g' (txtWeight в UI) используетс€ как фасовка по замовчуванню.
        // ≈го значение интерпретируетс€ в той же единице, что и InventoryUnit.
        // “.е. если InventoryUnit == "g" Ч это число граммов; если "pcs" Ч это число штук (обычно 1).
        public int? WeightPerUnitG { get; set; }

        public NutritionInfo Nutrition { get; set; } = new NutritionInfo();
        public Dictionary<string, decimal> Prices { get; set; } = new Dictionary<string, decimal>();
        public List<string> Barcodes { get; set; } = new List<string>();
        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();

        // "g" or "pcs"
        public string InventoryUnit { get; set; } = "g";

        // manual price sources set (names)
        public HashSet<string> ManualPriceSources { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // per-store pack sizes (store -> size), interpreted in same unit as InventoryUnit
        public Dictionary<string, int> PackSizes { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public static Product FromReader(SqliteDataReader r)
        {
            var p = new Product();
            try
            {
                // safe reads by column name where possible
                if (HasColumn(r, "id") && !r.IsDBNull(r.GetOrdinal("id"))) p.Id = r.GetInt32(r.GetOrdinal("id"));
                if (HasColumn(r, "name") && !r.IsDBNull(r.GetOrdinal("name"))) p.Name = r.GetString(r.GetOrdinal("name"));
                if (HasColumn(r, "weight_per_unit_g") && !r.IsDBNull(r.GetOrdinal("weight_per_unit_g"))) p.WeightPerUnitG = r.GetInt32(r.GetOrdinal("weight_per_unit_g"));

                if (HasColumn(r, "nutrition_json") && !r.IsDBNull(r.GetOrdinal("nutrition_json")))
                {
                    try { p.Nutrition = JsonSerializer.Deserialize<NutritionInfo>(r.GetString(r.GetOrdinal("nutrition_json"))) ?? new NutritionInfo(); }
                    catch { p.Nutrition = new NutritionInfo(); }
                }

                if (HasColumn(r, "price_json") && !r.IsDBNull(r.GetOrdinal("price_json")))
                {
                    try { p.Prices = JsonSerializer.Deserialize<Dictionary<string, decimal>>(r.GetString(r.GetOrdinal("price_json"))) ?? new Dictionary<string, decimal>(); }
                    catch { p.Prices = new Dictionary<string, decimal>(); }
                }

                if (HasColumn(r, "barcodes_json") && !r.IsDBNull(r.GetOrdinal("barcodes_json")))
                {
                    try { p.Barcodes = JsonSerializer.Deserialize<List<string>>(r.GetString(r.GetOrdinal("barcodes_json"))) ?? new List<string>(); }
                    catch { p.Barcodes = new List<string>(); }
                }

                if (HasColumn(r, "links_json") && !r.IsDBNull(r.GetOrdinal("links_json")))
                {
                    try { p.Links = JsonSerializer.Deserialize<Dictionary<string, string>>(r.GetString(r.GetOrdinal("links_json"))) ?? new Dictionary<string, string>(); }
                    catch { p.Links = new Dictionary<string, string>(); }
                }

                if (HasColumn(r, "manual_sources_json") && !r.IsDBNull(r.GetOrdinal("manual_sources_json")))
                {
                    try
                    {
                        var arr = JsonSerializer.Deserialize<List<string>>(r.GetString(r.GetOrdinal("manual_sources_json"))) ?? new List<string>();
                        p.ManualPriceSources = new HashSet<string>(arr, StringComparer.OrdinalIgnoreCase);
                    }
                    catch { p.ManualPriceSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "pack_json") && !r.IsDBNull(r.GetOrdinal("pack_json")))
                {
                    try { p.PackSizes = JsonSerializer.Deserialize<Dictionary<string, int>>(r.GetString(r.GetOrdinal("pack_json"))) ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); }
                    catch { p.PackSizes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); }
                }

                if (HasColumn(r, "inventory_unit") && !r.IsDBNull(r.GetOrdinal("inventory_unit")))
                {
                    p.InventoryUnit = r.GetString(r.GetOrdinal("inventory_unit")) ?? "g";
                }
            }
            catch
            {
                // best-effort
            }
            return p;
        }

        private static bool HasColumn(SqliteDataReader r, string name)
        {
            try { var _ = r.GetOrdinal(name); return true; } catch { return false; }
        }

        // ѕолучить фасовку дл€ конкретного магазина (в тех же единицах, что и product.InventoryUnit).
        // Ћогика: per-store pack -> WeightPerUnitG (значение txtWeight) -> null (unknown).
        public int? GetPackSizeForStore(string? store)
        {
            if (!string.IsNullOrEmpty(store) && PackSizes != null && PackSizes.TryGetValue(store!, out var v) && v > 0)
                return v;

            if (WeightPerUnitG.HasValue && WeightPerUnitG.Value > 0)
                return WeightPerUnitG.Value;

            // No default known
            return null;
        }

        // ÷ена за единицу (в единице InventoryUnit) из storePrice и store pack.
        // ¬озвращает null если storePack неизвестен или равен 0.
        public decimal? PricePerUnitFromStore(string store, decimal storePrice)
        {
            var storePack = GetPackSizeForStore(store);
            if (!storePack.HasValue || storePack.Value <= 0) return null;
            try { return storePrice / (decimal)storePack.Value; }
            catch { return null; }
        }

        // Ќормализовать цену магазина к значению по умолчанию (используетс€ GetPackSizeForStore(null) -> WeightPerUnitG)
        public decimal? NormalizePriceToDefault(string store, decimal storePrice)
        {
            var storePack = GetPackSizeForStore(store);
            var defaultPack = GetPackSizeForStore(null);
            if (!storePack.HasValue || !defaultPack.HasValue) return null;
            if (storePack.Value == 0) return null;
            return storePrice * (decimal)defaultPack.Value / (decimal)storePack.Value;
        }
    }

    public class NutritionInfo
    {
        // For InventoryUnit == "g" these are per-100g,
        // for InventoryUnit == "pcs" these are per-piece (interpretation depends on product.InventoryUnit)
        public double Carbs_100g { get; set; }
        public double Sugars_100g { get; set; }
        public double Fats_100g { get; set; }
        public double Saturated_100g { get; set; }
        public double Protein_100g { get; set; }
        public double Kcal_100g { get; set; }
        public double Fiber_100g { get; set; }
    }

    public class IngredientAmount
    {
        public int Quantity { get; set; } = 0;
        // "g" or "pcs"
        public string Unit { get; set; } = "g";
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Status { get; set; } = "";
        // ingredient name -> amount + unit
        public Dictionary<string, IngredientAmount> Ingredients { get; set; } = new Dictionary<string, IngredientAmount>();

        public static Recipe FromReader(SqliteDataReader r)
        {
            var rec = new Recipe();
            rec.Id = r.GetInt32(0);
            rec.Name = r.GetString(1);
            if (!r.IsDBNull(2)) rec.Status = r.GetString(2);
            if (!r.IsDBNull(3))
            {
                var json = r.GetString(3);
                try
                {
                    rec.Ingredients = JsonSerializer.Deserialize<Dictionary<string, IngredientAmount>>(json)
                        ?? new Dictionary<string, IngredientAmount>();
                }
                catch
                {
                    try
                    {
                        var legacy = JsonSerializer.Deserialize<Dictionary<string, int>>(json)
                            ?? new Dictionary<string, int>();
                        foreach (var kv in legacy)
                        {
                            rec.Ingredients[kv.Key] = new IngredientAmount { Quantity = kv.Value, Unit = "g" };
                        }
                    }
                    catch
                    {
                        rec.Ingredients = new Dictionary<string, IngredientAmount>();
                    }
                }
            }
            return rec;
        }

        // Calculate totals: IMPORTANT Ч no automatic conversion between g and pcs.
        // Only include ingredient contribution if ingredient.Unit == product.InventoryUnit.
        public NutritionTotals CalculateTotals()
        {
            var totals = new NutritionTotals();

            foreach (var kv in Ingredients)
            {
                var ingName = kv.Key;
                var amount = kv.Value;
                var prod = DataAccess.GetProductByName(ingName);
                if (prod == null) continue;

                // Only compute if units match (we don't auto-convert g<->pcs)
                if (!string.Equals(amount.Unit, prod.InventoryUnit, StringComparison.OrdinalIgnoreCase))
                {
                    // skip Ч cannot compute
                    continue;
                }

                if (prod.InventoryUnit == "g")
                {
                    // amount.Quantity is grams
                    double factor = amount.Quantity / 100.0;
                    totals.Kcal += prod.Nutrition.Kcal_100g * factor;
                    totals.Protein += prod.Nutrition.Protein_100g * factor;
                    totals.Carbs += prod.Nutrition.Carbs_100g * factor;
                    totals.Sugars += prod.Nutrition.Sugars_100g * factor;
                    totals.Fats += prod.Nutrition.Fats_100g * factor;
                    totals.Saturated += prod.Nutrition.Saturated_100g * factor;
                    totals.Fiber += prod.Nutrition.Fiber_100g * factor;
                }
                else // pcs: nutrition interpreted per piece
                {
                    double pieces = amount.Quantity;
                    totals.Kcal += prod.Nutrition.Kcal_100g * pieces;
                    totals.Protein += prod.Nutrition.Protein_100g * pieces;
                    totals.Carbs += prod.Nutrition.Carbs_100g * pieces;
                    totals.Sugars += prod.Nutrition.Sugars_100g * pieces;
                    totals.Fats += prod.Nutrition.Fats_100g * pieces;
                    totals.Saturated += prod.Nutrition.Saturated_100g * pieces;
                    totals.Fiber += prod.Nutrition.Fiber_100g * pieces;
                }

                // Cost: choose cheapest store by computed price-per-unit (in same unit)
                decimal bestPerUnit = 0;
                bool found = false;
                if (prod.Prices != null)
                {
                    foreach (var pr in prod.Prices)
                    {
                        var st = pr.Key;
                        var pv = pr.Value;
                        var ppu = prod.PricePerUnitFromStore(st, pv);
                        if (ppu.HasValue)
                        {
                            if (!found || ppu.Value < bestPerUnit) { bestPerUnit = ppu.Value; found = true; }
                        }
                    }
                }

                if (found)
                {
                    if (prod.InventoryUnit == "g")
                    {
                        totals.Cost += bestPerUnit * amount.Quantity;
                    }
                    else
                    {
                        totals.Cost += bestPerUnit * amount.Quantity;
                    }
                }
            }

            totals.Round();
            return totals;
        }
    }

    public class NutritionTotals
    {
        public double Carbs { get; set; } = 0;
        public double Sugars { get; set; } = 0;
        public double Fats { get; set; } = 0;
        public double Saturated { get; set; } = 0;
        public double Protein { get; set; } = 0;
        public double Kcal { get; set; } = 0;
        public double Fiber { get; set; } = 0;
        public decimal Cost { get; set; } = 0;

        public void Round()
        {
            Carbs = Math.Round(Carbs, 2);
            Sugars = Math.Round(Sugars, 2);
            Fats = Math.Round(Fats, 2);
            Saturated = Math.Round(Saturated, 2);
            Protein = Math.Round(Protein, 2);
            Kcal = Math.Round(Kcal, 1);
            Fiber = Math.Round(Fiber, 2);
            Cost = Math.Round(Cost, 2);
        }
    }
}