using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Commands.Create;

/// Yeni bir indirim oluşturmak için kullanılan komut. Rules Engine tarafından otomatik business rule kontrolü yapılır.
[DefaultRoles("Admin")]
public class CreateDiscountCommand : IRequest<CreateDiscountResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public string? Description { get; set; }
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal? MinimumPurchaseAmount { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public int? MaxUses { get; set; }
    public int? MaxUsesPerUser { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsGlobal { get; set; } = false;

    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
}

public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, CreateDiscountResponseDto>
{
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscountCommandHandler(IDiscountRepository discountRepository, IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _discountRepository =discountRepository;
        _mapper = mapper;
    }

    public async Task<CreateDiscountResponseDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        Discount discount = _mapper.Map<Discount>(request);

        await _discountRepository.AddAsync(discount, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        CreateDiscountResponseDto response = _mapper.Map<CreateDiscountResponseDto>(discount);
        response.Message = "İndirim başarıyla oluşturuldu.";

        return response;
    }
}