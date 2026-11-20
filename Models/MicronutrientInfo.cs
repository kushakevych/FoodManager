using System;
using System.Collections.Generic;

namespace FoodManager
{
    /// <summary>
    /// Information on vitamins and minerals/micronutrients per 100g (or per 1 piece).
    /// </summary>
    public class MicronutrientInfo
    {
        // Vitamins
        public double VitaminA_mcg { get; set; }      // Retinol equiv, mcg (RDA ~900 mcg)
        public double VitaminC_mg { get; set; }       // Ascorbic acid, mg (RDA ~90 mg)
        public double VitaminD_mcg { get; set; }      // Cholecalciferol, mcg (RDA ~15 mcg / 600 IU)
        public double VitaminE_mg { get; set; }       // Tocopherol, mg (RDA ~15 mg)
        public double VitaminB1_mg { get; set; }      // Thiamine, mg (RDA ~1.2 mg)
        public double VitaminB2_mg { get; set; }      // Riboflavin, mg (RDA ~1.3 mg)
        public double VitaminB6_mg { get; set; }      // Pyridoxine, mg (RDA ~1.7 mg)
        public double VitaminB9_mcg { get; set; }     // Folic acid, mcg (RDA ~400 mcg)
        public double VitaminB12_mcg { get; set; }    // Cobalamin, mcg (RDA ~2.4 mcg)

        // Minerals
        public double Potassium_mg { get; set; }      // Калій (K), mg (RDA ~3500 mg)
        public double Calcium_mg { get; set; }        // Кальцій (Ca), mg (RDA ~1000 mg)
        public double Magnesium_mg { get; set; }      // Магній (Mg), mg (RDA ~400 mg)
        public double Iron_mg { get; set; }           // Залізо (Fe), mg (RDA ~14 mg)
        public double Zinc_mg { get; set; }           // Цинк (Zn), mg (RDA ~11 mg)
        public double Sodium_mg { get; set; }         // Натрій (Na), mg (RDA ~2000 mg)
        public double Phosphorus_mg { get; set; }     // Фосфор (P), mg (RDA ~700 mg)
        public double Iodine_mcg { get; set; }        // Йод (I), mcg (RDA ~150 mcg)

        public MicronutrientInfo Clone() => (MicronutrientInfo)MemberwiseClone();

        public void AddScaled(MicronutrientInfo other, double factor)
        {
            VitaminA_mcg += other.VitaminA_mcg * factor;
            VitaminC_mg += other.VitaminC_mg * factor;
            VitaminD_mcg += other.VitaminD_mcg * factor;
            VitaminE_mg += other.VitaminE_mg * factor;
            VitaminB1_mg += other.VitaminB1_mg * factor;
            VitaminB2_mg += other.VitaminB2_mg * factor;
            VitaminB6_mg += other.VitaminB6_mg * factor;
            VitaminB9_mcg += other.VitaminB9_mcg * factor;
            VitaminB12_mcg += other.VitaminB12_mcg * factor;

            Potassium_mg += other.Potassium_mg * factor;
            Calcium_mg += other.Calcium_mg * factor;
            Magnesium_mg += other.Magnesium_mg * factor;
            Iron_mg += other.Iron_mg * factor;
            Zinc_mg += other.Zinc_mg * factor;
            Sodium_mg += other.Sodium_mg * factor;
            Phosphorus_mg += other.Phosphorus_mg * factor;
            Iodine_mcg += other.Iodine_mcg * factor;
        }

        public void Round()
        {
            VitaminA_mcg = Math.Round(VitaminA_mcg, 1);
            VitaminC_mg = Math.Round(VitaminC_mg, 1);
            VitaminD_mcg = Math.Round(VitaminD_mcg, 2);
            VitaminE_mg = Math.Round(VitaminE_mg, 2);
            VitaminB1_mg = Math.Round(VitaminB1_mg, 2);
            VitaminB2_mg = Math.Round(VitaminB2_mg, 2);
            VitaminB6_mg = Math.Round(VitaminB6_mg, 2);
            VitaminB9_mcg = Math.Round(VitaminB9_mcg, 1);
            VitaminB12_mcg = Math.Round(VitaminB12_mcg, 2);

            Potassium_mg = Math.Round(Potassium_mg, 1);
            Calcium_mg = Math.Round(Calcium_mg, 1);
            Magnesium_mg = Math.Round(Magnesium_mg, 1);
            Iron_mg = Math.Round(Iron_mg, 2);
            Zinc_mg = Math.Round(Zinc_mg, 2);
            Sodium_mg = Math.Round(Sodium_mg, 1);
            Phosphorus_mg = Math.Round(Phosphorus_mg, 1);
            Iodine_mcg = Math.Round(Iodine_mcg, 1);
        }
    }

    /// <summary>
    /// Recommended Daily Allowance (RDA) reference metadata for vitamins and minerals.
    /// </summary>
    public record NutrientRdaItem(string Key, string Name, string Unit, double DailyRda, string Category);

