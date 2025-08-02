using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.OperationClaims.Specifications;

/// <summary>
/// Sadece Handler ve Query'ler tarafından kullanılan karmaşık specifications
/// Rule'lar tarafından kullanılan basit specs buradan çıkarıldı ve rule'lara taşındı
/// </summary>
public static class OperationClaimSpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    /// <summary>
    /// ID'ye göre operasyon yetkisini getirme - Handler'larda kullanılır
    /// Rules zaten existence kontrolü yaptı, Handler sadece get yapar
    /// </summary>
    public class ById : ByIdSpecification<OperationClaim, int>
    {
        public ById(int id) : base(id) { }
    }

    /// <summary>
    /// Operasyon adına göre yetki kaydını getiren spesifikasyon.
    /// GetList Query'sinde filtreleme için kullanılır.
    /// </summary>
    public class ByOperationName : Specification<OperationClaim>
    {
        public ByOperationName(string operationName)
            : base(oc => oc.OperationName.Contains(operationName))
        {
            AddOrderBy(oc => oc.OperationName);
        }
    }

    #endregion

    #region Query Specifications

    /// <summary>
    /// Feature adına göre yetki kayıtlarını getiren spesifikasyon.
    /// Sıralı liste halinde döner.
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
            AddOrderBy(oc => oc.FeatureName);
            AddOrderBy(oc => oc.OperationName);
        }
    }

    /// <summary>
    /// Veritabanındaki tüm operasyon yetkilerini getiren spesifikasyon.
    /// Seeder ve bulk operations için kullanılır.
    /// </summary>
    public class All : Specification<OperationClaim>
    {
        public All()
            : base(claim => true)
        {
            AddOrderBy(oc => oc.FeatureName);
            AddOrderBy(oc => oc.OperationName);
        }
    }

    #endregion
}