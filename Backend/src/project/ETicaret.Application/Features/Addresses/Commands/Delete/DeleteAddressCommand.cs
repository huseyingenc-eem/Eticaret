using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Application.Pipelines.Transactional;
using Core.Application.Pipelines.Caching;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

public class DeleteAddressCommand : IRequest<DeleteAddressResponseDto> , ITransactionalRequest , ICacheRemoverRequest
{
    public int Id { get; set; }
    public string UserId { get; set; }


    public string CacheKey => $"address:{Id}";


    public string? CacheGroupKey => $"Addresses";
                                                                

    public bool ByPassCache { get; set; }

    public class AddressDeleteCommandHandler : IRequestHandler<DeleteAddressCommand, DeleteAddressResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressDeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteAddressResponseDto> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            Address? addressToDelete = await _unitOfWork.AddressRepository.GetAsync(
                filter: a => a.Id == request.Id,
                cancellationToken: cancellationToken);

            if (addressToDelete == null)
            {
                throw new NotFoundException($"Address with Id {request.Id} not found.");
            }

            if (string.IsNullOrEmpty(request.UserId) || addressToDelete.UserId != request.UserId)
            {
                throw new AuthorizationException("Bu adresi silme yetkiniz yok veya kullanıcı kimliği eksik.");
            }

            await _unitOfWork.AddressRepository.DeleteAsync(addressToDelete, permanent: true, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            return new DeleteAddressResponseDto
            {
                Id = request.Id,
                Message = "Adres başarıyla silindi.",
                IsSuccess = true
            };
        }
    }
}