using FluentValidation;

namespace ETicaret.Application.Features.Addresses.Queries.GetByUserId;

/// <summary>
/// GetByUserIdAddressQuery için doğrulama kuralları.
/// </summary>
public class GetByUserIdAddressQueryValidator : AbstractValidator<GetByUserIdAddressQuery>
{
    public GetByUserIdAddressQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.")
            .NotNull().WithMessage("Kullanıcı ID'si null olamaz.");

        RuleFor(x => x.PageIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Sayfa indeksi 0 veya daha büyük olmalıdır.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");
    }
}