namespace Core.Shared.Extensions;

/// <summary>
/// Koleksiyonlar (IEnumerable) için genel amaçlı genişletme metotları içerir.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Bir koleksiyonun null veya boş olup olmadığını kontrol eder.
    /// 'source == null || !source.Any()' ifadesinin daha okunaklı bir alternatifidir.
    /// </summary>
    /// <typeparam name="T">Koleksiyondaki öğelerin türü.</typeparam>
    /// <param name="source">Kontrol edilecek koleksiyon.</param>
    /// <returns>Koleksiyon null veya boş ise true, aksi takdirde false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
}