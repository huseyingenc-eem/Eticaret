using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Results;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Features.ProductVariants.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetList;

/// <summary>
/// Admin paneli için sayfalanmış ürün varyantları listesini getirir. Filtreleme ve önbellekleme desteği ile.
/// </summary>
[DefaultRoles("Admin")]
public class GetListProductVariantQuery : IRequest<PagedResult<GetListProductVariantResponseDto>>,
    ICachableRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;
    public string? SkuFilter { get; set; }
    public Guid? ProductIdFilter { get; set; }
    public bool? IsActiveFilter { get; set; }
    public bool? OnlyLowStock { get; set; }
    public int LowStockThreshold { get; set; } = 10;

    #region Cache Settings
    public string CacheKey => $"product-variants-list-{PageIndex}-{PageSize}-{SkuFilter}-{ProductIdFilter}-{IsActiveFilter}-{OnlyLowStock}-{LowStockThreshold}";
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;

    public TimeSpan? SlidingExpiration => throw new NotImplementedException();

    public TimeSpan? AbsoluteExpirationRelativeToNow => throw new NotImplementedException();
    #endregion
}

public class GetListProductVariantQueryHandler : IRequestHandler<GetListProductVariantQuery, PagedResult<GetListProductVariantResponseDto>>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;

    public GetListProductVariantQueryHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetListProductVariantResponseDto>> Handle(GetListProductVariantQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductVariantSpecifications.AdminPagedAndFiltered(
            request.PageIndex,
            request.PageSize,
            request.SkuFilter,
            request.ProductIdFilter,
            request.IsActiveFilter,
            request.OnlyLowStock,
            request.LowStockThreshold
        );

        var paginatedResult = await _productVariantRepository.GetPaginatedListAsync(spec, cancellationToken);
        var mappedItems = _mapper.Map<List<GetListProductVariantResponseDto>>(paginatedResult.Items);

        return new PagedResult<GetListProductVariantResponseDto>(
            mappedItems,
            paginatedResult.Count,
            request.PageIndex,
            request.PageSize
        );
    }
}