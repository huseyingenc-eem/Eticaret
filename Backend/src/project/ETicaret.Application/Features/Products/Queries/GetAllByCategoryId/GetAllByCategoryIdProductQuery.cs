using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetAllByCategoryId;

public class GetAllByCategoryIdProductQuery: IRequest<List<GetAllByCategoryIdProductResponseDto>>
{
    public int CategoryId { get; set; }

    public class GetAllByCategoryIdProductQueryHandler :
        IRequestHandler<GetAllByCategoryIdProductQuery, List<GetAllByCategoryIdProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetAllByCategoryIdProductQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<List<GetAllByCategoryIdProductResponseDto>> Handle(GetAllByCategoryIdProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetAllAsync(x =>
            x.CategoryID == request.CategoryId,
            enableTracking: false,
            cancellationToken:cancellationToken
            );

            var responses = _mapper.Map<List<GetAllByCategoryIdProductResponseDto>>(products);

            return responses;
        }
    }
}
