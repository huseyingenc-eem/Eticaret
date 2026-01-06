using FluentValidation;
using ETicaret.Application.Features.ProductVariants.Constants;

namespace ETicaret.Application.Features.ProductVariants.Commands.Create;

public class CreateProductVariantCommandValidator : AbstractValidator<CreateProductVariantCommand>
{
    public CreateProductVariantCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("Ürün ID'si zorunludur.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU zorunludur.")
            .MinimumLength(ProductVariantConstants.Validation.MinSkuLength)
            .WithMessage($"SKU en az {ProductVariantConstants.Validation.MinSkuLength} karakter olmalıdır.")
            .MaximumLength(ProductVariantConstants.Validation.MaxSkuLength)
            .WithMessage($"SKU en fazla {ProductVariantConstants.Validation.MaxSkuLength} karakter olabilir.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(ProductVariantConstants.Validation.MinPrice)
            .WithMessage($"Fiyat en az {ProductVariantConstants.Validation.MinPrice:C} olmalıdır.")
            .LessThanOrEqualTo(ProductVariantConstants.Validation.MaxPrice)
            .WithMessage($"Fiyat en fazla {ProductVariantConstants.Validation.MaxPrice:C} olabilir.");

        RuleFor(x => x.CompareAtPrice)
            .GreaterThan(x => x.Price)
            .WithMessage("Karşılaştırma fiyatı normal fiyattan yüksek olmalıdır.")
            .When(x => x.CompareAtPrice.HasValue);

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo(ProductVariantConstants.Validation.MinStock)
            .WithMessage($"Stok miktarı en az {ProductVariantConstants.Validation.MinStock} olmalıdır.")
            .LessThanOrEqualTo(ProductVariantConstants.Validation.MaxStock)
            .WithMessage($"Stok miktarı en fazla {ProductVariantConstants.Validation.MaxStock} olabilir.");

        RuleFor(x => x.VariantImageUrl)
            .MaximumLength(ProductVariantConstants.Validation.MaxVariantImageUrlLength)
            .WithMessage($"Varyant resim URL'si en fazla {ProductVariantConstants.Validation.MaxVariantImageUrlLength} karakter olabilir.")
            .Must(BeValidUrl)
            .WithMessage("Geçerli bir URL formatı giriniz.")
            .When(x => !string.IsNullOrWhiteSpace(x.VariantImageUrl));

        RuleFor(x => x.AttributeDescription)
            .MaximumLength(ProductVariantConstants.Validation.MaxAttributeDescriptionLength)
            .WithMessage($"Özellik açıklaması en fazla {ProductVariantConstants.Validation.MaxAttributeDescriptionLength} karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.AttributeDescription));
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}