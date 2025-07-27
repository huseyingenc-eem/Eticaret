using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;

namespace ETicaret.Application.Features.Addresses.Rules;

#region User Exists Rule Implementation

/// <summary>
/// Kullanıcının sistemde var olduğunu kontrol eden kural.
/// Bu kural tüm address komutlarında ve sorgularında kullanılabilir.
/// Single Responsibility Principle gereği sadece kullanıcı varlığı kontrolü yapar.
/// </summary>
public class UserExistsRule :
    IRule<CreateAddressCommand>,
    IRule<UpdateAddressCommand>,
    IRule<DeleteAddressCommand>,
    IRule<GetByIdAddressQuery>,
    IRule<GetListByUserIdAddressQuery>
{
    #region Properties

    public int Priority => 1; // En yüksek öncelik - diğer tüm kontroller bu kurala bağlı
    public string RuleName => nameof(UserExistsRule);

    #endregion

    #region Fields

    private readonly AddressBusinessRules _addressBusinessRules;

    #endregion

    #region Constructor

    /// <summary>
    /// UserExistsRule sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="addressBusinessRules">Adres iş kuralları servisi.</param>
    public UserExistsRule(AddressBusinessRules addressBusinessRules)
    {
        _addressBusinessRules = addressBusinessRules;
    }

    #endregion

    #region Rule Implementations

    /// <summary>
    /// CreateAddressCommand için kullanıcı varlığı kontrolü.
    /// </summary>
    /// <param name="command">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserExistsAsync(command.UserId, cancellationToken);
    }

    /// <summary>
    /// UpdateAddressCommand için kullanıcı varlığı kontrolü.
    /// </summary>
    /// <param name="command">Update address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserExistsAsync(command.UserId, cancellationToken);
    }

    /// <summary>
    /// DeleteAddressCommand için kullanıcı varlığı kontrolü.
    /// </summary>
    /// <param name="command">Delete address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(DeleteAddressCommand command, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserExistsAsync(command.UserId, cancellationToken);
    }

    /// <summary>
    /// GetByIdAddressQuery için kullanıcı varlığı kontrolü.
    /// </summary>
    /// <param name="query">Get by id address sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(GetByIdAddressQuery query, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserExistsAsync(query.UserId, cancellationToken);
    }

    /// <summary>
    /// GetListByUserIdAddressQuery için kullanıcı varlığı kontrolü.
    /// ✅ EKSİK OLAN METOT EKLENDİ
    /// </summary>
    /// <param name="query">Get list by user id address sorgusu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    public async Task ExecuteAsync(GetListByUserIdAddressQuery query, CancellationToken cancellationToken = default)
    {
        await _addressBusinessRules.CheckUserExistsAsync(query.UserId, cancellationToken);
    }

    #endregion
}

#endregion