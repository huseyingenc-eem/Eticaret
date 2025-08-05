using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Queries.GetByCode;

/// <summary>
/// Belirtilen discount code'a sahip aktif indirimi getiren sorgu. Müşterilerin kupon kodu ile indirim araması için.
/// </summary>
public class GetByCodeDiscountQuery : IRequest<GetByCodeDiscountResponseDto>,
    ICachableRequest,
    IPublicRequest  // Müşteriler kupon kodunu kontrol edebilir
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
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;

    public GetByCodeDiscountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _discountRepository = unitOfWork.GetRepository<Discount, Guid>();
        _mapper = mapper;
    }

    public async Task<GetByCodeDiscountResponseDto> Handle(GetByCodeDiscountQuery request, CancellationToken cancellationToken)
    {
        // Minimal validation
        if (string.IsNullOrWhiteSpace(request.DiscountCode))
        {
            throw new BusinessException(
                message: "Discount code cannot be empty.",
                userFriendlyMessage: "İndirim kodu boş olamaz.",
                errorCode: "INVALID_DISCOUNT_CODE"
            );
        }

        var spec = new DiscountSpecifications.ByDiscountCode(request.DiscountCode);
        Discount? discount = await _discountRepository.GetAsync(spec, cancellationToken);

        if (discount == null)
        {
            throw new NotFoundException(
                message: $"Discount with code '{request.DiscountCode}' not found.",
                userFriendlyMessage: "Belirtilen indirim kodu bulunamadı.",
                errorCode: DiscountConstants.ErrorCodes.DiscountNotFound
            );
        }

        // İndirim aktif mi ve geçerli tarih aralığında mı kontrol et
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