using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications; // <-- Our new specification
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;

public class GetCategoryWithProductsQuery : IRequest<GetCategoryWithProductsResponseDto>
{
    public int Id { get; set; }

    public class GetCategoryWithProductsQueryHandler : IRequestHandler<GetCategoryWithProductsQuery, GetCategoryWithProductsResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Category, int> _categoryRepository;

        public GetCategoryWithProductsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            // Use the generic repository from the Unit of Work.
            _categoryRepository = unitOfWork.GetRepository<Category, int>();
        }

        public async Task<GetCategoryWithProductsResponseDto> Handle(GetCategoryWithProductsQuery request, CancellationToken cancellationToken)
        {
            // 1. Create the specific specification that also includes products.
            var spec = new CategoryWithProductsSpecification(request.Id);

            // 2. Fetch the category using the specification.
            var category = await _categoryRepository.GetAsync(spec, cancellationToken);

            // 3. If not found, throw a clear exception.
            if (category == null)
            {
                throw new NotFoundException($"Category with Id {request.Id} not found.");
            }

            // 4. Map the resulting entity (which now includes products) to the response DTO.
            var response = _mapper.Map<GetCategoryWithProductsResponseDto>(category);

            return response;
        }
    }
}