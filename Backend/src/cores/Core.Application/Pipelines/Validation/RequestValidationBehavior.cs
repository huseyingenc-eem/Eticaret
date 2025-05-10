using FluentValidation;
using MediatR;

using Core.CrossCuttingConcerns.Exceptions;

namespace Core.Application.Pipelines.Validation;

/// <summary>
/// MediatR pipeline'ında istek doğrulama işlemlerini gerçekleştiren davranış (behavior).
/// Gelen istekler için tanımlanmış FluentValidation validatörlerini çalıştırır.
/// </summary>
/// <typeparam name="TRequest">Doğrulanacak MediatR isteğinin tipi.</typeparam>
/// <typeparam name="TResponse">MediatR isteğinin dönüş tipi.</typeparam>
public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// RequestValidationBehavior sınıfının bir örneğini başlatır.
    /// </summary>
    /// <param name="validators">Enjekte edilen IValidator<TRequest> koleksiyonu.</param>
    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// MediatR isteğini işler ve öncesinde doğrulama yapar.
    /// </summary>
    /// <param name="request">İşlenecek MediatR isteği.</param>
    /// <param name="next">Pipeline'daki bir sonraki davranışı temsil eden delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilip edilemeyeceğini belirten bir token.</param>
    /// <returns>İstek işlendikten sonraki yanıtı içeren bir görev.</returns>
    /// <exception cref="FluentValidationException">Doğrulama hataları varsa fırlatılır.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                var validationExceptionModels = failures.Select(f => new ValidationExceptionModel
                {
                    Property = f.PropertyName,
                    Errors = new[] { f.ErrorMessage } 
                }).ToList();

                var groupedFailures = validationExceptionModels
                    .GroupBy(f => f.Property)
                    .Select(g => new ValidationExceptionModel
                    {
                        Property = g.Key,
                        Errors = g.SelectMany(f => f.Errors ?? Enumerable.Empty<string>()).Distinct().ToList()
                    }).ToList();

                throw new FluentValidationException(groupedFailures);
            }
        }
        return await next();
    }
}
