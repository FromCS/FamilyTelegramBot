using System.Data.SQLite;

namespace TelegramBot.Database;

public static class DatabaseFS
{
    private static string mainFolderPath = @"C:\Users\devid\Desktop\БД для бота\";

    private static string? CurrentDatabaseFilename { get; set; }

    private static void CreateNewFolder(string folderName)
    {
        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
    }

    public static void CreateNewDatabaseFile()
    {
        int currentMonth = DateTime.Now.Month;
        int currentYear = DateTime.Now.Year;
        string folderName = mainFolderPath + Utils.months[currentMonth] + " " + currentYear;
        CreateNewFolder(folderName);
        string databaseFilename = folderName + "\\" + Utils.months[currentMonth] + " " + currentYear + ".db";
        CurrentDatabaseFilename = databaseFilename;
        if (!File.Exists(databaseFilename))
        {
            //string connectionString = "Data Source=mydatabase.db;Version=3;";
            SQLiteConnection.CreateFile(databaseFilename);

            AddStandartCategoriesToDbFile();
            
        }
    }

    public static void AddStandartCategoriesToDbFile()
    {
        List<string> categories = new List<string>
        {
            "Квартира", "Сад", "БАД", "Одежда", "Бьюти", "Поборы", "Аптека", "Машина", "Анализы", "Прогулки",
            "Развивашки", "Доходы", "Продукты"
        };
        categories.ForEach(category =>
        {
            using (SQLiteConnection connection =
                   new SQLiteConnection($@"Data Source={GetCurrentDatabaseFilename()};"))
            {
                Console.WriteLine(GetCurrentDatabaseFilename());
                connection.Open();
                string sql = $"CREATE TABLE {category} (id INTEGER PRIMARY KEY, Count INTEGER, Caption TEXT, Datetime TEXT)";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
                
            }
        });
    }

    public static string? GetCurrentDatabaseFilename() => CurrentDatabaseFilename;
}