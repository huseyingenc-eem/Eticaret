using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;

public class GetCategoryWithProductsQuery : IRequest<GetCategoryWithProductsResponseDto>
{
    public int Id { get; set; }

    public class GetCategoryWithProductsQueryHandler : IRequestHandler<GetCategoryWithProductsQuery, GetCategoryWithProductsResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public GetCategoryWithProductsQueryHandler(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<GetCategoryWithProductsResponseDto> Handle(GetCategoryWithProductsQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetAsync(filter: x =>
                x.Id == request.Id,
                cancellationToken: cancellationToken
            );
            var response = _mapper.Map<GetCategoryWithProductsResponseDto>(category);

            return response;
        }
    }
}
