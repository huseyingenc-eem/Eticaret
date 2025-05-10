namespace Core.Application.Pipelines.Caching;

public class CacheSettings
{
    /// <summary>
    /// Varsayılan kayan son kullanma süresi (dakika cinsinden).
    /// Bir önbellek öğesine belirtilen süre boyunca erişilmezse, öğe önbellekten kaldırılır.
    /// </summary>
    public int SlidingExpirationInMinutes { get; set; } = 60; // Örnek varsayılan değer

    /// <summary>
    /// Varsayılan mutlak son kullanma süresi (dakika cinsinden).
    /// Bir önbellek öğesi, eklendikten sonra belirtilen süre sonunda (erişilip erişilmediğine bakılmaksızın)
    /// önbellekten kaldırılır. Null veya 0 ise, varsayılan olarak mutlak süre uygulanmayabilir.
    /// </summary>
    public int? AbsoluteExpirationInMinutes { get; set; } = 120;
}
