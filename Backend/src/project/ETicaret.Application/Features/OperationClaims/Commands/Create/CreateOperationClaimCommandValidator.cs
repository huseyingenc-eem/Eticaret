using FluentValidation;

namespace ETicaret.Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimCommandValidator : AbstractValidator<CreateOperationClaimCommand>
{
    public CreateOperationClaimCommandValidator()
    {
        RuleFor(x => x.OperationName)
            .NotEmpty().WithMessage("Operasyon adı zorunludur.")
            .MaximumLength(255).WithMessage("Operasyon adı 255 karakterden uzun olamaz.");

        RuleFor(x => x.FeatureName)
            .NotEmpty().WithMessage("Özellik adı zorunludur.")
            .MaximumLength(255).WithMessage("Özellik adı 255 karakterden uzun olamaz.");

        RuleFor(x => x.RequiredRoles)
            .NotEmpty().WithMessage("En az bir rol belirtilmelidir.")
            .MaximumLength(500).WithMessage("Rol listesi 500 karakterden uzun olamaz.");
    }
}