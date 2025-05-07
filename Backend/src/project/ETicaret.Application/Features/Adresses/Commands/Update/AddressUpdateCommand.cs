using AutoMapper;
using ETicaret.Application.Services.Repositories; 
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

public class AddressUpdateCommand : IRequest<AddressUpdateResponseDto>
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public bool IsBillingAddress { get; set; }
    public bool IsShippingAddress { get; set; }

    public class AddressUpdateCommandHandler : IRequestHandler<AddressUpdateCommand, AddressUpdateResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AddressUpdateCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<AddressUpdateResponseDto> Handle(AddressUpdateCommand request, CancellationToken cancellationToken)
        {
            Address? addressToUpdate = await _unitOfWork.AddressRepository.GetAsync(
                filter: a => a.Id == request.Id,
                cancellationToken: cancellationToken);

            if (addressToUpdate == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            if (addressToUpdate.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi güncelleme yetkiniz yok.");
            }

            _mapper.Map(request, addressToUpdate);

            await _unitOfWork.AddressRepository.UpdateAsync(addressToUpdate, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            AddressUpdateResponseDto response = _mapper.Map<AddressUpdateResponseDto>(addressToUpdate);
            response.Message = "Adres başarıyla güncellendi.";
            return response;
        }
    }
}