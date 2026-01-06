using FluentValidation;

namespace ETicaret.Application.Features.ProductVariants.Commands.Delete;

public class DeleteProductVariantCommandValidator : AbstractValidator<DeleteProductVariantCommand>
{
    public DeleteProductVariantCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ürün varyantı ID'si zorunludur.")
            .NotEqual(Guid.Empty).WithMessage("Geçerli bir ürün varyantı ID'si giriniz.");
    }
}