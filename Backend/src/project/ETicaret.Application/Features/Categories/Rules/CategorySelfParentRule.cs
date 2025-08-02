using Core.Application.Behaviors.Rules;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Commands.Update;

namespace ETicaret.Application.Features.Categories.Rules;

/// <summary>
/// Kategorinin kendisini üst kategori olarak seçmesini engelleyen tek sorumlu rule
/// </summary>
public class CategorySelfParentRule :
    IBusinessRule<UpdateCategoryCommand>
{
    public bool ShouldExecute(UpdateCategoryCommand command) => command.ParentId.HasValue;

    public Task ExecuteAsync(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Id == command.ParentId)
        {
            throw new BusinessException(
                message: $"Category with ID {command.Id} cannot be its own parent.",
                userFriendlyMessage: "Bir kategori kendisini üst kategori olarak seçemez.",
                errorCode: "CATEGORY_CANNOT_BE_ITS_OWN_PARENT"
            );
        }

        return Task.CompletedTask;
    }

    public int Priority => 0;
}