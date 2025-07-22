using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Products.Constants;
using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductCommand : IRequest<CreateProductResponseDto> , ICacheRemoverRequest , ITransactionalRequest
{
    
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; }

    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public string? CacheKey => null;

    public bool BypassCache => false;

    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);

            var productRepository = _unitOfWork.GetRepository<Product,Guid>();
            var addedProduct = await productRepository.AddAsync(product, cancellationToken: cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            CreateProductResponseDto response = _mapper.Map<CreateProductResponseDto>(addedProduct);
            response.Message = "Ürün başarıyla eklendi.";
            return response;
        }
    }
}
