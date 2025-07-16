using AutoMapper;
using Core.Application.Pipelines.Caching;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using System.Linq.Expressions;
using Core.Application.Abstractions.Paging;

namespace ETicaret.Application.Features.Addresses.Queries.GetList;

public class GetListAddressQuery : IRequest<IPaginate<GetListAddressResponseDto>>, ICachableRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    public bool ByPassCache { get; set; }
    public string CacheKey => $"address-list_page_{PageIndex}_size_{PageSize}"; 
    public string? CacheGroupKey => "Addresses";
    public TimeSpan? SlidingExpiration { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(30);
    public string? UserNameSearch { get; set; }
    public string? CityFilter { get; set; }

    public class GetListAddressQueryHandler : IRequestHandler<GetListAddressQuery, IPaginate<GetListAddressResponseDto>>
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public GetListAddressQueryHandler(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<IPaginate<GetListAddressResponseDto>> Handle(GetListAddressQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Address, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(request.UserNameSearch))
            {
                string searchTerm = request.UserNameSearch.ToLower();
                predicate = a => (a.User != null &&
                                   ((a.User.FirstName != null && a.User.FirstName.ToLower().Contains(searchTerm)) ||
                                    (a.User.LastName != null && a.User.LastName.ToLower().Contains(searchTerm)) ||
                                    (a.User.Email != null && a.User.Email.ToLower().Contains(searchTerm)))) ||
                                 (a.AddressLine != null && a.AddressLine.ToLower().Contains(searchTerm));
            }
            //IPaginate<Address> addressesPaginate = await _addressRepository.GetListWithUserDetailsAsync(
            //predicate: predicate,
            //index: request.PageIndex,
            //size: request.PageSize,
            //orderBy: q => q.OrderByDescending(a => a.CreatedTime),
            //enableTracking: false,
            //cancellationToken: cancellationToken
            //);
            //IPaginate<GetListAddressResponseDto> mappedAddressesPaginate = _mapper.Map<IPaginate<GetListAddressResponseDto>>(addressesPaginate);

            return mappedAddressesPaginate;
        }
    }

}