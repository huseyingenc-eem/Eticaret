using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Application.Pipelines.Caching;
using ETicaret.Application.Features.Suppliers.Constants;

namespace ETicaret.Application.Features.Suppliers.Queries.GetById;

public class GetByIdSupplierQuery : IRequest<GetByIdSupplierResponseDto> , ICachableRequest
{
    public int Id { get; set; }
    public bool ByPassCache { get; set; }
    public string CacheKey => $"supplier:{Id}";
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    public TimeSpan? SlidingExpiration { get; set; }

    public class GetByIdSupplierQueryHandler : IRequestHandler<GetByIdSupplierQuery, GetByIdSupplierResponseDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetByIdSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<GetByIdSupplierResponseDto> Handle(GetByIdSupplierQuery request, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _supplierRepository.GetAsync(
                                       filter: s => s.Id == request.Id,
                                       enableTracking: false,
                                       cancellationToken: cancellationToken
                                       );

            if (supplier == null)
                throw new NotFoundException($"Supplier with Id {request.Id} not found.");

            GetByIdSupplierResponseDto response = _mapper.Map<GetByIdSupplierResponseDto>(supplier);

            return response;
        }
    }
}