using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Common;

namespace Order.Tests;

public class RepositoryTests
{
    private readonly MoqRepository<Orders.Service.Entities.Order> _orderRepository;
    private readonly MoqRepository<Orders.Service.Entities.Client> _clientRepository;

    public RepositoryTests()
    {
        _orderRepository = new  ();
        _clientRepository = new  ();
    }
    [Fact()]
    public void TestRepository_ShouldBe()
    {
        
    }
}