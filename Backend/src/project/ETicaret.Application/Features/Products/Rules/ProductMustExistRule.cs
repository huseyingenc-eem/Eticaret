using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Rules;

/// <summary>
/// Product'un mevcut olduğunu kontrol eden tek sorumlu rule.
/// </summary>
public class ProductMustExistRule :
    IBusinessRule<UpdateProductCommand>,
    IBusinessRule<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public ProductMustExistRule(IProductRepository productRepository)
    {
        _repository = productRepository;
    }

    public bool ShouldExecute(UpdateProductCommand command) => command.Id != Guid.Empty;
    public bool ShouldExecute(DeleteProductCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateProductExists(command.Id, cancellationToken);
    }

    public async Task ExecuteAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateProductExists(command.Id, cancellationToken);
    }

    private async Task ValidateProductExists(Guid productId, CancellationToken cancellationToken)
    {
        var spec = new ByIdSpec(productId);
        var product = await _repository.GetAsync(spec, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(
                message: $"Product with ID {productId} not found.",
                userFriendlyMessage: "Belirtilen ürün bulunamadı.",
                errorCode: ProductConstants.ErrorCodes.ProductNotFound
            );
        }
    }

    public int Priority => 0;

    #region Private Specification - Bu rule'a özel

    private class ByIdSpec : Specification<Product>
    {
        public ByIdSpec(Guid id)
            : base(product => product.Id == id) { }
    }

    #endregion
}