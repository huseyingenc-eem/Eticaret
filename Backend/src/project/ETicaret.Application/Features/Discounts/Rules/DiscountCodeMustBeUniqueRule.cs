using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Create;
using ETicaret.Application.Features.Discounts.Commands.Update;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Rules;

/// <summary>
/// Discount code'un benzersiz olması gerektiğini kontrol eder.
/// </summary>
public class DiscountCodeMustBeUniqueRule :
    IBusinessRule<CreateDiscountCommand>,
    IBusinessRule<UpdateDiscountCommand>
{
    private readonly IRepository<Discount, Guid> _repository;

    public DiscountCodeMustBeUniqueRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Discount, Guid>();
    }

    public bool ShouldExecute(CreateDiscountCommand command) => !string.IsNullOrWhiteSpace(command.DiscountCode);
    public bool ShouldExecute(UpdateDiscountCommand command) => !string.IsNullOrWhiteSpace(command.DiscountCode);

    public async Task ExecuteAsync(CreateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByDiscountCodeSpec(command.DiscountCode!);
        await ValidateUniqueness(spec, command.DiscountCode!, null, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByDiscountCodeExcludingIdSpec(command.Id, command.DiscountCode!);
        await ValidateUniqueness(spec, command.DiscountCode!, command.Id, cancellationToken);
    }

    private async Task ValidateUniqueness(Specification<Discount> spec, string discountCode, Guid? excludeId, CancellationToken cancellationToken)
    {
        var existingDiscount = await _repository.GetAsync(spec, cancellationToken);

        if (existingDiscount != null)
        {
            throw new BusinessException(
                message: $"A discount with code '{discountCode}' already exists.",
                userFriendlyMessage: $"'{discountCode}' kodlu bir indirim zaten mevcut.",
                errorCode: DiscountConstants.ErrorCodes.DuplicateDiscountCode
            );
        }
    }

    public int Priority => 1;

    #region Private Specifications

    private class ByDiscountCodeSpec : Specification<Discount>
    {
        public ByDiscountCodeSpec(string discountCode)
            : base(d => d.DiscountCode != null && d.DiscountCode.ToLower() == discountCode.ToLower()) { }
    }

    private class ByDiscountCodeExcludingIdSpec : Specification<Discount>
    {
        public ByDiscountCodeExcludingIdSpec(Guid excludeId, string discountCode)
            : base(d => d.Id != excludeId && d.DiscountCode != null && d.DiscountCode.ToLower() == discountCode.ToLower()) { }
    }

    #endregion
}