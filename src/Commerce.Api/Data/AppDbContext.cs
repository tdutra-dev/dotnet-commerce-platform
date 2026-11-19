using Commerce.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Data;

// EF Core uses DbContext as a unit of work, similar to EF6 but with modern LINQ and provider APIs.
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Mario Rossi", Email = "mario.rossi@example.com", Phone = "+39 333 1234567", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new Customer { Id = 2, Name = "Anna Bianchi", Email = "anna.bianchi@example.com", Phone = "+39 333 7654321", CreatedAt = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero) });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "T-shirt", Sku = "TSHIRT-001", Price = 19.90m, Stock = 50 },
            new Product { Id = 2, Name = "Sneakers", Sku = "SHOE-001", Price = 79.50m, Stock = 30 },
            new Product { Id = 3, Name = "Backpack", Sku = "BAG-001", Price = 45.00m, Stock = 20 });
    }
}
