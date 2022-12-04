using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers.Orders;

public class OrderCreationConsumer : IConsumer<Contracts.OrderContract.OrderCreation>
{
    private readonly IRepository<Order> _repository;

    public OrderCreationConsumer(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderContract.OrderCreation> context)
    {
        var message = context.Message;
        var order = new Order()
                    {
                        Id = Guid.NewGuid(), ClientId = message.ClientId, Description = message.Description, Price = message.Price, CreationDate =
                            DateTimeOffset.Now
                      , DueDate = message.DueDate
                    };
        await _repository.CreateAsync(order);
    }
}