    public static class MicronutrientStandards
    {
        public static readonly IReadOnlyList<NutrientRdaItem> Standards = new[]
        {
            // Vitamins
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminA_mcg), "Вітамін A", "мкг", 900, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminC_mg), "Вітамін C", "мг", 90, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminD_mcg), "Вітамін D", "мкг", 15, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminE_mg), "Вітамін E", "мг", 15, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminB1_mg), "Вітамін B1 (Тіамін)", "мг", 1.2, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminB2_mg), "Вітамін B2 (Рибофлавін)", "мг", 1.3, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminB6_mg), "Вітамін B6", "мг", 1.7, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminB9_mcg), "Вітамін B9 (Фолієва к-та)", "мкг", 400, "Вітаміни"),
            new NutrientRdaItem(nameof(MicronutrientInfo.VitaminB12_mcg), "Вітамін B12", "мкг", 2.4, "Вітаміни"),

            // Minerals
            new NutrientRdaItem(nameof(MicronutrientInfo.Potassium_mg), "Калій (K)", "мг", 3500, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Calcium_mg), "Кальцій (Ca)", "мг", 1000, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Magnesium_mg), "Магній (Mg)", "мг", 400, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Iron_mg), "Залізо (Fe)", "мг", 14, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Zinc_mg), "Цинк (Zn)", "мг", 11, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Sodium_mg), "Натрій (Na)", "мг", 2000, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Phosphorus_mg), "Фосфор (P)", "мг", 700, "Мінерали"),
            new NutrientRdaItem(nameof(MicronutrientInfo.Iodine_mcg), "Йод (I)", "мкг", 150, "Мінерали")
        };

        public static double GetValue(MicronutrientInfo info, string key) => key switch
        {
            nameof(MicronutrientInfo.VitaminA_mcg) => info.VitaminA_mcg,
            nameof(MicronutrientInfo.VitaminC_mg) => info.VitaminC_mg,
            nameof(MicronutrientInfo.VitaminD_mcg) => info.VitaminD_mcg,
            nameof(MicronutrientInfo.VitaminE_mg) => info.VitaminE_mg,
            nameof(MicronutrientInfo.VitaminB1_mg) => info.VitaminB1_mg,
            nameof(MicronutrientInfo.VitaminB2_mg) => info.VitaminB2_mg,
            nameof(MicronutrientInfo.VitaminB6_mg) => info.VitaminB6_mg,
            nameof(MicronutrientInfo.VitaminB9_mcg) => info.VitaminB9_mcg,
            nameof(MicronutrientInfo.VitaminB12_mcg) => info.VitaminB12_mcg,

            nameof(MicronutrientInfo.Potassium_mg) => info.Potassium_mg,
            nameof(MicronutrientInfo.Calcium_mg) => info.Calcium_mg,
            nameof(MicronutrientInfo.Magnesium_mg) => info.Magnesium_mg,
            nameof(MicronutrientInfo.Iron_mg) => info.Iron_mg,
            nameof(MicronutrientInfo.Zinc_mg) => info.Zinc_mg,
            nameof(MicronutrientInfo.Sodium_mg) => info.Sodium_mg,
            nameof(MicronutrientInfo.Phosphorus_mg) => info.Phosphorus_mg,
            nameof(MicronutrientInfo.Iodine_mcg) => info.Iodine_mcg,
            _ => 0
        };

        public static void SetValue(MicronutrientInfo info, string key, double value)
        {
            switch (key)
            {
                case nameof(MicronutrientInfo.VitaminA_mcg): info.VitaminA_mcg = value; break;
                case nameof(MicronutrientInfo.VitaminC_mg): info.VitaminC_mg = value; break;
                case nameof(MicronutrientInfo.VitaminD_mcg): info.VitaminD_mcg = value; break;
                case nameof(MicronutrientInfo.VitaminE_mg): info.VitaminE_mg = value; break;
                case nameof(MicronutrientInfo.VitaminB1_mg): info.VitaminB1_mg = value; break;
                case nameof(MicronutrientInfo.VitaminB2_mg): info.VitaminB2_mg = value; break;
                case nameof(MicronutrientInfo.VitaminB6_mg): info.VitaminB6_mg = value; break;
                case nameof(MicronutrientInfo.VitaminB9_mcg): info.VitaminB9_mcg = value; break;
                case nameof(MicronutrientInfo.VitaminB12_mcg): info.VitaminB12_mcg = value; break;

                case nameof(MicronutrientInfo.Potassium_mg): info.Potassium_mg = value; break;
                case nameof(MicronutrientInfo.Calcium_mg): info.Calcium_mg = value; break;
                case nameof(MicronutrientInfo.Magnesium_mg): info.Magnesium_mg = value; break;
                case nameof(MicronutrientInfo.Iron_mg): info.Iron_mg = value; break;
                case nameof(MicronutrientInfo.Zinc_mg): info.Zinc_mg = value; break;
                case nameof(MicronutrientInfo.Sodium_mg): info.Sodium_mg = value; break;
                case nameof(MicronutrientInfo.Phosphorus_mg): info.Phosphorus_mg = value; break;
                case nameof(MicronutrientInfo.Iodine_mcg): info.Iodine_mcg = value; break;
            }
        }
    }
}
