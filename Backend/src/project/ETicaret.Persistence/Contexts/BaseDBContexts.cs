using Core.Domain.Entities;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ETicaret.Persistence.Contexts;

/// <summary>
/// Projenin ana veritabanı bağlam (DbContext) sınıfıdır.
/// Identity (kullanıcı yönetimi), varlık konfigürasyonları ve merkezi iş kurallarını
/// (zaman damgaları, geçici silme) yönetir.
/// </summary>
public class BaseDBContexts : IdentityDbContext<User, IdentityRole, string>
{
    #region Yapıcı Metot (Constructor)

    public BaseDBContexts(DbContextOptions<BaseDBContexts> options) : base(options) { }

    #endregion

    #region Model Yapılandırması (Model Configuration)

    /// <summary>
    /// Entity Framework model oluşturma sürecini yapılandırır.
    /// Entity konfigürasyonları ve ilişkileri burada tanımlanır.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity için benzersiz email indeksi
        modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();

        // Bu assembly (ETicaret.Persistence) içindeki tüm IEntityTypeConfiguration
        // arayüzünü uygulayan sınıfları bulur ve otomatik olarak uygular.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    #endregion

    #region Merkezi İş Kuralları (Central Business Rules)

    /// <summary>
    /// Değişiklikleri kaydetme işlemini üzerine yazar (override) ve merkezi iş kurallarını uygular.
    /// Bu metot, her SaveChangesAsync çağrısından önce otomatik olarak çalışır.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity.GetType().GetProperty("CreatedTime") != null ||
            e.Entity.GetType().GetProperty("UpdateTime") != null ||
            e.Entity is ISoftDeletable);

        foreach (var entry in entries)
        {
            #region Geçici Silme (Soft Delete) Yönetimi

            // Eğer bir varlık 'Deleted' olarak işaretlendiyse VE 'ISoftDeletable' arayüzünü uyguluyorsa:
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletable softDeletableEntity)
            {
                // 1. Durumu 'Deleted' yerine 'Modified' olarak değiştir. Bu, kaydın veritabanından silinmesini engeller.
                entry.State = EntityState.Modified;

                // 2. ISoftDeletable arayüzü üzerinden DeletedTime alanını şimdiki zaman (UTC) olarak ayarla.
                // Not: Bu property'nin 'set' erişiminin olması gerekir.
                var deletedTimeProperty = softDeletableEntity.GetType().GetProperty(nameof(ISoftDeletable.DeletedTime));
                if (deletedTimeProperty?.CanWrite == true)
                {
                    deletedTimeProperty.SetValue(softDeletableEntity, DateTime.UtcNow);
                }
            }

            #endregion

            #region Otomatik Zaman Damgası (Audit) Yönetimi

            // Eğer bir varlık yeni ekleniyorsa:
            // ✅ DÜZELTME: Added state için CreatedTime ata
            if (entry.State == EntityState.Added)
            {
                var createdTimeProperty = entry.Entity.GetType().GetProperty("CreatedTime");
                if (createdTimeProperty?.CanWrite == true)
                {
                    var currentValue = createdTimeProperty.GetValue(entry.Entity);
                    // Eğer değer default(DateTime) ise yeni değer ata
                    if (currentValue == null || (DateTime)currentValue == default(DateTime))
                    {
                        createdTimeProperty.SetValue(entry.Entity, DateTime.UtcNow);
                        Console.WriteLine($"✅ CreatedTime atandı: {entry.Entity.GetType().Name} - {DateTime.UtcNow}"); // ✅ Debug için
                    }
                }
            }

            // Eğer bir varlık güncelleniyorsa:
            if (entry.State == EntityState.Modified)
            {
                var updateTimeProperty = entry.Entity.GetType().GetProperty("UpdateTime");
                if (updateTimeProperty?.CanWrite == true)
                {
                    updateTimeProperty.SetValue(entry.Entity, DateTime.UtcNow);
                    Console.WriteLine($"✅ UpdateTime atandı: {entry.Entity.GetType().Name} - {DateTime.UtcNow}"); // ✅ Debug için
                }
            }

            #endregion
        }

        // Değişiklikler uygulandıktan sonra, orijinal SaveChangesAsync metodunu çağırarak işlemi tamamla.
        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion
}