using Core.Application.Abstractions.Services;
using Core.Infrastructure.Adapters.Converters;
using System.Text.Json;

namespace Core.Infrastructure.Adapters;

/// <summary>
/// ISerializerService arayüzünün System.Text.Json kütüphanesini kullanarak somut implementasyonu.
/// </summary>
public class JsonSerializerService : ISerializerService
{
    // JsonSerializerOptions'ı static ve readonly olarak tanımlayarak
    // her seferinde yeniden oluşturulmasını önlüyor ve performansı artırıyoruz.
    private static readonly JsonSerializerOptions _options = new()
    {
        Converters = { new PagedResultJsonConverterFactory() }
    };

    /// <inheritdoc/>
    public T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data, _options);
    }

    /// <inheritdoc/>
    public byte[] SerializeToUtf8Bytes<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _options);
    }
}