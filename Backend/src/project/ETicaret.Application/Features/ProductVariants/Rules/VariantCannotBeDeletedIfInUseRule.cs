using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.ProductVariants.Commands.Delete;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using ETicaret.Domain.Enums;

namespace ETicaret.Application.Features.ProductVariants.Rules;

public class VariantCannotBeDeletedIfInUseRule : IBusinessRule<DeleteProductVariantCommand>
{
    private readonly IProductVariantRepository _repository;

    public VariantCannotBeDeletedIfInUseRule(IProductVariantRepository productVariantRepository)
    {
        _repository = productVariantRepository;
    }

    public int Priority => 3;

    public bool ShouldExecute(DeleteProductVariantCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(DeleteProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        if (!command.IsHardDelete)
            return;

        var spec = new VariantInCriticalUseSpec(command.Id);
        var variantInUse = await _repository.GetAsync(spec, cancellationToken);

        if (variantInUse != null)
        {
            var usageDetails = GetCriticalUsageDetails(variantInUse);

            throw new BusinessException(
                message: $"ProductVariant with ID {command.Id} cannot be permanently deleted because it is in critical use. Usage: {usageDetails}",
                userFriendlyMessage: $"Bu ürün varyantı kalıcı olarak silinemez çünkü aktif kullanımda. {usageDetails} " +
                                     "Ürünü geçici olarak devre dışı bırakmak için soft delete kullanabilirsiniz.",
                errorCode: ProductVariantConstants.ErrorCodes.VariantInUse
            );
        }
    }

    private static string GetCriticalUsageDetails(ProductVariant variant)
    {
        var usages = new List<string>();
        var activeOrders = variant.OrderItems
            .Count(oi => oi.Order != null &&
                         (oi.Order.Status == OrderStatus.Pending ||
                          oi.Order.Status == OrderStatus.Processing || 
                          oi.Order.Status == OrderStatus.Shipped));

        if (activeOrders > 0)
            usages.Add($"{activeOrders} aktif sipariş");

        if (variant.CartItems.Any())
            usages.Add($"{variant.CartItems.Count} sepet");

        return usages.Any()
            ? string.Join(", ", usages) + " kaydında bulunuyor."
            : "Bilinmeyen kritik kullanım.";
    }

    #region Private Specification

    private class VariantInCriticalUseSpec : Specification<ProductVariant>
    {
        public VariantInCriticalUseSpec(Guid variantId)
            : base(pv => pv.Id == variantId &&
                        (
                          pv.OrderItems.Any(oi =>
                              oi.Order.Status == OrderStatus.Pending ||
                              oi.Order.Status == OrderStatus.Processing ||
                              oi.Order.Status == OrderStatus.Shipped)
                          ||
                          pv.CartItems.Any()
                        ))
        {
            AddInclude(pv => pv.OrderItems);
            AddInclude(pv => pv.OrderItems.Select(oi => oi.Order));
            AddInclude(pv => pv.CartItems);
        }
    }

    #endregion
}
