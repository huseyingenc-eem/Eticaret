using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Update;

public class ProductUpdateCommand : IRequest<string>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Stock { get; set; }

    public int CategoryID { get; set; }

    public class ProductUpdateCommandHandler : IRequestHandler<ProductUpdateCommand, string>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductUpdateCommandHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(ProductUpdateCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            await _productRepository.UpdateAsync(product,cancellationToken);
            return "Ürün güncellendi.";
        }
    }
}
