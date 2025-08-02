using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Üst kategori varlığını kontrol eden tek sorumlu rule
/// </summary>
public class ParentCategoryExistsRule :
    IBusinessRule<CreateCategoryCommand>,
    IBusinessRule<UpdateCategoryCommand>
{
    private readonly IRepository<Category, int> _repository;

    public ParentCategoryExistsRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Category, int>();
    }

    public bool ShouldExecute(CreateCategoryCommand command) => command.ParentId.HasValue;
    public bool ShouldExecute(UpdateCategoryCommand command) => command.ParentId.HasValue;

    public async Task ExecuteAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
        => await ValidateParentExists(command.ParentId!.Value, true, cancellationToken);

    public async Task ExecuteAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
        => await ValidateParentExists(command.ParentId!.Value, false, cancellationToken);

    private async Task ValidateParentExists(int parentId, bool checkIfActive, CancellationToken cancellationToken)
    {
        Specification<Category> spec = checkIfActive
            ? new ByIdAndActiveSpec(parentId)
            : new ByIdSpec(parentId);

        var parentCategory = await _repository.GetAsync(spec, cancellationToken);

        if (parentCategory == null)
        {
            throw new NotFoundException(
                message: $"Parent category with ID {parentId} not found.",
                userFriendlyMessage: "Seçilen üst kategori bulunamadı.",
                errorCode: "PARENT_CATEGORY_NOT_FOUND"
            );
        }

        if (checkIfActive && !parentCategory.IsActive)
        {
            throw new BusinessException(
                message: $"Parent category with ID {parentId} is not active.",
                userFriendlyMessage: "Seçilen üst kategori aktif değil. Lütfen aktif bir üst kategori seçiniz.",
                errorCode: "PARENT_CATEGORY_NOT_ACTIVE"
            );
        }
    }

    public int Priority => 2;

    #region Private Specifications
    private class ByIdSpec : Specification<Category>
    {
        public ByIdSpec(int id)
            : base(category => category.Id == id)
        {
        }
    }

    private class ByIdAndActiveSpec : Specification<Category>
    {
        public ByIdAndActiveSpec(int id)
            : base(category => category.Id == id && category.IsActive)
        {
        }
    }
    #endregion
}