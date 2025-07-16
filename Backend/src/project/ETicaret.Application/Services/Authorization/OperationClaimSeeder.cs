using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using System.Reflection;

namespace ETicaret.Application.Services.Authorization;

public class OperationClaimSeeder : IOperationClaimSeeder
{
    private readonly IOperationClaimRepository _operationClaimRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OperationClaimSeeder(IOperationClaimRepository operationClaimRepository, IUnitOfWork unitOfWork)
    {
        _operationClaimRepository = operationClaimRepository ?? throw new ArgumentNullException(nameof(operationClaimRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task SeedOperationClaimsAsync()
    {
        // Hedef assembly (Command/Query'lerin bulunduğu yer)
        var applicationAssembly = Assembly.GetAssembly(typeof(ETicaret.Application.Extensions)); // Application katmanındaki herhangi bir sınıf
        if (applicationAssembly == null)
        {
            Console.WriteLine("OperationClaimSeeder: Application Assembly bulunamadı.");
            return;
        }

        // IRequest arayüzünü implemente eden tüm public, concrete (abstract olmayan) tipleri bul
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


        // Mevcut OperationClaim'leri veritabanından çek
        // GetAllAsync metodu kullanılıyor (EfRepositoryBase'den)
        var existingClaims = await _operationClaimRepository.GetAllAsync(enableTracking: false);
        var existingOperationNames = new HashSet<string>(existingClaims.Select(c => c.OperationName));
        Console.WriteLine($"OperationClaimSeeder: Veritabanında {existingClaims.Count} adet mevcut yetki bulundu.");

        var newClaims = new List<OperationClaim>();

        foreach (var type in commandQueryTypes)
        {
            string operationName = type.Name;
            if (!existingOperationNames.Contains(operationName))
            {
                // Namespace'den FeatureName'i çıkarmaya çalışalım
                string featureName = ExtractFeatureName(type.Namespace);

                Console.WriteLine($"OperationClaimSeeder: Yeni operasyon ekleniyor - OperationName: {operationName}, FeatureName: {featureName}");
                newClaims.Add(new OperationClaim
                {
                    OperationName = operationName,
                    FeatureName = featureName,
                    RequiredRoles = "Admin",
                    CreatedTime = DateTime.UtcNow
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

        return defaultFeature; // Uygun yapı bulunamazsa varsayılanı dön
    }
}
