using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Application.Features.OperationClaims.Specifications;
using MediatR;
using System.Reflection;

namespace ETicaret.Application.Services.Authorization;

public class OperationClaimSeeder : IOperationClaimSeeder
{
    private readonly IRepository<OperationClaim, int> _operationClaimRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OperationClaimSeeder(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _operationClaimRepository = _unitOfWork.GetRepository<OperationClaim, int>();
    }

    public async Task SeedOperationClaimsAsync()
    {
        var applicationAssembly = Assembly.GetAssembly(typeof(ETicaret.Application.ApplicationServiceRegistration));
        if (applicationAssembly == null)
        {
            Console.WriteLine("OperationClaimSeeder: Application Assembly bulunamadı.");
            return;
        }

        var commandQueryTypes = applicationAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic &&
                         (t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)) ||
                          t.GetInterfaces().Contains(typeof(IRequest))))
            .ToList();

        if (!commandQueryTypes.Any())
        {
            Console.WriteLine("OperationClaimSeeder: IRequest implementasyonu bulunamadı.");
            return;
        }
        Console.WriteLine($"OperationClaimSeeder: {commandQueryTypes.Count} adet IRequest implementasyonu bulundu.");

        var spec = new OperationClaimSpecifications.All();
        var existingClaims = await _operationClaimRepository.GetListAsync(spec);
        var existingOperationNames = new HashSet<string>(existingClaims.Select(c => c.OperationName));
        Console.WriteLine($"OperationClaimSeeder: Veritabanında {existingClaims.Count} adet mevcut yetki bulundu.");

        var newClaims = new List<OperationClaim>();

        foreach (var type in commandQueryTypes)
        {
            string operationName = type.Name;
            if (!existingOperationNames.Contains(operationName))
            {
                string featureName = ExtractFeatureName(type.Namespace);
                Console.WriteLine($"OperationClaimSeeder: Yeni operasyon ekleniyor - OperationName: {operationName}, FeatureName: {featureName}");

                newClaims.Add(new OperationClaim
                {
                    OperationName = operationName,
                    FeatureName = featureName,
                    RequiredRoles = "Admin"
                });
            }
        }

        if (newClaims.Any())
        {
            await _operationClaimRepository.AddRangeAsync(newClaims);
            await _unitOfWork.CompleteAsync();
            Console.WriteLine($"OperationClaimSeeder: {newClaims.Count} yeni operasyon yetkisi başarıyla eklendi.");
        }
        else
        {
            Console.WriteLine("OperationClaimSeeder: Eklenecek yeni operasyon yetkisi bulunamadı, tablo güncel.");
        }
    }

    private string ExtractFeatureName(string? typeNamespace)
    {
        const string defaultFeature = "Default";
        if (string.IsNullOrEmpty(typeNamespace))
        {
            return defaultFeature;
        }

        var namespaceParts = typeNamespace.Split('.');
        int featuresIndex = Array.IndexOf(namespaceParts, "Features");
        if (featuresIndex != -1 && featuresIndex + 1 < namespaceParts.Length)
        {
            return namespaceParts[featuresIndex + 1];
        }

        return defaultFeature;
    }
}