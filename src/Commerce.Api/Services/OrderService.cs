using Commerce.Api.Data;
using Commerce.Api.Domain;
using Commerce.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Api.Services;

public sealed class OrderService(AppDbContext db) : IOrderService
{
    public async Task<OrderResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var order = await db.Orders.AsNoTracking().Include(x => x.Customer).Include(x => x.Items).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        return order is null ? null : Map(order);
    }

    public async Task<PagedResponse<OrderResponse>> GetPagedAsync(int page, int pageSize, int? customerId, OrderStatus? status, CancellationToken ct)
    {
        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Orders.AsNoTracking().Include(x => x.Customer).Include(x => x.Items).ThenInclude(x => x.Product).AsQueryable();
        if (customerId.HasValue) query = query.Where(x => x.CustomerId == customerId.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        var total = await query.CountAsync(ct);
        var orders = await query.OrderByDescending(x => x.OrderDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResponse<OrderResponse>(orders.Select(Map).ToList(), page, pageSize, total);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        if (!await db.Customers.AnyAsync(x => x.Id == request.CustomerId, ct)) throw new NotFoundException("Customer not found.");
        if (request.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1)) throw new ValidationException("Duplicate products are not allowed in one order.");

        var order = new Order { CustomerId = request.CustomerId, OrderDate = DateTimeOffset.UtcNow, Status = OrderStatus.Pending };
        foreach (var itemRequest in request.Items)
        {
            var product = await db.Products.FirstOrDefaultAsync(x => x.Id == itemRequest.ProductId, ct);
            if (product is null) throw new NotFoundException($"Product {itemRequest.ProductId} not found.");
            if (product.Stock < itemRequest.Quantity) throw new ConflictException($"Insufficient stock for product {product.Name}.");
            product.Stock -= itemRequest.Quantity;
            order.Items.Add(new OrderItem { ProductId = product.Id, Product = product, Quantity = itemRequest.Quantity, UnitPrice = product.Price });
        }
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        await db.Entry(order).Reference(x => x.Customer).LoadAsync(ct);
        foreach (var item in order.Items) await db.Entry(item).Reference(x => x.Product).LoadAsync(ct);
        return Map(order);
    }

    public async Task<bool> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct)
    {
        var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == id, ct); if (order is null) return false;
        if (!IsAllowedTransition(order.Status, status)) throw new ConflictException($"Cannot transition order from {order.Status} to {status}.");
        order.Status = status; await db.SaveChangesAsync(ct); return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var order = await db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct); if (order is null) return false;
        if (order.Status != OrderStatus.Pending) throw new ConflictException("Only pending orders can be deleted.");
        foreach (var item in order.Items) { var product = await db.Products.FirstAsync(x => x.Id == item.ProductId, ct); product.Stock += item.Quantity; }
        db.Orders.Remove(order); await db.SaveChangesAsync(ct); return true;
    }

    private static bool IsAllowedTransition(OrderStatus current, OrderStatus next) => current switch
    {
        OrderStatus.Pending => next is OrderStatus.Paid or OrderStatus.Cancelled,
        OrderStatus.Paid => next is OrderStatus.Shipped or OrderStatus.Cancelled,
        OrderStatus.Shipped => false,
        OrderStatus.Cancelled => false,
        _ => false
    };

    private static OrderResponse Map(Order order) => new(order.Id, order.CustomerId, order.Customer.Name,
        order.OrderDate, order.Status,
        order.Items.Sum(x => x.Quantity * x.UnitPrice),
        order.Items.Select(x => new OrderItemResponse(x.ProductId, x.Product.Name, x.Quantity, x.UnitPrice, x.Quantity * x.UnitPrice)).ToList());
}
