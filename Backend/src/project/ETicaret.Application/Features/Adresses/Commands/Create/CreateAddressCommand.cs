using AutoMapper;
using Core.Application.Pipelines.Transactional;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities; 
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

public class CreateAddressCommand : IRequest<CreateAddressResponseDto> , ITransactionalRequest
{

    public string UserId { get; set; } = string.Empty;

    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; } = false;
    public bool IsShippingAddress { get; set; } = false;

    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateAddressResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {
            Address address = _mapper.Map<Address>(request);

            Address addedAddress = await _unitOfWork.AddressRepository.AddAsync(address, cancellationToken);

            CreateAddressResponseDto response = _mapper.Map<CreateAddressResponseDto>(addedAddress);
            response.Message = "Adres başarıyla eklendi.";

            return response;
        }
    }
}