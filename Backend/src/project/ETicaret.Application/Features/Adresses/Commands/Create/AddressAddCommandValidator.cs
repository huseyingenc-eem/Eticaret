using FluentValidation;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

public class AddressAddCommandValidator : AbstractValidator<AddressAddCommand>
{
    public AddressAddCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("Kullanıcı kimliği boş olamaz.");

        RuleFor(c => c.AddressTitle)
            .NotEmpty().WithMessage("Adres başlığı boş olamaz.")
            .MaximumLength(100).WithMessage("Adres başlığı en fazla 100 karakter olabilir.");

        RuleFor(c => c.Country)
            .NotEmpty().WithMessage("Ülke boş olamaz.")
            .MaximumLength(50).WithMessage("Ülke adı en fazla 50 karakter olabilir.");

        RuleFor(c => c.City)
            .NotEmpty().WithMessage("Şehir boş olamaz.")
            .MaximumLength(50).WithMessage("Şehir adı en fazla 50 karakter olabilir.");

        RuleFor(c => c.District)
            .NotEmpty().WithMessage("İlçe/Semt boş olamaz.")
            .MaximumLength(50).WithMessage("İlçe/Semt adı en fazla 50 karakter olabilir.");

        RuleFor(c => c.Street)
            .NotEmpty().WithMessage("Cadde/Sokak boş olamaz.")
            .MaximumLength(100).WithMessage("Cadde/Sokak bilgisi en fazla 100 karakter olabilir.");

        RuleFor(c => c.FullAddress)
            .NotEmpty().WithMessage("Tam adres boş olamaz.")
            .MaximumLength(250).WithMessage("Tam adres en fazla 250 karakter olabilir.");

        RuleFor(c => c.PostalCode)
            .MaximumLength(10).WithMessage("Posta kodu en fazla 10 karakter olabilir.");
        // PostalCode boş olabilir, bu yüzden NotEmpty kontrolü yok.
    }
}