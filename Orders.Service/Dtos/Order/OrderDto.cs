using System.ComponentModel.DataAnnotations;

namespace Orders.Service.Dtos.Order;

public record OrderDto(Guid Id,Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate, DateTimeOffset CreationDate);

public record CreateOrderDto([Required] Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

public record UpdateOrderDto([Required] Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);