
using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetListPriceRange;

public class GetListProductPriceRangeQuery : IRequest<List<GetListProductPriceRangeResponseDto>>
{
    public decimal Min { get; set; }
    public decimal Max { get; set; }

    public class GetListProductPriceRangeQueryHandler : IRequestHandler<GetListProductPriceRangeQuery, List<GetListProductPriceRangeResponseDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetListProductPriceRangeQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<GetListProductPriceRangeResponseDto>> Handle(GetListProductPriceRangeQuery request, CancellationToken cancellationToken)
        {
           
            var products = await _productRepository.GetAllAsync
                (filter: x =>
                    x.Price <= request.Max && x.Price >= request.Min,
                    enableTracking: false,
                    cancellationToken: cancellationToken
                );
            var response = _mapper.Map<List<GetListProductPriceRangeResponseDto>>(products);

            return response;
        }
    }
}