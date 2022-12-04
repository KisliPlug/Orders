namespace Orders.Contracts;

public class ClientContract
{
    public record ClientCreated(Guid Id, string Name, string Description);

    public record ClientUpdated(Guid Id, string Name, string Description);

    public record ClientDeleted(Guid Id);
}