namespace Orders.Contracts;

public class ClientContract
{
    public record ClientCreation(Guid Id, string Name, string Description);

    public record ClientUpdate(Guid Id, string Name, string Description);
    public record ClientOrderRemove(Guid Id, Guid OrderId);
    public record ClientOrderRemoved(Guid Id );

    public record ClientDeletion(Guid Id);
    
    public record ClientCreated(Guid Id, string Name, string Description);

    public record ClientUpdated(Guid Id, string Name, string Description);

    public record ClientDeleted(Guid Id);
}