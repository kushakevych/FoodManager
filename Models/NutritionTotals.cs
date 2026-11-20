using System;

namespace FoodManager
{
    /// <summary>
    /// Calculated aggregate nutritional values and financial cost.
    /// </summary>
    public class NutritionTotals
    {
        public double Carbs { get; set; }
        public double Sugars { get; set; }
        public double Fats { get; set; }
        public double Saturated { get; set; }
        public double Protein { get; set; }
        public double Kcal { get; set; }
        public double Fiber { get; set; }
        public decimal Cost { get; set; }

        public void Round()
        {
            Carbs = Math.Round(Carbs, 2);
            Sugars = Math.Round(Sugars, 2);
            Fats = Math.Round(Fats, 2);
            Saturated = Math.Round(Saturated, 2);
            Protein = Math.Round(Protein, 2);
            Kcal = Math.Round(Kcal, 1);
            Fiber = Math.Round(Fiber, 2);
            Cost = Math.Round(Cost, 2, MidpointRounding.AwayFromZero);
        }

        public void Add(NutritionTotals other)
        {
            Carbs += other.Carbs;
            Sugars += other.Sugars;
            Fats += other.Fats;
            Saturated += other.Saturated;
            Protein += other.Protein;
            Kcal += other.Kcal;
            Fiber += other.Fiber;
            Cost += other.Cost;
        }

        public NutritionTotals Multiply(double factor)
        {
            return new NutritionTotals
            {
                Carbs = Carbs * factor,
                Sugars = Sugars * factor,
                Fats = Fats * factor,
                Saturated = Saturated * factor,
                Protein = Protein * factor,
                Kcal = Kcal * factor,
                Fiber = Fiber * factor,
                Cost = Math.Round(Cost * (decimal)factor, 2, MidpointRounding.AwayFromZero)
            };
        }
    }
}
