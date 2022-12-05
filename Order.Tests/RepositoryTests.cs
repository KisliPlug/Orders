using Bogus;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using Order.Tests.Moq;
using Orders.Common;
using Orders.Service.Controllers;
using Orders.Service.Entities;

namespace Order.Tests;

public class RepositoryTests
{
    private readonly MoqRepository<Orders.Service.Entities.Order> _orderRepository;
    private readonly MoqRepository<Orders.Service.Entities.Client> _clientRepository;
    private readonly IPublishEndpoint _publisherMock;

    public RepositoryTests()
    {
        _orderRepository = new(Helpers.UpdateOrder);
        _clientRepository = new(Helpers.UpdateClient);
        _publisherMock = new MoqPublisher(_clientRepository);
    }

    [Fact()]
    public async Task TestGetById()
    {
        //Arrange
        var ordersController = new OrdersController(_orderRepository, _publisherMock);
        var cl = (await _orderRepository.GetAllAsync()).FirstOrDefault();
        //Act
        var searched = (await ordersController.GetAsync(cl.Id)).Value;
        //Assert
        Assert.True(searched.Id == cl.Id);
    }

    [Fact()]
    public async Task AddOrderToClient()
    {
        //Arrange
        var ordersController = new OrdersController(_orderRepository, _publisherMock);
        var clientController = new ClientsController(_clientRepository, _publisherMock);
        var client = (await clientController.GetAsync()).GetElements().FirstOrDefault();
        var createOrderDto = new CreateOrderDto
                             {
                                 Description = "Test", Price = 500, ClientId = client.Id
                               , DueDate = DateTimeOffset.Now
                             };
        //Act

        await ordersController.PostAsync(createOrderDto);
        client = (await clientController.GetAsync(client.Id)).GetElement();
        //Assert
        Assert.True(client.Orders.Count == 1);
    }
}