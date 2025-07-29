using ETicaret.Application.Common.Mappings;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

/// <summary>
/// Adres silme işlemi sonucu döndürülen DTO.
/// </summary>
public class DeleteAddressResponseDto : IMapFrom<Address>
{
    #region Properties

    /// <summary>
    /// Silinen adresin benzersiz kimliği.
    /// </summary>
    public Guid Id { get; set; }
    public string AddressTitle { get; set; }

    /// <summary>
    /// İşlem sonucu hakkında bilgilendirme mesajı.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// İşlemin başarılı olup olmadığını belirtir.
    /// </summary>
    public bool IsSuccess { get; set; }

    #endregion
}