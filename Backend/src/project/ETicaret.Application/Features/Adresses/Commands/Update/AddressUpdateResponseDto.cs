namespace ETicaret.Application.Features.Addresses.Commands.Update;

/// <summary>
/// Adres güncelleme işlemi sonucunu döndüren DTO.
/// </summary>
public class AddressUpdateResponseDto
{
    public int Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? UpdateTime { get; set; }
}