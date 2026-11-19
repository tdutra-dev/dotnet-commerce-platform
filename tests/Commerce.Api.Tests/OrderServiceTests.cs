using Commerce.Api.Data;
using Commerce.Api.Domain;
using Commerce.Api.Dtos;
using Commerce.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Tests;

public sealed class OrderServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;

    public OrderServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateRejectsInsufficientStockAndRollsBack()
    {
        var customer = new Customer { Name = "Test", Email = "test@example.com" }; 
        var product = new Product { Name = "Book", Sku = "BOOK-1", Price = 10m, Stock = 2 };
        _db.AddRange(customer, product); await _db.SaveChangesAsync();
        var service = new OrderService(_db);

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(new CreateOrderRequest
        { CustomerId = customer.Id, Items = [new OrderItemRequest { ProductId = product.Id, Quantity = 3 }] }, CancellationToken.None));

        Assert.Equal(2, (await _db.Products.SingleAsync(x => x.Id == product.Id)).Stock);
        Assert.Empty(await _db.Orders.ToListAsync());
    }

    [Fact]
    public async Task CreateDecrementsStockAndCreatesOrder()
    {
        var customer = new Customer { Name = "Test", Email = "test2@example.com" };
        var product = new Product { Name = "Pen", Sku = "PEN-1", Price = 3m, Stock = 5 };
        _db.AddRange(customer, product); await _db.SaveChangesAsync();
        var service = new OrderService(_db);

        var result = await service.CreateAsync(new CreateOrderRequest
        { CustomerId = customer.Id, Items = [new OrderItemRequest { ProductId = product.Id, Quantity = 2 }] }, CancellationToken.None);

        Assert.Equal(3, (await _db.Products.SingleAsync(x => x.Id == product.Id)).Stock);
        Assert.Equal(6m, result.Total);
    }

    [Fact]
    public async Task StatusRulesAllowPaidThenShipped()
    {
        var customer = new Customer { Name = "Test", Email = "test3@example.com" };
        var order = new Order { Customer = customer, Status = OrderStatus.Pending };
        _db.Add(order); await _db.SaveChangesAsync();
        var service = new OrderService(_db);

        Assert.True(await service.UpdateStatusAsync(order.Id, OrderStatus.Paid, CancellationToken.None));
        Assert.True(await service.UpdateStatusAsync(order.Id, OrderStatus.Shipped, CancellationToken.None));
        await Assert.ThrowsAsync<ConflictException>(() => service.UpdateStatusAsync(order.Id, OrderStatus.Pending, CancellationToken.None));
    }

    [Fact]
    public async Task PendingOrderCanBeDeletedAndRestoresStock()
    {
        var customer = new Customer { Name = "Test", Email = "test4@example.com" };
        var product = new Product { Name = "Mouse", Sku = "MOUSE-1", Price = 25m, Stock = 10 };
        var order = new Order { Customer = customer, Status = OrderStatus.Pending };
        order.Items.Add(new OrderItem { Order = order, Product = product, Quantity = 2, UnitPrice = 25m });
        product.Stock = 8;
        _db.Add(order); await _db.SaveChangesAsync();
        var service = new OrderService(_db);

        Assert.True(await service.DeleteAsync(order.Id, CancellationToken.None));
        Assert.Equal(10, (await _db.Products.SingleAsync(x => x.Id == product.Id)).Stock);
    }

    public void Dispose() { _db.Dispose(); _connection.Dispose(); }
}
