using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Create;

namespace ETicaret.Application.Features.Addresses.Rules;

#region Address Limit Rule Implementation

/// <summary>
/// Kullanıcının adres limitini kontrol eden kural.
/// Bu kural sadece CreateAddressCommand için geçerlidir.
/// Single Responsibility Principle gereği sadece adres limit kontrolü yapar.
/// </summary>
public class AddressLimitRule : IRule<CreateAddressCommand>
{
    #region Properties
    public int Priority => 2; // UserExistsRule'dan sonra çalışır
    public string RuleName => nameof(AddressLimitRule);
    #endregion
    #region Fields
    private readonly AddressBusinessRules _addressBusinessRules;
    #endregion

    #region Constructor
    public AddressLimitRule(AddressBusinessRules addressBusinessRules)
    {
        _addressBusinessRules = addressBusinessRules;
    }

    #endregion

    #region Rule Implementation

    /// <summary>
    /// CreateAddressCommand için adres limit kontrolü.
    /// Kullanıcının maksimum adres sayısını aşıp aşmadığını kontrol eder.
    /// </summary>
    /// <param name="command">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserAddressLimitAsync(command.UserId, cancellationToken);
    }

    #endregion
}

#endregion