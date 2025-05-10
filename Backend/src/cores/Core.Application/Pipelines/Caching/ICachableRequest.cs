namespace Core.Application.Pipelines.Caching;

public interface ICachableRequest
{
    /// <summary>
    /// Önbellek için benzersiz anahtar.
    /// </summary>
    string? CacheKey { get; }

    /// <summary>
    /// Önbelleği atlayıp doğrudan kaynağa gidilip gidilmeyeceğini belirtir.
    /// True ise önbellek kullanılmaz.
    /// </summary>
    bool ByPassCache { get; }

    /// <summary>
    /// Bu isteğin ait olduğu önbellek grubu anahtarı.
    /// Grup bazlı önbellek temizleme işlemleri için kullanılır.
    /// </summary>
    string? CacheGroupKey { get; }

    /// <summary>
    /// Önbellek öğesi için kayan son kullanma süresi.
    /// Belirtilen süre boyunca erişilmezse öğe önbellekten kaldırılır.
    /// Null ise, varsayılan kayan son kullanma süresi (CacheSettings'ten) kullanılır.
    /// </summary>
    TimeSpan? SlidingExpiration { get; }

    /// <summary>
    /// Önbellek öğesi için mutlak son kullanma süresi (şu andan itibaren).
    /// Öğenin önbelleğe eklendiği andan itibaren belirtilen süre sonunda,
    /// erişilip erişilmediğine bakılmaksızın önbellekten kaldırılır.
    /// Null ise, varsayılan mutlak son kullanma süresi (CacheSettings'ten) kullanılabilir veya hiç ayarlanmayabilir.
    /// </summary>
    TimeSpan? AbsoluteExpirationRelativeToNow { get; }
}
