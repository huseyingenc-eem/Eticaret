using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Authorization.Specifications;

/// <summary>
/// OperationClaim entity'si için spesifikasyonlar.
/// </summary>
public static class OperationClaimSpecifications
{
    /// <summary>
    /// Operasyon adına göre yetki kaydını getiren spesifikasyon.
    /// </summary>
    public class ByOperationName : Specification<OperationClaim>
    {
        public ByOperationName(string operationName)
            : base(oc => oc.OperationName == operationName)
        {
        }
    }

    /// <summary>
    /// Feature adına göre yetki kayıtlarını getiren spesifikasyon.
    /// </summary>
    public class ByFeatureName : Specification<OperationClaim>
    {
        public ByFeatureName(string featureName)
            : base(oc => oc.FeatureName == featureName)
        {
            AddOrderBy(oc => oc.OperationName);
        }
    }

    /// <summary>
    /// Belirli rolleri gerektiren operasyonları getiren spesifikasyon.
    /// </summary>
    public class ByRequiredRoles : Specification<OperationClaim>
    {
        public ByRequiredRoles(string requiredRole)
            : base(oc => oc.RequiredRoles.Contains(requiredRole))
        {
        }
    }
}