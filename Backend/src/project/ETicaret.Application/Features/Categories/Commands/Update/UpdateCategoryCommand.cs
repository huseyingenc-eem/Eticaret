using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Rules;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Update;

/// <summary>
/// Mevcut bir kategoriyi güncelleme işlemini temsil eden komut.
/// ITransactionalRequest: Ensures the entire operation runs within a single database transaction.
/// ICacheRemoverRequest: Clears relevant cache entries upon successful completion.
/// </summary>
public class UpdateCategoryCommand : IRequest<UpdateCategoryResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    #region Cache Settings
    // Clears the specific cache for this category ID
    public string CacheKey => $"category:{Id}";
    // Also clears the general group cache for categories
    public string? CacheGroupKey => "CategoriesGroup";
    public bool BypassCache { get; set; }
    #endregion

    /// <summary>
    /// Handles the UpdateCategoryCommand request.
    /// </summary>
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponseDto>
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules;

        public UpdateCategoryCommandHandler(
            IUnitOfWork unitOfWork, // <-- Injected IUnitOfWork
            IMapper mapper,
            CategoryBusinessRules categoryBusinessRules)
        {
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules;
            // Get the generic repository from the Unit of Work
            _categoryRepository = unitOfWork.GetRepository<Category, int>();
        }

        public async Task<UpdateCategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the category to update using our new specification.
            var spec = new CategorySpecifications.ById(request.Id);
            Category? categoryToUpdate = await _categoryRepository.GetAsync(spec, cancellationToken);

            if (categoryToUpdate == null)
            {
                throw new NotFoundException($"Category with Id {request.Id} not found.");
            }

            // 2. Execute all business rules to validate the operation.
            // The business rules class now handles all validation logic.
            await _categoryBusinessRules.CheckCategoryNameCanNotBeDuplicatedWhenUpdatedAsync(request.Id, request.Name, cancellationToken);
            _categoryBusinessRules.CheckCategoryCanNotBeItsOwnParent(request.Id, request.ParentId);
            await _categoryBusinessRules.CheckParentCategoryExistsAsync(request.ParentId, cancellationToken);
            await _categoryBusinessRules.CheckNoCyclicDependencyWhenUpdatingParentAsync(request.Id, request.ParentId, cancellationToken);

            // 3. Map the incoming request data onto the entity fetched from the database.
            _mapper.Map(request, categoryToUpdate);

            // 4. Mark the entity for update.
            await _categoryRepository.UpdateAsync(categoryToUpdate, cancellationToken);

            // 5. The call to _unitOfWork.CompleteAsync() is no longer needed here.
            // Since our command implements ITransactionalRequest, the TransactionBehavior
            // will automatically call CompleteAsync and CommitTransactionAsync for us
            // after this handler executes successfully.

            UpdateCategoryResponseDto response = _mapper.Map<UpdateCategoryResponseDto>(categoryToUpdate);
            response.Message = "Category updated successfully.";

            return response;
        }
    }
}