using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Suppliers.Rules;

/// <summary>
/// Şirket adının benzersiz olması gerektiğini kontrol eden tek sorumlu rule.
/// AddressLimitRule pattern'ı kullanılarak oluşturulmuştur.
/// </summary>
public class CompanyNameMustBeUniqueRule :
    IBusinessRule<CreateSupplierCommand>,
    IBusinessRule<UpdateSupplierCommand>
{
    private readonly ISupplierRepository _repository;

    public CompanyNameMustBeUniqueRule(ISupplierRepository supplierRepository)
    {
        _repository = supplierRepository;
    }

    public bool ShouldExecute(CreateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.CompanyName);
    public bool ShouldExecute(UpdateSupplierCommand command) => !string.IsNullOrWhiteSpace(command.CompanyName);

    public async Task ExecuteAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new CompanyNameExistsSpec(command.CompanyName);
        await ValidateUniqueness(spec, command.CompanyName, null, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new CompanyNameExistsExcludingIdSpec(command.Id, command.CompanyName);
        await ValidateUniqueness(spec, command.CompanyName, command.Id, cancellationToken);
    }

    private async Task ValidateUniqueness(Specification<Supplier> spec, string companyName, Guid? excludeId, CancellationToken cancellationToken)
    {
        var existingSupplier = await _repository.GetAsync(spec, cancellationToken);

        if (existingSupplier != null)
        {
            throw new BusinessException(
                message: $"A supplier with company name '{companyName}' already exists.",
                userFriendlyMessage: $"'{companyName}' şirket adıyla bir tedarikçi zaten mevcut.",
                errorCode: "DUPLICATE_COMPANY_NAME"
            );
        }
    }

    public int Priority => 1;

    #region Private Specifications

    /// <summary>
    /// Şirket adına göre tedarikçi arama 
    /// </summary>
    private class CompanyNameExistsSpec : Specification<Supplier>
    {
        public CompanyNameExistsSpec(string companyName)
            : base(s => s.CompanyName.ToLower() == companyName.ToLower()) { }
    }

    /// <summary>
    /// Şirket adına göre arama (belirtilen ID hariç)
    /// </summary>
    private class CompanyNameExistsExcludingIdSpec : Specification<Supplier>
    {
        public CompanyNameExistsExcludingIdSpec(Guid excludeId, string companyName)
            : base(s => s.Id != excludeId && s.CompanyName.ToLower() == companyName.ToLower()) { }
    }

    #endregion
}