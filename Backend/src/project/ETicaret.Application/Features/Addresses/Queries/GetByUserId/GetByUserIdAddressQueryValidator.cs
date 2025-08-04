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

    }
}