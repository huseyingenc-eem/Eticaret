using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetMyAddresses;
using ETicaret.Application.Features.Addresses.Queries.GetByUserId;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.Addresses.Rules;

/// <summary>
/// User varlığını kontrol eden tek sorumlu rule
/// </summary>
public class UserExistsRule :
    IBusinessRule<CreateAddressCommand>,
    IBusinessRule<UpdateAddressCommand>,
    IBusinessRule<DeleteAddressCommand>,
    IBusinessRule<GetByIdAddressQuery>,
    IBusinessRule<GetMyAddressesQuery>,
    IBusinessRule<GetByUserIdAddressQuery>
{
    private readonly UserManager<User> _userManager;

    public UserExistsRule(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public bool ShouldExecute(CreateAddressCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(UpdateAddressCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(DeleteAddressCommand command) => !string.IsNullOrWhiteSpace(command.UserId);
    public bool ShouldExecute(GetByIdAddressQuery query) => !string.IsNullOrWhiteSpace(query.UserId);
    public bool ShouldExecute(GetMyAddressesQuery query) => !string.IsNullOrWhiteSpace(query.UserId);

    public bool ShouldExecute(GetByUserIdAddressQuery query) => !string.IsNullOrWhiteSpace(query.UserId);
    public async Task ExecuteAsync(CreateAddressCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(UpdateAddressCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(DeleteAddressCommand command, CancellationToken cancellationToken = default)
        => await ValidateUserExists(command.UserId, cancellationToken);

    public async Task ExecuteAsync(GetByIdAddressQuery query, CancellationToken cancellationToken = default)
        => await ValidateUserExists(query.UserId, cancellationToken);

    public async Task ExecuteAsync(GetMyAddressesQuery query, CancellationToken cancellationToken = default)
        => await ValidateUserExists(query.UserId, cancellationToken);

    public async Task ExecuteAsync(GetByUserIdAddressQuery query, CancellationToken cancellationToken = default)
        => await ValidateUserExists(query.UserId, cancellationToken);


    private async Task ValidateUserExists(string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BusinessException(
                message: "User ID cannot be null or empty.",
                userFriendlyMessage: "Kullanıcı ID'si boş olamaz.",
                errorCode: "INVALID_USER_ID"
            );
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(
                message: $"User with ID {userId} was not found.",
                userFriendlyMessage: "Kullanıcı bulunamadı.",
                errorCode: "USER_NOT_FOUND"
            );
        }
    }

    public int Priority => 1;
}