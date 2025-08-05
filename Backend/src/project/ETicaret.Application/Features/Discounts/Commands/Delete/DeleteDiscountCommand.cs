using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Commands.Delete;

/// Mevcut bir indirimi silmek için kullanılan komut. Rules Engine tarafından otomatik business rule kontrolü yapılır.
[DefaultRoles("Admin")]
public class DeleteDiscountCommand : IRequest<DeleteDiscountResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    public Guid Id { get; set; }

    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => DiscountConstants.DiscountsCacheGroup;
}

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, DeleteDiscountResponseDto>
{
    private readonly IRepository<Discount, Guid> _discountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _discountRepository = unitOfWork.GetRepository<Discount, Guid>();
        _mapper = mapper;
    }

    public async Task<DeleteDiscountResponseDto> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        // Rules Engine tarafından şunlar kontrol edildi:
        // 1. İndirimin mevcut olduğu (DiscountMustExistRule)
        // 2. İndirimin kullanımda olmadığı (DiscountCannotBeDeletedIfInUseRule)

        // Mevcut indirimi getir (rule'lar zaten varlığını doğruladı)
        var spec = new DiscountSpecifications.ById(request.Id);
        Discount discount = (await _discountRepository.GetAsync(spec, cancellationToken))!;

        // Soft delete işlemi
        await _discountRepository.DeleteAsync(discount, permanent: false, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // AutoMapper ile Entity'den DTO'ya dönüşüm
        DeleteDiscountResponseDto response = _mapper.Map<DeleteDiscountResponseDto>(discount);
        response.Message = "İndirim başarıyla silindi.";

        return response;
    }
}