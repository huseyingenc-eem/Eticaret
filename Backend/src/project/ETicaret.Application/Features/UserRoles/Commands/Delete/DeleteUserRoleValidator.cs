using FluentValidation;

namespace ETicaret.Application.Features.UserRoles.Commands.Delete;

/// <summary>
/// DeleteUserRoleCommand için doğrulama kuralları.
/// </summary>
public class DeleteUserRoleValidator : AbstractValidator<DeleteUserRoleCommand>
{
    public DeleteUserRoleValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.")
            .Length(1, 450).WithMessage("Kullanıcı ID'si 1-450 karakter arasında olmalıdır.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Rol ID'si boş olamaz.")
            .Length(1, 450).WithMessage("Rol ID'si 1-450 karakter arasında olmalıdır.");
    }
}