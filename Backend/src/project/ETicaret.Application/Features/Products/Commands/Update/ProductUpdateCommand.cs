using AutoMapper;
using ETicaret.Application.Services.RedisServices; // Redis için using
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions; // NotFoundException için

namespace ETicaret.Application.Features.Products.Commands.Update;

public class ProductUpdateCommand : IRequest<ProductUpdateResponseDto> // DTO döndürelim
{
    public int Id { get; set; }
    // Güncellenen ve Yeni Alanlar
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; } // double'dan decimal'e çevrildi
    public int Stock { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; } // Yeni eklendi

    // İsteğe bağlı yeni alanlar
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }

    public class ProductUpdateCommandHandler : IRequestHandler<ProductUpdateCommand, ProductUpdateResponseDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService; // Inject edildi

        public ProductUpdateCommandHandler(IProductRepository productRepository, IMapper mapper, IRedisService redisService) // Constructor güncellendi
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _redisService = redisService; // Atama yapıldı
        }

        public async Task<ProductUpdateResponseDto> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            // Önce ürünü bulalım
            var productToUpdate = await _productRepository.GetAsync(p => p.Id == request.Id, cancellationToken: cancellationToken);

            if (productToUpdate == null)
            {
                throw new NotFoundException($"Product with Id {request.Id} not found.");
            }

            // AutoMapper ile request'teki verileri mevcut entity üzerine mapleyelim
            _mapper.Map(request, productToUpdate);
            // productToUpdate.UpdateTime base repository'de atanıyor olmalı

            await _productRepository.UpdateAsync(productToUpdate, cancellationToken);

            // Cache temizleme
            await _redisService.RemoveDataAsync("products"); // Genel liste anahtarı [cite: 2]
            // Güncellenen ürünün ID'sine özel cache varsa o da temizlenmeli
            // Örnek: await _redisService.RemoveDataAsync($"product:{request.Id}");

            // DTO döndürelim
            ProductUpdateResponseDto response = _mapper.Map<ProductUpdateResponseDto>(productToUpdate);
            response.Message = "Ürün başarıyla güncellendi.";
            return response;
        }
    }
}
