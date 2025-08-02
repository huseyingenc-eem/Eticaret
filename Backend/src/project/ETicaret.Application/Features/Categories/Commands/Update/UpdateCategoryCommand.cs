using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
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
    public string CacheKey => $"category:{Id}";
    public string? CacheGroupKey => "Categories";
    public bool BypassCache { get; set; }
    #endregion

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponseDto>
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _categoryRepository = unitOfWork.GetRepository<Category, int>();
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