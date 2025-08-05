using FluentValidation;
using ETicaret.Application.Features.Products.Constants;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MinimumLength(ProductConstants.Validation.MinNameLength)
            .WithMessage($"Ürün adı en az {ProductConstants.Validation.MinNameLength} karakter olmalıdır.")
            .MaximumLength(ProductConstants.Validation.MaxNameLength)
            .WithMessage($"Ürün adı {ProductConstants.Validation.MaxNameLength} karakterden uzun olamaz.");

        RuleFor(x => x.Description)
            .MaximumLength(ProductConstants.Validation.MaxDescriptionLength)
            .WithMessage($"Açıklama {ProductConstants.Validation.MaxDescriptionLength} karakterden uzun olamaz.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Geçerli bir kategori seçiniz.");

        RuleFor(x => x.SupplierId)
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir tedarikçi seçiniz.")
            .When(x => x.SupplierId.HasValue);
    }
}