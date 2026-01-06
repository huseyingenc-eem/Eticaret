using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// Minimum adres kontrolü yapan tek sorumlu rule
/// </summary>
public class MinimumAddressRule : IBusinessRule<DeleteAddressCommand>
{
    private readonly IAddressRepository _repository;

    public MinimumAddressRule(IAddressRepository addressRepository)
    {
        _repository = addressRepository;
    }

    public bool ShouldExecute(DeleteAddressCommand command) => true;

    public async Task ExecuteAsync(DeleteAddressCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByUserIdSpec(command.UserId);
        var userAddresses = await _repository.GetListAsync(spec, cancellationToken);

        var remainingAddressCount = userAddresses.Count(a => a.Id != command.Id);

        if (remainingAddressCount == 0)
        {
            throw new BusinessException(
                message: $"User {command.UserId} must have at least one address.",
                userFriendlyMessage: "En az bir adresiniz olmalıdır. Son adresinizi silemezsiniz.",
                errorCode: "MINIMUM_ADDRESS_REQUIRED"
            );
        }
    }

    public int Priority => 2;

    #region Private Specification - Bu rule'a özel
    private class ByUserIdSpec : Specification<Address>
    {
        public ByUserIdSpec(string userId)
            : base(address => address.UserId == userId)
        {
        }
    }
    #endregion
}