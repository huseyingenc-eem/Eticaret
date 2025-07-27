namespace Core.Application.Behaviors.Rules;

/// <summary>
/// Bir kuralın hangi komutlarda çalışıp çalışmayacağını belirleyen attribute.
/// Command sınıflarına uygulanır.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RuleConfigurationAttribute : Attribute
{
    /// <summary>
    /// Çalıştırılacak kuralların türleri.
    /// </summary>
    public Type[] IncludeRules { get; set; } = Array.Empty<Type>();

    /// <summary>
    /// Çalıştırılmayacak kuralların türleri.
    /// </summary>
    public Type[] ExcludeRules { get; set; } = Array.Empty<Type>();

    /// <summary>
    /// Sadece belirtilen kuralları çalıştır (diğerlerini yok say).
    /// </summary>
    public Type[]? OnlyRules { get; set; }

    public RuleConfigurationAttribute(params Type[] includeRules)
    {
        IncludeRules = includeRules ?? Array.Empty<Type>();
    }
}