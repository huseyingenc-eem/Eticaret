using AutoMapper;
using Core.Application.Abstractions.Paging;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Features.Products.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetList;

public class GetListProductQuery : IRequest<PagedResult<GetListProductResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? NameSearch { get; set; }
    public int? CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public bool OnlyActive { get; set; } = true;

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"product-list_page_{PageIndex}_size_{PageSize}_name_{NameSearch}_category_{CategoryId}_supplier_{SupplierId}_active_{OnlyActive}";
    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);
    #endregion
}

public class GetListProductQueryHandler : IRequestHandler<GetListProductQuery, PagedResult<GetListProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetListProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetListProductResponseDto>> Handle(GetListProductQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductSpecifications.PagedAndFiltered(
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            nameSearch: request.NameSearch?.Trim(),
            categoryId: request.CategoryId,
            supplierId: request.SupplierId,
            onlyActive: request.OnlyActive
        );

        IPaginate<Product> productsPage = await _productRepository.GetPaginatedListAsync(spec, cancellationToken);

        if (productsPage.Count == 0 && request.PageIndex == 0)
        {
            throw new NotFoundException(
                message: "No products found in the system.",
                userFriendlyMessage: "Sistemde hiç ürün bulunamadı.",
                errorCode: "NO_PRODUCTS_FOUND"
            );
        }

        List<GetListProductResponseDto> productDtos = _mapper.Map<List<GetListProductResponseDto>>(productsPage.Items);

        return new PagedResult<GetListProductResponseDto>(
            items: productDtos,
            count: productsPage.Count,
            index: productsPage.Index,
            size: productsPage.Size
        );
    }
}