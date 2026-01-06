using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// E-posta adresinin benzersiz olması gerektiğini kontrol eden tek sorumlu rule.
/// </summary>
public class ContactEmailMustBeUniqueRule :
    IBusinessRule<CreateSupplierCommand>,
    IBusinessRule<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _repository;

    public ContactEmailMustBeUniqueRule(ISupplierRepository supplierRepository)
    {
        _repository =supplierRepository;
    }

    public bool ShouldExecute(CreateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.ContactEmail);
    public bool ShouldExecute(UpdateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.ContactEmail);

    public async Task ExecuteAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ContactEmailExistsSpec(command.ContactEmail!);
        await ValidateEmailUniqueness(spec, command.ContactEmail!, null, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ContactEmailExistsSpec(command.ContactEmail!);
        await ValidateEmailUniqueness(spec, command.ContactEmail!, command.Id, cancellationToken);
    }

    private async Task ValidateEmailUniqueness(ContactEmailExistsSpec spec, string email, Guid? excludeId, CancellationToken cancellationToken)
    {
        var existingSupplier = await _repository.GetAsync(spec, cancellationToken);

        if (existingSupplier != null && (!excludeId.HasValue || existingSupplier.Id != excludeId.Value))
        {
            throw new BusinessException(
                message: $"A supplier with contact email '{email}' already exists.",
                userFriendlyMessage: $"'{email}' e-posta adresiyle bir tedarikçi zaten mevcut.",
                errorCode: "DUPLICATE_CONTACT_EMAIL"
            );
        }
    }

    public int Priority => 2;

    #region Private Specification - Bu rule'a özel

    private class ContactEmailExistsSpec : Specification<Supplier>
    {
        public ContactEmailExistsSpec(string email)
            : base(s => s.ContactEmail != null && s.ContactEmail.ToLower() == email.ToLower()) { }
    }

    #endregion
}