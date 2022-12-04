namespace Orders.Contracts;

public class OrderContract
{
   
    
    public record OrderCreated(Guid Id,Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderUpdated(Guid Id,  Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderDeleted(Guid Id, Guid ClientId);
}