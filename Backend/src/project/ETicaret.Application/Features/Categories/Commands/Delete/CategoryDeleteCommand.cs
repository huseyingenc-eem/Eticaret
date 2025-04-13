using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class CategoryDeleteCommand : IRequest<string>
{
    public int Id { get; set; }

    public class CategoryDeleteCommandHandler : IRequestHandler<CategoryDeleteCommand, string>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryDeleteCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CategoryDeleteCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (category == null)
                return "Kategori bulunamadı.";

            await _categoryRepository.DeleteAsync(category, cancellationToken: cancellationToken);
            return "Kategori silindi.";
        }
    }
}