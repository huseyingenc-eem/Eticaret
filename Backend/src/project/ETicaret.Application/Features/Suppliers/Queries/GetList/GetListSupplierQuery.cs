using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;



using Core.Application.Pipelines.Caching;
using ETicaret.Application.Features.Suppliers.Constants;

namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierQuery : IRequest<IPaginate<GetListSupplierResponseDto>>, ICachableRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    public bool ByPassCache { get; set; }

    public string CacheKey => $"supplier-list_page_{PageIndex}_size_{PageSize}";
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    public TimeSpan? SlidingExpiration { get; set; }

    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);

    public class GetListSupplierQueryHandler : IRequestHandler<GetListSupplierQuery, IPaginate<GetListSupplierResponseDto>>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetListSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<IPaginate<GetListSupplierResponseDto>> Handle(GetListSupplierQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Supplier> suppliersPage = await _supplierRepository.GetListAsync(
                filter: s => s.IsActive,
                orderBy: q => q.OrderBy(s => s.CompanyName), 
                index: request.PageIndex,
                size: request.PageSize,
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            IPaginate<GetListSupplierResponseDto> responsePage = _mapper.Map<IPaginate<GetListSupplierResponseDto>>(suppliersPage);

            return responsePage;
        }
    }
}