namespace Commerce.Blazor.Models;

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Cancelled
}

public sealed record CustomerResponse(int Id, string Name, string Email, string? Phone, DateTimeOffset CreatedAt);
public sealed record ProductResponse(int Id, string Name, string Sku, decimal Price, int Stock);
public sealed record OrderItemResponse(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);
public sealed record OrderResponse(int Id, int CustomerId, string CustomerName, DateTimeOffset OrderDate, OrderStatus Status, decimal Total, IReadOnlyList<OrderItemResponse> Items);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
