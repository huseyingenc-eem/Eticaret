namespace Core.Shared.Constants;

/// <summary>
/// Önbellek anahtarları için jenerik bir üretici (factory) görevi görür.
/// Tip-güvenli (type-safe) ve merkezi bir anahtar yönetimi sağlar.
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Belirtilen varlık türü için önbellek anahtarı üreteci oluşturur.
    /// </summary>
    /// <typeparam name="TEntity">Önbellek anahtarı üretilecek varlık türü.</typeparam>
    /// <returns>O varlığa özel anahtar üretme metotlarını içeren bir nesne.</returns>
    public static EntityTypeCacheKeys<TEntity> For<TEntity>() where TEntity : class
        => new EntityTypeCacheKeys<TEntity>();
}

/// <summary>
/// Belirli bir varlık türü (TEntity) için standart önbellek anahtarlarını üretir.
/// </summary>
/// <typeparam name="TEntity">Varlık türü.</typeparam>
public class EntityTypeCacheKeys<TEntity> where TEntity : class
{
    private readonly string _typeName;

    public EntityTypeCacheKeys()
    {
        _typeName = typeof(TEntity).Name;  // Varlık türünün adını alıyoruz. Örn: "Product", "Order"
    }

    /// <summary>
    /// O varlık türüne ait tüm önbellek anahtarlarını gruplamak için kullanılan anahtar. Örn: "Product"
    /// </summary>
    public string GroupName => _typeName;

    /// <summary>
    /// O varlık türüne ait tüm kayıtların listesi için anahtar. Örn: "Product.All"
    /// </summary>
    public string All => $"{_typeName}.All";

    /// <summary>
    /// Belirli bir ID'ye sahip kayıt için anahtar oluşturur. Örn: "Product.ById.123"
    /// </summary>
    /// <param name="id">Varlığın ID'si.</param>
    public string GetById(object id) => $"{_typeName}.ById.{id}";

    /// <summary>
    /// Sayfalanmış listeler için anahtar oluşturur. Örn: "Product.Paginated.Page1.Size10"
    /// </summary>
    /// <param name="pageNumber">Sayfa numarası.</param>
    /// <param name="pageSize">Sayfa boyutu.</param>
    public string GetPaginated(int pageNumber, int pageSize) => $"{_typeName}.Paginated.Page{pageNumber}.Size{pageSize}";
}