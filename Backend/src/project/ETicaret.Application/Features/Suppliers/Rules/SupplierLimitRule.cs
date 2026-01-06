using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// Sistem genelinde maksimum tedarikçi sayısını kontrol eden tek sorumlu kuraldır.
/// </summary>
public class SupplierLimitRule : IBusinessRule<CreateSupplierCommand>
{
    private const int MAX_SUPPLIERS_IN_SYSTEM = 1000;
    private readonly IRepository<Supplier, Guid> _repository;

    public SupplierLimitRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(CreateSupplierCommand command) => true;

    public async Task ExecuteAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ActiveSuppliersCountSpec();
        var activeSupplierCount = await _repository.CountAsync(spec, cancellationToken);

        if (activeSupplierCount >= MAX_SUPPLIERS_IN_SYSTEM)
        {
            throw new BusinessException(
                message: $"System has reached the maximum supplier limit of {MAX_SUPPLIERS_IN_SYSTEM}.",
                userFriendlyMessage: $"Sistem maksimum {MAX_SUPPLIERS_IN_SYSTEM} tedarikçi limitine ulaştı. Yeni tedarikçi eklenemez.",
                errorCode: "SUPPLIER_LIMIT_EXCEEDED"
            );
        }
    }

    public int Priority => 5;

    #region Private Specification

    /// <summary>
    /// Aktif tedarikçi sayımı için özel specification
    /// </summary>
    private class ActiveSuppliersCountSpec : Specification<Supplier>
    {
        public ActiveSuppliersCountSpec()
            : base(supplier => supplier.IsActive) { }
    }

    #endregion
}