using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Services.Authorization;

/// <summary>
/// Belirtilen operasyon ismine (OperationName) sahip yetki kaydını getiren spesifikasyon.
/// </summary>
public class OperationClaimByNameSpecification : Specification<OperationClaim>
{
    public OperationClaimByNameSpecification(string operationName)
        : base(oc => oc.OperationName == operationName)
    {
    }
}