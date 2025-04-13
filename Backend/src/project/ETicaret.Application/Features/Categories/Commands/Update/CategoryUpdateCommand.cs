using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class CategoryUpdateCommand : IRequest<string>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? ParentId { get; set; }

    public class CategoryUpdateCommandHandler : IRequestHandler<CategoryUpdateCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryUpdateCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(c => c.Id == request.Id, cancellationToken: cancellationToken);

            if (category == null)
                return "Kategori bulunamadı.";

            category.Name = request.Name ?? category.Name;
            category.ParentId = request.ParentId;

            await _categoryRepository.UpdateAsync(category, cancellationToken);
            return "Kategori güncellendi.";
        }
    }
}