using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetById;

public class GetByIdProductQuery : IRequest<GetByIdProductResponseDto>
{
    public int Id { get; set; }



    public class GetByIdProductQueryHandler : IRequestHandler<GetByIdProductQuery, GetByIdProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetByIdProductQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<GetByIdProductResponseDto> Handle(GetByIdProductQuery request, CancellationToken cancellationToken)
        {
            Product? product = await _productRepository.GetAsync(filter:x=>x.Id==request.Id,enableTracking:false,cancellationToken:cancellationToken);
            var response = _mapper.Map<GetByIdProductResponseDto>(product);

            return response;
        }
    }


}
