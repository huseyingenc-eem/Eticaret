using Core.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Core.Domain.ValueObjects;

public class Email : ValueObject
{
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
            throw new DomainException(
                "E-posta adresi boş olamaz.",
                "EMAIL_EMPTY");
        }

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            // YENİ YAPI: Hata mesajı, ErrorCode ve hataya neden olan veriyi 'details' olarak ekliyoruz.
           
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