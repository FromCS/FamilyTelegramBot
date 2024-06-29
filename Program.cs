// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Data.SQLite;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot;
using TelegramBot.Database;

string currentMonth = Utils.months[DateTime.Now.Month];

string GetInfo() => $@"/info — узнать возможности бота;
/categories — узнать все доступные категории текущего месяца;
/stat — общая статистика по расходам в каждой категории текущего месяца;

<НАЗВАНИЕ КАТЕГОРИИ> <СУММА> <ОПИСАНИЕ В ОДНО СЛОВО> — добавить расход в категорию;

Покажи <НАЗВАНИЕ КАТЕГОРИИ> — узнать расходы в конкретной категории текущего месяца;

Покажи <НАЗВАНИЕ КАТЕГОРИИ> подробно — узнать расходы в конкретной категории текущего месяца с подробным описание; 

Новая категория <НАЗВАНИЕ> — добавить новую категорию в текущий месяц;
";

List<long> availableChatIds = new List<long> { 1993266327, 892176820 };

Dictionary<string, Delegate> actionsByCommands = new Dictionary<string, Delegate>()
{
    {"/info", GetInfo}, // пишем паттерны запросов
    {"/categories", GettingDatabaseRequests.GetNamesOfTables},
    {"/stat", GettingDatabaseRequests.GetStat},
    {"/analytics", AnalyticsDatabaseRequest.GetFullAnalytics}
};


var botClient = new TelegramBotClient("6345428504:AAGbGta3S3Y6QDO8xOc5Yki_HeVFhuBk0CA");
DatabaseFS.CreateNewDatabaseFile();
using CancellationTokenSource cts = new ();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new ()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();


Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();


// далее написано всё, для ответа на сообщения пользователя
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;
    var chatId = message.Chat.Id;

    
    var replyKeyboardMarkup = new ReplyKeyboardMarkup(new List<IEnumerable<KeyboardButton>>()
    {
        new[]
        {
            new KeyboardButton("Покажи продукты"),
            new KeyboardButton("Покажи машина")
        },
        new[]
        {
            new KeyboardButton("Покажи БАД"),
            new KeyboardButton("Покажи врачи")
        }
    });

    
    if (Utils.months[DateTime.Now.Month] != currentMonth) // FIX. Поменять на условие, которое проверяет наличие файла БД текущего месяца в папке сервера
    {
        DatabaseFS.CreateNewDatabaseFile();
        currentMonth = Utils.months[DateTime.Now.Month];
    }
    if (!availableChatIds.Contains(chatId))
    {
        Message sendMessageToUnknownChat = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Вы не являетесь членом семьи",
            cancellationToken: cancellationToken,
            replyMarkup: replyKeyboardMarkup);
    }
    if (actionsByCommands.Keys.Contains(messageText))
    {
        Message sendMessageOnStandartRequest = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: (string)actionsByCommands[messageText].DynamicInvoke(),
            cancellationToken: cancellationToken,
            replyMarkup: replyKeyboardMarkup);
    }
    else
    {
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: MessagesHandler.HandleMessage(messageText),
            cancellationToken: cancellationToken,
            replyMarkup: replyKeyboardMarkup);
    }

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    // Echo received message text
    
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    
    
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

