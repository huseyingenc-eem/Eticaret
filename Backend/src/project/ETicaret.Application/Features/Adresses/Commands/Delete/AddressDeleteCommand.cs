using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

/// <summary>
/// Mevcut bir adresi silmek için kullanılan komut nesnesi.
/// </summary>
public class AddressDeleteCommand : IRequest<AddressDeleteResponseDto>
{

    public int Id { get; set; }

    public string? UserId { get; set; }

    public class AddressDeleteCommandHandler : IRequestHandler<AddressDeleteCommand, AddressDeleteResponseDto>
    {
        private readonly IAddressRepository _addressRepository;

        public AddressDeleteCommandHandler(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<AddressDeleteResponseDto> Handle(AddressDeleteCommand request, CancellationToken cancellationToken)
        {
            Address? addressToDelete = await _addressRepository.GetAsync(
                filter: a => a.Id == request.Id,
                cancellationToken: cancellationToken);

            if (addressToDelete == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            if (addressToDelete.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi silme yetkiniz yok.");
            }

            await _addressRepository.DeleteAsync(addressToDelete, cancellationToken);

            return new AddressDeleteResponseDto { Id = request.Id, Message = "Adres başarıyla silindi." };
        }
    }
}