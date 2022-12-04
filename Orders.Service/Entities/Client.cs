using Orders.Common;

namespace Orders.Service.Entities;

public class Client : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
}