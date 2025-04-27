using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Application.Pipelines.Authorization;
// using ETicaret.Application.Services.RedisServices;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

public class SupplierDeleteCommand : IRequest<SupplierDeleteResponseDto>  , IRoleExists
{
    public int Id { get; set; }

    public string[] Roles => ["Admin"];

    public class SupplierDeleteCommandHandler : IRequestHandler<SupplierDeleteCommand, SupplierDeleteResponseDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        // private readonly IRedisService _redisService; // Cache için

        public SupplierDeleteCommandHandler(ISupplierRepository supplierRepository /*, IRedisService redisService */)
        {
            _supplierRepository = supplierRepository;
            // _redisService = redisService;
        }

        public async Task<SupplierDeleteResponseDto> Handle(SupplierDeleteCommand request, CancellationToken cancellationToken)
        {
            // Silinecek tedarikçiyi bul
            Supplier? supplierToDelete = await _supplierRepository.GetAsync(filter: s => s.Id == request.Id, cancellationToken: cancellationToken);

            if (supplierToDelete == null)
                throw new NotFoundException($"Supplier with Id {request.Id} not found.");

            // ÖNEMLİ NOT: SupplierConfiguration'da Product ilişkisi için OnDelete(DeleteBehavior.Restrict)
            // ayarladığımız için, eğer bu tedarikçiye bağlı ürünler varsa, aşağıdaki DeleteAsync
            // işlemi veritabanı seviyesinde DbUpdateException fırlatacaktır. Bu hatayı
            // Presentation katmanındaki global exception handler (HttpExceptionHandler) yakalayıp
            // kullanıcıya uygun bir mesaj gösterebilir ("İlişkili ürünleri olan tedarikçi silinemez" gibi).
            // Alternatif olarak, burada silmeden önce ürün kontrolü yapılabilir:
            // bool hasProducts = await _supplierRepository.AnyAsync(predicate: s => s.Id == request.Id && s.Products.Any(), cancellationToken: cancellationToken);
            // if (hasProducts) throw new BusinessException("Bu tedarikçiye bağlı ürünler olduğundan silemezsiniz.");

            await _supplierRepository.DeleteAsync(supplierToDelete, cancellationToken);

            // Cache temizleme eklenebilir (varsa)
            // await _redisService.RemoveDataAsync("suppliers");
            // await _redisService.RemoveDataAsync($"supplier:{request.Id}");

            return new SupplierDeleteResponseDto { Id = request.Id, Message = "Tedarikçi başarıyla silindi." };
        }
    }
}