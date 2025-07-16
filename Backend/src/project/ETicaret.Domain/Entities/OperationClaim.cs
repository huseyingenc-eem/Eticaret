using Core.Domain.Entities;

namespace ETicaret.Domain.Entities;
public class OperationClaim : Entity<int>
{
    public string OperationName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string RequiredRoles { get; set; } = string.Empty;
}