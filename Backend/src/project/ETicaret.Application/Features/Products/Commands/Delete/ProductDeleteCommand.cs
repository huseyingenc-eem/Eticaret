using AutoMapper;
using ETicaret.Application.Services.RedisServices; // Redis için using
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions; // NotFoundException için


namespace ETicaret.Application.Features.Products.Commands.Delete;

public class ProductDeleteCommand : IRequest<ProductDeleteResponseDto> // DTO döndürelim
{
    public int Id { get; set; }
    public class ProductDeleteCommandHandler : IRequestHandler<ProductDeleteCommand, ProductDeleteResponseDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly IRedisService _redisService; // Inject edildi

        public ProductDeleteCommandHandler(IProductRepository productRepository, IRedisService redisService) // Constructor güncellendi
        {
            _productRepository = productRepository;
            _redisService = redisService; // Atama yapıldı
        }

        public async Task<ProductDeleteResponseDto> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(filter: x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (product == null)
            {
                throw new NotFoundException($"Product with Id {request.Id} not found.");
            }

            await _productRepository.DeleteAsync(product, cancellationToken: cancellationToken);

            // Cache temizleme
            await _redisService.RemoveDataAsync("products"); // Genel liste anahtarı [cite: 2]
            // Silinen ürünün ID'sine özel cache varsa o da temizlenmeli
            // Örnek: await _redisService.RemoveDataAsync($"product:{request.Id}");


            return new ProductDeleteResponseDto { Id = request.Id, Message = "Ürün başarıyla silindi." };
        }
    }
}