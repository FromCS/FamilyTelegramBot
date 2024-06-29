using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace TelegramBot.Database;

public static class ChangingDatabaseRequests
{
    public static string CreateNewCategory(string newCategoryName)
    {
        using (SQLiteConnection connection =
               new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;"))
        {
            connection.Open();
            string sql = $"CREATE TABLE {Utils.SetFirstLetterToUpper(newCategoryName)} (id INTEGER PRIMARY KEY, Count INTEGER, Caption TEXT, Datetime TEXT)";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQueryAsync();
            }
        }

        return $"Добавлена категория \"{Utils.SetFirstLetterToUpper(newCategoryName)}\"";
    }

    private static void AddDataInDB(string? category, int count, string? caption = null, string? date = null)
    {
        date ??= DateTime.Now.ToShortDateString();
        date += date.EndsWith($".{DateTime.Now.Year}") ? String.Empty : $".{DateTime.Now.Year}";
        using (SQLiteConnection connection =
               new SQLiteConnection($@"Data Source={DatabaseFS.GetCurrentDatabaseFilename()};Version=3;"))
        {
            connection.Open();
            using (var command = new SQLiteCommand($"INSERT INTO {category} (Count, Caption, Datetime) VALUES (@Сумма, @Описание, @Дата)", connection))
            {
                command.Parameters.AddWithValue("@Сумма", count);
                command.Parameters.AddWithValue("@Описание", caption);
                command.Parameters.AddWithValue("@Дата", date);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public static void AddExpenseToDb(string?[] messageParts)
    {
        string? category = Utils.SetFirstLetterToUpper(messageParts[0]);
        int count = Convert.ToInt32(messageParts[1]);
        if (Regex.IsMatch(string.Join(" ", messageParts), @"\d+\.\d+"))
        {
            string captionPattern = @"\d+\s(.+?)\s\d+\.\d+";
            Match match = Regex.Match(string.Join(" ", messageParts), captionPattern);
            string? caption = match.Success ? match.Groups[1].Value : null;
            Console.WriteLine(caption);
        }
        
        switch (messageParts.Length)
        {
            case 2:
                AddDataInDB(category, count);
                break;
            case 3:
            {
                string? caption = messageParts[2];
                AddDataInDB(category, count, caption);
                break;
            }
            case 4:
            {
                string? caption = messageParts[2];
                string? date = messageParts[3];
                AddDataInDB(category, count, caption, date);
                break;
            }
        }
    }
}