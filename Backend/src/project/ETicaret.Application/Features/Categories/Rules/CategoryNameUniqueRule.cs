using Core.Application.Abstractions.Repositories;
using Core.Application.Abstractions.Specifications;
using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Kategori adının benzersiz olmasını kontrol eden tek sorumlu rule
/// </summary>
public class CategoryNameUniqueRule :
    IBusinessRule<CreateCategoryCommand>,
    IBusinessRule<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryNameUniqueRule(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public bool ShouldExecute(CreateCategoryCommand command) => !string.IsNullOrWhiteSpace(command.Name);
    public bool ShouldExecute(UpdateCategoryCommand command) => !string.IsNullOrWhiteSpace(command.Name);

    public async Task ExecuteAsync(CreateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByNameSpec(command.Name);
        bool isDuplicate = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"A category with the name '{command.Name}' already exists in the database.",
                userFriendlyMessage: $"'{command.Name}' isminde bir kategori zaten mevcut. Lütfen farklı bir kategori adı seçiniz.",
                errorCode: "DUPLICATE_CATEGORY_NAME"
            );
        }
    }

    public async Task ExecuteAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByNameExcludingIdSpec(command.Name, command.Id);
        bool isDuplicate = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"A category with the name '{command.Name}' already exists in the database.",
                userFriendlyMessage: $"'{command.Name}' isminde başka bir kategori zaten mevcut. Lütfen farklı bir kategori adı seçiniz.",
                errorCode: "DUPLICATE_CATEGORY_NAME"
            );
        }
    }

    public int Priority => 1;

    #region Private Specifications - Bu rule'a özel
    private class ByNameSpec : Specification<Category>
    {
        public ByNameSpec(string name)
            : base(category => category.Name.ToLower() == name.ToLower())
        {
        }
    }

    private class ByNameExcludingIdSpec : Specification<Category>
    {
        public ByNameExcludingIdSpec(string name, int excludeId)
            : base(category => category.Name.ToLower() == name.ToLower() && category.Id != excludeId)
        {
        }
    }
    #endregion
}