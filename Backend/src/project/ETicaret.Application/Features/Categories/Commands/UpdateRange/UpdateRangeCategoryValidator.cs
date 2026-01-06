using FluentValidation;

namespace ETicaret.Application.Features.Categories.Commands.UpdateRange;

public class UpdateRangeCategoryValidator : AbstractValidator<UpdateRangeCategoryCommand>
{
    public UpdateRangeCategoryValidator()
    {
        RuleFor(x => x.Categories)
            .NotEmpty().WithMessage("En az bir kategori belirtilmelidir.")
            .Must(HaveMaximumAllowedItems).WithMessage("En fazla 20 kategori aynı anda eklenebilir.");

        RuleFor(x => x.ParentId)
            .GreaterThan(0).WithMessage("Parent kategori ID'si geçerli bir değer olmalıdır.")
            .When(x => x.ParentId.HasValue);

        RuleForEach(x => x.Categories).SetValidator(new CategoryUpdateItemValidator());

        RuleFor(x => x.Categories)
            .Must(NotHaveDuplicateNames).WithMessage("Aynı isimde birden fazla kategori eklenemez.");
    }

    private static bool HaveMaximumAllowedItems(List<CategoryUpdateItem> categories)
    {
        return categories.Count <= 20;
    }

    private static bool NotHaveDuplicateNames(List<CategoryUpdateItem> categories)
    {
        if (categories == null || !categories.Any()) return true;

        var names = categories.Select(c => c.Name.Trim().ToLowerInvariant()).ToList();
        return names.Count == names.Distinct().Count();
    }
}

public class CategoryUpdateItemValidator : AbstractValidator<CategoryUpdateItem>
{
    public CategoryUpdateItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}