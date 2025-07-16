using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.Shared.Exceptions;
using Core.Application.Pipelines.Transactional;
using Core.Application.Pipelines.Caching;
using ETicaret.Application.Features.Suppliers.Constants;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

public class SupplierDeleteCommand : IRequest<SupplierDeleteResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    public string? CacheKey => $"supplier:{Id}";

    public bool ByPassCache => false;

    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup; 

    public class SupplierDeleteCommandHandler : IRequestHandler<SupplierDeleteCommand, SupplierDeleteResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork; 

        public SupplierDeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SupplierDeleteResponseDto> Handle(SupplierDeleteCommand request, CancellationToken cancellationToken)
        {
            Supplier? supplierToDelete = await _unitOfWork.SupplierRepository.GetAsync(
                filter: s => s.Id == request.Id,
                cancellationToken: cancellationToken);

            if (supplierToDelete == null)
                throw new NotFoundException($"Kimliği {request.Id} olan tedarikçi bulunamadı.");

            await _unitOfWork.SupplierRepository.DeleteAsync(supplierToDelete,permanent: false, cancellationToken); 
            await _unitOfWork.CompleteAsync(cancellationToken); 

            return new SupplierDeleteResponseDto { Id = request.Id, Message = "Tedarikçi başarıyla silindi." };
        }
    }
}