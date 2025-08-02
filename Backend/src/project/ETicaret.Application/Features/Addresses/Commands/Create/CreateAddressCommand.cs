using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.RequestInfo;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Addresses.Specifications;
using ETicaret.Domain.Entities;
using MediatR;
using System.Text.Json.Serialization;
using AutoMapper;
using Core.Application.Behaviors.Authorization;

namespace ETicaret.Application.Features.Addresses.Commands.Create;

#region Create Address Command

[DefaultRoles("Admin", "User")]
public class CreateAddressCommand : IRequest<CreateAddressResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRequestInfoRequest
{
    #region Properties

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public bool IsDefaultBilling { get; set; } = false;
    public bool IsDefaultShipping { get; set; } = false;

    #endregion

    #region Cache Settings
    public string? CacheKey => $"user-addresses_{UserId}";
    public bool BypassCache => false;
    public string? CacheGroupKey => null;
    #endregion
}

#endregion

#region Create Address Command Handler

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, CreateAddressResponseDto>
{
    #region Fields

    private readonly IRepository<Address, Guid> _addressRepository;
    private readonly IMapper _mapper;

    #endregion

    #region Constructor
    public CreateAddressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _addressRepository = unitOfWork.GetRepository<Address, Guid>();
    }

    #endregion

    #region Handler Implementation
    public async Task<CreateAddressResponseDto> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        await HandleDefaultAddressOperationsAsync(request, cancellationToken);

        Address newAddress = await CreateAndSaveAddressAsync(request, cancellationToken);

        var response = _mapper.Map<CreateAddressResponseDto>(newAddress);
        response.Message = "Adres başarıyla oluşturuldu.";
        return response;
    }

    #endregion

    #region Helper Methods
    private async Task HandleDefaultAddressOperationsAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        if (request.IsDefaultShipping)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: true, cancellationToken);
        }

        if (request.IsDefaultBilling)
        {
            await ClearExistingDefaultAddressAsync(request.UserId, isShipping: false, cancellationToken);
        }
    }

    /// <summary>
    /// Mevcut varsayılan adres işaretlerini temizler.
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si.</param>
    /// <param name="isShipping">Kargo adresi mi (true) yoksa fatura adresi mi (false).</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task ClearExistingDefaultAddressAsync(string userId, bool isShipping, CancellationToken cancellationToken)
    {
        var spec = new AddressSpecifications.DefaultAddresses(userId,
            isShipping: isShipping ? true : null,
            isBilling: isShipping ? null : true);

        var existingDefaults = await _addressRepository.GetListAsync(spec, cancellationToken);

        foreach (var address in existingDefaults)
        {
            if (isShipping)
                address.IsDefaultShipping = false;
            else
                address.IsDefaultBilling = false;

            await _addressRepository.UpdateAsync(address, cancellationToken);
        }
    }

    /// <summary>
    /// Yeni adresi oluşturur ve veritabanına kaydeder.
    /// </summary>
    /// <param name="request">Create address komutu.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Oluşturulan adres entity'si.</returns>
    private async Task<Address> CreateAndSaveAddressAsync(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        Address addressEntity = _mapper.Map<Address>(request);
        await _addressRepository.AddAsync(addressEntity, cancellationToken);
        return addressEntity;
    }

    #endregion
}

#endregion