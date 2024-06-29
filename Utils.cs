namespace TelegramBot;

public static class Utils
{
    public static string? SetFirstLetterToUpper(string? text)
    {
        string? result = String.Empty;
        for (int i = 0; i < text.Length; i += 1)
        {
            result += i == 0 ? text[i].ToString().ToUpper() : text[i];
        }

        Console.WriteLine(result);
        return result;
    }
    
    public static Dictionary<int, string> months = new Dictionary<int, string>
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

    public static Dictionary<string, int> AggregateExpensesInDetailToDictionary(Dictionary<string, int> acc,
        ExpenseItem expenseItem)
    {
        if (acc.Keys.Contains(expenseItem.Caption))
        {
            acc[expenseItem.Caption] += expenseItem.Count;
        }
        else
        {
            acc.Add(expenseItem.Caption, expenseItem.Count);
        }

        return acc;
    }
}