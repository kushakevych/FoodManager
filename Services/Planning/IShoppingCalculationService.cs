using System.Collections.Generic;

namespace FoodManager.Services.Planning
{
    public record DirectBuyRequest(string ProductName, int Quantity, string Unit);
    public record RecipeNeedRequest(int RecipeId, int Servings);

    public class ShoppingCalculationResult
    {
        public List<PlanEntry> PlanEntries { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> OutputLines { get; set; } = new();
        public decimal TotalCheapestCost { get; set; }
        public Dictionary<string, decimal> TotalCostByStore { get; set; } = new();
    }

    /// <summary>
    /// Service for calculating shopping plans, inventory deductions and pack roundings.
    /// </summary>
    public interface IShoppingCalculationService
    {
        ShoppingCalculationResult CalculatePlan(
            IEnumerable<DirectBuyRequest> directBuys,
            IEnumerable<RecipeNeedRequest> recipes,
            bool considerInventoryForDirectBuys,
            bool roundToPack,
            IReadOnlyDictionary<string, string?>? selectedStoresByProduct = null);

        void ApplyRounding(
            PlanEntry entry,
            Product? product,
            string? selectedStore);
    }
}
