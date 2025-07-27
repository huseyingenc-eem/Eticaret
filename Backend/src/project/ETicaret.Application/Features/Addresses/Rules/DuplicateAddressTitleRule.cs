using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;

namespace ETicaret.Application.Features.Addresses.Rules;

#region Duplicate Address Title Rule Implementation

/// <summary>
/// Aynı kullanıcıda tekrar eden adres başlığını kontrol eden kural.
/// Bu kural Create ve Update komutlarında çalışır.
/// Single Responsibility Principle gereği sadece adres başlığı tekrarı kontrolü yapar.
/// </summary>
public class DuplicateAddressTitleRule :
    IRule<CreateAddressCommand>,
    IRule<UpdateAddressCommand>
{
    #region Properties
    public int Priority => 3;
    public string RuleName => nameof(DuplicateAddressTitleRule);

    #endregion

    #region Fields
    private readonly AddressBusinessRules _addressBusinessRules;
    #endregion
    #region Constructor
    public DuplicateAddressTitleRule(AddressBusinessRules addressBusinessRules)
    {
        _addressBusinessRules = addressBusinessRules;
    }
    #endregion

    #region Rule Implementations

    /// <summary>
    /// CreateAddressCommand için adres başlığı tekrarı kontrolü.
    /// </summary>
    /// <param name="command">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckDuplicateAddressTitleAsync(
            command.UserId, command.AddressTitle, cancellationToken);
    }

    /// <summary>
    /// UpdateAddressCommand için adres başlığı tekrarı kontrolü.
    /// Güncelleme sırasında aynı kullanıcının başka adreslerinde tekrar olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="command">Update address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckDuplicateAddressTitleForUpdateAsync(
            command.UserId, command.AddressTitle, command.Id, cancellationToken);
    }

    #endregion
}

#endregion