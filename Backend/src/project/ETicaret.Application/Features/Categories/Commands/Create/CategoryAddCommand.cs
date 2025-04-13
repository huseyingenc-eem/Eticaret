using Core.Application.Pipelines.Authorization;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Create;

public class CategoryAddCommand : IRequest<Category>, IRoleExists
{
    public string? Name { get; set; }
    public int? ParentId { get; set; }
    public string[] Roles => ["Admin"];

    public class CategoryAddCommandHandler : IRequestHandler<CategoryAddCommand, Category>
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryAddCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> Handle(CategoryAddCommand request, CancellationToken cancellationToken)
        {
            Category category = new Category
            {
                Name = request.Name,
                ParentId = request.ParentId
            };

            await _categoryRepository.AddAsync(category, cancellationToken);
            return category;
        }
    }
}