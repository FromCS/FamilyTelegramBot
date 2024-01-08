namespace TelegramBot;

public static class DatabaseFS
{
    private static string mainFolderPath = @"C:\Users\devid\Desktop\БД для бота\";

    private static Dictionary<int, string> months = new Dictionary<int, string>
    {
        {1, "Январь"},
        {2, "Февраль"},
        {3, "Март"},
        {4, "Апрель"},
        {5, "Май"},
        {6, "Июнь"},
        {7, "Июль"},
        {8, "Август"},
        {9, "Сентябрь"},
        {10, "Октябрь"},
        {11, "Ноябрь"},
        {12, "Декабрь"},
    };

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
        string folderName = mainFolderPath + months[currentMonth] + " " + currentYear;
        CreateNewFolder(folderName);
        string databaseFilename = folderName + "/" + months[currentMonth] + " " + currentYear + ".db";
        CurrentDatabaseFilename = databaseFilename;
        if (!File.Exists(databaseFilename))
        {
            File.Create(databaseFilename);
        }
    }

    public static string? GetCurrentDatabaseFilename() => CurrentDatabaseFilename;
}