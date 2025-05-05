using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

public class GetListByUserIdAddressQuery : IRequest<List<GetListAddressResponseDto>>
{
    public string? UserId { get; set; }

    public class GetListByUserIdAddressQueryHandler : IRequestHandler<GetListByUserIdAddressQuery, List<GetListAddressResponseDto>>
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public GetListByUserIdAddressQueryHandler(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Kullanıcının adreslerini getirme isteğini işler.
        /// </summary>
        public async Task<List<GetListAddressResponseDto>> Handle(GetListByUserIdAddressQuery request, CancellationToken cancellationToken)
        {
            var addresses = await _addressRepository.GetListAsync(
                filter: a => a.UserId == request.UserId,
                orderBy: q => q.OrderByDescending(a => a.CreatedTime),
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            List<GetListAddressResponseDto> response = _mapper.Map<List<GetListAddressResponseDto>>(addresses);
            return response;
        }
    }
}