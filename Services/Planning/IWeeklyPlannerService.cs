using System.Collections.Generic;

namespace FoodManager.Services.Planning
{
    public record WeeklyPlanItem(string Day, int RecipeId, string RecipeName, int Servings);

    public class WeeklyPlanResult
    {
        public Dictionary<string, NutritionTotals> DayTotals { get; set; } = new();
        public NutritionTotals WeekTotals { get; set; } = new();
        public List<string> FormattedSummary { get; set; } = new();
    }

    /// <summary>
    /// Service for meal planning and weekly nutritional calculations.
    /// </summary>
    public interface IWeeklyPlannerService
    {
        WeeklyPlanResult ComputeWeeklyTotals(IEnumerable<WeeklyPlanItem> planItems);
        NutritionTotals ComputeSingleRecipeServings(int recipeId, int servings);
        Dictionary<string, (int qty, string unit)> GenerateShoppingListPrefill(IEnumerable<WeeklyPlanItem> planItems);
    }
}
