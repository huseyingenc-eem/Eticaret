using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Domain.Entities;
using MediatR;
using Core.Application.Behaviors.Authorization;
using ETicaret.Application.Services.Repositories;

namespace ETicaret.Application.Features.Categories.Commands.Create;

[DefaultRoles("Admin")]
public class CreateCategoryCommand : IRequest<CreateCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public string? CacheKey => null;
    public string? CacheGroupKey => "Categories";

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CreateCategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            Category category = _mapper.Map<Category>(request);
            await _categoryRepository.AddAsync(category, cancellationToken);

            CreateCategoryResponseDto response = _mapper.Map<CreateCategoryResponseDto>(category);
            response.Message = "Kategori başarıyla eklendi.";

            return response;
        }
    }
}