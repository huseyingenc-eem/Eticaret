using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Commands.Update;

/// Mevcut bir indirimi güncellemek için kullanılan komut. Rules Engine tarafından otomatik business rule kontrolü yapılır.
[DefaultRoles("Admin")]
public class UpdateDiscountCommand : IRequest<UpdateDiscountResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MaxUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; }
    public bool IsGlobal { get; set; }

    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
}

public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, UpdateDiscountResponseDto>
{
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDiscountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _discountRepository = unitOfWork.GetRepository<Discount, Guid>();
        _mapper = mapper;
    }

    public async Task<UpdateDiscountResponseDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var spec = new DiscountSpecifications.ById(request.Id);
        Discount discount = (await _discountRepository.GetAsync(spec, cancellationToken))!;

        _mapper.Map(request, discount);

        await _discountRepository.UpdateAsync(discount, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        UpdateDiscountResponseDto response = _mapper.Map<UpdateDiscountResponseDto>(discount);
        response.Message = "İndirim başarıyla güncellendi.";

        return response;
    }
}