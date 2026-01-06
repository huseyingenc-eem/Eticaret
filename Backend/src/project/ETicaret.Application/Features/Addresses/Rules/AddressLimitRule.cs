using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// Adres limitini kontrol eden tek sorumlu rule
/// </summary>
public class AddressLimitRule : IBusinessRule<CreateAddressCommand>
{
    private const int MAX_ADDRESSES_PER_USER = 10;
    private readonly IAddressRepository _addressRepository;

    public AddressLimitRule(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public bool ShouldExecute(CreateAddressCommand command) => true;

    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new CountByUserIdSpec(command.UserId);
        var userAddressCount = await _addressRepository.CountAsync(spec, cancellationToken);

        if (userAddressCount >= MAX_ADDRESSES_PER_USER)
        {
            throw new BusinessException(
                message: $"User {command.UserId} has reached the maximum address limit of {MAX_ADDRESSES_PER_USER}.",
                userFriendlyMessage: $"En fazla {MAX_ADDRESSES_PER_USER} adres ekleyebilirsiniz.",
                errorCode: "ADDRESS_LIMIT_EXCEEDED"
            );
        }
    }

    public int Priority => 2;

    #region Private Specification - Bu rule'a özel
    private class CountByUserIdSpec : Specification<Address>
    {
        public CountByUserIdSpec(string userId)
            : base(address => address.UserId == userId){}
    }
    #endregion
}