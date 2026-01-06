using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Delete;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Kategorinin ürünlerinin olup olmadığını kontrol eden tek sorumlu rule
/// </summary>
public class CategoryHasProductsRule : IBusinessRule<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryHasProductsRule(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public bool ShouldExecute(DeleteCategoryCommand command) => true;

    public async Task ExecuteAsync(DeleteCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new CategoryWithProductsSpec(command.Id);
        var category = await _categoryRepository.GetAsync(spec, cancellationToken);

        if (category?.Products.Any() == true)
        {
            throw new BusinessException(
                message: $"Category with ID {command.Id} cannot be deleted because it has {category.Products.Count} associated products.",
                userFriendlyMessage: "Bu kategori silinemez çünkü kendisine bağlı ürünler bulunmaktadır. Önce ürünleri başka kategoriye taşıyın veya silin.",
                errorCode: "CATEGORY_HAS_PRODUCTS"
            );
        }
    }

    public int Priority => 1;

    #region Private Specification - Bu rule'a özel
    private class CategoryWithProductsSpec : Specification<Category>
    {
        public CategoryWithProductsSpec(int categoryId)
            : base(category => category.Id == categoryId)
        {
            AddInclude(c => c.Products);
        }
    }
    #endregion
}