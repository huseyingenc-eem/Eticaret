using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

/// <summary>
/// Mevcut bir tedarikçiyi silme (pasif hale getirme) işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
public class SupplierDeleteCommand : IRequest<SupplierDeleteResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }

    #region Önbellek Ayarları
    public string? CacheKey => $"supplier:{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    #endregion

    public class SupplierDeleteCommandHandler : IRequestHandler<SupplierDeleteCommand, SupplierDeleteResponseDto>
    {
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        // Business rules could be injected here if needed
        // private readonly SupplierBusinessRules _supplierBusinessRules;

        public SupplierDeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            // Get the generic repository from the Unit of Work
            _supplierRepository = unitOfWork.GetRepository<Supplier, Guid>();
        }

        public async Task<SupplierDeleteResponseDto> Handle(SupplierDeleteCommand request, CancellationToken cancellationToken)
        {
            // 1. Find the supplier to delete using our reusable specification.
            var spec = new SupplierSpecifications.ById(request.Id);
            Supplier? supplierToDelete = await _supplierRepository.GetAsync(spec, cancellationToken);

            if (supplierToDelete == null)
                throw new NotFoundException(
                    message: $"Supplier with ID {request.Id} was not found.",
                    userFriendlyMessage: "Belirtilen tedarikçi bulunamadı.",
                    errorCode: "SUPPLIER_NOT_FOUND"
                    );

            // Here you could add business rule checks if necessary, for example:
            // await _supplierBusinessRules.CheckIfSupplierHasActiveProductsAsync(request.Id);

            // 2. Perform the soft delete.
            // The `permanent: false` flag will trigger the logic in your DbContext to set the 'DeletedTime'.
            await _supplierRepository.DeleteAsync(supplierToDelete, permanent: false, cancellationToken);

            // 3. The manual call to `_unitOfWork.CompleteAsync()` is no longer needed.
            // The TransactionBehavior will handle saving and committing the changes automatically.

            return new SupplierDeleteResponseDto
            {
                Id = request.Id,
                Message = "Tedarikçi başarıyla pasif hale getirildi."
            };
        }
    }
}