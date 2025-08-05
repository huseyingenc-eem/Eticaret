using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Commands.Delete;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Discounts.Rules;

/// <summary>
/// İndirimin kullanımda olmadığı durumda silinebileceğini kontrol eder.
/// </summary>
public class DiscountCannotBeDeletedIfInUseRule : IBusinessRule<DeleteDiscountCommand>
{
    private readonly IRepository<Discount, Guid> _repository;

    public DiscountCannotBeDeletedIfInUseRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Discount, Guid>();
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