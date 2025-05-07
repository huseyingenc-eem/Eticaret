using FluentValidation;

namespace ETicaret.Application.Features.Roles.Commands.Delete;

public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Rol ID boş olamaz.");
    }
}