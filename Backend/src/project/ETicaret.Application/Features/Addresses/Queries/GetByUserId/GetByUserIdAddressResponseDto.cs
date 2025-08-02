using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetByUserId;

public class GetByUserIdAddressResponseDto
{
    public Guid Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }

    public string UserFullName => $"{UserFirstName} {UserLastName}".Trim();

    public string UserEmail { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }

    [JsonIgnore]
    public string UserFirstName { get; set; } = string.Empty;

    [JsonIgnore]
    public string UserLastName { get; set; } = string.Empty;
}