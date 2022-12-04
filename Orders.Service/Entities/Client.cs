using Orders.Common;

namespace Orders.Service.Entities;

[RequestAttribute("")]
[RequestAttribute("Create", nameof(Id), nameof(RegistrationTime), nameof(Orders))]
[RequestAttribute("Update", nameof(Id), nameof(RegistrationTime) )]
public class Client : IEntity
{
    
    public Guid Id { get; set; }
    public List<Guid> Orders { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset RegistrationTime { get; set; }
}