namespace TelegramBot;

public class ExpenseItem
{
    public ExpenseItem(string caption, int count)
    {
        Caption = caption;
        Count = count;
    }
    
    public string Caption { get; set; }
    public int Count { get; set; }
}