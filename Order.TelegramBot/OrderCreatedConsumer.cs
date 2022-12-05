using MassTransit;
using Orders.Contracts;

namespace Order.TelegramBot;

public class OrderCreatedConsumer : IConsumer<OrderContract.OrderCreated>
{
    public OrderCreatedConsumer()
    { }

    public async Task Consume(ConsumeContext<Orders.Contracts.OrderContract.OrderCreated> context)
    {
        //Some actions that will make it clear to the user that the order has been taken for processing
        await Task.Delay(500);
    }
}
