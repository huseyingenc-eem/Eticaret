using FluentValidation;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("Tedarikçi adı boş olamaz.")
            .MaximumLength(150).WithMessage("Tedarikçi adı en fazla 150 karakter olabilir.");

        RuleFor(s => s.ContactPerson)
            .MaximumLength(100).WithMessage("Yetkili kişi adı en fazla 100 karakter olabilir.");

        RuleFor(s => s.ContactEmail)
            .EmailAddress().When(s => !string.IsNullOrEmpty(s.ContactEmail)).WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(100).WithMessage("Yetkili e-posta adresi en fazla 100 karakter olabilir.");

        RuleFor(s => s.PhoneNumber)
            .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olabilir.");

        RuleFor(s => s.Address)
            .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.");
    }
}