using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Categories.Commands.Update;

public class CategoryUpdateCommand : IRequest<CategoryUpdateResponseDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    // Yeni eklenen alanlar
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    public class CategoryUpdateCommandHandler : IRequestHandler<CategoryUpdateCommand, CategoryUpdateResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryUpdateCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryUpdateResponseDto> Handle(CategoryUpdateCommand request, CancellationToken cancellationToken)
        {
            var categoryToUpdate = await _categoryRepository.GetAsync(
                c => c.Id == request.Id,
                include: false,
                cancellationToken: cancellationToken);

            if (categoryToUpdate == null)
            {
                throw new NotFoundException($"Category with Id {request.Id} not found.");
            }

            _mapper.Map(request, categoryToUpdate);

            await _categoryRepository.UpdateAsync(categoryToUpdate, cancellationToken);

            CategoryUpdateResponseDto response = _mapper.Map<CategoryUpdateResponseDto>(categoryToUpdate);
            response.Message = "Kategori başarıyla güncellendi.";
            return response;
        }
    }
}