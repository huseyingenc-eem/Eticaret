using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Delete;
using ETicaret.Application.Features.Discounts.Commands.Update;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Rules;

/// <summary>
/// Discount'un mevcut olduğunu kontrol eden tek sorumlu rule.
/// </summary>
public class DiscountMustExistRule :
    IBusinessRule<UpdateDiscountCommand>,
    IBusinessRule<DeleteDiscountCommand>
{
    private readonly IRepository<Discount, Guid> _repository;

    public DiscountMustExistRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Discount, Guid>();
    }

    public bool ShouldExecute(UpdateDiscountCommand command) => command.Id != Guid.Empty;
    public bool ShouldExecute(DeleteDiscountCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(UpdateDiscountCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateDiscountExists(command.Id, cancellationToken);
    }

    public async Task ExecuteAsync(DeleteDiscountCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateDiscountExists(command.Id, cancellationToken);
    }

    private async Task ValidateDiscountExists(Guid discountId, CancellationToken cancellationToken)
    {
        var spec = new ByIdSpec(discountId);
        var discount = await _repository.GetAsync(spec, cancellationToken);

        if (discount == null)
        {
            throw new NotFoundException(
                message: $"Discount with ID {discountId} not found.",
                userFriendlyMessage: "Belirtilen indirim bulunamadı.",
                errorCode: DiscountConstants.ErrorCodes.DiscountNotFound
            );
        }
    }

    public int Priority => 0;

    #region Private Specification

    private class ByIdSpec : Specification<Discount>
    {
        public ByIdSpec(Guid id)
            : base(discount => discount.Id == id) { }
    }

    #endregion
}