using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

/// <summary>
/// Tek bir alan (property) için doğrulama hatalarını temsil eden,
/// değiştirilemez (immutable) bir veri modeli.
/// </summary>
/// <param name="Property">Hatanın ait olduğu alanın adı.</param>
/// <param name="Errors">Bu alana ait hata mesajlarının listesi.</param>
public record ValidationExceptionModel(string? Property, IEnumerable<string> Errors);

/// <summary>
/// FluentValidation ile doğrulama başarısız olduğunda fırlatılan hata.
/// </summary>
public class FluentValidationException : ApplicationException
{
    /// <summary>
    /// Oluşan doğrulama hatalarının listesi.
    /// </summary>
    public IReadOnlyList<ValidationExceptionModel> Errors { get; }

    /// <summary>
    /// FluentValidationException sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="message">Genel hata mesajı.</param>
    /// <param name="errors">Gruplanmış ve modele dönüştürülmüş hata listesi.</param>
    public FluentValidationException(string message, IReadOnlyList<ValidationExceptionModel> errors)
        : base(
            message: message,
            userFriendlyMessage: "Lütfen girdiğiniz bilgileri kontrol ediniz.",
            errorCode: ApplicationErrorCodes.ValidationError,
            additionalData: errors
        )
    {
        Errors = errors;
    }
}