using Core.Application.Abstractions.Paging;
using Core.Application.Common.Results;
using System.Text.Json;
using System.Text.Json.Serialization;




namespace Core.Infrastructure.Adapters.Converters;
/// <summary>
/// IPaginate<T> arayüzünün, serileştirme ve deserileştirme sırasında
/// PagedResult<T> somut sınıfına doğru şekilde dönüştürülmesini sağlar.
/// </summary>
public class PagedResultJsonConverter<T> : JsonConverter<IPaginate<T>>
{
    /// <summary>
    /// JSON verisini okur ve PagedResult<T> tipine dönüştürür.
    /// </summary>
    public override IPaginate<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Gelen JSON'u, IPaginate<T>'nin somut implementasyonu olan PagedResult<T> olarak deserialize et.
        return JsonSerializer.Deserialize<PagedResult<T>>(ref reader, options);
    }

    /// <summary>
    /// IPaginate<T> tipindeki bir değeri JSON formatına yazar.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, IPaginate<T> value, JsonSerializerOptions options)
    {
        // Gelen değeri, gerçek somut tipiyle (örn: PagedResult<T>) serialize et.
        // Bu, polimorfizmi korur ve doğru alanların yazılmasını sağlar.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

/// <summary>
/// JsonSerializer'a, IPaginate<T> türündeki nesneler için hangi JsonConverter'ın
/// kullanılacağını bildiren fabrika sınıfı.
/// </summary>
public class PagedResultJsonConverterFactory : JsonConverterFactory
{
    /// <summary>
    /// Bir türün bu fabrika tarafından dönüştürülüp dönüştürülemeyeceğini belirler.
    /// </summary>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(IPaginate<>);
    }

    /// <summary>
    /// Belirtilen tür için uygun JsonConverter örneğini oluşturur.
    /// </summary>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type itemType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(PagedResultJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}