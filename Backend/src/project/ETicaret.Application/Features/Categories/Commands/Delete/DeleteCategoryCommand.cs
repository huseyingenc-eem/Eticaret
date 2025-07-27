using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Rules;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Delete;

/// <summary>
/// Mevcut bir kategoriyi silme (pasif hale getirme) işlemini temsil eden komut.
/// ITransactionalRequest: Bu işlemin bir transaction içinde çalışmasını sağlar.
/// ICacheRemoverRequest: İşlem başarılı olduğunda ilgili önbelleği temizler.
/// </summary>
public class DeleteCategoryCommand : IRequest<DeleteCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    #region Önbellek Ayarları
    // Silme işlemi hem belirli bir kategorinin detayını hem de kategori listelerini geçersiz kılar.
    public string CacheKey => $"category:{Id}";
    public string? CacheGroupKey => "CategoriesGroup"; // Kategori listelerinin olduğu grup
    public bool BypassCache { get; set; }
    #endregion

    /// <summary>
    /// DeleteCategoryCommand isteğini işleyen Handler.
    /// </summary>
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponseDto>
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, CategoryBusinessRules categoryBusinessRules)
        {
            _categoryBusinessRules = categoryBusinessRules;
            // Bağımlılığımızı somut repository yerine IUnitOfWork üzerinden alıyoruz.
            _categoryRepository = unitOfWork.GetRepository<Category, int>();
        }

        public async Task<DeleteCategoryResponseDto> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1. Silinecek kategoriyi, yeniden kullanılabilir spesifikasyonumuz ile veritabanından alıyoruz.
            var spec = new CategorySpecifications.ById(request.Id);
            Category? categoryToDelete = await _categoryRepository.GetAsync(spec, cancellationToken);

            // 2. Varlık Kontrolü: Kategori bulunamazsa hata fırlat.
            if (categoryToDelete == null)
                throw new NotFoundException($"Silinecek kategori bulunamadı (ID: {request.Id}).");

            // 3. İş Kurallarını Uygula: Silmeden önce, bu kategorinin aktif alt kategorileri olup olmadığını kontrol et.
            await _categoryBusinessRules.CheckIfCategoryHasActiveChildCategoriesAsync(request.Id, cancellationToken);

            // 4. Silme İşlemini Gerçekleştir: Repository üzerinden soft delete (geçici silme) işlemi yap.
            // 'permanent: false' bayrağı, DbContext'teki kuralımızın 'DeletedTime' alanını doldurmasını tetikleyecektir.
            await _categoryRepository.DeleteAsync(categoryToDelete, permanent: false, cancellationToken);

            // 5. CompleteAsync çağrısı burada gerekli değil. TransactionBehavior bu işi bizim için otomatik yapacak.

            return new DeleteCategoryResponseDto
            {
                Id = request.Id,
                Message = "Kategori başarıyla silindi (pasif hale getirildi).",
                IsSuccess = true
            };
        }
    }
}