using Commerce.Api.Dtos;

namespace Commerce.Api.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(CancellationToken ct);
    Task<CustomerResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
