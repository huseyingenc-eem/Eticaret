using FluentValidation;

namespace ETicaret.Application.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Rol ID boş olamaz.");

        RuleFor(c => c.NewName)
            .NotEmpty().WithMessage("Yeni rol adı boş olamaz.")
            .MinimumLength(2).WithMessage("Yeni rol adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Yeni rol adı en fazla 50 karakter olabilir.");
    }
}