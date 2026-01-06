using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Results;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Queries.GetList;

public class GetListDiscountQuery : IRequest<PagedResult<GetListDiscountResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? NameSearch { get; set; }
    public bool OnlyActive { get; set; } = true;

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"discount-list_page_{PageIndex}_size_{PageSize}_name_{NameSearch}_active_{OnlyActive}";
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(30);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(2);
    #endregion
}

public class GetListDiscountQueryHandler : IRequestHandler<GetListDiscountQuery, PagedResult<GetListDiscountResponseDto>>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public GetListDiscountQueryHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<GetListDiscountResponseDto>> Handle(GetListDiscountQuery request, CancellationToken cancellationToken)
    {
        var spec = new DiscountSpecifications.PagedAndFiltered(
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            nameSearch: request.NameSearch?.Trim(),
            onlyActive: request.OnlyActive
        );

        var discountsPage = await _discountRepository.GetPaginatedListAsync(spec, cancellationToken);

        if (discountsPage.Count == 0 && request.PageIndex == 0)
        {
            throw new NotFoundException(
                message: "No discounts found in the system.",
                userFriendlyMessage: "Sistemde hiç indirim bulunamadı.",
                errorCode: "NO_DISCOUNTS_FOUND"
            );
        }

        List<GetListDiscountResponseDto> discountDtos = _mapper.Map<List<GetListDiscountResponseDto>>(discountsPage.Items);

        return new PagedResult<GetListDiscountResponseDto>(
            items: discountDtos,
            count: discountsPage.Count,
            index: discountsPage.Index,
            size: discountsPage.Size
        );
    }
}