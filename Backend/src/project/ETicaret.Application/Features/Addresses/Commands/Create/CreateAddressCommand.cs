using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Transactional;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

public class CreateAddressCommand : IRequest<CreateAddressResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; }
    public string? zipCode { get; set; }
    public bool IsDefaultBilling { get; set; } = false;
    public bool IsDefaultShipping { get; set; } = false;

    public string? CacheKey => null;
    public bool ByPassCache => false;
    public string? CacheGroupKey => "Addresses";

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
            // Eğer kullanıcıya ait diğer adreslerde varsayılan varsa, onları false yap
            if (request.IsDefaultShipping)
            {
                var existingDefaults = await _unitOfWork.AddressRepository.GetListAsync(
                    filter: a => a.UserId == request.UserId && a.IsDefaultShipping,
                    enableTracking: true,
                    cancellationToken: cancellationToken
                );
                foreach (var adr in existingDefaults.Items)
                {
                    adr.IsDefaultShipping = false;
                    await _unitOfWork.AddressRepository.UpdateAsync(adr);
                }
            }

            if (request.IsDefaultBilling)
            {
                var existingDefaults = await _unitOfWork.AddressRepository.GetListAsync(
                   filter: a => a.UserId == request.UserId && a.IsDefaultBilling,
                   enableTracking: true,
                   cancellationToken: cancellationToken
               );
                foreach (var adr in existingDefaults.Items)
                {
                    adr.IsDefaultBilling = false;
                    await _unitOfWork.AddressRepository.UpdateAsync(adr);
                }
            }

            Address addressEntity = _mapper.Map<Address>(request);
            addressEntity.UserId = request.UserId;

            await _unitOfWork.AddressRepository.AddAsync(addressEntity, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
            CreateAddressResponseDto response = _mapper.Map<CreateAddressResponseDto>(addressEntity);
            response.Message = "Adres başarıyla eklendi.";

            return response;
        }
    }
}