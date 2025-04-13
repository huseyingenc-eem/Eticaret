using AutoMapper;
using ETicaret.Application.Services.RedisServices;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetList;

public class GetlistProductQuery : IRequest<List<GetListProductResponseDto>>
{
    public int Index { get; set; }
    public int Size { get; set; }

    public class GetlistProductQueryHandler : IRequestHandler<GetlistProductQuery, List<GetListProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _repository;
        private readonly IRedisService _redisService;

        public GetlistProductQueryHandler(IMapper mapper, IProductRepository repository, IRedisService redisService)
        {
            _mapper = mapper;
            _repository = repository;
            _redisService = redisService;
        }

        public async Task<List<GetListProductResponseDto>> Handle(GetlistProductQuery request, CancellationToken cancellationToken)
        {
            var cachedData = await _redisService.GetDataAsync<List<GetListProductResponseDto>>("products");
            if (cachedData != null)
                return cachedData;


            List<Product> products = await _repository.GetAllAsync(enableTracking: false, cancellationToken: cancellationToken);

            var responses = _mapper.Map<List<GetListProductResponseDto>>(products);

            await _redisService.AddDataAsync($"products({request.Index},{request.Size})", responses);
            return responses;
        }
    }
}
