using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetDetails;

public class GetDetailsProductQuery : IRequest<List<GetDetailsProductResponseDto>>
{
    public class GetDetailsProductQueryHandler : IRequestHandler<GetDetailsProductQuery, List<GetDetailsProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetDetailsProductQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<List<GetDetailsProductResponseDto>> Handle(GetDetailsProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(enableTracking:false,cancellationToken:cancellationToken);

            var response = _mapper.Map<List<GetDetailsProductResponseDto>>(products);

            return response;
        }
    }
}
