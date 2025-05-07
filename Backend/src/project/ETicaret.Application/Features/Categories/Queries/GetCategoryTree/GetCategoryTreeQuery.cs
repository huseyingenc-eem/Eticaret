using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeQuery : IRequest<List<GetCategoryTreeResponseDto>>
{
    public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, List<GetCategoryTreeResponseDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryTreeQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<GetCategoryTreeResponseDto>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
        {
            var allCategories = await _categoryRepository.GetAllAsync(
                include: true,
                cancellationToken: cancellationToken);

            var categoryDtos = _mapper.Map<List<GetCategoryTreeResponseDto>>(allCategories);

            var result = categoryDtos
                .Where(c => c.ParentId == null)
                .Select(parent => BuildCategoryTree(parent, categoryDtos))
                .ToList();

            return result;
        }

        private GetCategoryTreeResponseDto BuildCategoryTree(GetCategoryTreeResponseDto category, List<GetCategoryTreeResponseDto> allCategories)
        {
            category.Children = allCategories
                .Where(c => c.ParentId == category.Id)
                .Select(child => BuildCategoryTree(child, allCategories))
                .ToList();

            return category;
        }
    }

}
