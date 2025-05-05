using FluentValidation.Results;

namespace Core.CrossCuttingConcerns.Exceptions;

public class FluentValidationException : Exception
{
    public IEnumerable<ValidationExceptionModel> Errors { get; } = Enumerable.Empty<ValidationExceptionModel>();
    public FluentValidationException(string message) : base (message)
    {

    }

    public FluentValidationException(IEnumerable<ValidationExceptionModel> errors) : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    private static string BuildErrorMessage(IEnumerable<ValidationExceptionModel> errors)
    {
        var arr = errors.Select(x => $"{x.Property} : {string.Join(" ",x.Errors)}");

        return string.Join("\n", arr);
    }

    public FluentValidationException(IEnumerable<ValidationFailure> failures)
        : base(BuildErrorMessage(ConvertFailures(failures)))
    {
        Errors = ConvertFailures(failures);
    }

    // Çevirme Metodu: ValidationFailure -> ValidationExceptionModel dönüşümünü yapar.
    private static IEnumerable<ValidationExceptionModel> ConvertFailures(IEnumerable<ValidationFailure> failures)
    {
        return failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                       .Select(group => new ValidationExceptionModel { /* ... */ })
                       .ToList();
    }
}

