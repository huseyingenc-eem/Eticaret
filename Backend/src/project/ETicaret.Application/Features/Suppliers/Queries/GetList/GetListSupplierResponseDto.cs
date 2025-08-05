using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierResponseDto : IMapFrom<Supplier>
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
}