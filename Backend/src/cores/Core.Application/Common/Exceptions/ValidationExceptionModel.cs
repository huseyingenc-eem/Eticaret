namespace Core.Application.Common.Exceptions;

public class ValidationExceptionModel
{
    public string? Property { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
