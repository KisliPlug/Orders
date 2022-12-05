using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Orders.Common;
using Orders.Service.Entities;

namespace Orders.Service.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> _repository;

    private readonly IPublishEndpoint _publishEndpoint;

    public OrdersController(IRepository<Order> repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAsync()
    {
        return Ok((await _repository.GetAllAsync()).AsOrderDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetAsync(Guid id)
    {
        if (await _repository.GetAsync(id) is not { } item)
        {
            return NotFound();
        }

        return (OrderDto)item;
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(CreateOrderDto dto)
    {
        var item = (Order)dto;
        await _repository.CreateAsync(item);
        await _publishEndpoint.Publish(new Contracts.OrderContract.OrderCreated(item.Id
                                                                              , item.ClientId
                                                                              , item.Description
                                                                              , item.Price
                                                                              , item.CreationDate));
        return CreatedAtAction(nameof(GetAsync).Replace("Async", ""), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateOrderDto updateOrderDto)
    {
        if (await _repository.GetAsync(id) is not { } order)
        {
            return NotFound();
        }

        updateOrderDto.SetProps(order);
        await _repository.UpdateAsync(order);
        await _publishEndpoint.Publish(new Contracts.OrderContract.OrderUpdated(order.Id
                                                                              , order.ClientId
                                                                              , order.Description
                                                                              , order.Price
                                                                              , order.DueDate));
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(DeleteOrderDto dto)
    {
        if (_repository.GetAsync(dto.Id) is not { })
        {
            return NotFound();
        }

        await _publishEndpoint.Publish(new Contracts.OrderContract.OrderDeleted(dto.Id, dto.ClientId));
        await _publishEndpoint.Publish(new Contracts.ClientContract.ClientOrderRemove(dto.ClientId, dto.Id));
        await _repository.RemoveAsync(dto.Id);
        return NoContent();
    }
}
