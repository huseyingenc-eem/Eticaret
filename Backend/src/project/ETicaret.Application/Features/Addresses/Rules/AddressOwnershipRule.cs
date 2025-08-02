using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// Address ownership kontrolü yapan tek sorumlu rule
/// </summary>
public class AddressOwnershipRule :
    IBusinessRule<UpdateAddressCommand>,
    IBusinessRule<DeleteAddressCommand>,
    IBusinessRule<GetByIdAddressQuery>
{
    private readonly IRepository<Address, Guid> _repository;

    public AddressOwnershipRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Address, Guid>();
    }

    public bool ShouldExecute(UpdateAddressCommand command) => true;
    public bool ShouldExecute(DeleteAddressCommand command) => true;
    public bool ShouldExecute(GetByIdAddressQuery query) => true;

    public async Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
        => await ValidateOwnership(command.Id, command.UserId, cancellationToken);

    public async Task ExecuteAsync(DeleteAddressCommand command, CancellationToken cancellationToken = default)
        => await ValidateOwnership(command.Id, command.UserId, cancellationToken);

    public async Task ExecuteAsync(GetByIdAddressQuery query, CancellationToken cancellationToken = default)
        => await ValidateOwnership(query.Id, query.UserId, cancellationToken);

    private async Task ValidateOwnership(Guid addressId, string userId, CancellationToken cancellationToken)
    {
        var spec = new ByIdAndUserIdSpec(addressId, userId);
        var address = await _repository.GetAsync(spec, cancellationToken);

        if (address == null)
        {
            throw new NotFoundException(
                message: $"Address with ID {addressId} not found for user {userId}.",
                userFriendlyMessage: "Adres bulunamadı veya bu adrese erişim yetkiniz yok.",
                errorCode: "ADDRESS_NOT_FOUND_OR_UNAUTHORIZED"
            );
        }
    }

    public int Priority => 1;

    #region Private Specification - Bu rule'a özel
    private class ByIdAndUserIdSpec : Specification<Address>
    {
        public ByIdAndUserIdSpec(Guid id, string userId)
            : base(address => address.Id == id && address.UserId == userId)
        {
        }
    }
    #endregion
}