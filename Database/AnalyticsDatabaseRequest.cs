using System.Data.SqlClient;
using System.Data.SQLite;

namespace TelegramBot.Database;

public static class AnalyticsDatabaseRequest
{
    private static string _mainFolderPath = @"C:\Users\devid\Desktop\БД для бота\";

    public static string GetFullAnalytics()
    {
        string[] dbFiles = Directory.GetFiles(_mainFolderPath, "*.db", SearchOption.AllDirectories);
        Dictionary<string, string> analytics = new Dictionary<string, string>();
        string resultAnalytics = String.Empty;
        foreach (var dbFile in dbFiles)
        {
            string filename = dbFile.Split("\\").Last().Split(".")[0];
            
            List<string> categoriesList = GettingDatabaseRequests.GetNamesOfTablesFor(dbFile).Split("\n").ToList();
            categoriesList.ForEach(category =>
            {
                if (analytics.ContainsKey(category))
                {
                    analytics[category] += "\n" + filename + ": " + GetSumCountOfCategoryFromDb(dbFile, category);
                }
                else
                {
                    analytics[category] = filename + ": " + GetSumCountOfCategoryFromDb(dbFile, category);
                }
            });
            
        }

        foreach (var categoryAnalytics in analytics)
        {
            resultAnalytics += "\n" + "**" + categoryAnalytics.Key + "**" +"\n"  + categoryAnalytics.Value + "\n------------------------";
        }

        return resultAnalytics;
    }

    /*
    public static string GetHistoryInfo(string?[] messageParts)
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection($@"Data Source={messageParts[0]},Version=3;"))
            {
                connection.Open();
                using var command = new SQLiteCommand($"SELECT SUM(Count) FROM {messageParts[1]}");
                
            }
        }
        catch (Exception e)
        {
            return $"В \"{messageParts[0]}\" нет указанной категории";

        }
        
        
    }
    */
    
    private static string? GetSumCountOfCategoryFromDb(string dbFilename, string category)
    {
        using SQLiteConnection connection =
            new SQLiteConnection($@"Data Source={dbFilename};Version=3;");
        connection.Open();
        using var command = new SQLiteCommand($"SELECT SUM(Count) FROM {category}", connection);
        var result = command.ExecuteScalar();
        return result.ToString();
    }
}