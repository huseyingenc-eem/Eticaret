using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Transactional;
using Core.Shared.Exceptions;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<DeleteProductResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }

    public string CacheKey => $"product:{Id}";
    public string? CacheGroupKey => "ProductsGroup";
    public bool ByPassCache { get; set; }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork /*, ProductBusinessRules productBusinessRules */)
        {
            _unitOfWork = unitOfWork;
            // _productBusinessRules = productBusinessRules;
        }

        public async Task<DeleteProductResponseDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            
            Domain.Entities.Product? productToDelete = await _unitOfWork.ProductRepository.GetAsync(
                filter: p => p.Id == request.Id, // Product.Id string ise bu karşılaştırma doğru.
                enableTracking: true, // Soft delete için entity'nin takip edilmesi iyi bir pratiktir.
                cancellationToken: cancellationToken
            );

            if (productToDelete == null)
            {
                throw new NotFoundException($"Silinecek ürün bulunamadı (ID: {request.Id}).");
            }

            // --- İş Kuralları Kontrolleri (Örnek) ---
            // Örneğin, ürünün aktif bir siparişte olup olmadığını kontrol et.
            // await _productBusinessRules.CheckIfProductIsInActiveOrderAsync(request.Id, cancellationToken);
            // --- İş Kuralları Kontrolleri Sonu ---

            await _unitOfWork.ProductRepository.DeleteAsync(productToDelete, permanent: false, cancellationToken);

            return new DeleteProductResponseDto
            {
                Id = request.Id,
                Message = "Ürün başarıyla silindi (pasif hale getirildi).",
                IsSuccess = true
            };
        }
    }
}
