using System;
using System.Collections.Generic;
using System.Linq;
using FoodManager.Common;
using FoodManager.Services.Database;

namespace FoodManager.Services.Planning
{
    /// <summary>
    /// Pure business logic for shopping list calculation, inventory deduction and packaging rounding.
    /// Decoupled from WinForms controls and UI state.
    /// </summary>
    public class ShoppingCalculationService : IShoppingCalculationService
    {
        private readonly IDatabaseService _db;

        public ShoppingCalculationService(IDatabaseService? db = null)
        {
            _db = db ?? DatabaseService.Default;
        }

        public ShoppingCalculationResult CalculatePlan(
            IEnumerable<DirectBuyRequest> directBuys,
            IEnumerable<RecipeNeedRequest> recipes,
            bool considerInventoryForDirectBuys,
            bool roundToPack,
            IReadOnlyDictionary<string, string?>? selectedStoresByProduct = null)
        {
            var result = new ShoppingCalculationResult();

            // 1) Direct purchases dictionary
            var directDict = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            foreach (var db in directBuys.Where(d => d.Quantity > 0))
            {
                directDict[db.ProductName] = (db.Quantity, db.Unit);
            }

            // 2) Recipe ingredients aggregation
            var allRecipes = _db.GetRecipes().ToDictionary(r => r.Id);
            var allProducts = _db.GetProducts().ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var recipeNeeds = new Dictionary<string, (int qty, string unit)>(StringComparer.OrdinalIgnoreCase);
            foreach (var rReq in recipes.Where(r => r.Servings > 0))
            {
                if (!allRecipes.TryGetValue(rReq.RecipeId, out var recipe)) continue;

                foreach (var kv in recipe.Ingredients)
                {
                    var ingName = kv.Key;
                    var amount = kv.Value;
                    var totalQty = amount.Quantity * rReq.Servings;

                    if (!recipeNeeds.TryGetValue(ingName, out var cur))
                    {
                        recipeNeeds[ingName] = (totalQty, amount.Unit);
                    }
                    else if (string.Equals(cur.unit, amount.Unit, StringComparison.OrdinalIgnoreCase))
                    {
                        recipeNeeds[ingName] = (cur.qty + totalQty, cur.unit);
                    }
                    else
                    {
                        // Unit mismatch between recipe requirements: try to convert using product info
                        allProducts.TryGetValue(ingName, out var prod);
                        if (prod?.WeightPerUnitG > 0)
                        {
                            int curGrams = StoreConstants.IsPieces(cur.unit) ? cur.qty * prod.WeightPerUnitG.Value : cur.qty;
                            int addGrams = StoreConstants.IsPieces(amount.Unit) ? totalQty * prod.WeightPerUnitG.Value : totalQty;
                            recipeNeeds[ingName] = (curGrams + addGrams, StoreConstants.UnitGrams);
                        }
                        else
                        {
                            recipeNeeds[ingName] = (cur.qty + totalQty, cur.unit);
                            result.Warnings.Add($"Інгредієнт '{ingName}' вказаний у різних одиницях ({cur.unit} та {amount.Unit}) без відомої ваги шт.");
                        }
                    }
                }
            }

            var allNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            allNames.UnionWith(directDict.Keys);
            allNames.UnionWith(recipeNeeds.Keys);

            var planEntries = new List<PlanEntry>();
            var outputLines = new List<string>();

            foreach (var name in allNames.OrderBy(n => n))
            {
                allProducts.TryGetValue(name, out var product);
                string canonicalUnit = product?.InventoryUnit ?? "g";

                directDict.TryGetValue(name, out var direct);
                recipeNeeds.TryGetValue(name, out var recipeNeed);

                // Convert direct amount to canonical
                int directInCanonical = direct.qty;
                if (!string.IsNullOrEmpty(direct.unit) && !string.Equals(direct.unit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                {
                    if (product?.WeightPerUnitG > 0)
                    {
                        if (StoreConstants.IsGrams(direct.unit) && StoreConstants.IsPieces(canonicalUnit))
                        {
                            directInCanonical = (int)Math.Ceiling((double)direct.qty / product.WeightPerUnitG.Value);
                        }
                        else if (StoreConstants.IsPieces(direct.unit) && StoreConstants.IsGrams(canonicalUnit))
                        {
                            directInCanonical = direct.qty * product.WeightPerUnitG.Value;
                        }
                    }
                }

                // Convert recipe amount to canonical
                int recipeInCanonical = recipeNeed.qty;
                if (!string.IsNullOrEmpty(recipeNeed.unit) && !string.Equals(recipeNeed.unit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                {
                    if (product?.WeightPerUnitG > 0)
                    {
                        if (StoreConstants.IsGrams(recipeNeed.unit) && StoreConstants.IsPieces(canonicalUnit))
                        {
                            recipeInCanonical = (int)Math.Ceiling((double)recipeNeed.qty / product.WeightPerUnitG.Value);
                        }
                        else if (StoreConstants.IsPieces(recipeNeed.unit) && StoreConstants.IsGrams(canonicalUnit))
                        {
                            recipeInCanonical = recipeNeed.qty * product.WeightPerUnitG.Value;
                        }
                    }
                    else
                    {
                        result.Warnings.Add($"'{name}': неможливо конвертувати {recipeNeed.unit} у {canonicalUnit} для обліку.");
                    }
                }

                // Inventory available
                var (haveQty, haveUnit) = _db.GetInventoryQuantityForProduct(name);
                int haveInCanonical = haveQty;
                if (!string.Equals(haveUnit, canonicalUnit, StringComparison.OrdinalIgnoreCase))
                {
                    if (product?.WeightPerUnitG > 0)
                    {
                        if (StoreConstants.IsGrams(haveUnit) && StoreConstants.IsPieces(canonicalUnit))
                        {
                            haveInCanonical = (int)Math.Floor((double)haveQty / product.WeightPerUnitG.Value);
                        }
                        else if (StoreConstants.IsPieces(haveUnit) && StoreConstants.IsGrams(canonicalUnit))
                        {
                            haveInCanonical = haveQty * product.WeightPerUnitG.Value;
                        }
                    }
                    else if (haveQty > 0)
                    {
                        result.Warnings.Add($"'{name}': одиниця в наявності ({haveUnit}) відрізняється від канонічної ({canonicalUnit}).");
                    }
                }

                // Compute needed quantity
                int toBuy;
                int originalQty;
                if (considerInventoryForDirectBuys)
                {
                    int totalNeeded = directInCanonical + recipeInCanonical;
                    toBuy = Math.Max(0, totalNeeded - Math.Max(0, haveInCanonical));
                    originalQty = toBuy;
                }
                else
                {
                    int fromRecipes = Math.Max(0, recipeInCanonical - Math.Max(0, haveInCanonical));
                    toBuy = directInCanonical + fromRecipes;
                    originalQty = toBuy;
                }

                if (toBuy <= 0) continue;

                var entry = new PlanEntry
                {
                    Name = name,
                    Unit = canonicalUnit,
                    OriginalQty = originalQty,
                    TotalQty = toBuy,
                    DirectQty = directInCanonical,
                    RecipeQty = recipeInCanonical
                };

                // Check store and apply pack rounding
                string? selStore = null;
                selectedStoresByProduct?.TryGetValue(name, out selStore);

                if (roundToPack)
                {
                    ApplyRounding(entry, product, selStore);
                }

                planEntries.Add(entry);
                outputLines.Add($"{name}: {entry.TotalQty} {StoreConstants.GetDisplayUnit(entry.Unit)}" +
                                (entry.Packs.HasValue ? $" ({entry.Packs.Value} упак.)" : ""));
            }

            result.PlanEntries = planEntries;
            result.OutputLines = outputLines;
            return result;
        }

        public void ApplyRounding(PlanEntry entry, Product? product, string? selectedStore)
        {
            if (product == null) return;

            var packSize = product.GetPackSizeForStore(selectedStore);
            if (packSize.HasValue && packSize.Value > 0)
            {
                int packs = (int)Math.Ceiling((double)entry.OriginalQty / packSize.Value);
                if (packs <= 0 && entry.OriginalQty > 0) packs = 1;

                entry.TotalQty = packs * packSize.Value;
                entry.Packs = packs;
                entry.Note = $"Округлено до {packs} упак. по {packSize.Value} {StoreConstants.GetDisplayUnit(entry.Unit)}";
            }
            else
            {
                entry.Packs = null;
            }
        }
    }
}
