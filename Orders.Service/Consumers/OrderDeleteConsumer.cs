using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers;

public class OrderDeleteConsumer : IConsumer<Contracts.OrderContract.OrderDeleted>
{
    private readonly IRepository<Order> _repository;

    public OrderDeleteConsumer(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderContract.OrderDeleted> context)
    {
        var message = context.Message;
        if (await _repository.GetAsync(message.Id) is { } item)
        {
            await _repository.RemoveAsync(item.Id);
        }
    }
}