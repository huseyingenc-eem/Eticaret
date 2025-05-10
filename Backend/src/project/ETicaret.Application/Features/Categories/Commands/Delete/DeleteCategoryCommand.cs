using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Transactional;
using Core.CrossCuttingConcerns.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Rules; // CategoryBusinessRules için
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ETicaret.Application.Features.Categories.Commands.Delete;

public class DeleteCategoryCommand : IRequest<DeleteCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    /// <summary>
    /// Silinecek kategorinin Id'si.
    /// </summary>
    public int Id { get; set; }
    public string? UserId { get; set; } 

    public string CacheKey => $"category:{Id}";
    public string? CacheGroupKey => "CategoriesGroup";
    public bool ByPassCache { get; set; }

    public class CategoryDeleteCommandHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public CategoryDeleteCommandHandler(IUnitOfWork unitOfWork, CategoryBusinessRules categoryBusinessRules)
        {
            _unitOfWork = unitOfWork;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<DeleteCategoryResponseDto> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            Category? categoryToDelete = await _unitOfWork.CategoryRepository.GetAsync(
                filter: x => x.Id == request.Id,
                enableTracking: true,
                cancellationToken: cancellationToken
            );

            if (categoryToDelete == null)
                throw new NotFoundException($"Silinecek kategori bulunamadı (ID: {request.Id}).");

            await _categoryBusinessRules.CheckIfCategoryHasActiveChildCategoriesAsync(request.Id, cancellationToken);

            await _unitOfWork.CategoryRepository.DeleteAsync(categoryToDelete, permanent: false, cancellationToken);


            return new DeleteCategoryResponseDto
            {
                Id = request.Id,
                Message = "Kategori başarıyla silindi (pasif hale getirildi).", // Mesaj güncellendi
                IsSuccess = true
            };
        }
    }
}
