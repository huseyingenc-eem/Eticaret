using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;


namespace ETicaret.Application.Features.OperationClaims.Queries.GetList;

/// <summary>
/// Operasyon yetkileri listesi sorgusunun yanıt DTO'su.
/// Liste görünümü için gerekli temel bilgileri içerir.
/// </summary>
public class GetListOperationClaimsResponseDto : IMapFrom<OperationClaim>
{
    public int Id { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string FeatureName { get; set; } = string.Empty;
    public string RequiredRoles { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// Gerekli rollerin array formatında gösterimi (UI için kolaylık).
    /// RequiredRoles string'inden parse edilir.
    /// </summary>
    public string[] RequiredRolesArray =>
        string.IsNullOrWhiteSpace(RequiredRoles)
            ? Array.Empty<string>()
            : RequiredRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(role => role.Trim())
                          .ToArray();
}