namespace Core.Application.Abstractions.Services;

public interface ISerializerService
{
    T? Deserialize<T>(byte[] data);
}