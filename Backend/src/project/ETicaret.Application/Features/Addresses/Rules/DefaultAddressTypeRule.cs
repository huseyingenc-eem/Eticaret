using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;

namespace ETicaret.Application.Features.Addresses.Rules;

#region Default Address Type Rule Implementation

/// <summary>
/// En az bir varsayılan adres tipinin seçilmesini kontrol eden kural.
/// Bu kural Create ve Update komutlarında çalışır.
/// Single Responsibility Principle gereği sadece varsayılan adres tipi kontrolü yapar.
/// </summary>
public class DefaultAddressTypeRule :
    IRule<CreateAddressCommand>,
    IRule<UpdateAddressCommand>
{
    #region Properties

    public int Priority => 4;
    public string RuleName => nameof(DefaultAddressTypeRule);

    #endregion

    #region Fields
    private readonly AddressBusinessRules _addressBusinessRules;
    #endregion
    #region Constructor
    public DefaultAddressTypeRule(AddressBusinessRules addressBusinessRules)
    {
        _addressBusinessRules = addressBusinessRules;
    }
    #endregion

    #region Rule Implementations

    /// <summary>
    /// CreateAddressCommand için varsayılan adres tipi kontrolü.
    /// En az bir adres tipinin (fatura veya kargo) seçildiğinden emin olur.
    /// </summary>
    /// <param name="command">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        _addressBusinessRules.CheckAtLeastOneDefaultAddressType(
            command.IsDefaultBilling, command.IsDefaultShipping);
        return Task.CompletedTask;
    }

    /// <summary>
    /// UpdateAddressCommand için varsayılan adres tipi kontrolü.
    /// Güncelleme sırasında en az bir adres tipinin seçili olduğundan emin olur.
    /// </summary>
    /// <param name="command">Update address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
    {
        _addressBusinessRules.CheckAtLeastOneDefaultAddressType(
            command.IsDefaultBilling, command.IsDefaultShipping);
        return Task.CompletedTask;
    }

    #endregion
}

#endregion