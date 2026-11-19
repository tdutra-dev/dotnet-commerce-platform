using Commerce.Api.Domain;
using Commerce.Api.Dtos;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers.Api;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken ct)
    { var result = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(int id, CancellationToken ct)
    { var result = await service.GetByIdAsync(id, ct); return result is null ? NotFound() : Ok(result); }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? customerId = null, [FromQuery] OrderStatus? status = null, CancellationToken ct = default) =>
        Ok(await service.GetPagedAsync(page, pageSize, customerId, status, ct));

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request, CancellationToken ct) =>
        await service.UpdateStatusAsync(id, request.Status, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
