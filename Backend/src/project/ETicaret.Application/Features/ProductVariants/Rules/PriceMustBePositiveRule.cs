using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.ProductVariants.Commands.Create;
using ETicaret.Application.Features.ProductVariants.Commands.Update;
using ETicaret.Application.Features.ProductVariants.Constants;

namespace ETicaret.Application.Features.ProductVariants.Rules;

/// <summary>
/// Fiyatın pozitif olması gerektiğini kontrol eder.
/// </summary>
public class PriceMustBePositiveRule :
    IBusinessRule<CreateProductVariantCommand>,
    IBusinessRule<UpdateProductVariantCommand>
{
    public int Priority => 1;

    public bool ShouldExecute(CreateProductVariantCommand command) => true;
    public bool ShouldExecute(UpdateProductVariantCommand command) => true;

    public Task ExecuteAsync(CreateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        ValidatePrice(command.Price);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateProductVariantCommand command, CancellationToken cancellationToken = default)
    {
        ValidatePrice(command.Price);
        return Task.CompletedTask;
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
        {
            throw new BusinessException(
                message: $"Price must be positive. Current value: {price}",
                userFriendlyMessage: "Fiyat pozitif bir değer olmalıdır.",
                errorCode: ProductVariantConstants.ErrorCodes.PriceMustBePositive
            );
        }
    }
}