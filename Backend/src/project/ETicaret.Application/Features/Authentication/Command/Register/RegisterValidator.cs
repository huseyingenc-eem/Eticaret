using FluentValidation;

namespace ETicaret.Application.Features.Authentication.Command.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // --- İsim Kuralları ---
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("İsim alanı boş olamaz.")
            .MinimumLength(2).WithMessage("İsim en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("İsim en fazla 50 karakter olabilir.");

        // --- Soyisim Kuralları ---
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyisim alanı boş olamaz.")
            .MinimumLength(2).WithMessage("Soyisim en az 2 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Soyisim en fazla 50 karakter olabilir.");

        // --- Kullanıcı Adı Kuralları ---
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Kullanıcı adı alanı boş olamaz.")
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
            .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.");

        // --- Email Kuralları ---
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email alanı boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi formatı giriniz.");

        RuleFor(c => c.City)
            .MaximumLength(50).WithMessage("Şehir adı en fazla 50 karakter olabilir.");

        // --- Şifre Kuralları ---
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre alanı boş olamaz.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter uzunluğunda olmalıdır.");
            //.Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            //.Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            //.Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
            //.Matches(@"[\!\?\*\.\-_@#\$%\^&\*\(\)\+\=]").WithMessage("Şifre en az bir özel karakter (!?*.-_@#$%^&*()+=) içermelidir.");

        // --- City Kuralları (Eğer komutta kaldıysa) ---
        // RuleFor(x => x.City)
        //     .MaximumLength(50).WithMessage("Şehir en fazla 50 karakter olabilir.");
    }
}