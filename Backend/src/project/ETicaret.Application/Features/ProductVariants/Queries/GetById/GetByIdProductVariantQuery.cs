using AutoMapper;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Features.ProductVariants.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetById;

public class GetByIdProductVariantQuery : IRequest<GetByIdProductVariantResponseDto>,
    ICachableRequest
{
    public Guid Id { get; set; }

    #region Cache Settings
    public string CacheKey => $"product-variant-{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;

    public TimeSpan? SlidingExpiration => throw new NotImplementedException();

    public TimeSpan? AbsoluteExpirationRelativeToNow => throw new NotImplementedException();
    #endregion
}

public class GetByIdProductVariantQueryHandler : IRequestHandler<GetByIdProductVariantQuery, GetByIdProductVariantResponseDto>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;

    public GetByIdProductVariantQueryHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdProductVariantResponseDto> Handle(GetByIdProductVariantQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductVariantSpecifications.ByIdWithDetails(request.Id);
        var productVariant = await _productVariantRepository.GetAsync(spec, cancellationToken);

        if (productVariant == null)
        {
            throw new NotFoundException(
                message: $"ProductVariant with ID {request.Id} not found.",
                userFriendlyMessage: "Belirtilen ürün varyantı bulunamadı.",
                errorCode: ProductVariantConstants.ErrorCodes.ProductVariantNotFound
            );
        }

        return _mapper.Map<GetByIdProductVariantResponseDto>(productVariant);
    }
}