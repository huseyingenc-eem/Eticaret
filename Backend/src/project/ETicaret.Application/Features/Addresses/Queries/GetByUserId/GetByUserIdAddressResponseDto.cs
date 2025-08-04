using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetByUserId;

/// <summary>
/// Kullanıcının tüm adreslerini getiren sorgu için immutable response DTO.
/// Record pattern kullanılarak performans ve memory management optimize edilmiştir.
/// </summary>
public sealed record GetByUserIdAddressResponseDto
{
    public required Guid Id { get; init; }
    public required string AddressTitle { get; init; }
    public required string Country { get; init; }
    public required string City { get; init; }
    public required string District { get; init; }
    public string? ZipCode { get; init; }
    public required string AddressLine { get; init; }
    public string? PhoneNumber { get; init; }

    public required bool IsDefaultShipping { get; init; }
    public required bool IsDefaultBilling { get; init; }

    /// <summary>
    /// Kullanıcının tam adı. Computed property olarak FirstName + LastName birleşimi.
    /// </summary>
    public string UserFullName => $"{UserFirstName} {UserLastName}".Trim();

    public required string UserEmail { get; init; }
    public required DateTime CreatedTime { get; init; }

    [JsonIgnore]
    public required string UserFirstName { get; init; }

    [JsonIgnore]
    public required string UserLastName { get; init; }
}