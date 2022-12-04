using System.ComponentModel.DataAnnotations;

namespace Orders.Service.Dtos.Costumer;

public record ClientDto(Guid Id,string Name, string Description,DateTimeOffset RegistrationTime );

public record CreateClientDto([Required] string Name, string Description  );

public record UpdateClientDto([Required] string Name, string Description );