namespace ETicaret.Application.Features.Addresses.Commands.Delete;

/// <summary>
/// Adres silme işlemi sonucunu döndüren DTO.
/// </summary>
public class AddressDeleteResponseDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}