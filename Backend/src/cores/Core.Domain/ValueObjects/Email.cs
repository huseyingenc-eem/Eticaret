using System.Text.RegularExpressions;
using Core.Domain.Exceptions;

namespace Core.Domain.ValueObjects;

/// <summary>
/// Bir e-posta adresini değer nesnesi olarak temsil eder.
/// </summary>
public class Email : ValueObject
{
    /// <summary>
    /// E-posta adresinin metin değerini alır.
    /// </summary>
    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Formatı doğruladıktan sonra yeni bir Email nesnesi oluşturur.
    /// </summary>
    /// <param name="email">Email nesnesine dönüştürülecek metin.</param>
    /// <returns>Yeni bir Email nesnesi.</returns>
    /// <exception cref="DomainException">E-posta formatı geçersizse fırlatılır.</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            // YENİ YAPI: Hata mesajına ek olarak bir ErrorCode sağlıyoruz.
            throw new DomainException(
                "E-posta adresi boş olamaz.",
                "EMAIL_EMPTY");
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            // YENİ YAPI: Hata mesajı, ErrorCode ve hataya neden olan veriyi 'details' olarak ekliyoruz.
            throw new DomainException(
                "E-posta adresi geçersiz bir formata sahip.",
                "EMAIL_INVALID_FORMAT",
                new { InvalidEmail = email });
        }

        return new Email(email.Trim().ToLowerInvariant());
    }

    /// <summary>
    /// Eşitlik karşılaştırması için bileşenleri sağlar.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}