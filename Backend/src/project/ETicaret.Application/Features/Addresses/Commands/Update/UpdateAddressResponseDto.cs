namespace ETicaret.Application.Features.Addresses.Commands.Update;


public class UpdateAddressResponseDto
{
    
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsDefaultBilling { get; set; } = false;
    public bool IsDefaultShipping { get; set; } = false;
    public DateTime UpdateTime { get; set; } 
    public string Message { get; set; } = string.Empty;
}
