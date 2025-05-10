using AutoMapper;
using ETicaret.Application.Services.Repositories; 
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Application.Pipelines.Transactional;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

public class UpdateAddressCommand : IRequest<UpdateAddressResponseDto> , ITransactionalRequest
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; }
    public bool IsShippingAddress { get; set; }

    public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, UpdateAddressResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAddressCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<UpdateAddressResponseDto> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
        {
            Address? addressToUpdate = await _unitOfWork.AddressRepository.GetAsync(
                filter: a => a.Id == request.Id,
                cancellationToken: cancellationToken);

            if (addressToUpdate == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            if (string.IsNullOrEmpty(request.UserId) || addressToUpdate.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi güncelleme yetkiniz yok veya kullanıcı kimliği eksik.");
            }

            _mapper.Map(request, addressToUpdate);
            await _unitOfWork.AddressRepository.UpdateAsync(addressToUpdate, cancellationToken);

            UpdateAddressResponseDto response = _mapper.Map<UpdateAddressResponseDto>(addressToUpdate);
            response.Message = "Adres başarıyla güncellendi.";
            return response;
        }
    }
}