using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Delete;

namespace ETicaret.Application.Features.Addresses.Rules;

#region Minimum Address Rule Implementation

/// <summary>
/// Kullanıcının en az bir adresinin kalmasını kontrol eden kural.
/// Bu kural sadece DeleteAddressCommand için geçerlidir.
/// Single Responsibility Principle gereği sadece minimum adres kontrolü yapar.
/// </summary>
public class MinimumAddressRule : IRule<DeleteAddressCommand>
{
    #region Properties

    public int Priority => 2;
    public string RuleName => nameof(MinimumAddressRule);

    #endregion

    #region Fields
    private readonly AddressBusinessRules _addressBusinessRules;
    #endregion
    #region Constructor
    public MinimumAddressRule(AddressBusinessRules addressBusinessRules)
    {
        _addressBusinessRules = addressBusinessRules;
    }
    #endregion

    #region Rule Implementation

    /// <summary>
    /// DeleteAddressCommand için minimum adres kontrolü.
    /// Kullanıcının silme işleminden sonra en az bir adresinin kalacağından emin olur.
    /// </summary>
    /// <param name="command">Delete address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(DeleteAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserMustHaveAtLeastOneAddressAsync(
            command.UserId, command.Id, cancellationToken);
    }

    #endregion
}

#endregion