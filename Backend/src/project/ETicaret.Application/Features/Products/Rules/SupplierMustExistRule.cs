using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Rules;

/// <summary>
/// Ürün eklenirken/güncellenirken Supplier'ın mevcut olduğunu kontrol eden rule.
/// Sadece SupplierId belirtilmişse çalışır.
/// </summary>
public class SupplierMustExistRule :
    IBusinessRule<CreateProductCommand>,
    IBusinessRule<UpdateProductCommand>
{
    private readonly ISupplierRepository _repository;

    public SupplierMustExistRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(CreateProductCommand command) => command.SupplierId.HasValue && command.SupplierId != Guid.Empty;
    public bool ShouldExecute(UpdateProductCommand command) => command.SupplierID != Guid.Empty;

    public async Task ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (command.SupplierId.HasValue && command.SupplierId != Guid.Empty)
        {
            await ValidateSupplierExists(command.SupplierId.Value, cancellationToken);
        }
    }

    public async Task ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (command.SupplierID != Guid.Empty)
        {
            await ValidateSupplierExists(command.SupplierID, cancellationToken);
        }
    }

    private async Task ValidateSupplierExists(Guid supplierId, CancellationToken cancellationToken)
    {
        var spec = new SupplierExistsSpec(supplierId);
        var supplierExists = await _repository.AnyAsync(spec, cancellationToken);

        if (!supplierExists)
        {
            throw new NotFoundException(
                message: $"Supplier with ID {supplierId} not found.",
                userFriendlyMessage: "Belirtilen tedarikçi bulunamadı.",
                errorCode: ProductConstants.ErrorCodes.InvalidSupplier
            );
        }
    }

    public int Priority => 3;

    #region Private Specification - Bu rule'a özel

    private class SupplierExistsSpec : Specification<Supplier>
    {
        public SupplierExistsSpec(Guid supplierId)
            : base(s => s.Id == supplierId && s.IsActive) { }
    }

    #endregion
}