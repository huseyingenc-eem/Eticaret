using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Queries.GetByCode;

public class GetByCodeDiscountQuery : IRequest<GetByCodeDiscountResponseDto>,
    ICachableRequest,
    IPublicRequest
{
    public string DiscountCode { get; set; } = string.Empty;

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"discount-code_{DiscountCode.ToLower()}";
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(15);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);
    #endregion
}

public class GetByCodeDiscountQueryHandler : IRequestHandler<GetByCodeDiscountQuery, GetByCodeDiscountResponseDto>
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public GetByCodeDiscountQueryHandler(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<GetByCodeDiscountResponseDto> Handle(GetByCodeDiscountQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DiscountCode))
        {
            throw new BusinessException(
                message: "Discount code cannot be empty.",
                userFriendlyMessage: "İndirim kodu boş olamaz.",
                errorCode: "INVALID_DISCOUNT_CODE"
            );
        }

        var spec = new DiscountSpecifications.ByDiscountCode(request.DiscountCode);
        var discount = await _discountRepository.GetAsync(spec, cancellationToken);

        if (discount == null)
        {
            throw new NotFoundException(
                message: $"Discount with code '{request.DiscountCode}' not found.",
                userFriendlyMessage: "Belirtilen indirim kodu bulunamadı.",
                errorCode: DiscountConstants.ErrorCodes.DiscountNotFound
            );
        }

        DateTime currentDate = DateTime.UtcNow;
        if (!discount.IsActive ||
            discount.StartDate > currentDate ||
            (discount.EndDate.HasValue && discount.EndDate.Value < currentDate))
        {
            throw new BusinessException(
                message: $"Discount with code '{request.DiscountCode}' is not active or has expired.",
                userFriendlyMessage: "Bu indirim kodu aktif değil veya süresi dolmuş.",
                errorCode: DiscountConstants.ErrorCodes.DiscountExpired
            );
        }

        return _mapper.Map<GetByCodeDiscountResponseDto>(discount);
    }
}