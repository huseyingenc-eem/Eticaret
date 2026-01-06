using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// Duplicate başlık kontrolü yapan tek sorumlu rule
/// </summary>
public class DuplicateAddressTitleRule :
    IBusinessRule<CreateAddressCommand>,
    IBusinessRule<UpdateAddressCommand>
{
    private readonly IAddressRepository _repository;

    public DuplicateAddressTitleRule(IAddressRepository addressRepository)
    {
        _repository = addressRepository;
    }

    public bool ShouldExecute(CreateAddressCommand command) => !string.IsNullOrWhiteSpace(command.AddressTitle);
    public bool ShouldExecute(UpdateAddressCommand command) => !string.IsNullOrWhiteSpace(command.AddressTitle);

    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByUserIdAndTitleSpec(command.UserId, command.AddressTitle);
        bool isDuplicate = await _repository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"User {command.UserId} already has an address with title '{command.AddressTitle}'.",
                userFriendlyMessage: $"'{command.AddressTitle}' başlığında zaten bir adresiniz mevcut.",
                errorCode: "DUPLICATE_ADDRESS_TITLE"
            );
        }
    }

    public async Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByUserIdAndTitleExcludingIdSpec(command.UserId, command.AddressTitle, command.Id);
        bool isDuplicate = await _repository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"User {command.UserId} already has another address with title '{command.AddressTitle}'.",
                userFriendlyMessage: $"'{command.AddressTitle}' başlığında zaten başka bir adresiniz mevcut.",
                errorCode: "DUPLICATE_ADDRESS_TITLE"
            );
        }
    }

    public int Priority => 3;

    #region Private Specifications - Bu rule'a özel
    private class ByUserIdAndTitleSpec : Specification<Address>
    {
        public ByUserIdAndTitleSpec(string userId, string title)
            : base(address => address.UserId == userId &&
                            address.AddressTitle.ToLower() == title.ToLower())
        {
        }
    }

    private class ByUserIdAndTitleExcludingIdSpec : Specification<Address>
    {
        public ByUserIdAndTitleExcludingIdSpec(string userId, string title, Guid excludeId)
            : base(address => address.UserId == userId &&
                            address.AddressTitle.ToLower() == title.ToLower() &&
                            address.Id != excludeId)
        {
        }
    }
    #endregion
}