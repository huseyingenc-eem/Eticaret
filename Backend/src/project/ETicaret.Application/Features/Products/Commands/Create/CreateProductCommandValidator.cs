using FluentValidation;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ürün adı boş olamaz.")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(p => p.Price)
            .NotEmpty().WithMessage("Fiyat boş olamaz.")
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(p => p.Stock)
            .NotEmpty().WithMessage("Stok miktarı boş olamaz.")
            .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı 0 veya daha büyük olmalıdır."); 

        RuleFor(p => p.CategoryID)
            .NotEmpty().WithMessage("Kategori ID boş olamaz.")
            .GreaterThan(0).WithMessage("Geçerli bir Kategori ID girilmelidir.");

        RuleFor(p => p.SupplierID)
            .NotEmpty().WithMessage("Tedarikçi ID boş olamaz.")
            .GreaterThan(0).WithMessage("Geçerli bir Tedarikçi ID girilmelidir.");

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir."); 

        RuleFor(p => p.SKU)
            .MaximumLength(100).WithMessage("SKU en fazla 100 karakter olabilir."); 

        RuleFor(p => p.ImageUrl)
            .MaximumLength(500).WithMessage("Resim URL'si en fazla 500 karakter olabilir.");
    }
}