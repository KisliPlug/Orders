using MassTransit;
using Orders.Contracts;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Order.TelegramBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IPublishEndpoint _publishEndpoint;

    public Worker(ILogger<Worker> logger, ITelegramBotClient bot, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _bot = bot;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
                                  {
                                      AllowedUpdates = { }, // receive all update types
                                  };
            _bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
        }
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Некоторые действия для получения данных
        var someData = await GetSomeDataFromMessage(update);
        if (someData.Price > 0)
        {
            var message = update.Message;
            await _publishEndpoint.Publish(someData, cancellationToken);
            await botClient.SendTextMessageAsync(message.Chat, "Заказ принят!", cancellationToken: cancellationToken);
        }
    }

    private async Task<OrderContract.OrderCreation> GetSomeDataFromMessage(Update update)
    {
        await Task.Delay(500);
        var dueDat = new DateTimeOffset(DateTime.Today, TimeSpan.FromDays(60));
        return new OrderContract.OrderCreation(Guid.NewGuid(), "Some new order", 500, dueDat);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }
}