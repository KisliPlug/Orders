using System.ComponentModel.DataAnnotations;
using Orders.Common;

namespace Orders.Service.Entities;

[RequestAttribute("")]
[RequestAttribute("Create", nameof(Id), nameof(CreationDate))]
[RequestAttribute("Update", nameof(Id), nameof(CreationDate))]
[RequestAttribute("Delete", nameof(Description), nameof(Price),nameof(CreationDate),nameof(DueDate))]
public class Order : IEntity
{
    public Guid Id { get; set; }

    [Required]
    public Guid ClientId { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
}