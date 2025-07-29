using FluentValidation;
using Core.Application.Common.Constants;

namespace ETicaret.Application.Features.Authentication.Command.Register;

/// <summary>
/// RegisterCommand için doğrulama kurallarını tanımlayan validator sınıfı.
/// FluentValidation kütüphanesi kullanılarak comprehensive input validation sağlar.
/// </summary>
public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        ConfigureFirstNameRules();
        ConfigureLastNameRules();
        ConfigureEmailRules();
        ConfigureCityRules();
        ConfigurePasswordRules();
    }

    /// <summary>
    /// Ad (FirstName) alanı için doğrulama kuralları.
    /// </summary>
    private void ConfigureFirstNameRules()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Ad alanı zorunludur.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "FIRST_NAME_REQUIRED"))

            .MinimumLength(2)
            .WithMessage("Ad en az 2 karakter olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "FIRST_NAME_TOO_SHORT"))

            .MaximumLength(50)
            .WithMessage("Ad en fazla 50 karakter olabilir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "FIRST_NAME_TOO_LONG"))

            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$")
            .WithMessage("Ad sadece harflerden oluşmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "FIRST_NAME_INVALID_CHARACTERS"));
    }

    /// <summary>
    /// Soyad (LastName) alanı için doğrulama kuralları.
    /// </summary>
    private void ConfigureLastNameRules()
    {
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Soyad alanı zorunludur.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "LAST_NAME_REQUIRED"))

            .MinimumLength(2)
            .WithMessage("Soyad en az 2 karakter olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "LAST_NAME_TOO_SHORT"))

            .MaximumLength(50)
            .WithMessage("Soyad en fazla 50 karakter olabilir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "LAST_NAME_TOO_LONG"))

            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]+$")
            .WithMessage("Soyad sadece harflerden oluşmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "LAST_NAME_INVALID_CHARACTERS"));
    }

    /// <summary>
    /// E-posta (Email) alanı için doğrulama kuralları.
    /// </summary>
    private void ConfigureEmailRules()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta alanı zorunludur.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "EMAIL_REQUIRED"))

            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi formatı giriniz.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "EMAIL_INVALID_FORMAT"))

            .MaximumLength(255)
            .WithMessage("E-posta adresi en fazla 255 karakter olabilir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "EMAIL_TOO_LONG"))

            .Must(BeValidEmailDomain)
            .WithMessage("E-posta adresi @gmail.com, @outlook.com, @hotmail.com veya @yahoo.com ile bitmelidir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "EMAIL_DOMAIN_NOT_ALLOWED"));
    }

    /// <summary>
    /// Şehir (City) alanı için doğrulama kuralları.
    /// </summary>
    private void ConfigureCityRules()
    {
        RuleFor(x => x.City)
            .MaximumLength(100)
            .WithMessage("Şehir adı en fazla 100 karakter olabilir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "CITY_TOO_LONG"))

            .Matches(@"^[a-zA-ZğüşıöçĞÜŞİÖÇ\s]*$")
            .When(x => !string.IsNullOrWhiteSpace(x.City))
            .WithMessage("Şehir adı sadece harflerden oluşmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "CITY_INVALID_CHARACTERS"));
    }

    /// <summary>
    /// Şifre (Password) alanı için doğrulama kuralları.
    /// </summary>
    private void ConfigurePasswordRules()
    {
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Şifre alanı zorunludur.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_REQUIRED"))

            .MinimumLength(8)
            .WithMessage("Şifre en az 8 karakter uzunluğunda olmalıdır.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_TOO_SHORT"))

            .MaximumLength(128)
            .WithMessage("Şifre en fazla 128 karakter uzunluğunda olabilir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_TOO_LONG"))

            .Matches("[A-Z]")
            .WithMessage("Şifre en az bir büyük harf içermelidir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_MISSING_UPPERCASE"))

            .Matches("[a-z]")
            .WithMessage("Şifre en az bir küçük harf içermelidir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_MISSING_LOWERCASE"))

            .Matches("[0-9]")
            .WithMessage("Şifre en az bir rakam içermelidir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_MISSING_DIGIT"))

            .Matches(@"[\!\?\*\.\-_@#\$%\^&\*\(\)\+\=]")
            .WithMessage("Şifre en az bir özel karakter (!?*.-_@#$%^&*()+=) içermelidir.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_MISSING_SPECIAL_CHAR"))

            .Must(NotContainCommonPasswords)
            .WithMessage("Bu şifre çok yaygın kullanılan şifreler listesinde. Lütfen daha güvenli bir şifre seçiniz.")
            .WithErrorCode(ApplicationErrorCodes.CreateErrorCode("VALIDATION", "PASSWORD_TOO_COMMON"));
    }

    #region Custom Validation Methods

    /// <summary>
    /// E-posta alan adının izin verilen domainler listesinde olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="email">Kontrol edilecek e-posta adresi.</param>
    /// <returns>Geçerli domain ise true, değilse false.</returns>
    private static bool BeValidEmailDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var allowedDomains = new[]
        {
            "@gmail.com",
            "@outlook.com",
            "@hotmail.com",
            "@yahoo.com"
        };

        return allowedDomains.Any(domain =>
            email.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Şifrenin yaygın kullanılan zayıf şifreler listesinde olmadığını kontrol eder.
    /// </summary>
    /// <param name="password">Kontrol edilecek şifre.</param>
    /// <returns>Güvenli şifre ise true, yaygın şifre ise false.</returns>
    private static bool NotContainCommonPasswords(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        var commonPasswords = new[]
        {
            "12345678", "password", "Password1", "123456789",
            "qwerty123", "Qwerty123", "admin123", "Admin123",
            "password123", "Password123", "123qwerty", "123Qwerty",
            "welcome123", "Welcome123", "letmein123", "Letmein123"
        };

        return !commonPasswords.Contains(password, StringComparer.OrdinalIgnoreCase);
    }

    #endregion
}