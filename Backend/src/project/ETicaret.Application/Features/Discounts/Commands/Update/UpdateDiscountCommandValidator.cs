using FluentValidation;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Commands.Update;

public class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("İndirim ID'si zorunludur.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("İndirim adı zorunludur.")
            .MaximumLength(DiscountConstants.Validation.MaxNameLength)
            .WithMessage($"İndirim adı {DiscountConstants.Validation.MaxNameLength} karakterden uzun olamaz.");

        RuleFor(x => x.DiscountCode)
            .MaximumLength(DiscountConstants.Validation.MaxDiscountCodeLength)
            .WithMessage($"İndirim kodu {DiscountConstants.Validation.MaxDiscountCodeLength} karakterden uzun olamaz.")
            .When(x => !string.IsNullOrWhiteSpace(x.DiscountCode));

        RuleFor(x => x.Description)
            .MaximumLength(DiscountConstants.Validation.MaxDescriptionLength)
            .WithMessage($"Açıklama {DiscountConstants.Validation.MaxDescriptionLength} karakterden uzun olamaz.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.DiscountType)
            .IsInEnum().WithMessage("Geçerli bir indirim tipi seçiniz.");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(DiscountConstants.Validation.MinDiscountValue)
            .WithMessage("İndirim değeri 0'dan büyük olmalıdır.");

        RuleFor(x => x.MinimumPurchaseAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum satın alma tutarı 0'dan küçük olamaz.")
            .When(x => x.MinimumPurchaseAmount.HasValue);

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Başlangıç tarihi zorunludur.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.MaxUses)
            .GreaterThan(0)
            .WithMessage("Maksimum kullanım sayısı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(DiscountConstants.Validation.MaxUsesLimit)
            .WithMessage($"Maksimum kullanım sayısı {DiscountConstants.Validation.MaxUsesLimit}'den fazla olamaz.")
            .When(x => x.MaxUses.HasValue);

        RuleFor(x => x.MaxUsesPerUser)
            .GreaterThan(0)
            .WithMessage("Kullanıcı başına maksimum kullanım sayısı 0'dan büyük olmalıdır.")
            .LessThanOrEqualTo(x => x.MaxUses ?? int.MaxValue)
            .WithMessage("Kullanıcı başına maksimum kullanım sayısı, toplam maksimum kullanım sayısından fazla olamaz.")
            .When(x => x.MaxUsesPerUser.HasValue);
    }
}