using FluentValidation;

namespace ETicaret.Application.Features.UserRoles.Commands.Update;

/// <summary>
/// UpdateUserRolesCommand için doğrulama kuralları.
/// </summary>
public class UpdateUserRolesValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.")
            .Length(1, 450).WithMessage("Kullanıcı ID'si 1-450 karakter arasında olmalıdır.");

        RuleFor(x => x)
            .Must(HaveAtLeastOneOperation)
            .WithMessage("En az bir rol işlemi (ekleme veya kaldırma) belirtilmelidir.");

        RuleForEach(x => x.RolesToAdd)
            .NotEmpty().WithMessage("Eklenecek rol ID'si boş olamaz.")
            .When(x => x.RolesToAdd != null && x.RolesToAdd.Any());

        RuleForEach(x => x.RolesToRemove)
            .NotEmpty().WithMessage("Kaldırılacak rol ID'si boş olamaz.")
            .When(x => x.RolesToRemove != null && x.RolesToRemove.Any());

        RuleFor(x => x.RolesToAdd)
            .Must(NotContainDuplicates)
            .WithMessage("Eklenecek roller listesinde tekrar eden rol ID'leri bulunmamalıdır.")
            .When(x => x.RolesToAdd != null && x.RolesToAdd.Any());

        RuleFor(x => x.RolesToRemove)
            .Must(NotContainDuplicates)
            .WithMessage("Kaldırılacak roller listesinde tekrar eden rol ID'leri bulunmamalıdır.")
            .When(x => x.RolesToRemove != null && x.RolesToRemove.Any());

        RuleFor(x => x)
            .Must(NotHaveConflictingOperations)
            .WithMessage("Aynı rol hem eklenecek hem de kaldırılacak listede bulunamaz.");
    }

    /// <summary>
    /// En az bir rol işleminin (ekleme veya kaldırma) tanımlandığını kontrol eder.
    /// </summary>
    /// <param name="command">Doğrulanacak komut.</param>
    /// <returns>En az bir işlem varsa true, yoksa false.</returns>
    private static bool HaveAtLeastOneOperation(UpdateUserRolesCommand command)
    {
        bool hasRolesToAdd = command.RolesToAdd != null && command.RolesToAdd.Any();
        bool hasRolesToRemove = command.RolesToRemove != null && command.RolesToRemove.Any();

        return hasRolesToAdd || hasRolesToRemove;
    }

    /// <summary>
    /// Listede tekrar eden öğe olmadığını kontrol eder.
    /// </summary>
    /// <param name="roleIds">Kontrol edilecek rol ID'leri listesi.</param>
    /// <returns>Tekrar yoksa true, varsa false.</returns>
    private static bool NotContainDuplicates(List<string>? roleIds)
    {
        if (roleIds == null || !roleIds.Any())
            return true;

        return roleIds.Count == roleIds.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }

    /// <summary>
    /// Aynı rolün hem ekleme hem de kaldırma listesinde bulunmadığını kontrol eder.
    /// </summary>
    /// <param name="command">Doğrulanacak komut.</param>
    /// <returns>Çakışma yoksa true, varsa false.</returns>
    private static bool NotHaveConflictingOperations(UpdateUserRolesCommand command)
    {
        if (command.RolesToAdd == null || !command.RolesToAdd.Any() ||
            command.RolesToRemove == null || !command.RolesToRemove.Any())
        {
            return true;
        }

        // Aynı rol ID'sinin hem ekleme hem de kaldırma listesinde olup olmadığını kontrol et
        var conflictingRoles = command.RolesToAdd
            .Intersect(command.RolesToRemove, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return !conflictingRoles.Any();
    }
}