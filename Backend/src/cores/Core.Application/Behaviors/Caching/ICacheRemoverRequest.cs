namespace Core.Application.Behaviors.Caching;
public interface ICacheRemoverRequest
{
    string? CacheKey { get; }
    bool ByPassCache { get; }
    string? CacheGroupKey { get; }
}