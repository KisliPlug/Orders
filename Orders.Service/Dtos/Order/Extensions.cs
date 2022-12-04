namespace Orders.Service.Dtos.Order;

public static class DtoExtensions
{
    public static OrderDto AsDto(this Entities.Order item)
    {
        return new OrderDto(item.Id
                          , item.ClientId
                          , item.Description
                          , item.Price
                          , item.DueDate
                          , item.CreationDate);
    }

    public static Entities.Order CreateOrder(this CreateOrderDto createDto)
    {
        return new Entities.Order
               {
                   Id = Guid.NewGuid()
                 , ClientId = createDto.ClientId
                 , Description = createDto.Description
                 , Price = createDto.Price
                 , DueDate = createDto.DueDate
                 , CreationDate = DateTimeOffset.Now
               };
    }

    public static Entities.Order UpdateOrder(this UpdateOrderDto updateDto, Guid id)
    {
        return new Entities.Order
               {
                   Id = id
                 , ClientId = updateDto.ClientId
                 , Description = updateDto.Description
                 , Price = updateDto.Price
                 , DueDate = updateDto.DueDate
                 , CreationDate = DateTimeOffset.Now
               };
    }

    public static async Task<IEnumerable<OrderDto>> AsDtoAsync(this Task<IReadOnlyCollection<Entities.Order>> items)
    {
        return (await items).Select(x => x.AsDto());
    }
}