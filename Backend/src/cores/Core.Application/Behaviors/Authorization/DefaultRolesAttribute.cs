namespace Core.Application.Behaviors.Authorization;

/// <summary>
/// Bir Command veya Query'nin veritabanına ilk kez eklenirken
/// sahip olması gereken varsayılan rolleri belirtir.
/// OperationClaimSeeder bu attribute'ü okuyarak RequiredRoles alanını doldurur.
/// 
/// ÖNEMLI: Bu attribute sadece seeding (veritabanına ilk ekleme) sırasında kullanılır.
/// Bir kere veritabanına eklendikten sonra yetki yönetimi tamamen dinamik olarak
/// admin paneli üzerinden yapılır.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DefaultRolesAttribute : Attribute
{
    public string[] Roles { get; }
    public DefaultRolesAttribute(params string[] roles)
    {
        // Eğer hiç rol verilmezse, "Admin" varsayılan olarak kalsın
        Roles = roles.Any() ? roles : new[] { "Admin" };
    }
}