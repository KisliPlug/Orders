using Order.TelegramBot;
using Orders.Common.MassTransit;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services =>
                                    {
                                        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                                        services
                                           .AddScoped<ITelegramBotClient>(pr =>
                                                                          {
                                                                              var telegrammSettings = configuration.GetSection(nameof
                                                                                                                                   (TelegramSettings))
                                                                                                                   .Get<TelegramSettings>();
                                                                              return new TelegramBotClient(telegrammSettings.Token);
                                                                          })
                                           .AddMassTransitWithRabbitMq()
                                           .AddHostedService<Worker>();
                                    })
                 .Build();
await host.RunAsync();