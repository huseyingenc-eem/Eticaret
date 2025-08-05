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
/// Product adının benzersiz olması gerektiğini kontrol eden tek sorumlu rule.
/// </summary>
public class ProductNameMustBeUniqueRule :
    IBusinessRule<CreateProductCommand>,
    IBusinessRule<UpdateProductCommand>
{
    private readonly IRepository<Product, Guid> _repository;

    public ProductNameMustBeUniqueRule(IUnitOfWork unitOfWork)
    {
        _repository = unitOfWork.GetRepository<Product, Guid>();
    }

    public bool ShouldExecute(CreateProductCommand command) => !string.IsNullOrWhiteSpace(command.Name);
    public bool ShouldExecute(UpdateProductCommand command) => !string.IsNullOrWhiteSpace(command.Name);

    public async Task ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByNameSpec(command.Name);
        await ValidateUniqueness(spec, command.Name, null, cancellationToken);
    }

    public async Task ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        var spec = new ByNameExcludingIdSpec(command.Id, command.Name);
        await ValidateUniqueness(spec, command.Name, command.Id, cancellationToken);
    }

    private async Task ValidateUniqueness(Specification<Product> spec, string productName, Guid? excludeId, CancellationToken cancellationToken)
    {
        var existingProduct = await _repository.GetAsync(spec, cancellationToken);

        if (existingProduct != null)
        {
            throw new BusinessException(
                message: $"A product with name '{productName}' already exists.",
                userFriendlyMessage: $"'{productName}' adlı bir ürün zaten mevcut.",
                errorCode: ProductConstants.ErrorCodes.DuplicateProductName
            );
        }
    }

    public int Priority => 1;

    #region Private Specifications - Bu rule'a özel

    private class ByNameSpec : Specification<Product>
    {
        public ByNameSpec(string name)
            : base(p => p.Name.ToLower() == name.ToLower()) { }
    }

    private class ByNameExcludingIdSpec : Specification<Product>
    {
        public ByNameExcludingIdSpec(Guid excludeId, string name)
            : base(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower()) { }
    }

    #endregion
}