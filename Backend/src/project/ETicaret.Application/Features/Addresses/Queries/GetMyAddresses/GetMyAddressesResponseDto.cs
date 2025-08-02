using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Queries.GetMyAddresses;
public class GetMyAddressesResponseDto : IMapFrom<Address>
{
    #region Properties
    public Guid Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public bool IsDefaultBilling { get; set; }
    public bool IsDefaultShipping { get; set; }
    public DateTime CreatedTime { get; set; }

    #endregion
}