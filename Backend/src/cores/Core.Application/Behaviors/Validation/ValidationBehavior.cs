using Core.Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace Core.Application.Behaviors.Validation;

/// <summary>
/// MediatR pipeline'ında istek doğrulama işlemlerini gerçekleştiren davranış (behavior).
/// Gelen istekler için tanımlanmış FluentValidation validatörlerini çalıştırır.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Eğer bu istek için herhangi bir validatör tanımlanmamışsa, doğrudan bir sonraki adıma geç.
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        // Tüm validatörleri paralel olarak çalıştır ve sonuçlarını topla.
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        // Sonuçlardan gelen tüm hata mesajlarını (failures) tek bir listeye al.
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Eğer herhangi bir hata varsa, onları grupla ve özel exception'ımızı fırlat.
        if (failures.Any())
        {
            // İYİLEŞTİRME: Hataları tek bir LINQ sorgusuyla doğrudan gruplayıp modele dönüştür.
            var groupedFailures = failures
                .GroupBy(
                    f => f.PropertyName, // Hataları property ismine göre grupla
                    f => f.ErrorMessage, // Her grupta sadece hata mesajını al
                    (propertyName, errorMessages) => new ValidationExceptionModel(
                        Property: propertyName,
                        Errors: errorMessages.Distinct().ToList()))
                .ToList();

            throw new FluentValidationException("Doğrulama hataları oluştu.", groupedFailures);
        }

        // Hata yoksa, pipeline'daki bir sonraki adıma devam et.
        return await next();
    }
}