using System;

namespace ETicaret.Application.Features.Addresses.Queries.GetById;

/// <summary>
/// Tek bir adresin tüm detaylarını içeren DTO.
/// </summary>
public class GetByIdAddressResponseDto
{
    public int Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool isDefaultBilling { get; set; }
    public bool isDefaultShipping { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }
}