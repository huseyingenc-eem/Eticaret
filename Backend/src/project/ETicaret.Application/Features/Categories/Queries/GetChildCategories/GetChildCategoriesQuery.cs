using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetChildCategories;

public class GetChildCategoriesQuery : IRequest<List<GetChildCategoriesResponseDto>>
{
    public int ParentId { get; set; }

    public class GetListCategoryQueryHandler : IRequestHandler<GetChildCategoriesQuery, List<GetChildCategoriesResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public GetListCategoryQueryHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<GetChildCategoriesResponseDto>> Handle(GetChildCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(c =>
                    c.ParentId == request.ParentId,
                    cancellationToken: cancellationToken
                );
            var response = _mapper.Map<List<GetChildCategoriesResponseDto>>(categories);

            return response;
        }
    }
}
