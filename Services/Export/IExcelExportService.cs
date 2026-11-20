using System.Collections.Generic;
using FoodManager.Services.Planning;

namespace FoodManager.Services.Export
{
    /// <summary>
    /// Service for generating Excel (.xlsx) workbooks for meal plans and shopping lists.
    /// </summary>
    public interface IExcelExportService
    {
        void ExportWeeklyPlan(string filePath, IEnumerable<WeeklyPlanItem> planItems);
        void ExportShoppingPlan(
            string filePath,
            IEnumerable<PlanEntry> planEntries,
            IReadOnlyDictionary<string, Product> products,
            IReadOnlyList<string> stores,
            IReadOnlyDictionary<string, string?>? selectedStores = null);
    }
}
