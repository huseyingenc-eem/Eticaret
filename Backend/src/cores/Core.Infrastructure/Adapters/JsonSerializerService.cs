
namespace Core.Infrastructure.Adapters;

using Core.Application.Interfaces;
using Core.Application.Interfaces.Paging;
using Core.Infrastructure.Persistence.Paging;
using System.Text;
using System.Text.Json;

public class JsonSerializerService : ISerializerService
{
    public T? Deserialize<T>(byte[] data)
    {
        string json = Encoding.UTF8.GetString(data);

        // Dönüştürülecek tip bir IPaginate<> arayüzü ise,
        // onun somut sınıfı olan Paginate<>'i kullanarak Deserialize et.
        if (typeof(T).IsInterface && typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IPaginate<>))
        {
            Type itemType = typeof(T).GetGenericArguments()[0];
            Type concreteType = typeof(Paginate<>).MakeGenericType(itemType);

            object? deserialized = JsonSerializer.Deserialize(json, concreteType);
            return (T?)deserialized;
        }

        // Diğer tüm tipler için standart Deserialize işlemi yap.
        return JsonSerializer.Deserialize<T>(json);
    }
}