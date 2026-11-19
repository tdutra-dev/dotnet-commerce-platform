using Commerce.Api.Data;
using Commerce.Api.Domain;
using Commerce.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Services;

public sealed class CustomerService(AppDbContext db) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Customers.AsNoTracking().OrderBy(x => x.Name).Select(x => new CustomerResponse(x.Id, x.Name, x.Email, x.Phone, x.CreatedAt)).ToListAsync(ct);

    public async Task<CustomerResponse?> GetByIdAsync(int id, CancellationToken ct) =>
        await db.Customers.AsNoTracking().Where(x => x.Id == id).Select(x => new CustomerResponse(x.Id, x.Name, x.Email, x.Phone, x.CreatedAt)).FirstOrDefaultAsync(ct);

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        if (await db.Customers.AnyAsync(x => x.Email == request.Email, ct)) throw new InvalidOperationException("A customer with this email already exists.");
        var customer = new Customer { Name = request.Name, Email = request.Email, Phone = request.Phone };
        db.Customers.Add(customer); await db.SaveChangesAsync(ct);
        return new CustomerResponse(customer.Id, customer.Name, customer.Email, customer.Phone, customer.CreatedAt);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct)
    {
        var customer = await db.Customers.FindAsync([id], ct); if (customer is null) return false;
        if (await db.Customers.AnyAsync(x => x.Id != id && x.Email == request.Email, ct)) throw new InvalidOperationException("A customer with this email already exists.");
        customer.Name = request.Name; customer.Email = request.Email; customer.Phone = request.Phone;
        await db.SaveChangesAsync(ct); return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var customer = await db.Customers.FindAsync([id], ct); if (customer is null) return false;
        db.Customers.Remove(customer); await db.SaveChangesAsync(ct); return true;
    }
}
