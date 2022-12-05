using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers.Orders;

public class OrderDeleteConsumer : IConsumer<Contracts.OrderContract.OrderDelete>
{
    private readonly IRepository<Order> _repository;

    public OrderDeleteConsumer(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<Contracts.OrderContract.OrderDelete> context)
    {
        var message = context.Message;
        if (await _repository.GetAsync(message.Id) is { } item)
        {
            await _repository.RemoveAsync(item.Id);
        }
    }
}
