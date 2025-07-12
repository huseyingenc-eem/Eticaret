using AutoMapper;
using Core.Infrastructure.Mappings.Converters;
using Core.Application.Interfaces.Paging;

namespace Core.Infrastructure.Persistence.Paging;

public class PagingProfile : Profile
{
    public PagingProfile()
    {
        CreateMap(typeof(Paginate<>), typeof(Paginate<>)) // <<-- SOMUT TİPLER
        .ConvertUsing(typeof(PaginateTypeConverter<,>));

        CreateMap(typeof(IPaginate<>), typeof(IPaginate<>))
        .ConvertUsing(typeof(PaginateTypeConverter<,>));
    }
}
