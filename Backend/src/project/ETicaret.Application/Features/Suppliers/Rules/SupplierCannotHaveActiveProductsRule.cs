using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Delete;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// Tedarikçinin aktif ürünlerinin olmaması gerektiğini kontrol eden tek sorumlu rule.
/// Tedarikçi silinmeden önce aktif ürünlerinin olmadığından emin olur.
/// </summary>
public class SupplierCannotHaveActiveProductsRule : IBusinessRule<DeleteSupplierCommand>
{
    private readonly ISupplierRepository _repository;

    public SupplierCannotHaveActiveProductsRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(DeleteSupplierCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(DeleteSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new SupplierWithActiveProductsSpec(command.Id);
        var supplierWithProducts = await _repository.GetAsync(spec, cancellationToken);

        if (supplierWithProducts != null)
        {
            throw new BusinessException(
                message: $"Supplier with ID {command.Id} cannot be deleted because it has active products.",
                userFriendlyMessage: "Bu tedarikçiye ait aktif ürünler bulunduğu için silinemez. Önce ürünleri pasif hale getirin.",
                errorCode: "SUPPLIER_HAS_ACTIVE_PRODUCTS"
            );
        }
    }

    public int Priority => 4;

    #region Private Specification - Bu rule'a özel

    /// <summary>
    /// Aktif ürünlere sahip tedarikçi kontrolü
    /// </summary>
    private class SupplierWithActiveProductsSpec : Specification<Supplier>
    {
        public SupplierWithActiveProductsSpec(Guid supplierId)
            : base(s => s.Id == supplierId && s.Products.Any(p => p.IsActive))
        {
            AddInclude(s => s.Products);
        }
    }

    #endregion
}