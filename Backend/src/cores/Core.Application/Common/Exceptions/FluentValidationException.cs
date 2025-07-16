using Core.Application.Common.Constants;

namespace Core.Application.Common.Exceptions;
public class FluentValidationException : ApplicationException
{
    public IEnumerable<ValidationExceptionModel> Errors { get; }

    public FluentValidationException(IEnumerable<ValidationExceptionModel> errors)
        : base(
            ApplicationErrorCodes.ValidationError,
            "Bir veya daha fazla doğrulama hatası oluştu.",
            "Lütfen girdiğiniz bilgileri kontrol ediniz.",
            errors.GroupBy(e => e.Property ?? "GeneralErrors")
                  .ToDictionary(
                      g => g.Key,
                      g => g.SelectMany(e => e.Errors ?? Enumerable.Empty<string>()).Distinct().ToArray()
                  ),
            null)
    {
        Errors = errors;
    }
}