using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Delete;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Rules;


public class DiscountCannotBeDeletedIfInUseRule : IBusinessRule<DeleteDiscountCommand>
{
    private readonly IDiscountRepository _repository;

    public DiscountCannotBeDeletedIfInUseRule(IDiscountRepository discountRepository)
    {
        _repository = discountRepository;
    }

    public bool ShouldExecute(DeleteDiscountCommand command) => command.Id != Guid.Empty;

    public async Task ExecuteAsync(DeleteDiscountCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new HasActiveUsagesSpec(command.Id);
        var discountWithUsages = await _repository.GetAsync(spec, cancellationToken);

        if (discountWithUsages != null)
        {
            throw new BusinessException(
                message: $"Discount with ID {command.Id} cannot be deleted because it has active usages.",
                userFriendlyMessage: "Bu indirim kullanımda olduğu için silinemez. Önce tüm kullanımları kaldırın.",
                errorCode: DiscountConstants.ErrorCodes.DiscountInUse
            );
        }
    }

    public int Priority => 5;

    #region Private Specification

    private class HasActiveUsagesSpec : Specification<Discount>
    {
        public HasActiveUsagesSpec(Guid discountId)
            : base(d => d.Id == discountId && d.Usages.Any())
        {
            AddInclude(d => d.Usages);
        }
    }

    #endregion
}