using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Delete;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// Tedarikçinin mevcut olduğunu kontrol eden tek sorumlu rule.
/// </summary>
public class SupplierMustExistRule :
    IBusinessRule<UpdateSupplierCommand>,
    IBusinessRule<DeleteSupplierCommand>
{
    private readonly ISupplierRepository _repository;

    public SupplierMustExistRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(UpdateSupplierCommand command) => command.Id != Guid.Empty;
    public bool ShouldExecute(DeleteSupplierCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateSupplierExists(command.Id, cancellationToken);
    }

    public async Task ExecuteAsync(DeleteSupplierCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateSupplierExists(command.Id, cancellationToken);
    }

    private async Task ValidateSupplierExists(Guid supplierId, CancellationToken cancellationToken)
    {
        var spec = new SupplierByIdSpec(supplierId);
        var supplier = await _repository.GetAsync(spec, cancellationToken);

        if (supplier == null)
        {
            throw new NotFoundException(
                message: $"Supplier with ID {supplierId} not found.",
                userFriendlyMessage: "Belirtilen tedarikçi bulunamadı.",
                errorCode: "SUPPLIER_NOT_FOUND"
            );
        }
    }

    public int Priority => 0;

    #region Private Specification

    private class SupplierByIdSpec : Specification<Supplier>
    {
        public SupplierByIdSpec(Guid id)
            : base(supplier => supplier.Id == id) { }
    }

    #endregion
}