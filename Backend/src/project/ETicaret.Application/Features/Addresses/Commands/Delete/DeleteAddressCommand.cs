using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Rules;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Rules;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Addresses.Commands.Delete;

#region Delete Address Command

/// <summary>
/// Kullanıcının kendi adresini silme işlemini temsil eden komut.
/// Sadece belirli kuralları çalıştırır - RuleConfiguration attribute ile kontrol edilir.
/// FluentValidation: Input validation yapar
/// BusinessRulesValidationBehavior: Business logic kontrolü yapar
/// Handler: Sadece core business işlemlerini yapar
/// </summary>
[RuleConfiguration(
    OnlyRules = new[]
    {
        typeof(UserExistsRule),
        typeof(MinimumAddressRule)
    }
)]
public class DeleteAddressCommand : IRequest<DeleteAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    #region Properties

    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    #endregion

    #region Cache Settings

    public string CacheKey => $"address:{Id}";
    public string? CacheGroupKey => "Addresses";
    public bool BypassCache { get; set; }

    #endregion
}

#endregion

#region Delete Address Command Handler

/// <summary>
/// DeleteAddressCommand komutunu işleyen handler sınıfı.
/// Business rules artık RuleEngine tarafından otomatik çalıştırılır.
/// Handler sadece core business logic'e odaklanır.
/// </summary>
public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, DeleteAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;

    #endregion

    #region Constructor

    /// <summary>
    /// DeleteAddressCommandHandler sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work implementasyonu.</param>
    public DeleteAddressCommandHandler(IUnitOfWork unitOfWork)
    {
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation

    /// <summary>
    /// Adres silme komutunu işler.
    /// Business rules RuleEngine tarafından otomatik çalıştırıldı.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme işlemi sonucu bilgilerini içeren yanıt DTO'su.</returns>
    public async Task<DeleteAddressResponseDto> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        // 1. Adresi güvenli bir şekilde getir ve yetki kontrolü yap
        Address addressToDelete = await GetAndValidateAddressAsync(request, cancellationToken);

        // 2. Adresi sil
        await DeleteAddressAsync(addressToDelete, cancellationToken);

        // 3. Yanıt DTO'sunu oluştur ve döndür
        return CreateResponseDto(request.Id);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Adresi güvenli bir şekilde getirir ve yetki kontrolü yapar.
    /// </summary>
    /// <param name="request">Adres silme komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Silme yetkisi olan adres entity'si.</returns>
    private async Task<Address> GetAndValidateAddressAsync(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        // Güvenlik odaklı specification ile hem ID hem de UserId kontrol et
        var spec = new AddressSpecifications.ByIdAndUserId(request.Id, request.UserId);
        Address? addressToDelete = await _addressRepository.GetAsync(spec, cancellationToken);

        if (addressToDelete == null)
        {
            throw new NotFoundException(
                message: $"Address with ID {request.Id} not found or user {request.UserId} does not have permission to delete it.",
                userFriendlyMessage: "Belirtilen adres bulunamadı veya bu adresi silme yetkiniz bulunmuyor.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }

        return addressToDelete;
    }

    /// <summary>
    /// Adresi veritabanından kalıcı olarak siler.
    /// </summary>
    /// <param name="addressToDelete">Silinecek adres entity'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task DeleteAddressAsync(Address addressToDelete, CancellationToken cancellationToken)
    {
        // Kalıcı silme işlemi (permanent: true)
        // Adres verileri genellikle GDPR uyum nedeniyle kalıcı olarak silinir
        await _addressRepository.DeleteAsync(addressToDelete, permanent: true, cancellationToken);
    }

    /// <summary>
    /// Silme işlemi sonucunu içeren yanıt DTO'sunu oluşturur.
    /// </summary>
    /// <param name="deletedAddressId">Silinen adresin ID'si.</param>
    /// <returns>Yanıt DTO'su.</returns>
    private static DeleteAddressResponseDto CreateResponseDto(Guid deletedAddressId)
    {
        return new DeleteAddressResponseDto
        {
            Id = deletedAddressId,
            Message = "Adres başarıyla silindi.",
            IsSuccess = true
        };
    }

    #endregion
}

#endregion