using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.OperationClaims.Specifications;

/// <summary>
/// OperationClaim entity'si için veritabanı sorgu spesifikasyonları.
/// Bu sınıf sadece veritabanı sorgu mantığını içerir, iş kuralları içermez.
/// Specification pattern kullanarak karmaşık sorguları yeniden kullanılabilir hale getirir.
/// </summary>
public static class OperationClaimSpecifications
{
    /// <summary>
    /// ID'ye göre operasyon yetkisini getiren spesifikasyon.
    /// </summary>
    public class ById : Specification<OperationClaim>
    {
        public ById(int id)
            : base(oc => oc.Id == id)
        {
        }
    }

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

    /// <summary>
    /// Veritabanındaki tüm operasyon yetkilerini getiren spesifikasyon.
    /// </summary>
    public class All : Specification<OperationClaim>
    {
        public All()
            : base(claim => true) // Get all records
        {
            // No specific ordering is needed for this operation.
        }
    }
}