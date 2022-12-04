using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers;

public class OrderCreatedConsumer : IConsumer<Contracts.OrderContract.OrderCreated>
{
    private readonly IRepository<Order> _repository;

    public OrderCreatedConsumer(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderContract.OrderCreated> context)
    {
        var message = context.Message;
        if (await _repository.GetAsync(message.Id) is { } order)
        {
            return;
        }

        order = new Order()
               {
                   Id = message.Id, ClientId = message.ClientId, Description = message.Description, Price = message.Price, CreationDate =
                       DateTimeOffset.Now
                 , DueDate = message.DueDate
               };
        await _repository.CreateAsync(order);
    }
}