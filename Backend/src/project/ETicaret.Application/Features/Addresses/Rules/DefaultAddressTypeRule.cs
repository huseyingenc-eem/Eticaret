using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// Default address type kontrolü yapan tek sorumlu rule
/// </summary>
public class DefaultAddressTypeRule :
    IBusinessRule<CreateAddressCommand>,
    IBusinessRule<UpdateAddressCommand>
{
    public bool ShouldExecute(CreateAddressCommand command) => true;
    public bool ShouldExecute(UpdateAddressCommand command) => true;

    public Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDefaultAddressType(command.IsDefaultBilling, command.IsDefaultShipping);
        return Task.CompletedTask;
    }

    public Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
    {
        ValidateDefaultAddressType(command.IsDefaultBilling, command.IsDefaultShipping);
        return Task.CompletedTask;
    }

    private static void ValidateDefaultAddressType(bool isDefaultBilling, bool isDefaultShipping)
    {
        if (!isDefaultBilling && !isDefaultShipping)
        {
            throw new BusinessException(
                message: "Address must be marked as either default billing or default shipping.",
                userFriendlyMessage: "Adres en az fatura veya kargo adresi olarak işaretlenmelidir.",
                errorCode: "DEFAULT_ADDRESS_TYPE_REQUIRED"
            );
        }
    }

    public int Priority => 4;
}