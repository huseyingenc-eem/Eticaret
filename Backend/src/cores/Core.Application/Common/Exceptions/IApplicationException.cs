
namespace Core.Application.Common.Exceptions;

public interface IApplicationException
{
    string? UserFriendlyMessage { get; }
    string? ErrorCode { get; }
    object? AdditionalData { get; }
}
