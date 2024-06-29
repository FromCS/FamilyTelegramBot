namespace TelegramBot;

public class Category
{
    public Category(string name, int count)
    {
        Name = name;
        Count = count;
    }
    
    public string Name { get; set; }
    public int Count { get; set; }
}