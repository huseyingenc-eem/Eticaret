using ETicaret.Application.Services.Authorization;

namespace ETicaret.Presentation.Configuration.StartupTasks;

/// <summary>
/// Veritabanı seeding işlemlerini gerçekleştirir.
/// </summary>
public class DatabaseSeedingTask : IStartupTask
{
    public int Order => 1;

    public async Task ExecuteAsync(IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            var seeder = serviceProvider.GetRequiredService<IOperationClaimSeeder>();
            await seeder.SeedOperationClaimsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"OperationClaim seeding sırasında bir hata oluştu: {ex}");
        }
    }
}