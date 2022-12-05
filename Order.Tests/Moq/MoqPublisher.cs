using MassTransit;
using Orders.Common;
using Orders.Contracts;
using Orders.Service.Entities;

namespace Order.Tests.Moq;

public class MoqPublisher : IPublishEndpoint
{
    private readonly IRepository<Client> _clientRepo;

    public MoqPublisher(IRepository<Client> repository)
    {
        _clientRepo = repository;
    }

    public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
    {
        throw new NotImplementedException();
    }

    public async Task Publish<T>(T message, CancellationToken cancellationToken = new CancellationToken()) where T : class
    {
        await AddOrderToClient(message);
    }

    public async Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        where T : class
    {
        await AddOrderToClient(message);
    }

    public async Task Publish<T>(T message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        where T : class
    {
        await AddOrderToClient(message);
    }

    public Task Publish(object message, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public async Task Publish(object message, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
    {
        await AddOrderToClient(message);
    }

    public async Task Publish(object message, Type messageType, CancellationToken cancellationToken = new CancellationToken())
    {
        await AddOrderToClient(message);
    }

    public async Task Publish(object message
                            , Type messageType
                            , IPipe<PublishContext> publishPipe
                            , CancellationToken cancellationToken = new CancellationToken())
    {
        await AddOrderToClient(message);
    }

    private async Task AddOrderToClient(object message)
    {
        if (message is Orders.Contracts.ClientContract.ClientUpdated)
        { } else if (message is OrderContract.OrderCreated orderCreated)
        {
            if (await _clientRepo.GetAsync(orderCreated.ClientId) is not { } client)
            {
                return;
            }

            if (client.Orders is not { })
            {
                client.Orders = new List<Guid>();
            }

            client.Orders.Add(orderCreated.Id);
            await _clientRepo.UpdateAsync(client);
        }
    }

    public async Task Publish<T>(object message, CancellationToken cancellationToken = new CancellationToken()) where T : class
    {
        if (message is Orders.Contracts.ClientContract.ClientUpdated)
        { } else if (message is OrderContract.OrderCreated orderCreated)
        {
            if (await _clientRepo.GetAsync(orderCreated.ClientId) is not { } client)
            {
                return;
            }

            client.Orders.Add(orderCreated.Id);
            await _clientRepo.UpdateAsync(client);
        }
    }

    public Task Publish<T>(object values, IPipe<PublishContext<T>> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        where T : class
    {
        throw new NotImplementedException();
    }

    public Task Publish<T>(object values, IPipe<PublishContext> publishPipe, CancellationToken cancellationToken = new CancellationToken())
        where T : class
    {
        throw new NotImplementedException();
    }
}