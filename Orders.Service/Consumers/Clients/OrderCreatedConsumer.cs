using MassTransit;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Consumers.Clients;

public class ClientOrderRemoveConsumer : IConsumer<Contracts.ClientContract.ClientOrderRemove>
{
    private readonly IRepository<Client> _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ClientOrderRemoveConsumer(IRepository<Client> repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<Contracts.ClientContract.ClientOrderRemove> context)
    {
        var message = context.Message;
        if (await _repository.GetAsync(message.Id) is not { } client)
        {
            return;
        }

        client.Orders.Remove(message.OrderId);
        await _publishEndpoint.Publish(new Contracts.ClientContract.ClientOrderRemoved(message.Id));
    }
}
