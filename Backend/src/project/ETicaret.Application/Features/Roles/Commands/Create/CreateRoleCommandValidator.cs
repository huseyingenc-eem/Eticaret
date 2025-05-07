using FluentValidation;

namespace ETicaret.Application.Features.Roles.Commands.Create;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Rol adı boş olamaz.")
            .MinimumLength(2).WithMessage("Rol adı en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Rol adı en fazla 50 karakter olabilir.");
    }
}