namespace Core.Application.Behaviors.Caching;
public interface ICacheRemoverRequest
{
    string? CacheKey { get; }
    bool BypassCache { get => false; }
    string? CacheGroupKey { get; }
}