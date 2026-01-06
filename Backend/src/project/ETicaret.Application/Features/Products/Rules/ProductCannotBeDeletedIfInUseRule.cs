using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Rules;

/// <summary>
/// Ürünün kullanımda olmadığı durumda silinebileceğini kontrol eden rule.
/// Ürün variants'ları siparişlerde, sepetlerde veya istek listelerinde varsa silinemez.
/// </summary>
public class ProductCannotBeDeletedIfInUseRule : IBusinessRule<DeleteProductCommand>
{
    private readonly IProductRepository _repository;

    public ProductCannotBeDeletedIfInUseRule(IProductRepository productRepository)
    {
        _repository = productRepository;
    }

    public bool ShouldExecute(DeleteProductCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ProductInUseSpec(command.Id);
        var productInUse = await _repository.GetAsync(spec, cancellationToken);

        if (productInUse != null)
        {
            throw new BusinessException(
                message: $"Product with ID {command.Id} cannot be deleted because it has active variants in use.",
                userFriendlyMessage: "Bu ürün aktif olarak kullanımda olduğu için silinemez. Önce tüm varyantları ve kullanımları kaldırın.",
                errorCode: ProductConstants.ErrorCodes.ProductInUse
            );
        }
    }

    public int Priority => 5;

    #region Private Specification - Bu rule'a özel

    private class ProductInUseSpec : Specification<Product>
    {
        public ProductInUseSpec(Guid productId)
            : base(p => p.Id == productId &&
                   p.Variants.Any(v => v.OrderItems.Any() || v.CartItems.Any() || v.WishlistItems.Any()))
        {
            AddInclude(p => p.Variants.Where(v => v.OrderItems.Any() || v.CartItems.Any() || v.WishlistItems.Any()));
        }
    }

    #endregion
}