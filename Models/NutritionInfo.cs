namespace FoodManager
{
    /// <summary>
    /// Nutritional values for a product.
    /// When product.InventoryUnit == "g", values represent nutrients per 100g.
    /// When product.InventoryUnit == "pcs", values represent nutrients per 1 piece.
    /// </summary>
    public class NutritionInfo
    {
        public double Carbs_100g { get; set; }
        public double Sugars_100g { get; set; }
        public double Fats_100g { get; set; }
        public double Saturated_100g { get; set; }
        public double Protein_100g { get; set; }
        public double Kcal_100g { get; set; }
        public double Fiber_100g { get; set; }

        public NutritionInfo Clone() => (NutritionInfo)MemberwiseClone();
    }
}
