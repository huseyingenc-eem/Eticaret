using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Specifications; // <-- Spesifikasyonumuzu ekliyoruz
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Update;

/// <summary>
/// Mevcut bir ürünü güncelleme işlemini temsil eden komut.
/// ITransactionalRequest: Bu komutun bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: Bu komut başarılı olduğunda ilgili önbelleği otomatik olarak temizler.
/// </summary>
public class UpdateProductCommand : IRequest<UpdateProductResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    #region Komut Parametreleri
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryID { get; set; }
    public int SupplierID { get; set; }
    public string? Description { get; set; }
    public string? SKU { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    #endregion

    #region Önbellek Ayarları
    // Bu komut başarılı olduğunda, hem bu ürüne ait spesifik cache'i
    // hem de tüm ürün listelerini içeren grubu temizle.
    public string CacheKey => $"product:{Id}";
    public string? CacheGroupKey => "ProductsGroup";
    public bool BypassCache { get; set; }
    #endregion

    /// <summary>
    /// UpdateProductCommand isteğini işleyen Handler.
    /// </summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponseDto>
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IMapper _mapper;

        // IRedisService ve IUnitOfWork bağımlılıkları, Behavior'lar sayesinde artık gerekli değil.
        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            // Repository'yi IUnitOfWork üzerinden alıyoruz.
            _productRepository = unitOfWork.GetRepository<Product, Guid>();
        }

        public async Task<UpdateProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Spesifikasyonu kullanarak güncellenecek ürünü al.
            var spec = new ProductByIdSpecification(request.Id);
            Product? productToUpdate = await _productRepository.GetAsync(spec, cancellationToken);

            if (productToUpdate == null)
                throw new NotFoundException($"{request.Id} kimliğine sahip ürün bulunamadı.");

            // (Varsa) İş kurallarını burada çalıştır.
            // await _productBusinessRules.CheckSomethingAsync(...);

            // 2. Gelen verileri veritabanından çekilen entity'e map'le.
            _mapper.Map(request, productToUpdate);

            // 3. Entity'nin güncellenmek üzere işaretlenmesini sağla.
            await _productRepository.UpdateAsync(productToUpdate, cancellationToken);


            UpdateProductResponseDto response = _mapper.Map<UpdateProductResponseDto>(productToUpdate);
            response.Message = "Ürün başarıyla güncellendi.";
            return response;
        }
    }
}