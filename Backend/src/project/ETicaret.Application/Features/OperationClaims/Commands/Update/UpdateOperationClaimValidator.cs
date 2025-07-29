using Core.Application.Common.Constants;
using FluentValidation;

namespace ETicaret.Application.Features.OperationClaims.Commands.Update;

/// <summary>
/// UpdateOperationClaimCommand için doğrulama kuralları.
/// FluentValidation kullanarak input validation yapar.
/// </summary>
public class UpdateOperationClaimValidator : AbstractValidator<UpdateOperationClaimCommand>
{
    public UpdateOperationClaimValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Operasyon yetkisi ID'si geçerli bir değer olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.OperationClaim.NotFound);

        RuleFor(x => x.OperationName)
            .NotEmpty()
            .WithMessage("Operasyon adı boş olamaz.")
            .WithErrorCode(ApplicationErrorCodes.OperationClaim.OperationNameRequired)
            .Length(1, 255)
            .WithMessage("Operasyon adı 1-255 karakter arasında olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.ValidationError);

        RuleFor(x => x.FeatureName)
            .NotEmpty()
            .WithMessage("Feature adı boş olamaz.")
            .WithErrorCode(ApplicationErrorCodes.OperationClaim.FeatureNameRequired)
            .Length(1, 100)
            .WithMessage("Feature adı 1-100 karakter arasında olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.ValidationError);

        RuleFor(x => x.RequiredRoles)
            .NotEmpty()
            .WithMessage("Gerekli roller belirtilmelidir.")
            .WithErrorCode(ApplicationErrorCodes.OperationClaim.RequiredRolesRequired)
            .Must(BeValidRoleFormat)
            .WithMessage("Roller geçerli formatta olmalıdır (örn: Admin,User veya Admin;User).")
            .WithErrorCode(ApplicationErrorCodes.OperationClaim.RequiredRolesInvalid);
    }

    /// <summary>
    /// Rollerin geçerli formatta olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="requiredRoles">Kontrol edilecek roller string'i</param>
    /// <returns>Geçerli format ise true, değilse false</returns>
    private static bool BeValidRoleFormat(string requiredRoles)
    {
        if (string.IsNullOrWhiteSpace(requiredRoles))
            return false;

        var roles = requiredRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        return roles.Length > 0 && roles.All(role => !string.IsNullOrWhiteSpace(role.Trim()));
    }
}