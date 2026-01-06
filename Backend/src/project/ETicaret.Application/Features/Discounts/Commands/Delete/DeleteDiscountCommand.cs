using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Discounts.Constants;
using ETicaret.Application.Features.Discounts.Specifications;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Discounts.Commands.Delete;

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
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountCommandHandler(IDiscountRepository discountRepository,IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<DeleteDiscountResponseDto> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        var spec = new DiscountSpecifications.ById(request.Id);
        Discount discount = (await _discountRepository.GetAsync(spec, cancellationToken))!;

        await _discountRepository.DeleteAsync(discount, permanent: false, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        DeleteDiscountResponseDto response = _mapper.Map<DeleteDiscountResponseDto>(discount);
        response.Message = "İndirim başarıyla silindi.";

        return response;
    }
}