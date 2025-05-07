using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Commands.Create;

public class CategoryAddCommand : IRequest<CategoryAddResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public class CategoryAddCommandHandler : IRequestHandler<CategoryAddCommand, CategoryAddResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryAddCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryAddResponseDto> Handle(CategoryAddCommand request, CancellationToken cancellationToken)
        {
            
            Category category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);

            CategoryAddResponseDto response = _mapper.Map<CategoryAddResponseDto>(category);
            response.Message = "Kategori başarıyla eklendi.";

            return response;
        }
    }
}
