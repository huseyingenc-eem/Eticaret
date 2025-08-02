using FluentValidation;

namespace ETicaret.Application.Features.UserRoles.Commands.Update;

public class UpdateUserRolesValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.")
            .Length(1, 450).WithMessage("Kullanıcı ID'si 1-450 karakter arasında olmalıdır.");

        RuleFor(x => x)
            .Must(HaveAtLeastOneValidOperation)
            .WithMessage("En az bir geçerli rol işlemi (ekleme veya kaldırma) belirtilmelidir.");

        RuleFor(x => x.RolesToAdd)
            .Must(NotContainDuplicates)
            .WithMessage("Eklenecek roller listesinde tekrar eden rol ID'leri bulunmamalıdır.")
            .When(x => x.RolesToAdd != null);

        RuleFor(x => x.RolesToRemove)
            .Must(NotContainDuplicates)
            .WithMessage("Kaldırılacak roller listesinde tekrar eden rol ID'leri bulunmamalıdır.")
            .When(x => x.RolesToRemove != null);

        RuleFor(x => x)
            .Must(NotHaveConflictingOperations)
            .WithMessage("Aynı rol hem eklenecek hem de kaldırılacak listede bulunamaz.");
    }
    private static bool HaveAtLeastOneValidOperation(UpdateUserRolesCommand command)
    {
        bool hasValidRolesToAdd = command.RolesToAdd?.Any(id => !string.IsNullOrWhiteSpace(id)) ?? false;

        bool hasValidRolesToRemove = command.RolesToRemove?.Any(id => !string.IsNullOrWhiteSpace(id)) ?? false;

        return hasValidRolesToAdd || hasValidRolesToRemove;
    }

    private static bool NotContainDuplicates(List<string>? roleIds)
    {
        if (roleIds == null) return true;
        var validIds = roleIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
        if (!validIds.Any()) return true;

        return validIds.Count == validIds.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }
    private static bool NotHaveConflictingOperations(UpdateUserRolesCommand command)
    {
        if (command.RolesToAdd == null || command.RolesToRemove == null) return true;

        // Her iki listeden de sadece geçerli (boş olmayan) ID'leri al
        var validRolesToAdd = command.RolesToAdd.Where(id => !string.IsNullOrWhiteSpace(id));
        var validRolesToRemove = command.RolesToRemove.Where(id => !string.IsNullOrWhiteSpace(id));

        // Bu iki temizlenmiş listenin kesişimi var mı diye bak
        return !validRolesToAdd.Intersect(validRolesToRemove, StringComparer.OrdinalIgnoreCase).Any();
    }
}