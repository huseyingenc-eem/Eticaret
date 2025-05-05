using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetListPriceRange;

public class GetListProductPriceRangeQuery: IRequest<List<GetListProductPriceRangeResponseDto>>
{
    public double Min { get; set; }
    public double Max { get; set; }

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
            decimal min = (decimal)request.Min;
            decimal max = (decimal)request.Max;
            var products = await _productRepository.GetAllAsync
                ( filter: x=>
                    x.Price<=max && x.Price>=min ,
                    enableTracking:false,
                    cancellationToken:cancellationToken
                );
            var response = _mapper.Map<List<GetListProductPriceRangeResponseDto>>(products);

            return response;

        }
    }
}
