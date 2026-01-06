
using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

public class GetListSupplierQuery : IRequest<PagedResult<GetListSupplierResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? CompanyNameSearch { get; set; }
    public bool OnlyActive { get; set; } = true;

    public bool BypassCache { get; set; }
    public string CacheKey => $"supplier-list_page_{PageIndex}_size_{PageSize}_company_{CompanyNameSearch}_active_{OnlyActive}";
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    public TimeSpan? SlidingExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);
}

public class GetListSupplierQueryHandler : IRequestHandler<GetListSupplierQuery, PagedResult<GetListSupplierResponseDto>>
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public GetListSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetListSupplierResponseDto>> Handle(GetListSupplierQuery request, CancellationToken cancellationToken)
    {
        var spec = new SupplierSpecifications.PagedAndFiltered(
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            companyNameSearch: request.CompanyNameSearch?.Trim(),
            onlyActive: request.OnlyActive
        );

        IPaginate<Supplier> suppliersPage = await _supplierRepository.GetPaginatedListAsync(spec, cancellationToken);

        if (suppliersPage.Count == 0 && request.PageIndex == 0)
            throw new NotFoundException(
                message: "No suppliers found in the system.", 
                userFriendlyMessage: "Sistemde hiç tedarikçi bulunamadı.", 
                errorCode: "NO_SUPPLIERS_FOUND");

        List<GetListSupplierResponseDto> supplierDtos = _mapper.Map<List<GetListSupplierResponseDto>>(suppliersPage.Items);

        return new PagedResult<GetListSupplierResponseDto>(
            items: supplierDtos,
            count: suppliersPage.Count,
            index: suppliersPage.Index,
            size: suppliersPage.Size
        );
    }
}