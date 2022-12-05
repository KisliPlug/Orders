using Orders.Service.Entities;

namespace Order.Tests;

public class DtoTransferTests
{
    private List<OrderDto> _dtos;
    private List<Orders.Service.Entities.Order> _entities;
    private List<CreateOrderDto> _createDtos;

    public DtoTransferTests()
    {
        _dtos = new List<OrderDto>
                {
                    new()
                    {
                        Description = "Description"
                      , Id = Guid.NewGuid()
                      , Price = 500
                      , ClientId = Guid.NewGuid()
                      , CreationDate = DateTimeOffset.Now
                      , DueDate = DateTimeOffset.Now
                       ,
                    }
                  , new()
                    {
                        Description = "Description1"
                      , Id = Guid.NewGuid()
                      , Price = 501
                      , ClientId = Guid.NewGuid()
                      , CreationDate = DateTimeOffset.Now
                      , DueDate = DateTimeOffset.Now
                       ,
                    }
                }
            ;
        _entities = new List<Orders.Service.Entities.Order>
                    {
                        new()
                        {
                            Description = "Description"
                          , Id = Guid.NewGuid()
                          , Price = 500
                          , ClientId = Guid.NewGuid()
                          , CreationDate = DateTimeOffset.Now
                          , DueDate = DateTimeOffset.Now
                           ,
                        }
                      , new()
                        {
                            Description = "Description1"
                          , Id = Guid.NewGuid()
                          , Price = 501
                          , ClientId = Guid.NewGuid()
                          , CreationDate = DateTimeOffset.Now
                          , DueDate = DateTimeOffset.Now
                           ,
                        }
                    };
        _createDtos = _entities.Select(x => (CreateOrderDto)x).ToList();
    }

    [Fact]
    public void TestDtoExplicitOperator()
    {
        //Arrange
        var dto = _dtos.First();
        //Act
        var order = (Orders.Service.Entities.Order)dto;
        //Assert
        Assert.True(order.Id == dto.Id);
    }

    [Fact]
    public void TestDtoExplicitOperatorWithList()
    {
        //Arrange
        //Act
        var entities = _dtos.Select(x => (Orders.Service.Entities.Order)x).ToList();

        //Assert
        for (int i = 0; i < entities.Count; i++)
        {
            Assert.True(entities[i].Id == _dtos[i].Id);
        }
    }

    [Fact]
    public void TestDtoImplicitOperator()
    {
        //Arrange
        var dto = _entities.First();
        //Act
        var order = (OrderDto)dto;
        //Assert
        Assert.True(order.Id == dto.Id);
    }

    [Fact]
    public void TestDtoImplicitOperatorWithList()
    {
        //Arrange
        //Act
        var entities = _entities.Select(x => (OrderDto)x).ToList();

        //Assert
        for (int i = 0; i < entities.Count; i++)
        {
            Assert.True(entities[i].Id == _entities[i].Id);
        }
    }
}
