using System.Collections.Generic;
using FoodManager;
using FoodManager.Services.Database;
using FoodManager.Services.Planning;
using Xunit;

namespace FoodManager.Tests
{
    public class ShoppingCalculationTests
    {
        private class FakeDatabaseService : IDatabaseService
        {
            public List<Product> Products { get; } = new();
            public List<Recipe> Recipes { get; } = new();
            public Dictionary<string, (int qty, string unit)> Inventory { get; } = new();

            public void EnsureDatabase() { }
            public System.Data.DataTable QueryTable(string sql) => new();
            public IEnumerable<Product> GetProducts() => Products;
            public Product? GetProductById(int id) => Products.Find(p => p.Id == id);
            public Product? GetProductByName(string name) => Products.Find(p => p.Name == name);
            public int InsertProduct(Product p) => 1;
            public void UpdateProduct(Product p) { }
            public void DeleteProduct(int id) { }
            public IEnumerable<Recipe> GetRecipes() => Recipes;
            public Recipe? GetRecipeById(int id) => Recipes.Find(r => r.Id == id);
            public int InsertRecipe(Recipe r) => 1;
            public void UpdateRecipe(Recipe r) { }
            public void DeleteRecipe(int id) { }
            public (int quantity, string unit) GetInventoryQuantityForProduct(string productName) =>
                Inventory.TryGetValue(productName, out var v) ? v : (0, "g");
            public (int quantity, string unit) GetInventoryQuantityForProductById(int productId) => (0, "g");
            public IReadOnlyList<InventorySummary> GetAllInventorySummaries() => new List<InventorySummary>();
            public void AddInventory(int productId, int quantity, string unit) { }
            public void ReduceInventory(int productId, int quantity, string unit) { }
            public void PerformRecount(int productId, int newQuantity, string unit, string? note = null) { }
            public void PerformBulkRecount(BulkRecountItem[] items, string? note = null) { }
            public void PerformBulkRecount((int productId, int oldQty, int newQty, string unit)[] items, string? note = null) { }
            public List<RecountSession> GetRecountSessions() => new();
            public List<RecountItemRow> GetRecountItemsByRecountId(long recountId) => new();
            public void BackupDatabase(string targetFilePath) { }
            public void RestoreDatabase(string sourceFilePath) { }
        }

        [Fact]
        public void CalculatePlan_DirectBuysAndInventory_SubtractsInventoryCorrectly()
        {
            var fakeDb = new FakeDatabaseService();
            fakeDb.Products.Add(new Product { Id = 1, Name = "Молоко", InventoryUnit = "g", WeightPerUnitG = 1000 });
            fakeDb.Inventory["Молоко"] = (300, "g"); // 300g in stock

            var service = new ShoppingCalculationService(fakeDb);

            var directBuys = new[] { new DirectBuyRequest("Молоко", 1000, "g") };
            var recipes = System.Array.Empty<RecipeNeedRequest>();

            // When considering inventory for direct buys: 1000 needed - 300 have = 700 to buy
            var result = service.CalculatePlan(directBuys, recipes, considerInventoryForDirectBuys: true, roundToPack: false);

            Assert.Single(result.PlanEntries);
            Assert.Equal("Молоко", result.PlanEntries[0].Name);
            Assert.Equal(700, result.PlanEntries[0].TotalQty);
        }

        [Fact]
        public void CalculatePlan_RoundToPack_RoundsUpToNearestPack()
        {
            var fakeDb = new FakeDatabaseService();
            var milk = new Product
            {
                Id = 1,
                Name = "Молоко",
                InventoryUnit = "g",
                WeightPerUnitG = 900,
                PackSizes = new Dictionary<string, int> { ["АТБ"] = 900 }
            };
            fakeDb.Products.Add(milk);

            var service = new ShoppingCalculationService(fakeDb);

            var directBuys = new[] { new DirectBuyRequest("Молоко", 1500, "g") };
            var recipes = System.Array.Empty<RecipeNeedRequest>();
            var selectedStores = new Dictionary<string, string?> { ["Молоко"] = "АТБ" };

            // 1500g needed with 900g packs => ceil(1500 / 900) = 2 packs => 1800g
            var result = service.CalculatePlan(
                directBuys, recipes,
                considerInventoryForDirectBuys: false,
                roundToPack: true,
                selectedStoresByProduct: selectedStores);

            Assert.Single(result.PlanEntries);
            var entry = result.PlanEntries[0];
            Assert.Equal(1500, entry.OriginalQty);
            Assert.Equal(1800, entry.TotalQty);
            Assert.Equal(2, entry.Packs);
        }

        [Fact]
        public void CalculatePlan_RecipeIngredients_ScalesWithServings()
        {
            var fakeDb = new FakeDatabaseService();
            fakeDb.Products.Add(new Product { Id = 1, Name = "Борошно", InventoryUnit = "g", WeightPerUnitG = 1000 });
            fakeDb.Recipes.Add(new Recipe
            {
                Id = 10,
                Name = "Млинці",
                Ingredients = new Dictionary<string, IngredientAmount>
                {
                    ["Борошно"] = new() { Quantity = 250, Unit = "g" }
                }
            });

            var service = new ShoppingCalculationService(fakeDb);

            var recipes = new[] { new RecipeNeedRequest(10, 4) }; // 4 servings => 250 * 4 = 1000g
            var result = service.CalculatePlan(System.Array.Empty<DirectBuyRequest>(), recipes, false, false);

            Assert.Single(result.PlanEntries);
            Assert.Equal(1000, result.PlanEntries[0].TotalQty);
        }
    }
}
