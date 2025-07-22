using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Authorization;

/// <summary>
/// Veritabanındaki tüm operasyon yetkilerini (OperationClaim) getiren spesifikasyon.
/// </summary>
public class AllOperationClaimsSpecification : Specification<OperationClaim>
{
    public AllOperationClaimsSpecification()
        : base(claim => true) // Get all records
    {
        // No specific ordering is needed for this operation.
    }
}