using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.ProductVariants.Commands.Create;
using ETicaret.Application.Features.ProductVariants.Commands.Update;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.ProductVariants.Rules;


public class SkuMustBeUniqueRule :
    IBusinessRule<CreateProductVariantCommand>,
    IBusinessRule<UpdateProductVariantCommand>
{
    private readonly IProductVariantRepository _repository;

    public SkuMustBeUniqueRule(IProductVariantRepository productVariantRepository)
    {
        _repository = productVariantRepository;
    }

    public int Priority => 2;

    public bool ShouldExecute(CreateProductVariantCommand command) => !string.IsNullOrWhiteSpace(command.Sku);
    public bool ShouldExecute(UpdateProductVariantCommand command) => !string.IsNullOrWhiteSpace(command.Sku);

    public async Task ExecuteAsync(CreateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new BySkuSpec(command.Sku);
        var existingVariant = await _repository.GetAsync(spec, cancellationToken);

        if (existingVariant != null)
        {
            throw new BusinessException(
                message: $"SKU '{command.Sku}' already exists.",
                userFriendlyMessage: "Bu SKU zaten kullanılmaktadır.",
                errorCode: ProductVariantConstants.ErrorCodes.DuplicateSku
            );
        }
    }

    public async Task ExecuteAsync(UpdateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new BySkuExcludingIdSpec(command.Id, command.Sku);
        var existingVariant = await _repository.GetAsync(spec, cancellationToken);

        if (existingVariant != null)
        {
            throw new BusinessException(
                message: $"SKU '{command.Sku}' already exists.",
                userFriendlyMessage: "Bu SKU zaten kullanılmaktadır.",
                errorCode: ProductVariantConstants.ErrorCodes.DuplicateSku
            );
        }
    }

    #region Private Specifications
    private class BySkuSpec : Specification<ProductVariant>
    {
        public BySkuSpec(string sku) : base(pv => pv.Sku.ToLower() == sku.ToLower()) { }
    }

    private class BySkuExcludingIdSpec : Specification<ProductVariant>
    {
        public BySkuExcludingIdSpec(Guid excludeId, string sku)
            : base(pv => pv.Id != excludeId && pv.Sku.ToLower() == sku.ToLower()) { }
    }
    #endregion
}