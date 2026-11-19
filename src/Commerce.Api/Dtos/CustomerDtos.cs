using System.ComponentModel.DataAnnotations;

namespace Commerce.Api.Dtos;

public sealed record CustomerResponse(int Id, string Name, string Email, string? Phone, DateTimeOffset CreatedAt);

public sealed record CreateCustomerRequest
{
    [Required, MaxLength(200)] public required string Name { get; init; }
    [Required, EmailAddress, MaxLength(320)] public required string Email { get; init; }
    [MaxLength(30)] public string? Phone { get; init; }
}

public sealed record UpdateCustomerRequest
{
    [Required, MaxLength(200)] public required string Name { get; init; }
    [Required, EmailAddress, MaxLength(320)] public required string Email { get; init; }
    [MaxLength(30)] public string? Phone { get; init; }
}
