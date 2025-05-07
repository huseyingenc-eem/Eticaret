using AutoMapper;
using Core.Application.Pipelines.Caching;
using ETicaret.Application.Services.RedisServices; // Redis için using
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class CreateProductCommand : IRequest<CreateProductResponseDto> , ICacheRemoverRequest
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

    public bool ByPassCache => false;

    public string? CacheGroupKey => "Products";

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService) // Constructor güncellendi
        {
            _unitOfWork = unitOfWork; // Güncellendi
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<CreateProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);
            

            var addedProduct = await _unitOfWork.ProductRepository.AddAsync(product, cancellationToken: cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _redisService.RemoveDataAsync("products");

            CreateProductResponseDto response = _mapper.Map<CreateProductResponseDto>(addedProduct);
            response.Message = "Ürün başarıyla eklendi.";
            return response;
        }
    }
}
