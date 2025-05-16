using FluentValidation;

namespace ETicaret.Application.Features.Addresses.Commands.Update;

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(c => c.UserId)
             .NotEmpty().WithMessage("Kullanıcı kimliği boş olamaz.")
             .Length(36).WithMessage("Kullanıcı kimliği 36 karakter olmalıdır.");

        RuleFor(c => c.AddressTitle)
            .NotEmpty().WithMessage("Adres başlığı boş olamaz.")
            .MaximumLength(100).WithMessage("Adres başlığı en fazla 100 karakter olabilir.");

        RuleFor(c => c.Country)
            .NotEmpty().WithMessage("Ülke boş olamaz.")
            .MaximumLength(50).WithMessage("Ülke adı en fazla 50 karakter olabilir.")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]*$").WithMessage("Ülke adı sadece harf ve boşluk içerebilir.");

        RuleFor(c => c.City)
            .NotEmpty().WithMessage("Şehir boş olamaz.")
            .MaximumLength(50).WithMessage("Şehir adı en fazla 50 karakter olabilir.")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ ]*$").WithMessage("Şehir adı sadece harf ve boşluk içerebilir.");

        RuleFor(c => c.District)
            .NotEmpty().WithMessage("İlçe/Semt boş olamaz.")
            .MaximumLength(50).WithMessage("İlçe/Semt adı en fazla 50 karakter olabilir.")
            .Matches("^[a-zA-ZğüşıöçĞÜŞİÖÇ0-9 ]*$").WithMessage("İlçe/Semt adı sadece harf, rakam ve boşluk içerebilir.");

        RuleFor(c => c.Street)
            .NotEmpty().WithMessage("Cadde/Sokak boş olamaz.")
            .MaximumLength(100).WithMessage("Cadde/Sokak bilgisi en fazla 100 karakter olabilir.");

        RuleFor(c => c.AddressLine)
            .NotEmpty().WithMessage("Tam adres boş olamaz.")
            .MaximumLength(300).WithMessage("Tam adres en fazla 300 karakter olabilir.");

        RuleFor(c => c.PostalCode)
            .MaximumLength(10).WithMessage("Posta kodu en fazla 10 karakter olabilir.")
            .When(c => !string.IsNullOrEmpty(c.PostalCode))
            .Matches("^[0-9]{5}$").WithMessage("Posta kodu 5 haneli rakam olmalıdır.")
            .When(c => !string.IsNullOrEmpty(c.PostalCode) && c.Country == "Türkiye");

        RuleFor(c => c)
            .Must(c => c.IsDefaultBilling || c.IsDefaultShipping)
            .WithMessage("Adres, fatura adresi veya gönderi adresi olarak en az biri işaretlenmelidir.")
            .WithName("AddressPurpose");
    }
}