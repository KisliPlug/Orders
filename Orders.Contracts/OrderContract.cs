namespace Orders.Contracts;

public class OrderContract
{
   
    
    public record OrderCreation(Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderUpdate(Guid Id,  Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderDelete(Guid Id, Guid ClientId);
    
    
    public record OrderCreated(Guid Id,Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderUpdated(Guid Id,  Guid ClientId, string Description, decimal Price, DateTimeOffset DueDate);

    public record OrderDeleted(Guid Id, Guid ClientId);
}