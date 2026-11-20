namespace FoodManager
{
    /// <summary>
    /// Represents an aggregated shopping plan item with quantities, pack calculation, and store notes.
    /// </summary>
    public class PlanEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = "g";

        /// <summary>
        /// Preserved original computed amount before rounding.
        /// </summary>
        public int OriginalQty { get; set; }

        /// <summary>
        /// Possibly rounded to nearest package size.
        /// </summary>
        public int TotalQty { get; set; }

        public int DirectQty { get; set; }
        public int RecipeQty { get; set; }

        /// <summary>
        /// Number of packs after rounding (if applicable).
        /// </summary>
        public int? Packs { get; set; }

        public string Note { get; set; } = string.Empty;
    }
}
