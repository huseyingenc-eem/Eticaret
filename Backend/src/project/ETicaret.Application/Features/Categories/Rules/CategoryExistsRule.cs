using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Domain.Entities;
using ETicaret.Application.Services.Repositories;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Kategorinin var olduğunu kontrol eden tek sorumlu rule
/// </summary>
public class CategoryExistsRule :
    IBusinessRule<UpdateCategoryCommand>,
    IBusinessRule<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryExistsRule(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public bool ShouldExecute(UpdateCategoryCommand command) => true;
    public bool ShouldExecute(DeleteCategoryCommand command) => true;

    public async Task ExecuteAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
        => await ValidateExists(command.Id, cancellationToken);

    public async Task ExecuteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
        => await ValidateExists(command.Id, cancellationToken);

    private async Task ValidateExists(int categoryId, CancellationToken cancellationToken)
    {
        var spec = new ByIdSpec(categoryId);
        var category = await _categoryRepository.GetAsync(spec, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(
                message: $"Category with ID {categoryId} not found.",
                userFriendlyMessage: "Kategori bulunamadı.",
                errorCode: "CATEGORY_NOT_FOUND"
            );
        }
    }

    public int Priority => 0;

    #region Private Specification - Bu rule'a özel
    private class ByIdSpec : Specification<Category>
    {
        public ByIdSpec(int id)
            : base(category => category.Id == id)
        {
        }
    }
    #endregion
}