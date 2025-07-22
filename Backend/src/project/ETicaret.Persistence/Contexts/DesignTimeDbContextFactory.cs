using ETicaret.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ETicaret.Persistence.Contexts;

/// <summary>
/// Bu sınıf, Entity Framework Core'un tasarım zamanı araçlarının (Add-Migration, Update-Database vb.)
/// DbContext'i doğru yapılandırma ile (özellikle veritabanı bağlantı cümlesi) oluşturabilmesini sağlar.
/// Bu, `Program.cs`'teki servis yapılandırmasından bağımsız olarak çalışır.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BaseDBContexts>
{
    public BaseDBContexts CreateDbContext(string[] args)
    {
        // 1. appsettings.json dosyasını bulmak için Presentation katmanına ulaşıyoruz.
        // Bu yol, projenizin klasör yapısına göre ayarlanmalıdır.
        ConfigurationManager configurationManager = new();
        configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../project/ETicaret.Presentation"));
        configurationManager.AddJsonFile("appsettings.json");

        // 2. DbContext için seçenekleri oluşturuyoruz.
        var optionsBuilder = new DbContextOptionsBuilder<BaseDBContexts>();

        // 3. appsettings.json dosyasındaki "SqlConnection" adlı bağlantı cümlesini okuyoruz.
        var connectionString = configurationManager.GetConnectionString("SqlConnection");
        optionsBuilder.UseSqlServer(connectionString);

        // 4. Yapılandırılmış seçeneklerle DbContext'i oluşturup geri döndürüyoruz.
        return new BaseDBContexts(optionsBuilder.Options);
    }
}