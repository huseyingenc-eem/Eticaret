using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Döngüsel bağımlılığı engelleyen tek sorumlu rule
/// </summary>
public class CategoryCyclicDependencyRule : IBusinessRule<UpdateCategoryCommand>
{
    private readonly IRepository<Category, int> _repository;

    public CategoryCyclicDependencyRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Category, int>();
    }

    public bool ShouldExecute(UpdateCategoryCommand command) => command.ParentId.HasValue;

    public async Task ExecuteAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        bool hasCycle = await CheckForCyclicDependencyAsync(command.Id, command.ParentId!.Value, cancellationToken);

        if (hasCycle)
        {
            throw new BusinessException(
                message: $"Setting category {command.ParentId} as parent of category {command.Id} would create a cyclic dependency.",
                userFriendlyMessage: "Bu üst kategori seçimi döngüsel bir bağımlılık yaratacaktır. Lütfen farklı bir üst kategori seçiniz.",
                errorCode: "CYCLIC_DEPENDENCY_DETECTED"
            );
        }
    }

    private async Task<bool> CheckForCyclicDependencyAsync(int categoryId, int proposedParentId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();

        int? currentId = proposedParentId;

        while (currentId.HasValue)
        {
            if (currentId.Value == categoryId)
                return true;

            if (visited.Contains(currentId.Value))
                break;

            visited.Add(currentId.Value);

            var spec = new ParentIdSpec(currentId.Value);
            var category = await _repository.GetAsync(spec, cancellationToken);

            currentId = category?.ParentId;
        }

        return false;
    }

    public int Priority => 3;

    #region Private Specification
    private class ParentIdSpec : Specification<Category>
    {
        public ParentIdSpec(int id)
            : base(category => category.Id == id)
        {
        }
    }
    #endregion
}