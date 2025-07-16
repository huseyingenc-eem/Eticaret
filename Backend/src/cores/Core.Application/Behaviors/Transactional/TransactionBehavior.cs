using Core.Application.Abstractions.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors.Transactional;

/// <summary>
/// MediatR pipeline'ı için veritabanı işlemlerini (transaction) yöneten davranış (behavior).
/// Bu pipeline, ITransactionalRequest arayüzünü implemente eden tüm komutları otomatik olarak
/// bir transaction bloğu içinde çalıştırır. İşlem başarılı olursa tüm değişiklikleri onaylar (Commit),
/// herhangi bir hata olursa tüm değişiklikleri geri alır (Rollback).
/// </summary>
/// <typeparam name="TRequest">İşlenecek MediatR isteği (ITransactionalRequest olmalı).</typeparam>
/// <typeparam name="TResponse">İsteğin dönüş tipi.</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// TransactionalPipeline sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemlerini ve transaction'ları yöneten birim.</param>
    /// <param name="logger">Loglama işlemleri için kullanılan servis.</param>
    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Gelen isteği bir veritabanı transaction'ı içinde işler.
    /// Başarılı olursa değişiklikleri kalıcı hale getirir, hata olursa geri alır.
    /// </summary>
    /// <param name="request">İşlenecek MediatR isteği.</param>
    /// <param name="next">Pipeline'daki bir sonraki adımı temsil eden delege.</param>
    /// <param name="cancellationToken">İşlemin iptal edilmesini sağlayan token.</param>
    /// <returns>İsteğin işlenmesi sonucu dönen yanıt.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[TRANSACTION-BAŞLANGIÇ] {typeof(TRequest).Name} için transaction'lı operasyon başlatılıyor.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken); // 1. Transaction'ı başlat

        try
        {
            // 2. Ana iş mantığını (handler) çalıştır
            TResponse response = await next();

            // 3. Handler'da yapılan değişiklikleri veritabanına kaydet (henüz onaylanmadı)
            await _unitOfWork.CompleteAsync(cancellationToken);

            // 4. Her şey yolunda gittiyse transaction'ı onayla ve değişiklikleri kalıcı yap
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation($"[TRANSACTION-ONAYLANDI] {typeof(TRequest).Name} için transaction başarıyla tamamlandı.");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TRANSACTION-GERİ ALINDI] {typeof(TRequest).Name} için operasyon sırasında hata oluştu. Değişiklikler geri alınıyor...");

            // 5. Hata durumunda tüm değişiklikleri geri al
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            // Hatayı, ele alınması için pipeline'ın yukarısındaki katmanlara fırlat
            throw;
        }
    }
}