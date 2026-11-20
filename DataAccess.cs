using System;
using System.Collections.Generic;
using System.Data;
using FoodManager.Services.Database;

namespace FoodManager
{
    /// <summary>
    /// Static facade delegating to <see cref="DatabaseService.Default"/>.
    /// Maintained for backwards compatibility.
    /// </summary>
    public static class DataAccess
    {
        public static IDatabaseService Service => DatabaseService.Default;

        public static void EnsureDatabase() => Service.EnsureDatabase();

        public static DataTable QueryTable(string sql) => Service.QueryTable(sql);

        public static IEnumerable<Product> GetProducts() => Service.GetProducts();

        public static Product? GetProductByName(string name) => Service.GetProductByName(name);

        public static Product? GetProductById(int id) => Service.GetProductById(id);

        public static IEnumerable<Recipe> GetRecipes() => Service.GetRecipes();

        public static Recipe? GetRecipeById(int id) => Service.GetRecipeById(id);

        public static int InsertProduct(Product p) => Service.InsertProduct(p);

        public static void UpdateProduct(Product p) => Service.UpdateProduct(p);

        public static void DeleteProduct(int id) => Service.DeleteProduct(id);

        public static int InsertRecipe(Recipe r) => Service.InsertRecipe(r);

        public static void UpdateRecipe(Recipe r) => Service.UpdateRecipe(r);

        public static void DeleteRecipe(int id) => Service.DeleteRecipe(id);

        public static (int quantity, string unit) GetInventoryQuantityForProduct(string productName) =>
            Service.GetInventoryQuantityForProduct(productName);

        public static (int quantity, string unit) GetInventoryQuantityForProductById(int productId) =>
            Service.GetInventoryQuantityForProductById(productId);

        public static IReadOnlyList<InventorySummary> GetAllInventorySummaries() =>
            Service.GetAllInventorySummaries();

        public static void AddInventory(int productId, int quantity, string unit) =>
            Service.AddInventory(productId, quantity, unit);

        public static void ReduceInventory(int productId, int quantity, string unit) =>
            Service.ReduceInventory(productId, quantity, unit);

        public static void PerformRecount(int productId, int newQuantity, string unit, string? note = null) =>
            Service.PerformRecount(productId, newQuantity, unit, note);

        public static void PerformBulkRecount((int productId, int oldQty, int newQty, string unit)[] items, string? note = null) =>
            Service.PerformBulkRecount(items, note);

        public static void PerformBulkRecount(BulkRecountItem[] items, string? note = null) =>
            Service.PerformBulkRecount(items, note);

        public static List<RecountSession> GetRecountSessions() => Service.GetRecountSessions();

        public static List<RecountItemRow> GetRecountItemsByRecountId(long recountId) =>
            Service.GetRecountItemsByRecountId(recountId);

        public static void BackupDatabase(string targetFilePath) => Service.BackupDatabase(targetFilePath);

        public static void RestoreDatabase(string sourceFilePath) => Service.RestoreDatabase(sourceFilePath);
    }
}
