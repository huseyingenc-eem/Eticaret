using FluentValidation;

namespace ETicaret.Application.Features.UserRoles.Commands.Create;

public class CreateUserRolesValidator : AbstractValidator<CreateUserRolesCommand>
{
    public CreateUserRolesValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("Kullanıcı kimliği (UserId) boş olamaz.")
            .Must(BeAValidGuidString).WithMessage("Kullanıcı kimliği geçerli bir formatta olmalıdır.")
            .When(c => !string.IsNullOrEmpty(c.UserId));

        RuleFor(c => c.RoleId)
            .NotEmpty().WithMessage("Rol kimliği (RoleId) boş olamaz.")
            .Must(BeAValidGuidString).WithMessage("Rol kimliği geçerli bir formatta olmalıdır.")
            .When(c => !string.IsNullOrEmpty(c.RoleId));
    }
     private bool BeAValidGuidString(string? idString)
    {
        if (string.IsNullOrWhiteSpace(idString))
        {
            return false;
        }
        return Guid.TryParse(idString, out _);
    }
}