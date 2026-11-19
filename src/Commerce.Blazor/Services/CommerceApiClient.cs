using Commerce.Blazor.Models;

namespace Commerce.Blazor.Services;

public sealed class CommerceApiClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _http = httpClientFactory.CreateClient("CommerceApi");

    public async Task<IReadOnlyList<CustomerResponse>> GetCustomersAsync(CancellationToken ct)
    {
        var data = await _http.GetFromJsonAsync<IReadOnlyList<CustomerResponse>>("/api/customers", ct);
        return data ?? [];
    }

    public async Task<IReadOnlyList<ProductResponse>> GetProductsAsync(CancellationToken ct)
    {
        var data = await _http.GetFromJsonAsync<IReadOnlyList<ProductResponse>>("/api/products", ct);
        return data ?? [];
    }

    public async Task<PagedResponse<OrderResponse>> GetOrdersAsync(int page, int pageSize, CancellationToken ct)
    {
        var data = await _http.GetFromJsonAsync<PagedResponse<OrderResponse>>($"/api/orders?page={page}&pageSize={pageSize}", ct);
        return data ?? new PagedResponse<OrderResponse>([], page, pageSize, 0);
    }
}
