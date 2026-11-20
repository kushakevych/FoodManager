using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace FoodManager
{
    /// <summary>
    /// Recipe entity representing a dish with named ingredients and portions.
    /// </summary>
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Dictionary<string, IngredientAmount> Ingredients { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public static Recipe FromReader(SqliteDataReader r)
        {
            var rec = new Recipe
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Status = !r.IsDBNull(2) ? r.GetString(2) : string.Empty
            };

            if (!r.IsDBNull(3))
            {
                var json = r.GetString(3);
                try
                {
                    rec.Ingredients = JsonSerializer.Deserialize<Dictionary<string, IngredientAmount>>(json)
                        ?? new(StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    try
                    {
                        var legacy = JsonSerializer.Deserialize<Dictionary<string, int>>(json)
                            ?? new();
                        rec.Ingredients = new(StringComparer.OrdinalIgnoreCase);
                        foreach (var kv in legacy)
                        {
                            rec.Ingredients[kv.Key] = new IngredientAmount { Quantity = kv.Value, Unit = "g" };
                        }
                    }
                    catch
                    {
                        rec.Ingredients = new(StringComparer.OrdinalIgnoreCase);
                    }
                }
            }
            return rec;
        }

        /// <summary>
        /// Calculates nutritional totals and cheapest cost using the default database lookup.
        /// </summary>
        public NutritionTotals CalculateTotals()
        {
            return CalculateTotals(DataAccess.GetProductByName);
        }

        /// <summary>
        /// High-performance calculation overload that avoids database queries by resolving products in-memory.
        /// No automatic conversion between "g" and "pcs" — only matching units are computed.
        /// </summary>
        public NutritionTotals CalculateTotals(Func<string, Product?> productResolver)
        {
            var totals = new NutritionTotals();

            foreach (var kv in Ingredients)
            {
                var ingName = kv.Key;
                var amount = kv.Value;
                var prod = productResolver(ingName);
                if (prod == null) continue;

                // Units must match
                if (!string.Equals(amount.Unit, prod.InventoryUnit, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (string.Equals(prod.InventoryUnit, "g", StringComparison.OrdinalIgnoreCase))
                {
                    double factor = amount.Quantity / 100.0;
                    totals.Kcal += prod.Nutrition.Kcal_100g * factor;
                    totals.Protein += prod.Nutrition.Protein_100g * factor;
                    totals.Carbs += prod.Nutrition.Carbs_100g * factor;
                    totals.Sugars += prod.Nutrition.Sugars_100g * factor;
                    totals.Fats += prod.Nutrition.Fats_100g * factor;
                    totals.Saturated += prod.Nutrition.Saturated_100g * factor;
                    totals.Fiber += prod.Nutrition.Fiber_100g * factor;
                }
                else // "pcs"
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

                // Choose cheapest store price per unit
                var (_, bestPerUnit, _) = prod.GetCheapestStoreInfo();
                if (bestPerUnit.HasValue)
                {
                    totals.Cost += bestPerUnit.Value * amount.Quantity;
                }
            }

            totals.Round();
            return totals;
        }

        public NutritionTotals CalculateTotals(IReadOnlyDictionary<string, Product> productsLookup)
        {
            return CalculateTotals(name => productsLookup.TryGetValue(name, out var p) ? p : null);
        }
    }
}
