using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Queries.GetActiveList;

/// <summary>
/// Şu anda aktif olan tüm indirimleri getiren sorgu. Müşteri arayüzü için optimize edilmiş.
/// </summary>
public class GetActiveDiscountQuery : IRequest<List<GetActiveDiscountResponseDto>>,
    ICachableRequest,
    IPublicRequest
{
    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => "active-discounts";
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromMinutes(30);
    #endregion
}

public class GetActiveDiscountQueryHandler : IRequestHandler<GetActiveDiscountQuery, List<GetActiveDiscountResponseDto>>
{
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;

    public GetActiveDiscountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _discountRepository = unitOfWork.GetRepository<Discount, Guid>();
        _mapper = mapper;
    }

    public async Task<List<GetActiveDiscountResponseDto>> Handle(GetActiveDiscountQuery request, CancellationToken cancellationToken)
    {
        var spec = new DiscountSpecifications.ActiveAndValid(DateTime.UtcNow);
        List<Discount> discounts = await _discountRepository.GetListAsync(spec, cancellationToken);

        return _mapper.Map<List<GetActiveDiscountResponseDto>>(discounts);
    }
}