using Commerce.Api.Domain;
using Commerce.Api.Dtos;

namespace Commerce.Api.Services;

public interface IOrderService
{
    Task<OrderResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<PagedResponse<OrderResponse>> GetPagedAsync(int page, int pageSize, int? customerId, OrderStatus? status, CancellationToken ct);
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
