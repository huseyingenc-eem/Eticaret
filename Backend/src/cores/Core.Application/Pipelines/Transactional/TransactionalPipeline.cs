using MediatR;
using System.Transactions; // TransactionScope için

namespace Core.Application.Pipelines.Transactional;


public class TransactionalPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    public TransactionalPipeline() { }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;

        
        using (TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                response = await next();

                transactionScope.Complete();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        return response;
    }
    //saga Chereography
    //saga Orchestration 
}

/* Önerilen IDbContextTransaction tabanlı yapı (Yorum Satırında)

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage; // IDbContextTransaction için
using Microsoft.Extensions.Logging; // ILogger için
using System; // Exception için
using System.Threading;
using System.Threading.Tasks;
// using ETicaret.Persistence.Contexts; // Kendi DbContext using'inizi ekleyin

namespace Core.Application.Pipelines.Transactional;

public class TransactionalBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest 
{
    private readonly DbContext _dbContext; // Projenizdeki DbContext tipi ile değiştirin, örneğin BaseDBContexts
    private readonly ILogger<TransactionalBehavior<TRequest, TResponse>> _logger;

    // Örnek: public TransactionalBehavior(BaseDBContexts dbContext, ILogger<...> logger)
    public TransactionalBehavior(DbContext dbContext, ILogger<TransactionalBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            _logger.LogInformation("[TRANSACTIONAL-BEHAVIOR] Mevcut bir transaction'a dahil olunuyor: {RequestName}", typeof(TRequest).Name);
            return await next();
        }

        IDbContextTransaction? strategyContextTransaction = null;
        try
        {
            IExecutionStrategy strategy = _dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                strategyContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                _logger.LogInformation("[TRANSACTIONAL-BEHAVIOR] Yeni DbContext transaction başlatıldı: {TransactionId} - {RequestName}", strategyContextTransaction.TransactionId, typeof(TRequest).Name);
            });
            
            TResponse response = await next(); // Handler'ı çalıştır

            await strategy.ExecuteAsync(async () =>
            {
                if (strategyContextTransaction != null)
                {
                    await strategyContextTransaction.CommitAsync(cancellationToken);
                    _logger.LogInformation("[TRANSACTIONAL-BEHAVIOR] DbContext transaction commit edildi: {TransactionId} - {RequestName}", strategyContextTransaction.TransactionId, typeof(TRequest).Name);
                }
            });

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[TRANSACTIONAL-BEHAVIOR] DbContext transaction içinde hata oluştu, Rollback yapılıyor: {RequestName}", typeof(TRequest).Name);
            if (strategyContextTransaction != null)
            {
                try
                {
                    await strategyContextTransaction.RollbackAsync(cancellationToken);
                    _logger.LogInformation("[TRANSACTIONAL-BEHAVIOR] DbContext transaction rollback edildi: {TransactionId} - {RequestName}", strategyContextTransaction.TransactionId, typeof(TRequest).Name);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "[TRANSACTIONAL-BEHAVIOR] Rollback sırasında hata oluştu: {TransactionId} - {RequestName}", strategyContextTransaction.TransactionId, typeof(TRequest).Name);
                }
            }
            throw; // Orijinal hatayı tekrar fırlat
        }
        finally
        {
            if (strategyContextTransaction != null)
            {
                await strategyContextTransaction.DisposeAsync();
                _logger.LogInformation("[TRANSACTIONAL-BEHAVIOR] DbContext transaction dispose edildi: {TransactionId} - {RequestName}", strategyContextTransaction.TransactionId, typeof(TRequest).Name);
            }
        }
    }
}

*/
