using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Addresses.Queries.GetById;

/// <summary>
/// Belirli bir ID'ye sahip adresi getirmek için kullanılan sorgu nesnesi.
/// </summary>
public class GetByIdAddressQuery : IRequest<GetByIdAddressResponseDto>
{
    public int Id { get; set; }

    public string? UserId { get; set; } // Controller'dan set edilecek

    public class GetByIdAddressQueryHandler : IRequestHandler<GetByIdAddressQuery, GetByIdAddressResponseDto>
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public GetByIdAddressQueryHandler(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<GetByIdAddressResponseDto> Handle(GetByIdAddressQuery request, CancellationToken cancellationToken)
        {
            Address? address = await _addressRepository.GetAsync(
                                        filter: a => a.Id == request.Id,
                                        enableTracking: false,
                                        cancellationToken: cancellationToken);

            if (address == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            if (address.UserId != request.UserId)
            {
                // Farklı bir hata tipi de kullanılabilir veya sadece null dönülebilir.
                // Ancak NotFoundException, başkasının adres ID'sini tahmin etmeyi zorlaştırır.
                throw new NotFoundException($"Address with Id {request.Id} not found for this user.");
                // Veya: throw new AuthorizationException("Bu adresi görme yetkiniz yok.");
            }

            GetByIdAddressResponseDto response = _mapper.Map<GetByIdAddressResponseDto>(address);
            return response;
        }
    }
}