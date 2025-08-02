using System.Text.Json.Serialization;

namespace ETicaret.Application.Features.Addresses.Queries.GetList;


public class GetListAddressResponseDto
{
    public Guid Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string UserFullName => $"{UserFirstName} {UserLastName}".Trim();
    public string UserEmail { get; set; } = string.Empty;
    public string? isDefaultBilling { get; set; }
    public string? isDefaultShipping { get; set; }


    [JsonIgnore]
    public string UserFirstName { get; set; }
    [JsonIgnore]
    public string UserLastName { get; set; }
}