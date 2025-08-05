using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Create;
using ETicaret.Application.Features.Discounts.Commands.Update;
using ETicaret.Application.Features.Discounts.Constants;

namespace ETicaret.Application.Features.Discounts.Rules;

/// <summary>
/// Discount tarih aralığının geçerli olduğunu kontrol eden rule.
/// </summary>
public class DiscountDateValidationRule :
    IBusinessRule<CreateDiscountCommand>,
    IBusinessRule<UpdateDiscountCommand>
{
    public bool ShouldExecute(CreateDiscountCommand command) => true;
    public bool ShouldExecute(UpdateDiscountCommand command) => true;

    public Task ExecuteAsync(CreateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDateRange(command.StartDate, command.EndDate);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDateRange(command.StartDate, command.EndDate);
        return Task.CompletedTask;
    }

    private static void ValidateDateRange(DateTime startDate, DateTime? endDate)
    {
        if (startDate < DateTime.UtcNow.AddDays(-1))
        {
            throw new BusinessException(
                message: "Discount start date cannot be in the past.",
                userFriendlyMessage: "İndirim başlangıç tarihi En fazla bir gün geçmişte olabilir.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDateRange
            );
        }

        if (endDate.HasValue && endDate.Value <= startDate)
        {
            throw new BusinessException(
                message: "Discount end date must be after start date.",
                userFriendlyMessage: "İndirim bitiş tarihi başlangıç tarihinden sonra olmalıdır.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDateRange
            );
        }

        if (endDate.HasValue && endDate.Value > DateTime.UtcNow.AddYears(2))
        {
            throw new BusinessException(
                message: "Discount end date cannot be more than 2 years in the future.",
                userFriendlyMessage: "İndirim bitiş tarihi 2 yıldan fazla ileri olamaz.",
                errorCode: DiscountConstants.ErrorCodes.InvalidDateRange
            );
        }
    }

    public int Priority => 2;
}