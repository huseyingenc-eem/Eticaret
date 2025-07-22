using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Categories.Rules;
using Core.Application.Abstractions.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Create;

public class CreateCategoryCommand : IRequest<CreateCategoryResponseDto> , ITransactionalRequest , ICacheRemoverRequest
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public string? CacheKey => null;
    public string? CacheGroupKey => "CategoriesGroup";
    public bool BypassCache { get; set; }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CategoryBusinessRules _categoryBusinessRules; // Enjekte edilen iş kuralları servisi


        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, CategoryBusinessRules categoryBusinessRules)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryBusinessRules = categoryBusinessRules; // Atama yapıldı
        }

        public async Task<CreateCategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            await _categoryBusinessRules.CheckCategoryNameCanNotBeDuplicatedWhenInsertedAsync(request.Name, cancellationToken);

            await _categoryBusinessRules.CheckParentCategoryExistsAsync(request.ParentId, cancellationToken, checkIfActive: true);
            // --- İş Kuralları Kontrolleri Sonu ---
            Category category = _mapper.Map<Category>(request);

            var categoryRepository = _unitOfWork.GetRepository<Category, int>();

            await categoryRepository.AddAsync(category, cancellationToken);
            CreateCategoryResponseDto response = _mapper.Map<CreateCategoryResponseDto>(category);
            response.Message = "Kategori başarıyla eklendi.";

            return response;
        }
    }
}
