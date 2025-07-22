namespace ETicaret.Application.Features.Addresses.Queries.GetList;

public class GetListAddressResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } 
    public string AddressTitle { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public string AddressLine { get; set; }
    public string? ZipCode { get; set; }
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
    public DateTime CreatedTime { get; set; }
}
