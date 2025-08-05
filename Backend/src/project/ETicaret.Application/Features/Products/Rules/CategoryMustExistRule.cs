using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Rules;

/// <summary>
/// Ürün eklenirken/güncellenirken Category'nin mevcut olduğunu kontrol eden rule.
/// </summary>
public class CategoryMustExistRule :
    IBusinessRule<CreateProductCommand>,
    IBusinessRule<UpdateProductCommand>
{
    private readonly IRepository<Category, int> _repository;

    public CategoryMustExistRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Category, int>();
    }

    public bool ShouldExecute(CreateProductCommand command) => command.CategoryId > 0;
    public bool ShouldExecute(UpdateProductCommand command) => command.CategoryID > 0;

    public async Task ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateCategoryExists(command.CategoryId, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        await ValidateCategoryExists(command.CategoryID, cancellationToken);
    }

    private async Task ValidateCategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        var spec = new CategoryExistsSpec(categoryId);
        var categoryExists = await _repository.AnyAsync(spec, cancellationToken);

        if (!categoryExists)
        {
            throw new NotFoundException(
                message: $"Category with ID {categoryId} not found.",
                userFriendlyMessage: "Belirtilen kategori bulunamadı.",
                errorCode: ProductConstants.ErrorCodes.InvalidCategory
            );
        }
    }

    public int Priority => 2;

    #region Private Specification - Bu rule'a özel

    private class CategoryExistsSpec : Specification<Category>
    {
        public CategoryExistsSpec(int categoryId)
            : base(c => c.Id == categoryId && c.IsActive) { }
    }

    #endregion
}