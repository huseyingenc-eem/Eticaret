using FluentValidation;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;
public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(s => s.CompanyName)
            .NotEmpty().WithMessage("Şirket adı boş olamaz.")
            .NotNull().WithMessage("Şirket adı null olamaz.")
            .MinimumLength(2).WithMessage("Şirket adı en az 2 karakter olmalıdır.")
            .MaximumLength(200).WithMessage("Şirket adı en fazla 200 karakter olabilir.");

        RuleFor(s => s.ContactPerson)
            .MaximumLength(100).WithMessage("İletişim kişisi adı en fazla 100 karakter olabilir.")
            .When(s => !string.IsNullOrWhiteSpace(s.ContactPerson));

        RuleFor(s => s.ContactEmail)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(100).WithMessage("E-posta adresi en fazla 100 karakter olabilir.")
            .When(s => !string.IsNullOrWhiteSpace(s.ContactEmail));

        RuleFor(s => s.PhoneNumber)
            .Matches(@"^[\d\s\-\+\(\)]+$").WithMessage("Telefon numarası geçerli formatta olmalıdır.")
            .MinimumLength(10).WithMessage("Telefon numarası en az 10 karakter olmalıdır.")
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.")
            .When(s => !string.IsNullOrWhiteSpace(s.PhoneNumber));

        RuleFor(s => s.Address)
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.")
            .When(s => !string.IsNullOrWhiteSpace(s.Address));
    }
}