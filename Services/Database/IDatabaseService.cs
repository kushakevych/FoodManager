using System.Collections.Generic;
using System.Data;

namespace FoodManager.Services.Database
{
    /// <summary>
    /// Contract for database operations and data access.
    /// </summary>
    public interface IDatabaseService
    {
        void EnsureDatabase();
        DataTable QueryTable(string sql);

        // Products
        IEnumerable<Product> GetProducts();
        Product? GetProductById(int id);
        Product? GetProductByName(string name);
        int InsertProduct(Product p);
        void UpdateProduct(Product p);
        void DeleteProduct(int id);

        // Recipes
        IEnumerable<Recipe> GetRecipes();
        Recipe? GetRecipeById(int id);
        int InsertRecipe(Recipe r);
        void UpdateRecipe(Recipe r);
        void DeleteRecipe(int id);

        // Inventory
        (int quantity, string unit) GetInventoryQuantityForProduct(string productName);
        (int quantity, string unit) GetInventoryQuantityForProductById(int productId);
        IReadOnlyList<InventorySummary> GetAllInventorySummaries();
        void AddInventory(int productId, int quantity, string unit);
        void ReduceInventory(int productId, int quantity, string unit);

        // Recounts
        void PerformRecount(int productId, int newQuantity, string unit, string? note = null);
        void PerformBulkRecount(BulkRecountItem[] items, string? note = null);
        void PerformBulkRecount((int productId, int oldQty, int newQty, string unit)[] items, string? note = null);
        List<RecountSession> GetRecountSessions();
        List<RecountItemRow> GetRecountItemsByRecountId(long recountId);

        // Backup & Restore
        void BackupDatabase(string targetFilePath);
        void RestoreDatabase(string sourceFilePath);
    }
}
