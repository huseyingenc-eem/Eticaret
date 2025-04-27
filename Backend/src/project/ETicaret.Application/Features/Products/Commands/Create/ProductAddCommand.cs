using AutoMapper;
using ETicaret.Application.Services.RedisServices; // Redis için using
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Create;

public class ProductAddCommand : IRequest<ProductAddResponseDto> // DTO döndürmek daha iyi olabilir
{
    // Güncellenen ve Yeni Alanlar
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } // double'dan decimal'e çevrildi
    public int Stock { get; set; }
    public int CategoryID { get; set; } // CategoryId -> CategoryID olarak güncellendi
    public int SupplierID { get; set; } // Yeni eklendi

    // İsteğe bağlı yeni alanlar
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true; // Varsayılan değer komut seviyesinde de atanabilir

    // Handler sınıfı aynı kalabilir, ancak RedisService inject edilmeli
    public class ProductAddCommandHandler : IRequestHandler<ProductAddCommand, ProductAddResponseDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService; // Inject edildi

        public ProductAddCommandHandler(IProductRepository productRepository, IMapper mapper, IRedisService redisService) // Constructor güncellendi
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _redisService = redisService; // Atama yapıldı
        }

        public async Task<ProductAddResponseDto> Handle(ProductAddCommand request, CancellationToken cancellationToken)
        {
            Product product = _mapper.Map<Product>(request);
            // product.CreatedTime base repository'de atanıyor olmalı

            var addedProduct = await _productRepository.AddAsync(product, cancellationToken: cancellationToken);

            // Cache temizleme (Genel product listesi için)
            await _redisService.RemoveDataAsync("products"); // products anahtarını kullanmaya devam ediyoruz. [cite: 2]
            // Daha spesifik cache anahtarları varsa onlar da temizlenmeli (örn: category/supplier bazlı listeler)

            // DTO döndürelim
            ProductAddResponseDto response = _mapper.Map<ProductAddResponseDto>(addedProduct);
            response.Message = "Ürün başarıyla eklendi.";
            return response;
        }
    }
}
