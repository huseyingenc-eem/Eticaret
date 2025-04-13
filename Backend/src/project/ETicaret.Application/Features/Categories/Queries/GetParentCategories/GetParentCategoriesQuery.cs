using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetParentCategories;

public class GetParentCategoriesQuery : IRequest<List<GetParentCategoriesResponseDto>>
{
    public class GetParentCategoriesQueryHandler : IRequestHandler<GetParentCategoriesQuery, List<GetParentCategoriesResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public GetParentCategoriesQueryHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<GetParentCategoriesResponseDto>> Handle(GetParentCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(c =>
                    c.ParentId == null,
                    cancellationToken: cancellationToken
                );
            var response = _mapper.Map<List<GetParentCategoriesResponseDto>>(categories);
            return response;
        }
    }
}
