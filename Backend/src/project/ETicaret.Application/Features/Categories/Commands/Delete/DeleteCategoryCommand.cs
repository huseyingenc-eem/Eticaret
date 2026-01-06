using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Delete;

[DefaultRoles("Admin")]
public class DeleteCategoryCommand : IRequest<DeleteCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }

    #region Cache Settings
    public string CacheKey => $"category:{Id}";
    public string? CacheGroupKey => "Categories";
    public bool BypassCache { get; set; }
    #endregion

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<DeleteCategoryResponseDto> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var spec = new CategorySpecifications.ById(request.Id);
            var categoryToDelete = (await _categoryRepository.GetAsync(spec, cancellationToken))!;

            await _categoryRepository.DeleteAsync(categoryToDelete, permanent: false, cancellationToken);

            DeleteCategoryResponseDto response = _mapper.Map<DeleteCategoryResponseDto>(categoryToDelete);
            response.Message = "Kategori başarıyla silindi.";

            return response;
        }
    }
}