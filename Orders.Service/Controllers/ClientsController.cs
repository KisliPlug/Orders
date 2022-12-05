using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IRepository<Client> _repository;

    private readonly IPublishEndpoint _publishEndpoint;

    public ClientsController(IRepository<Client> repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientDto>>> GetAsync()
    {
        var results = await _repository.GetAllAsync().Select(x =>  x.AsClientDto());
        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> GetAsync(Guid id)
    {  
        if (await _repository.GetAsync(id) is not { } item)
        {
            return NotFound();
        }

        return  Ok(item.AsClientDto());
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(CreateClientDto dto)
    {
        var item =  (Client)dto;
        await _repository.CreateAsync(item);
        await _publishEndpoint.Publish(new Contracts.ClientContract.ClientCreated(item.Id, item.Name, item.Description));
        return CreatedAtAction(nameof(GetAsync).Replace("Async", ""), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateClientDto updateClientDto)
    {
        if (await _repository.GetAsync(id) is not { } client)
        {
            return NotFound();
        }

        updateClientDto.SetProps(client);
        await _repository.UpdateAsync(client);
        await _publishEndpoint.Publish(new Contracts.ClientContract.ClientUpdated(client.Id, client.Name, client.Description));
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        if (_repository.GetAsync(id) is not { })
        {
            return NotFound();
        }

        await _repository.RemoveAsync(id);
        await _publishEndpoint.Publish(new Contracts.ClientContract.ClientDeleted(id));
        return NoContent();
    }
}