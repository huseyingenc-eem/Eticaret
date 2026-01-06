using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Kategorinin alt kategorilerinin olup olmadığını kontrol eden tek sorumlu rule
/// </summary>
public class CategoryHasChildrenRule : IBusinessRule<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryHasChildrenRule(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public bool ShouldExecute(DeleteCategoryCommand command) => true;

    public async Task ExecuteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new CategoryWithChildrenSpec(command.Id);
        var category = await _categoryRepository.GetAsync(spec, cancellationToken);

        if (category?.Children.Any() == true)
        {
            throw new BusinessException(
                message: $"Category with ID {command.Id} cannot be deleted because it has {category.Children.Count} child categories.",
                userFriendlyMessage: "Bu kategori silinemez çünkü alt kategorileri bulunmaktadır. Önce alt kategorileri silin veya başka bir üst kategoriye taşıyın.",
                errorCode: "CATEGORY_HAS_CHILDREN"
            );
        }
    }

    public int Priority => 2;

    #region Private Specification - Bu rule'a özel
    private class CategoryWithChildrenSpec : Specification<Category>
    {
        public CategoryWithChildrenSpec(int categoryId)
            : base(category => category.Id == categoryId)
        {
            AddInclude(c => c.Children);
        }
    }
    #endregion
}