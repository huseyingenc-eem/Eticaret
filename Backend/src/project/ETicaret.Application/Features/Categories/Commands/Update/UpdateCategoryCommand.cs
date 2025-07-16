using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Transactional;
using Core.Shared.Exceptions;
using ETicaret.Application.Features.Categories.Rules;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class UpdateCategoryCommand : IRequest<UpdateCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public string CacheKey => $"category:{Id}";
    public string? CacheGroupKey => "CategoriesGroup";
    public bool ByPassCache { get; set; } 

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules; 

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper,
            CategoryBusinessRules categoryBusinessRules) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules;
        }

        public async Task<UpdateCategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1. Güncellenecek kategoriyi veritabanından al.
            Category? categoryToUpdate = await _categoryRepository.GetAsync(
                filter: c => c.Id == request.Id, 
                enableTracking: true,
                cancellationToken: cancellationToken);

            if (categoryToUpdate == null)
            {
                // Hata mesajı düzeltildi: Kategori bulunamadı.
                throw new NotFoundException($"Category with Id {request.Id} not found.");
            }

            if (categoryToUpdate.Name.Trim().ToLower() != request.Name.Trim().ToLower())
            {
                await _categoryBusinessRules.CheckCategoryNameCanNotBeDuplicatedWhenUpdatedAsync(request.Id, request.Name, cancellationToken);
            }

            _categoryBusinessRules.CheckCategoryCanNotBeItsOwnParent(request.Id, request.ParentId);

            await _categoryBusinessRules.CheckParentCategoryExistsAsync(request.ParentId, cancellationToken, checkIfActive: true);

            await _categoryBusinessRules.CheckNoCyclicDependencyWhenUpdatingParentAsync(request.Id, request.ParentId, cancellationToken);
            

            
            _mapper.Map(request, categoryToUpdate);

            
            await _categoryRepository.UpdateAsync(categoryToUpdate, cancellationToken);

            UpdateCategoryResponseDto response = _mapper.Map<UpdateCategoryResponseDto>(categoryToUpdate);
            response.Message = "Kategori başarıyla güncellendi.";

            return response;
        }
    }
}
