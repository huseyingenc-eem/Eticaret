using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;

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