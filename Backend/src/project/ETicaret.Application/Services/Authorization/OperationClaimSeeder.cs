using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using ETicaret.Domain.Entities;
using ETicaret.Application.Features.OperationClaims.Specifications;
using MediatR;
using System.Reflection;
using ETicaret.Application.Services.Repositories;

namespace ETicaret.Application.Services.Authorization;

public class OperationClaimSeeder : IOperationClaimSeeder
{
    #region Fields

    private readonly IOperationClaimRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor

    public OperationClaimSeeder(IOperationClaimRepository operationClaimSeeder, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = operationClaimSeeder;
    }

    #endregion

    #region Public Methods

    public async Task SeedOperationClaimsAsync()
    {
        var commandQueryTypes = GetCommandAndQueryTypes();
        if (!commandQueryTypes.Any()) return;

        var existingClaims = (await _repository.GetListAsync(new OperationClaimSpecifications.All())).ToDictionary(c => c.OperationName);
        Console.WriteLine($"OperationClaimSeeder: Veritabanında {existingClaims.Count} adet mevcut yetki bulundu.");

        var claimsToAdd = new List<OperationClaim>();
        var claimsToUpdate = new List<OperationClaim>();

        foreach (var type in commandQueryTypes)
        {
            string operationName = type.Name;
            string defaultRoles = DetermineRequiredRoles(type);

            if (existingClaims.TryGetValue(operationName, out var existingClaim))
            {
                await ProcessExistingClaimSafely(existingClaim, defaultRoles, claimsToUpdate);
            }
            else
            {
                Console.WriteLine($"OperationClaimSeeder: Yeni yetki ekleniyor -> OperationName: {operationName}, Roller: '{defaultRoles}'");
                claimsToAdd.Add(new OperationClaim
                {
                    OperationName = operationName,
                    FeatureName = ExtractFeatureName(type.Namespace),
                    RequiredRoles = defaultRoles
                });
            }
        }

        await SaveChangesAsync(claimsToAdd, claimsToUpdate);
    }

    #endregion

    #region Private Helper Methods

    private List<Type> GetCommandAndQueryTypes()
    {
        var applicationAssembly = Assembly.GetAssembly(typeof(ApplicationServiceRegistration));
        if (applicationAssembly == null) return new List<Type>();

        return applicationAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic &&
                         (t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) ||
                          t.GetInterfaces().Contains(typeof(IRequest))))
            .ToList();
    }

    private string DetermineRequiredRoles(Type type)
    {
        if (type.GetInterfaces().Contains(typeof(IPublicRequest)))
        {
            return string.Empty;
        }

        var defaultRolesAttribute = type.GetCustomAttribute<DefaultRolesAttribute>();
        if (defaultRolesAttribute != null)
        {
            return string.Join(",", defaultRolesAttribute.Roles);
        }

        return GetSmartDefaultRoles(type.Name);
    }
    private async Task ProcessExistingClaimSafely(OperationClaim existingClaim, string defaultRoles, List<OperationClaim> claimsToUpdate)
    {
        var existingRoles = ParseRoles(existingClaim.RequiredRoles);
        var defaultRolesList = ParseRoles(defaultRoles);

        var missingRoles = defaultRolesList.Except(existingRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (missingRoles.Any())
        {
            var allRoles = existingRoles.Concat(missingRoles).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(r => r);
            var newRequiredRoles = string.Join(",", allRoles);

            Console.WriteLine($"OperationClaimSeeder: '{existingClaim.OperationName}' için eksik roller ekleniyor:");
            Console.WriteLine($"  Mevcut Roller: [{existingClaim.RequiredRoles}]");
            Console.WriteLine($"  Eksik Roller: [{string.Join(",", missingRoles)}]");
            Console.WriteLine($"  YENİ Roller: [{newRequiredRoles}]");

            existingClaim.RequiredRoles = newRequiredRoles;
            claimsToUpdate.Add(existingClaim);
        }
    }
    private static List<string> ParseRoles(string rolesString)
    {
        if (string.IsNullOrWhiteSpace(rolesString))
            return new List<string>();

        return rolesString
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(role => role.Trim())
            .Where(role => !string.IsNullOrEmpty(role))
            .ToList();
    }

    private async Task SaveChangesAsync(List<OperationClaim> claimsToAdd, List<OperationClaim> claimsToUpdate)
    {
        if (claimsToAdd.Any())
        {
            await _repository.AddRangeAsync(claimsToAdd);
            Console.WriteLine($"OperationClaimSeeder: {claimsToAdd.Count} yeni operasyon yetkisi başarıyla eklendi.");
        }

        if (claimsToUpdate.Any())
        {
            await _repository.UpdateRangeAsync(claimsToUpdate);
            Console.WriteLine($"OperationClaimSeeder: {claimsToUpdate.Count} mevcut operasyon yetkisi güvenli şekilde güncellendi.");
        }

        if (claimsToAdd.Any() || claimsToUpdate.Any())
        {
            await _unitOfWork.CompleteAsync();
        }
        else
        {
            Console.WriteLine("OperationClaimSeeder: Eklenecek veya güncellenecek yetki bulunamadı.");
        }
    }

    private string ExtractFeatureName(string? typeNamespace)
    {
        const string defaultFeature = "Default";
        if (string.IsNullOrEmpty(typeNamespace)) return defaultFeature;

        var namespaceParts = typeNamespace.Split('.');
        int featuresIndex = Array.IndexOf(namespaceParts, "Features");
        if (featuresIndex != -1 && featuresIndex + 1 < namespaceParts.Length)
        {
            return namespaceParts[featuresIndex + 1];
        }
        return defaultFeature;
    }

    #endregion

    #region Default Role Assignment Logic
    private static string GetSmartDefaultRoles(string operationName)
    {
        return "Admin";

        #region Commented Smart Logic - Manuel Aktivasyon Gerekiyor

        #endregion
    }

    #endregion
}