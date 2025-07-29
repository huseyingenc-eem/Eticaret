using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Constants;
using Core.Domain.Exceptions;
using ETicaret.Application.Features.OperationClaims.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.OperationClaims.Rules;

/// <summary>
/// OperationClaim entity'si için iş kurallarını içeren sınıflar.
/// Bu kurallar MediatR pipeline behavior'ı tarafından otomatik olarak çalıştırılır.
/// Her kural IRule<TCommand> interface'ini implement eder.
/// </summary>

#region Operation Name Unique Rule

/// <summary>
/// Operasyon adının benzersiz olmasını kontrol eden kural.
/// </summary>
public class OperationNameMustBeUniqueRule<TCommand> : IRule<TCommand>
    where TCommand : class
{
    public int Priority => 1;
    public string RuleName => nameof(OperationNameMustBeUniqueRule<TCommand>);

    private readonly IRepository<OperationClaim, int> _operationClaimRepository;

    public OperationNameMustBeUniqueRule(IUnitOfWork unitOfWork)
    {
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
    }

    public async Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var operationNameProperty = typeof(TCommand).GetProperty("OperationName");
        var excludeIdProperty = typeof(TCommand).GetProperty("Id");

        if (operationNameProperty == null)
            return;

        var operationName = operationNameProperty.GetValue(command)?.ToString();
        if (string.IsNullOrWhiteSpace(operationName))
            return;

        var excludeId = excludeIdProperty?.GetValue(command) as int?;

        var spec = new OperationClaimSpecifications.ByOperationName(operationName);
        var existingClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (existingClaim != null && existingClaim.Id != excludeId)
        {
            throw new DomainException(
                message: $"Operation name '{operationName}' already exists.",
                errorCode: ApplicationErrorCodes.OperationClaim.OperationNameNotUnique,
                userFriendlyMessage: $"'{operationName}' adlı operasyon zaten mevcut.",
                details: new { OperationName = operationName }
            );
        }
    }
}

#endregion

#region Operation Name Required Rule

/// <summary>
/// Operasyon adının boş olmamasını kontrol eden kural.
/// </summary>
public class OperationNameMustNotBeEmptyRule<TCommand> : IRule<TCommand>
    where TCommand : class
{
    public int Priority => 0;
    public string RuleName => nameof(OperationNameMustNotBeEmptyRule<TCommand>);

    public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var operationNameProperty = typeof(TCommand).GetProperty("OperationName");
        if (operationNameProperty == null)
            return Task.CompletedTask;

        var operationName = operationNameProperty.GetValue(command)?.ToString();

        if (string.IsNullOrWhiteSpace(operationName))
        {
            throw new DomainException(
                message: "Operation name cannot be empty.",
                errorCode: ApplicationErrorCodes.OperationClaim.OperationNameRequired,
                userFriendlyMessage: "Operasyon adı zorunludur."
            );
        }

        return Task.CompletedTask;
    }
}

#endregion

#region Feature Name Required Rule

/// <summary>
/// Feature adının boş olmamasını kontrol eden kural.
/// </summary>
public class FeatureNameMustNotBeEmptyRule<TCommand> : IRule<TCommand>
    where TCommand : class
{
    public int Priority => 0;
    public string RuleName => nameof(FeatureNameMustNotBeEmptyRule<TCommand>);

    public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var featureNameProperty = typeof(TCommand).GetProperty("FeatureName");
        if (featureNameProperty == null)
            return Task.CompletedTask;

        var featureName = featureNameProperty.GetValue(command)?.ToString();

        if (string.IsNullOrWhiteSpace(featureName))
        {
            throw new DomainException(
                message: "Feature name cannot be empty.",
                errorCode: ApplicationErrorCodes.OperationClaim.FeatureNameRequired,
                userFriendlyMessage: "Özellik adı zorunludur."
            );
        }

        return Task.CompletedTask;
    }
}

#endregion

#region Required Roles Valid Rule

/// <summary>
/// Gerekli rollerin geçerli formatta olmasını kontrol eden kural.
/// </summary>
public class RequiredRolesMustBeValidRule<TCommand> : IRule<TCommand>
    where TCommand : class
{
    public int Priority => 2;
    public string RuleName => nameof(RequiredRolesMustBeValidRule<TCommand>);

    public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var requiredRolesProperty = typeof(TCommand).GetProperty("RequiredRoles");
        if (requiredRolesProperty == null)
            return Task.CompletedTask;

        var requiredRoles = requiredRolesProperty.GetValue(command)?.ToString();

        if (string.IsNullOrWhiteSpace(requiredRoles))
        {
            throw new DomainException(
                message: "Required roles cannot be empty.",
                errorCode: ApplicationErrorCodes.OperationClaim.RequiredRolesRequired,
                userFriendlyMessage: "En az bir rol belirtilmelidir."
            );
        }

        // Rollerin virgül veya noktalı virgülle ayrılmış olmasını kontrol et
        var roles = requiredRoles.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (!roles.Any())
        {
            throw new DomainException(
                message: "Required roles must contain at least one valid role.",
                errorCode: ApplicationErrorCodes.OperationClaim.RequiredRolesInvalid,
                userFriendlyMessage: "En az bir geçerli rol belirtilmelidir."
            );
        }

        // Her rolün boş olmamasını kontrol et
        foreach (var role in roles)
        {
            if (string.IsNullOrWhiteSpace(role.Trim()))
            {
                throw new DomainException(
                    message: "Required roles cannot contain empty values.",
                    errorCode: ApplicationErrorCodes.OperationClaim.RequiredRolesContainsEmpty,
                    userFriendlyMessage: "Roller listesi boş değerler içeremez."
                );
            }
        }

        return Task.CompletedTask;
    }
}

#endregion

#region Operation Claim Must Exist Rule

/// <summary>
/// OperationClaim'in var olduğunu kontrol eden kural.
/// </summary>
public class OperationClaimMustExistRule<TCommand> : IRule<TCommand>
    where TCommand : class
{
    public int Priority => 0;
    public string RuleName => nameof(OperationClaimMustExistRule<TCommand>);

    private readonly IRepository<OperationClaim, int> _operationClaimRepository;

    public OperationClaimMustExistRule(IUnitOfWork unitOfWork)
    {
        _operationClaimRepository = unitOfWork.GetRepository<OperationClaim, int>();
    }

    public async Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var idProperty = typeof(TCommand).GetProperty("Id");
        if (idProperty == null)
            return;

        var id = idProperty.GetValue(command);
        if (id is not int operationClaimId)
            return;

        var spec = new OperationClaimSpecifications.ById(operationClaimId);
        var operationClaim = await _operationClaimRepository.GetAsync(spec, cancellationToken);

        if (operationClaim == null)
        {
            throw new DomainException(
                message: $"OperationClaim with ID {operationClaimId} not found.",
                errorCode: ApplicationErrorCodes.OperationClaim.NotFound,
                userFriendlyMessage: "Operasyon yetkisi bulunamadı.",
                details: new { OperationClaimId = operationClaimId }
            );
        }
    }
}

#endregion