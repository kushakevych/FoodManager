using System;
using System.IO;
using System.Linq;
using FoodManager;
using FoodManager.Services.Database;
using Xunit;

namespace FoodManager.Tests
{
    public class DatabaseServiceTests : IDisposable
    {
        private readonly string _tempDbPath;
        private readonly DatabaseService _db;

        public DatabaseServiceTests()
        {
            _tempDbPath = Path.Combine(Path.GetTempPath(), $"test_food_{Guid.NewGuid():N}.db");
            _db = new DatabaseService(_tempDbPath);
            _db.EnsureDatabase();
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_tempDbPath))
                {
                    File.Delete(_tempDbPath);
                }
            }
            catch { }
        }

        [Fact]
        public void EnsureDatabase_CreatesTablesAndSampleData()
        {
            var products = _db.GetProducts().ToList();
            Assert.NotEmpty(products);

            var recipes = _db.GetRecipes().ToList();
            Assert.NotEmpty(recipes);

            var summaries = _db.GetAllInventorySummaries();
            Assert.NotEmpty(summaries);
        }

        [Fact]
        public void InsertAndUpdateProduct_WorksCorrectly()
        {
            var p = new Product
            {
                Name = "Банан",
                WeightPerUnitG = 120,
                InventoryUnit = "pcs",
                Prices = new() { ["АТБ"] = 55.5m }
            };

            int id = _db.InsertProduct(p);
            Assert.True(id > 0);

            var loaded = _db.GetProductById(id);
            Assert.NotNull(loaded);
            Assert.Equal("Банан", loaded.Name);
            Assert.Equal(120, loaded.WeightPerUnitG);
            Assert.Equal("pcs", loaded.InventoryUnit);
            Assert.Equal(55.5m, loaded.Prices["АТБ"]);

            // Update
            loaded.Name = "Банан Еквадор";
            loaded.Prices["АТБ"] = 59.9m;
            _db.UpdateProduct(loaded);

            var updated = _db.GetProductById(id);
            Assert.NotNull(updated);
            Assert.Equal("Банан Еквадор", updated.Name);
            Assert.Equal(59.9m, updated.Prices["АТБ"]);

            // Clean up
            _db.DeleteProduct(id);
            Assert.Null(_db.GetProductById(id));
        }

        [Fact]
        public void Inventory_AddAndReduce_CalculatesNetCorrectly()
        {
            var p = new Product { Name = "Груша", InventoryUnit = "g" };
            int id = _db.InsertProduct(p);

            _db.AddInventory(id, 500, "g");
            _db.AddInventory(id, 300, "g");
            _db.ReduceInventory(id, 200, "g");

            var (qty, unit) = _db.GetInventoryQuantityForProductById(id);
            Assert.Equal(600, qty); // 500 + 300 - 200
            Assert.Equal("g", unit);
        }

        [Fact]
        public void PerformBulkRecount_RecordsSessionAndAdjustsInventory()
        {
            var p = new Product { Name = "Рис", InventoryUnit = "g" };
            int id = _db.InsertProduct(p);
            _db.AddInventory(id, 1000, "g");

            var items = new[]
            {
                new BulkRecountItem(id, 1000, 850, "g") // Delta is -150
            };

            _db.PerformBulkRecount(items, "Щомісячний переоблік");

            var (qty, _) = _db.GetInventoryQuantityForProductById(id);
            Assert.Equal(850, qty);

            var sessions = _db.GetRecountSessions();
            Assert.NotEmpty(sessions);
            var latest = sessions.First();
            Assert.Equal("Щомісячний переоблік", latest.Note);

            var recountItems = _db.GetRecountItemsByRecountId(latest.Id);
            Assert.Single(recountItems);
            Assert.Equal(-150, recountItems[0].Delta);
            Assert.Equal(850, recountItems[0].NewQuantity);
        }
    }
}
