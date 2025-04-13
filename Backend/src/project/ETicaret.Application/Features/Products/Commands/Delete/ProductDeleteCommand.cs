using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Delete;

public class ProductDeleteCommand : IRequest<string>
{
    public int Id { get; set; }
    public class ProductDeleteCommandHandler : IRequestHandler<ProductDeleteCommand, string>
    {
        private readonly IProductRepository _productRepository;

        public ProductDeleteCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<string> Handle(ProductDeleteCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetAsync(filter: x => x.Id == request.Id,cancellationToken:cancellationToken);
            await _productRepository.DeleteAsync(product, cancellationToken: cancellationToken);
            return "Ürün Silindi.";
        }
    }
}