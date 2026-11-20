using System;
using System.Collections.Generic;
using FoodManager;
using Xunit;

namespace FoodManager.Tests
{
    public class ModelsTests
    {
        [Fact]
        public void Product_GetPackSizeForStore_PrefersStoreSpecificPack()
        {
            var p = new Product
            {
                WeightPerUnitG = 100,
                PackSizes = new Dictionary<string, int>
                {
                    ["АТБ"] = 250,
                    ["Сільпо"] = 500
                }
            };

            Assert.Equal(250, p.GetPackSizeForStore("АТБ"));
            Assert.Equal(500, p.GetPackSizeForStore("Сільпо"));
            Assert.Equal(100, p.GetPackSizeForStore("Метро")); // Fallback to default
            Assert.Equal(100, p.GetPackSizeForStore(null));     // Default
        }

        [Fact]
        public void Product_PricePerUnitFromStore_CalculatesCorrectly()
        {
            var p = new Product
            {
                WeightPerUnitG = 200,
                PackSizes = new Dictionary<string, int>
                {
                    ["АТБ"] = 500
                }
            };

            // 50 грн for 500g => 0.10 грн per 1g
            var ppu = p.PricePerUnitFromStore("АТБ", 50m);
            Assert.NotNull(ppu);
            Assert.Equal(0.10m, ppu.Value);

            // Unknown store uses default 200g: 20 грн for 200g => 0.10 грн per 1g
            var ppuMetro = p.PricePerUnitFromStore("Метро", 20m);
            Assert.NotNull(ppuMetro);
            Assert.Equal(0.10m, ppuMetro.Value);
        }

        [Fact]
        public void Product_GetCheapestStoreInfo_ReturnsLowestPricePerUnit()
        {
            var p = new Product
            {
                WeightPerUnitG = 1000,
                PackSizes = new Dictionary<string, int>
                {
                    ["АТБ"] = 1000,
                    ["Сільпо"] = 500
                },
                Prices = new Dictionary<string, decimal>
                {
                    ["АТБ"] = 40m,    // 0.040 per g
                    ["Сільпо"] = 15m   // 0.030 per g (cheaper)
                }
            };

            var (store, ppu, rawPrice) = p.GetCheapestStoreInfo();

            Assert.Equal("Сільпо", store);
            Assert.Equal(0.03m, ppu);
            Assert.Equal(15m, rawPrice);
        }

        [Fact]
        public void Recipe_CalculateTotals_ComputesNutrientsAndCostCorrectly()
        {
            var apple = new Product
            {
                Id = 1,
                Name = "Яблуко",
                InventoryUnit = "g",
                WeightPerUnitG = 100,
                Nutrition = new NutritionInfo
                {
                    Kcal_100g = 50,
                    Protein_100g = 0.5,
                    Carbs_100g = 12,
                    Fats_100g = 0.2
                },
                Prices = new Dictionary<string, decimal> { ["АТБ"] = 10m } // 10 грн per 100g = 0.10 грн/g
            };

            var recipe = new Recipe
            {
                Name = "Фруктовий десерт",
                Ingredients = new Dictionary<string, IngredientAmount>
                {
                    ["Яблуко"] = new() { Quantity = 200, Unit = "g" }
                }
            };

            var totals = recipe.CalculateTotals(name => name == "Яблуко" ? apple : null);

            Assert.Equal(100.0, totals.Kcal);    // 50 * 2
            Assert.Equal(1.0, totals.Protein);   // 0.5 * 2
            Assert.Equal(24.0, totals.Carbs);    // 12 * 2
            Assert.Equal(0.4, totals.Fats);      // 0.2 * 2
            Assert.Equal(20.0m, totals.Cost);    // 0.10 * 200
        }

        [Fact]
        public void Recipe_CalculateTotals_IgnoresMismatchedUnits()
        {
            var egg = new Product
            {
                Id = 2,
                Name = "Яйце",
                InventoryUnit = "pcs",
                Nutrition = new NutritionInfo { Kcal_100g = 70 } // per piece
            };

            var recipe = new Recipe
            {
                Name = "Омлет",
                Ingredients = new Dictionary<string, IngredientAmount>
                {
                    ["Яйце"] = new() { Quantity = 100, Unit = "g" } // Mismatched unit!
                }
            };

            var totals = recipe.CalculateTotals(name => name == "Яйце" ? egg : null);

            // Mismatched unit is safely ignored without throwing
            Assert.Equal(0.0, totals.Kcal);
        }

        [Fact]
        public void NutritionTotals_MultiplyAndRound_WorksAccurately()
        {
            var totals = new NutritionTotals
            {
                Kcal = 123.456,
                Protein = 10.123,
                Cost = 15.555m
            };

            var multi = totals.Multiply(3);
            multi.Round();

            Assert.Equal(370.4, multi.Kcal);
            Assert.Equal(30.37, multi.Protein);
            Assert.Equal(46.67m, multi.Cost);
        }
    }
}
