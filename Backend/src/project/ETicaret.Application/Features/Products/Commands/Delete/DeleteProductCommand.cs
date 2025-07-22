using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Specifications; // <-- Our new specification
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<DeleteProductResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public Guid Id { get; set; }

    public string CacheKey => $"product:{Id}";
    public string? CacheGroupKey => "ProductsGroup";
    public bool BypassCache { get; set; }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponseDto>
    {
        private readonly IRepository<Domain.Entities.Product, Guid> _productRepository;
        // private readonly ProductBusinessRules _productBusinessRules;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            // Get the generic repository from the Unit of Work.
            _productRepository = unitOfWork.GetRepository<Domain.Entities.Product, Guid>();
            // _productBusinessRules = productBusinessRules;
        }

        public async Task<DeleteProductResponseDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Use our new, reusable specification to find the product.
            var spec = new ProductByIdSpecification(request.Id);
            Domain.Entities.Product? productToDelete = await _productRepository.GetAsync(spec, cancellationToken);

            if (productToDelete == null)
            {
                throw new NotFoundException($"Product to be deleted was not found (ID: {request.Id}).");
            }

            // --- Business Rule Checks (Example) ---
            // await _productBusinessRules.CheckIfProductIsInActiveOrderAsync(request.Id, cancellationToken);
            // --- End Business Rule Checks ---

            // 2. Perform the soft delete.
            await _productRepository.DeleteAsync(productToDelete, permanent: false, cancellationToken);

            // 3. No need to call CompleteAsync; TransactionBehavior handles it.
            return new DeleteProductResponseDto
            {
                Id = request.Id,
                Message = "Product successfully deleted (deactivated).",
                IsSuccess = true
            };
        }
    }
}