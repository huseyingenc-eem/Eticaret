using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Create;
using ETicaret.Application.Features.Discounts.Commands.Update;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Rules;

/// <summary>
/// Discount value'nun tipine göre geçerli olduğunu kontrol eden rule.
/// </summary>
public class DiscountValueValidationRule :
    IBusinessRule<CreateDiscountCommand>,
    IBusinessRule<UpdateDiscountCommand>
{
    public bool ShouldExecute(CreateDiscountCommand command) => true;
    public bool ShouldExecute(UpdateDiscountCommand command) => true;

    public Task ExecuteAsync(CreateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDiscountValue(command.DiscountType, command.DiscountValue);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDiscountValue(command.DiscountType, command.DiscountValue);
        return Task.CompletedTask;
    }

    private static void ValidateDiscountValue(DiscountType discountType, decimal discountValue)
    {
        // Value must be positive
        if (discountValue <= DiscountConstants.Validation.MinDiscountValue)
        {
            throw new BusinessException(
                message: "Discount value must be greater than 0.",
                userFriendlyMessage: "İndirim değeri 0'dan büyük olmalıdır.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDiscountValue
            );
        }

        if (discountType == DiscountType.Percentage &&
            discountValue > DiscountConstants.Validation.MaxPercentageValue)
        {
            throw new BusinessException(
                message: "Percentage discount cannot exceed 100%.",
                userFriendlyMessage: "Yüzdelik indirim %100'ü geçemez.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDiscountValue
            );
        }

        if (discountType == DiscountType.FixedAmount && discountValue > 1000000)
        {
            throw new BusinessException(
                message: "Fixed amount discount cannot exceed 1,000,000.",
                userFriendlyMessage: "Sabit tutar indirimi 1.000.000'u geçemez.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDiscountValue
            );
        }
    }

    public int Priority => 3;
}