using AutoMapper;
using Core.Application.Pipelines.Authorization;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Create;

public class CategoryAddCommand : IRequest<CategoryAddResponseDto>, IRoleExists
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public string[] Roles => ["Admin"];

    public class CategoryAddCommandHandler : IRequestHandler<CategoryAddCommand, CategoryAddResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryAddCommandHandler(ICategoryRepository categoryRepository, IMapper mapper) // Constructor güncellendi
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper; // Atama yapıldı
        }

        public async Task<CategoryAddResponseDto> Handle(CategoryAddCommand request, CancellationToken cancellationToken)
        {
            Category category = _mapper.Map<Category>(request);

            Category addedCategory = await _categoryRepository.AddAsync(category, cancellationToken);

            CategoryAddResponseDto response = _mapper.Map<CategoryAddResponseDto>(addedCategory);
            response.Message = "Kategori başarıyla eklendi.";
            return response;
        }
    }
}