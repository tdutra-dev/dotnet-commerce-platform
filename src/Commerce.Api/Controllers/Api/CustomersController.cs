using Commerce.Api.Dtos;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers.Api;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken ct) { var result = await service.GetByIdAsync(id, ct); return result is null ? NotFound() : Ok(result); }
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken ct)
    { var result = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdateCustomerRequest request, CancellationToken ct) => await service.UpdateAsync(id, request, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
