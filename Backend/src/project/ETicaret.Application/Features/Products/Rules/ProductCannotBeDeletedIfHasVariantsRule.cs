using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Rules;

/// <summary>
/// Ürünün varyantları varsa silinemeyeceğini kontrol eden rule.
/// Önce tüm varyantların silinmesi gerekir.
/// </summary>
public class ProductCannotBeDeletedIfHasVariantsRule : IBusinessRule<DeleteProductCommand>
{
    private readonly IRepository<Product, Guid> _repository;

    public ProductCannotBeDeletedIfHasVariantsRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Product, Guid>();
    }

    public bool ShouldExecute(DeleteProductCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(DeleteProductCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ProductHasVariantsSpec(command.Id);
        var productWithVariants = await _repository.GetAsync(spec, cancellationToken);

        if (productWithVariants != null)
        {
            throw new BusinessException(
                message: $"Product with ID {command.Id} cannot be deleted because it has variants.",
                userFriendlyMessage: "Bu ürün varyantları olduğu için silinemez. Önce tüm varyantları silin.",
                errorCode: ProductConstants.ErrorCodes.ProductHasVariants
            );
        }
    }

    public int Priority => 4;

    #region Private Specification - Bu rule'a özel

    private class ProductHasVariantsSpec : Specification<Product>
    {
        public ProductHasVariantsSpec(Guid productId)
            : base(p => p.Id == productId && p.Variants.Any())
        {
            AddInclude(p => p.Variants);
        }
    }

    #endregion
}