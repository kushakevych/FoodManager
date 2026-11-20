using System;
using System.Collections.Generic;
using System.Linq;
using FoodManager.Services.Database;

namespace FoodManager.Services.Planning
{
    /// <summary>
    /// Implements weekly meal planning calculations, recipe nutritional scaling,
    /// and prefilled shopping requirements generation.
    /// </summary>
    public class WeeklyPlannerService : IWeeklyPlannerService
    {
        private readonly IDatabaseService _db;

        public WeeklyPlannerService(IDatabaseService? db = null)
        {
            _db = db ?? DatabaseService.Default;
        }

        public WeeklyPlanResult ComputeWeeklyTotals(IEnumerable<WeeklyPlanItem> planItems)
        {
            var result = new WeeklyPlanResult();
            var allProducts = _db.GetProducts().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var allRecipes = _db.GetRecipes().ToDictionary(r => r.Id);

            foreach (var item in planItems.Where(i => i.Servings > 0))
            {
                if (!allRecipes.TryGetValue(item.RecipeId, out var recipe)) continue;

                var singleTotals = recipe.CalculateTotals(allProducts);
                var multi = singleTotals.Multiply(item.Servings);

                if (!result.DayTotals.TryGetValue(item.Day, out var dayTotal))
                {
                    dayTotal = new NutritionTotals();
                    result.DayTotals[item.Day] = dayTotal;
                }

                dayTotal.Add(multi);
                result.WeekTotals.Add(multi);
            }

            // Build human-readable formatted summary
            var lines = new List<string>();
            foreach (var kv in result.DayTotals)
            {
                var t = kv.Value;
                t.Round();
                lines.Add($"{kv.Key}: {t.Kcal} ккал | Білки={t.Protein}г | Жири={t.Fats}г (нас.={t.Saturated}г) | Вуглеводи={t.Carbs}г (цукри={t.Sugars}г) | Вартість={t.Cost} грн");
            }

            if (result.DayTotals.Count > 0)
            {
                lines.Add(string.Empty);
            }

            result.WeekTotals.Round();
            lines.Add($"Тиждень: {result.WeekTotals.Kcal} ккал | Білки={result.WeekTotals.Protein}г | Жири={result.WeekTotals.Fats}г (нас.={result.WeekTotals.Saturated}г) | Вуглеводи={result.WeekTotals.Carbs}г (цукри={result.WeekTotals.Sugars}г) | Вартість={result.WeekTotals.Cost} грн");

            result.FormattedSummary = lines;
            return result;
        }

        public NutritionTotals ComputeSingleRecipeServings(int recipeId, int servings)
        {
            var recipe = _db.GetRecipeById(recipeId);
            if (recipe == null || servings <= 0) return new NutritionTotals();

            var allProducts = _db.GetProducts().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var single = recipe.CalculateTotals(allProducts);
            var multi = single.Multiply(servings);
            multi.Round();
            return multi;
        }

        public Dictionary<string, (int qty, string unit)> GenerateShoppingListPrefill(IEnumerable<WeeklyPlanItem> planItems)
        {
            var prefill = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            var allProducts = _db.GetProducts().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            var allRecipes = _db.GetRecipes().ToDictionary(r => r.Id);

            foreach (var item in planItems.Where(i => i.Servings > 0))
            {
                if (!allRecipes.TryGetValue(item.RecipeId, out var recipe)) continue;

                foreach (var kv in recipe.Ingredients)
                {
                    var ingName = kv.Key;
                    var amount = kv.Value;
                    var total = amount.Quantity * item.Servings;

                    if (!prefill.TryGetValue(ingName, out var cur))
                    {
                        prefill[ingName] = (total, amount.Unit);
                    }
                    else if (string.Equals(cur.unit, amount.Unit, StringComparison.OrdinalIgnoreCase))
                    {
                        prefill[ingName] = (cur.qty + total, cur.unit);
                    }
                    else
                    {
                        // Convert if possible
                        allProducts.TryGetValue(ingName, out var p);
                        if (p?.WeightPerUnitG > 0)
                        {
                            int curG = cur.unit == "pcs" ? cur.qty * p.WeightPerUnitG.Value : cur.qty;
                            int addG = amount.Unit == "pcs" ? total * p.WeightPerUnitG.Value : total;
                            prefill[ingName] = (curG + addG, "g");
                        }
                        else
                        {
                            prefill[ingName] = (cur.qty + total, cur.unit);
                        }
                    }
                }
            }
            return prefill;
        }
    }
}
