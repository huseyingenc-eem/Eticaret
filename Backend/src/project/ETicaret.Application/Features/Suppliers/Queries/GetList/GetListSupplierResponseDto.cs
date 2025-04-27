namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; } // Listede gösterilebilir
    public string? PhoneNumber { get; set; } // Listede gösterilebilir
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
}