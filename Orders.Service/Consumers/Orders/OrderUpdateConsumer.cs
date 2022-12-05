using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers.Orders;

public class OrderUpdateConsumer : IConsumer<Contracts.OrderContract.OrderUpdate>
{
    private readonly IRepository<Order> _repository;

    public OrderUpdateConsumer(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderContract.OrderUpdate> context)
    {
        var message = context.Message;
        if (await _repository.GetAsync(message.Id) is not { } order)
        {
            order = new Order()
                    {
                        Id = message.Id
                      , ClientId = message.ClientId
                      , Description = message.Description
                      , Price = message.Price
                      , CreationDate =
                            DateTimeOffset.Now
                      , DueDate = message.DueDate
                    };
            await _repository.CreateAsync(order);
            return;
        }

        order.ClientId = message.ClientId;
        order.Description = message.Description;
        order.Price = message.Price;
        order.DueDate = message.DueDate;
        order.Description = message.Description;
        await _repository.UpdateAsync(order);
    }
}
