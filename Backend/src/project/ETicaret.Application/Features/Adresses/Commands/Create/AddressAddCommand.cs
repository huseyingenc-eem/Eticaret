using AutoMapper;
using ETicaret.Application.Services.Repositories; // IAddressRepository için
using ETicaret.Domain.Entities;
using MediatR;
using System; // Guid için

namespace ETicaret.Application.Features.Addresses.Commands.Create;

/// <summary>
/// Yeni bir adres eklemek için kullanılan komut nesnesi.
/// </summary>
public class AddressAddCommand : IRequest<AddressAddResponseDto>
{
    public Guid UserId { get; set; } // Bu alan handler'a gelmeden önce controller'da set edilmeli.

    public string AddressTitle { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;

    public string FullAddress { get; set; } = string.Empty;

    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; } = false;
    public bool IsShippingAddress { get; set; } = false;

    public class AddressAddCommandHandler : IRequestHandler<AddressAddCommand, AddressAddResponseDto>
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressAddCommandHandler(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }
        public async Task<AddressAddResponseDto> Handle(AddressAddCommand request, CancellationToken cancellationToken)
        {
            // Validation burada veya pipeline'da yapılmalı (örn: UserId boş mu kontrolü)
            Address address = _mapper.Map<Address>(request);
            // address.CreatedTime base repo'da atanmalı

            Address addedAddress = await _addressRepository.AddAsync(address, cancellationToken);

            AddressAddResponseDto response = _mapper.Map<AddressAddResponseDto>(addedAddress);
            response.Message = "Adres başarıyla eklendi.";
            return response;
        }
    }
}
