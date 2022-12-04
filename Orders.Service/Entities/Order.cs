using Orders.Common;

namespace Orders.Service.Entities;

[RequestAttribute("")]
[RequestAttribute("Create")]
[RequestAttribute("Update")]
public class Order:IEntity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    
}