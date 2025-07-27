using FluentValidation;

namespace ETicaret.Application.Features.Authentication.Command.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email alanı boş olamaz.")
            .EmailAddress().WithMessage("Geçerli bir email adresi formatı giriniz.");
            //.Must(EmailFormat).WithMessage("Email adresi @gmail.com veya @outlook.com ile bitmelidir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre alanı boş olamaz.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter uzunluğunda olmalıdır.");
            //.Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            //.Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            //.Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
            //.Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter (örn: !?*.) içermelidir.");
    }

    //private bool EmailFormat(string email)
    //{
    //    bool endsWithGmail = email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
    //    bool endsWithOutlook = email.EndsWith("@outlook.com", StringComparison.OrdinalIgnoreCase);

    //    return endsWithGmail || endsWithOutlook;
    //}
}