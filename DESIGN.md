UI Design (textual)
-------------------
Goal: Simple, clear layout so a student can maintain it.

Main window (MainForm)
- TabControl with three tabs:
  1. 'Продукти' (Products)
     - DataGridView columns: ID, Найменування, Вага г/шт, Ціни, Штрих-коди
     - Buttons: Додати, Видалити
     - Add opens AddProductForm
  2. 'Страви' (Recipes)
     - DataGridView columns: ID, Найменування, Статус, Інгредієнти
     - Buttons: Додати, Видалити
     - Add opens AddRecipeForm
  3. 'Облік' (Inventory)
     - DataGridView columns: ID, Найменування, Кількість (г)
     - Buttons: Додати в облік, Списати, Згенерувати закупки
     - Add/Split use InputBox to enter grams (integer)
     - Generate shopping aggregates ingredients from all recipes and subtracts current inventory.

Dialogs
- AddProductForm
  - TextBoxes for name, weight, nutrients (carbs, sugars, fats, sat, protein, kcal)
  - Textbox for prices (format Market:12.5;Other:11)
  - Textbox for barcodes (comma separated)
  - OK/Cancel
- AddRecipeForm
  - Name, Status (combo), Ingredients (format Product:grams;Other:50)
  - OK/Cancel

Data model decisions
- Prices and barcodes stored as JSON strings to avoid complex relational schema for this MVP.
- Inventory is stored in grams per entry to allow adding and subtracting as events.
- Recipe ingredients reference products by name for simplicity (could be changed to product_id in future).

Accessibility & UX
- Use full-row selection on DataGridView.
- Place action buttons near the grid they operate on.
- Keep labels and hints for expected input formats.

Future UI improvements
- Inline editing for grids.
- Better forms with validation and friendly controls (numeric up/down, currency masks).
- Calendar planning and per-day shopping list.
