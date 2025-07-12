
namespace Core.Application.Interfaces;

public interface ISerializerService
{
    T? Deserialize<T>(byte[] data);
}