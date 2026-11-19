using System.ComponentModel.DataAnnotations;
using Commerce.Api.Domain;

namespace Commerce.Api.Dtos;

public sealed record OrderItemRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; init; }
    [Range(1, int.MaxValue)] public int Quantity { get; init; }
}

public sealed record CreateOrderRequest
{
    [Range(1, int.MaxValue)] public int CustomerId { get; init; }
    [MinLength(1)] public required IReadOnlyCollection<OrderItemRequest> Items { get; init; }
}

public sealed record UpdateOrderStatusRequest
{
    [Required] public OrderStatus Status { get; init; }
}

public sealed record OrderItemResponse(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);
public sealed record OrderResponse(int Id, int CustomerId, string CustomerName, DateTimeOffset OrderDate, OrderStatus Status, decimal Total, IReadOnlyList<OrderItemResponse> Items);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
