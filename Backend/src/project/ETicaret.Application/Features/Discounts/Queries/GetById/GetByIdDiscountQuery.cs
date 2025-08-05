using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Queries.GetById;

/// <summary>
/// Belirtilen ID'ye sahip indirimi getiren sorgu. Önbellekleme ve yetkilendirme desteği ile.
/// </summary>
public class GetByIdDiscountQuery : IRequest<GetByIdDiscountResponseDto>,
    ICachableRequest,
    IPublicRequest  // Herkes indirim detaylarını görebilir
{
    public Guid Id { get; set; }

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"discount-detail_{Id}";
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(2);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(6);
    #endregion
}

public class GetByIdDiscountQueryHandler : IRequestHandler<GetByIdDiscountQuery, GetByIdDiscountResponseDto>
{
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;

    public GetByIdDiscountQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _discountRepository = unitOfWork.GetRepository<Discount, Guid>();
        _mapper = mapper;
    }

    public async Task<GetByIdDiscountResponseDto> Handle(GetByIdDiscountQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException(
                message: "Discount ID cannot be empty.",
                userFriendlyMessage: "Geçersiz indirim ID'si.",
                errorCode: "INVALID_DISCOUNT_ID"
            );
        }

        var spec = new DiscountSpecifications.ById(request.Id);
        Discount? discount = await _discountRepository.GetAsync(spec, cancellationToken);

        if (discount == null)
        {
            throw new NotFoundException(
                message: $"Discount with ID {request.Id} not found.",
                userFriendlyMessage: "Belirtilen indirim bulunamadı.",
                errorCode: DiscountConstants.ErrorCodes.DiscountNotFound
            );
        }

        return _mapper.Map<GetByIdDiscountResponseDto>(discount);
    }
}