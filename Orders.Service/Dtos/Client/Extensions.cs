namespace Orders.Service.Dtos.Costumer;

public static class DtoExtensions
{
 

    

    public static ClientDto AsDto(this Entities.Client item)
    {
        return new ClientDto(item.Id
                             , item.Name
                             , item.Description
                             , item.RegistrationTime);
    }

 

    public static Entities.Client CreateCostumer(this CreateClientDto item)
    {
        return new Entities.Client
               {
                   Id = Guid.NewGuid()
                 , Name = item.Name
                 , Description = item.Description
                 , RegistrationTime = DateTimeOffset.Now
               };
    }

    public static Entities.Client CreateCostumer(this UpdateClientDto item, Guid id)
    {
        return new Entities.Client
               {
                   Id = id
                 , Name = item.Name
                 , Description = item.Description
                 , RegistrationTime = DateTimeOffset.Now
               };
    }

    public static async Task<IEnumerable<ClientDto>> AsDtoAsync(this Task<IReadOnlyCollection<Entities.Client>> items)
    {
        return (await items).Select(x => x.AsDto());
    }
}