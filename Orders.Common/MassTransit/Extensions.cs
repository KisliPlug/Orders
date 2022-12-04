using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Common.Settings;

namespace Orders.Common.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
                                {
                                    configurator.AddConsumers(Assembly.GetEntryAssembly());
                                    configurator.UsingRabbitMq((context, configurator) =>
                                                               {
                                                                   var configuration = context.GetService<IConfiguration>();
                                                                   var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
                                                                                                      .Get<ServiceSettings>();
                                                                   var rabbitMqSettings = configuration.GetSection(nameof(RabbitMQSettings))
                                                                                                       .Get<RabbitMQSettings>();
                                                                   configurator.Host(rabbitMqSettings.Host);
                                                                   configurator.ConfigureEndpoints(context
                                                                                                 , new KebabCaseEndpointNameFormatter(serviceSettings
                                                                                                                                         .ServiceName
                                                                                                                                    , false));
                                                               });
                                });
        return services;
    }
}