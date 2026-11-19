using Commerce.Api.Data;
using Commerce.Api.Domain;
using Commerce.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Services;

public sealed class ProductService(AppDbContext db) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken ct) =>
        await db.Products.AsNoTracking().OrderBy(x => x.Name).Select(x => new ProductResponse(x.Id, x.Name, x.Sku, x.Price, x.Stock)).ToListAsync(ct);

    public async Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct) =>
        await db.Products.AsNoTracking().Where(x => x.Id == id).Select(x => new ProductResponse(x.Id, x.Name, x.Sku, x.Price, x.Stock)).FirstOrDefaultAsync(ct);

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken ct)
    {
        if (await db.Products.AnyAsync(x => x.Sku == request.Sku, ct)) throw new InvalidOperationException("A product with this SKU already exists.");
        var product = new Product { Name = request.Name, Sku = request.Sku, Price = request.Price, Stock = request.Stock };
        db.Products.Add(product); await db.SaveChangesAsync(ct);
        return new ProductResponse(product.Id, product.Name, product.Sku, product.Price, product.Stock);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProductRequest request, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([id], ct); if (product is null) return false;
        if (await db.Products.AnyAsync(x => x.Id != id && x.Sku == request.Sku, ct)) throw new InvalidOperationException("A product with this SKU already exists.");
        product.Name = request.Name; product.Sku = request.Sku; product.Price = request.Price; product.Stock = request.Stock;
        await db.SaveChangesAsync(ct); return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var product = await db.Products.FindAsync([id], ct); if (product is null) return false;
        db.Products.Remove(product); await db.SaveChangesAsync(ct); return true;
    }
}
