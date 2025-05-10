using FluentValidation;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{

    public UpdateCategoryValidator()
    {
       

        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("Kategori ID'si geçerli bir değer olmalıdır.");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");

        RuleFor(c => c.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(c => !string.IsNullOrEmpty(c.Description));
    }
}