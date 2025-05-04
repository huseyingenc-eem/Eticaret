namespace ETicaret.Application.Features.Addresses.Commands.Create;

public class AddressAddResponseDto
{
    public int Id { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
}