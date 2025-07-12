using Core.Application.Abstractions; // ICoreUnitOfWork için
using MediatR;
using Microsoft.Extensions.Logging; // ILogger için (opsiyonel)

namespace Core.Application.Pipelines.Transactional
{
    public class TransactionalPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ITransactionalRequest // ITransactionalRequest ile işaretlenmiş istekler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionalPipeline<TRequest, TResponse>> _logger; // Loglama için

        public TransactionalPipeline(IUnitOfWork unitOfWork, ILogger<TransactionalPipeline<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Executing transactional operation for {typeof(TRequest).Name}");

            try
            {
                TResponse response = await next();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during transactional operation for {typeof(TRequest).Name}. Rolling back...");
                throw; 
            }
        }
    }
}