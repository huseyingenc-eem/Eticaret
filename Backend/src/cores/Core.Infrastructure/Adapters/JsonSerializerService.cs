using Core.Application.Abstractions.Services;
using System.Text.Json;

namespace Core.Infrastructure.Adapters;

public class JsonSerializerService : ISerializerService
{
    private readonly JsonSerializerOptions _options;

    public JsonSerializerService()
    {
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new PaginateJsonConverterFactory());
    }

    public T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }

    public byte[] Serialize<T>(T obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
    }
}