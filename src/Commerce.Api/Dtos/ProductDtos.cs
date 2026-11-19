using System.ComponentModel.DataAnnotations;

namespace Commerce.Api.Dtos;

public sealed record ProductResponse(int Id, string Name, string Sku, decimal Price, int Stock);

public sealed record CreateProductRequest
{
    [Required, MaxLength(200)] public required string Name { get; init; }
    [Required, MaxLength(50)] public required string Sku { get; init; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal Price { get; init; }
    [Range(0, int.MaxValue)] public int Stock { get; init; }
}

public sealed record UpdateProductRequest
{
    [Required, MaxLength(200)] public required string Name { get; init; }
    [Required, MaxLength(50)] public required string Sku { get; init; }
    [Range(typeof(decimal), "0.01", "9999999999999999.99")] public decimal Price { get; init; }
    [Range(0, int.MaxValue)] public int Stock { get; init; }
}
