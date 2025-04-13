using AutoMapper;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetListNameContains;

public class GetListProductNameContainsQuery : IRequest<List<GetListProductNameContainsResponseDto>>
{
    public string Text { get; set; }


    public class GetListProductNameContainsQueryHandler : IRequestHandler<GetListProductNameContainsQuery, List<GetListProductNameContainsResponseDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public GetListProductNameContainsQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<GetListProductNameContainsResponseDto>> Handle(GetListProductNameContainsQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync
                (   filter: x=>
                    x.Name.Contains(request.Text),
                    enableTracking:false,
                    cancellationToken:cancellationToken
                );
            var response = _mapper.Map<List<GetListProductNameContainsResponseDto>>(products);
            return response;
        }
    }
}
