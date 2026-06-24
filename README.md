Food Manager - WinForms + SQLite (sample project)
================================================
What this contains
- A simple WinForms app (C#, .NET 6) that manages products, recipes, inventory, and generates a simple shopping list.
- SQLite database (food.db) auto-created at first run with sample data.
- Simple UI built with DataGridView and dialog forms for adding products and recipes.

How to open and run
1. Install .NET 6 SDK and Visual Studio 2022 (or Visual Studio 2022 Community with .NET desktop workload).
2. Open the folder in Visual Studio or via command-line.
3. Restore NuGet packages (Microsoft.Data.Sqlite).
4. Build and run.

Notes for students / maintainers
- Keep the code simple. DataAccess.cs contains SQL and basic CRUD operations.
- Prices and barcodes are stored as JSON strings to allow flexible dictionaries/arrays.
- Nutrition data is stored as JSON in the 'nutrition_json' column.
- Shopping list generation is simplistic: it aggregates ingredients from all recipes and subtracts inventory (by grams).
- Purchasing unit granularity: product.WeightPerUnitG is provided and can be used to round purchase quantities to integer units in future improvements.

Files of interest
- Program.cs, MainForm.cs (+ Designer), DataAccess.cs, Models.cs
- AddProductForm.*, AddRecipeForm.*: simple dialog forms
- food.db: initial database (created on first run if not present)

Limitations & suggested improvements
- No input validation beyond basic parsing.
- No transactions for complex operations.
- UI needs polish (labels, icons, localized texts).
- Shopping list currently considers ALL recipes; later allow selecting which recipes/meals to plan for.
- Price optimization (choose cheapest store) can be added by computing cost per gram using price dictionaries.

DESIGN.md contains a textual UI design and guidance.
