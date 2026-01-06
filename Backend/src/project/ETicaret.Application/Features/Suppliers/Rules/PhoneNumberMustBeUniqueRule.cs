using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// Telefon numarasının benzersiz olması gerektiğini kontrol eder.
/// </summary>
public class PhoneNumberMustBeUniqueRule :
    IBusinessRule<CreateSupplierCommand>,
    IBusinessRule<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _repository;

    public PhoneNumberMustBeUniqueRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(CreateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.PhoneNumber);
    public bool ShouldExecute(UpdateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.PhoneNumber);

    public async Task ExecuteAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new PhoneNumberExistsSpec(command.PhoneNumber!);
        await ValidatePhoneUniqueness(spec, command.PhoneNumber!, null, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new PhoneNumberExistsSpec(command.PhoneNumber!);
        await ValidatePhoneUniqueness(spec, command.PhoneNumber!, command.Id, cancellationToken);
    }

    private async Task ValidatePhoneUniqueness(PhoneNumberExistsSpec spec, string phoneNumber, Guid? excludeId, CancellationToken cancellationToken)
    {
        var existingSupplier = await _repository.GetAsync(spec, cancellationToken);

        if (existingSupplier != null && (!excludeId.HasValue || existingSupplier.Id != excludeId.Value))
        {
            throw new BusinessException(
                message: $"A supplier with phone number '{phoneNumber}' already exists.",
                userFriendlyMessage: $"'{phoneNumber}' telefon numarasıyla bir tedarikçi zaten mevcut.",
                errorCode: "DUPLICATE_PHONE_NUMBER"
            );
        }
    }

    public int Priority => 3;

    #region Private Specification

    private class PhoneNumberExistsSpec : Specification<Supplier>
    {
        public PhoneNumberExistsSpec(string phoneNumber)
            : base(s => s.PhoneNumber != null && s.PhoneNumber == phoneNumber) { }
    }

    #endregion
}