using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

public class AddressDeleteCommand : IRequest<AddressDeleteResponseDto>
{
    public int Id { get; set; }
    public string? UserId { get; set; }

    public class AddressDeleteCommandHandler : IRequestHandler<AddressDeleteCommand, AddressDeleteResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressDeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AddressDeleteResponseDto> Handle(AddressDeleteCommand request, CancellationToken cancellationToken)
        {
            Address? addressToDelete = await _unitOfWork.AddressRepository.GetAsync(
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

            await _unitOfWork.AddressRepository.DeleteAsync(addressToDelete, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return new AddressDeleteResponseDto { Id = request.Id, Message = "Adres başarıyla silindi." };
        }
    }
}