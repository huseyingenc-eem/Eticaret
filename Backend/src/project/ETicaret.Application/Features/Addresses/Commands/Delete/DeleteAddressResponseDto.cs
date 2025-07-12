namespace ETicaret.Application.Features.Addresses.Commands.Delete;


public class DeleteAddressResponseDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}