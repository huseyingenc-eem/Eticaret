using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using System;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

public class AddressUpdateCommand : IRequest<AddressUpdateResponseDto>
{
    public int Id { get; set; }

    public Guid UserId { get; set; }
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
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressUpdateCommandHandler(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }
        public async Task<AddressUpdateResponseDto> Handle(AddressUpdateCommand request, CancellationToken cancellationToken)
        {
            Address? addressToUpdate = await _addressRepository.GetAsync(
                filter: a => a.Id == request.Id, 
                cancellationToken: cancellationToken);

            if (addressToUpdate == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            // ÖNEMLİ: Adresin isteği yapan kullanıcıya ait olup olmadığını kontrol et!
            if (addressToUpdate.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi güncelleme yetkiniz yok.");
            }

            // Gelen isteği mevcut entity üzerine map'le
            _mapper.Map(request, addressToUpdate);
            // addressToUpdate.UpdateTime base repo'da atanmalı

            await _addressRepository.UpdateAsync(addressToUpdate, cancellationToken);

            AddressUpdateResponseDto response = _mapper.Map<AddressUpdateResponseDto>(addressToUpdate);
            response.Message = "Adres başarıyla güncellendi.";
            return response;
        }
    }
}