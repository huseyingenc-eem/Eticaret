using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Services.Repositories;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

[DefaultRoles("Admin")]
public class UpdateCategoryCommand : IRequest<UpdateCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    #region Cache Settings
    public string? CacheKey => $"category-detail_{Id}";
    public string? CacheGroupKey => "Categories";
    #endregion

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<UpdateCategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var spec = new CategorySpecifications.ById(request.Id);
            Category categoryToUpdate = (await _categoryRepository.GetAsync(spec, cancellationToken))!;

            _mapper.Map(request, categoryToUpdate);

            await _categoryRepository.UpdateAsync(categoryToUpdate, cancellationToken);

            UpdateCategoryResponseDto response = _mapper.Map<UpdateCategoryResponseDto>(categoryToUpdate);
            response.Message = "Kategori başarıyla güncellendi.";

            return response;
        }
    }
}