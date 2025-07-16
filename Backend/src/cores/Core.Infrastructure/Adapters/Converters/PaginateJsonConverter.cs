
using Core.Application.Abstractions.Paging;
using Core.Infrastructure.Persistence.Paging;
using System.Text.Json;
using System.Text.Json.Serialization;

public class PaginateJsonConverter<T> : JsonConverter<IPaginate<T>>
{
    public override IPaginate<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // IPaginate<T> arayüzü istendiğinde, onun somut sınıfı olan Paginate<T>'e deserialize et.
        return JsonSerializer.Deserialize<Paginate<T>>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, IPaginate<T> value, JsonSerializerOptions options)
    {
        // Değeri doğrudan serialize et.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

public class PaginateJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
            return false;

        return typeToConvert.GetGenericTypeDefinition() == typeof(IPaginate<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        Type itemType = typeToConvert.GetGenericArguments()[0];
        Type converterType = typeof(PaginateJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}