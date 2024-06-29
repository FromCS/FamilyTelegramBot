using System.Data.SQLite;
using System.Text.RegularExpressions;
using TelegramBot.Database;

namespace TelegramBot;

public static class MessagesHandler
{
    public static string HandleMessage(string message)
    {
        string?[] messageParts = message.Split(" ");
        if (IsAddNewExpense(messageParts))
        {
            string? category = messageParts[0];
            int count = Convert.ToInt32(messageParts[1]);
            ChangingDatabaseRequests.AddExpenseToDb(messageParts);
            return $"Добавил {count} рублей в категорию \"{category}\"";
        }

        if (message.StartsWith("Покажи"))
        {
            return GettingDatabaseRequests.ShowFullCount(message);
        }

        if (IsRequestForNewCategory(message))
        {
            return ChangingDatabaseRequests.CreateNewCategory(message.Split("Новая категория")[1].Trim());
        }

        /*
        if (IsHistoryRequest(message))
        {
            return AnalyticsDatabaseRequest.GetHistoryInfo(messageParts.Skip(1).ToArray());
        }
        */
            

        return "Не понимаю.\n\\info - узнать возможности бота";
    }

    private static bool IsAddNewExpense(string?[] messageParts) =>
        GettingDatabaseRequests.GetNamesOfTables().Contains(messageParts[0] ?? string.Empty) && messageParts.Length > 1;

    private static bool IsRequestForNewCategory(string message) => message.StartsWith("Новая категория") &&
                                                                   !GettingDatabaseRequests.GetNamesOfTables()
                                                                       .Contains(message.Split("Новая категория")[1]
                                                                           .Trim());

    private static bool IsHistoryRequest(string message) => message.StartsWith("История");

}