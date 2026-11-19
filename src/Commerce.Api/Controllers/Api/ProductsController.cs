using Commerce.Api.Dtos;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers.Api;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken ct) { var result = await service.GetByIdAsync(id, ct); return result is null ? NotFound() : Ok(result); }
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request, CancellationToken ct)
    { var result = await service.CreateAsync(request, ct); return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); }
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, UpdateProductRequest request, CancellationToken ct) => await service.UpdateAsync(id, request, ct) ? NoContent() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
