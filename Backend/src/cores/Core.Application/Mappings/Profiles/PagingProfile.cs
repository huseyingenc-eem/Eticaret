using AutoMapper;
using Core.Application.Mappings.Converters;
using Core.Persistence.Paging;

namespace Core.Application.Mappings.Profiles;

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
