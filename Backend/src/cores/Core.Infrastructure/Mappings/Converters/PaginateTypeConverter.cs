using AutoMapper;
using Core.Infrastructure.Persistence.Paging;
using Core.Application.Interfaces.Paging;


namespace Core.Infrastructure.Mappings.Converters;

public class PaginateTypeConverter<TSource, TDestination>
    : ITypeConverter<IPaginate<TSource>, IPaginate<TDestination>>
{
    public IPaginate<TDestination> Convert(IPaginate<TSource> source, IPaginate<TDestination> destination, ResolutionContext context)
    {
        var items = context.Mapper.Map<IList<TDestination>>(source.Items);
        return new Paginate<TDestination>
        {
            Index = source.Index,
            Size = source.Size,
            Count = source.Count,
            Pages = source.Pages,
            Items = items
        };
    }
}