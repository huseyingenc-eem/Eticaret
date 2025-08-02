using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimResponseDto : IMapFrom<OperationClaim>
{
    public int Id { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string RequiredRoles { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
    public string Message { get; set; } = string.Empty;
}