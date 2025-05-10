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
    public string FullAddress { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; }
    public bool IsShippingAddress { get; set; }
    public DateTime UpdateTime { get; set; } 
    public string Message { get; set; } = string.Empty;
}
