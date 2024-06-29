using System.Data.SQLite;

namespace TelegramBot.Database;

public static class GettingDatabaseRequests
{
    public static string GetNamesOfTables()
    {
        List<string> currentCategoriesList = new List<string>();
        using (SQLiteConnection connection =
                     new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;"))
        {
            connection.Open();
            string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name!='sqlite_sequence' ORDER BY name;";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        currentCategoriesList.Add(tableName);
                    }
                }
            }
            //currentCategoriesList.Remove("sqlite_sequence");
        }

        return String.Join("\n", currentCategoriesList);;
    }
    
    public static string GetNamesOfTablesFor(string databaseFilename)
    {
        List<string> currentCategoriesList = new List<string>();
        using (SQLiteConnection connection =
               new SQLiteConnection($@"Data Source={databaseFilename};Version=3;"))
        {
            connection.Open();
            string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name!='sqlite_sequence' ORDER BY name;";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        currentCategoriesList.Add(tableName);
                    }
                }
            }
            //currentCategoriesList.Remove("sqlite_sequence");
        }

        return String.Join("\n", currentCategoriesList);;
    }

    public static string? GetSumCountOfCategoryFromDb(string category)
    {
        using SQLiteConnection connection =
            new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;");
        connection.Open();
        using var command = new SQLiteCommand($"SELECT SUM(Count) FROM {category}", connection);
        var result = command.ExecuteScalar();
        return result.ToString();
    }

    public static string GetStat()
    {
        List<Category> categoriesInfo = new List<Category>();
        List<string> categories = GetNamesOfTables().Split("\n").ToList();
        using (SQLiteConnection connection =
               new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;"))
        {
            connection.Open();
            categories.ForEach(category =>
            {
                using (var command = new SQLiteCommand($"SELECT SUM(Count) FROM {category}", connection))
                {
                    var count = command.ExecuteScalar().ToString() == String.Empty ? 0 : Convert.ToInt32(command.ExecuteScalar());
                    categoriesInfo.Add(new Category(category, Convert.ToInt32(count)));
                }
            });
        }

        var stats = categoriesInfo.Where(categoryInfo => categoryInfo.Count != 0).ToList();
        stats.Sort((x, y) => y.Count.CompareTo(x.Count));
        return string.Join("\n", stats.Select(stat => $"{stat.Name} - {stat.Count}"));
    }

    private static string GetExpensesOfCategoryInDetail(string category)
    {
        List<string> expenses = new List<string>();
        using (SQLiteConnection connection = new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;"))
        {
            connection.Open();

            string query = $"SELECT * FROM {category}";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string currentExpenseItem = String.Empty;
                        for (int i = 1; i < 3; i++)
                        {
                            currentExpenseItem += " " + reader[i];
                        }
                        expenses.Add(currentExpenseItem.Trim());
                    }
                }
            }
        }

        Dictionary<string, int> aggregateExpenses = expenses
            .Select(expense =>
            {
                string[] expenseParts = expense.Split(" ");
                string caption = expenseParts.Length <= 1 ? "НЕИЗВЕСТНО" : expenseParts[1];
                return new ExpenseItem(caption, Convert.ToInt32(expenseParts[0]));
            }).ToList()
            .Aggregate(new Dictionary<string, int>(), Utils.AggregateExpensesInDetailToDictionary);

        List<ExpenseItem> result = new List<ExpenseItem>();
        foreach (var pair in aggregateExpenses)
        {
            result.Add(new ExpenseItem(pair.Key, pair.Value));
        }
        result.Sort((x, y) => y.Count.CompareTo(x.Count));
        
        return $"{string.Join("\n", result.Select(expense => $"{Utils.SetFirstLetterToUpper(expense.Caption)} - {expense.Count}"))}" +
               $"\n" + "----------------------------------------" + "\n" +
               $"Общий расход по категории: {GetSumCountOfCategoryFromDb(category)}";

    }

    public static string ShowFullCount(string message)
    {
        string[] messageParts = message.Split(" ");
        if (messageParts.Length <= 1)
        {
            return "Напишите категорию. Например: \"Покажи продукты\"";
        }
        string? category = Utils.SetFirstLetterToUpper(messageParts[1]);
        if (!GetNamesOfTables().Contains(category!))
        {
            return "Нет выбранной категории.\nНапишите /categories для просмотра доступных категорий.";
        }
        return message.Contains("подробно") 
            ? GetExpensesOfCategoryInDetail(category!)
            : $"Сумма расходов в категории {category} — {GetSumCountOfCategoryFromDb(category!)}";
        
    }
}