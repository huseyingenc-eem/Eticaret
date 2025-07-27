using FluentValidation;

namespace ETicaret.Application.Features.UserRoles.Queries.GetList;

/// <summary>
/// GetListUserRolesQuery için doğrulama kuralları.
/// </summary>
public class GetListUserRolesValidator : AbstractValidator<GetListUserRolesQuery>
{
    public GetListUserRolesValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Sayfa indeksi 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Arama terimi en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        RuleFor(x => x.RoleFilter)
            .MaximumLength(256).WithMessage("Rol filtresi en fazla 256 karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.RoleFilter));

        RuleFor(x => x.CityFilter)
            .MaximumLength(100).WithMessage("Şehir filtresi en fazla 100 karakter olabilir.")
            .When(x => !string.IsNullOrWhiteSpace(x.CityFilter));

        RuleFor(x => x.SortBy)
            .Must(BeValidSortField)
            .WithMessage("Geçerli sıralama alanları: Name, Email, CreatedDate, City");

        RuleFor(x => x.SortDirection)
            .Must(BeValidSortDirection)
            .WithMessage("Geçerli sıralama yönleri: asc, desc");
    }

    /// <summary>
    /// Sıralama alanının geçerli olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="sortBy">Sıralama alanı.</param>
    /// <returns>Geçerliyse true, değilse false.</returns>
    private static bool BeValidSortField(string sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return true; // Boşsa varsayılan değer kullanılır

        var validFields = new[] { "Name", "Email", "CreatedDate", "City" };
        return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Sıralama yönünün geçerli olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="sortDirection">Sıralama yönü.</param>
    /// <returns>Geçerliyse true, değilse false.</returns>
    private static bool BeValidSortDirection(string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
            return true; // Boşsa varsayılan değer kullanılır

        var validDirections = new[] { "asc", "desc" };
        return validDirections.Contains(sortDirection, StringComparer.OrdinalIgnoreCase);
    }
}