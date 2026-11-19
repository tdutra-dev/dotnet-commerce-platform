using Commerce.Api.Dtos;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers.Mvc;

public sealed class CustomersController(ICustomerService service) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(await service.GetAllAsync(ct));

    [HttpGet] public IActionResult Create() => View(new CreateCustomerRequest { Name = "", Email = "" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        await service.CreateAsync(request, ct); return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var customer = await service.GetByIdAsync(id, ct); if (customer is null) return NotFound();
        return View(new UpdateCustomerRequest { Name = customer.Name, Email = customer.Email, Phone = customer.Phone });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCustomerRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(request);
        return await service.UpdateAsync(id, request, ct) ? RedirectToAction(nameof(Index)) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? RedirectToAction(nameof(Index)) : NotFound();
}
