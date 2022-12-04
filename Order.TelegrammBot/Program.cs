using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Order.TelegrammBot
{

    class Program
    {
        static ITelegramBotClient bot = new TelegramBotClient("TOKEN");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
          
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message?.Text?.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!", cancellationToken: cancellationToken);
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Привет-привет!!", cancellationToken: cancellationToken);
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
                                  {
                                      AllowedUpdates = { }, // receive all update types
                                  };
            bot.StartReceiving(
                               HandleUpdateAsync,
                               HandleErrorAsync,
                               receiverOptions,
                               cancellationToken
                              );
            Console.ReadLine();
        }
    }
}