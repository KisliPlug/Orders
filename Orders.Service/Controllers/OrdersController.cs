// using MassTransit;
// using Microsoft.AspNetCore.Mvc;
// using Orders.Common;
// using Orders.Service.Dtos.Order;
// using Orders.Service.Entities;
//
// namespace Orders.Service.Controllers;
//
// [ApiController]
// [Route("[controller]")]
// public class OrdersController : ControllerBase
// {
//     private readonly IRepository<Order> _repository;
//
//     public OrdersController(IRepository<Order> repository)
//     {
//         _repository = repository;
//     }
//
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<OrderDto>>> GetAsync()
//     {
//         return Ok(await _repository.GetAllAsync().AsDtoAsync());
//     }
//
//     [HttpGet("{id}")]
//     public async Task<ActionResult<OrderDto>> GetAsync(Guid id)
//     {
//         if (await _repository.GetAsync(id) is not { } item)
//         {
//             return NotFound();
//         }
//
//         return item.AsDto();
//     }
//
//     [HttpPost]
//     public async Task<ActionResult> PostAsync(CreateOrderDto dto)
//     {
//         var item = dto.CreateOrder();
//         await _repository.CreateAsync(item);
//         return CreatedAtAction(nameof(GetAsync).Replace("Async", ""), new { id = item.Id }, item);
//     }
//
//     [HttpPut("{id}")]
//     public async Task<IActionResult> PutAsync(Guid id, UpdateOrderDto updateOrderDto)
//     {
//         if (await _repository.GetAsync(id) is not { })
//         {
//             return NotFound();
//         }
//
//         var existing = updateOrderDto.UpdateOrder(id);
//         await _repository.UpdateAsync(existing);
//         return Ok();
//     }
//
//     [HttpDelete("{id}")]
//     public async Task<ActionResult> Delete(Guid id)
//     {
//         if (_repository.GetAsync(id) is not { })
//         {
//             return NotFound();
//         }
//
//         await _repository.RemoveAsync(id);
//         return NoContent();
//     }
// }