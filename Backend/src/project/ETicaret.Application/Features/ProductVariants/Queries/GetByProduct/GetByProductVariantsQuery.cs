using AutoMapper;
using Core.Application.Behaviors.Caching;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Features.ProductVariants.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Queries.GetByProduct;

public class GetByProductVariantsQuery : IRequest<List<GetByProductVariantsResponseDto>>,
    ICachableRequest
{
    public Guid ProductId { get; set; }
    public bool OnlyActive { get; set; } = true;

    #region Cache Settings
    public string CacheKey => $"product-variants-{ProductId}-active-{OnlyActive}";
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;

    public TimeSpan? SlidingExpiration => throw new NotImplementedException();

    public TimeSpan? AbsoluteExpirationRelativeToNow => throw new NotImplementedException();
    #endregion
}

public class GetByProductVariantsQueryHandler : IRequestHandler<GetByProductVariantsQuery, List<GetByProductVariantsResponseDto>>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;

    public GetByProductVariantsQueryHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository;
        _mapper = mapper;
    }

    public async Task<List<GetByProductVariantsResponseDto>> Handle(GetByProductVariantsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductVariantSpecifications.ActiveByProduct(request.ProductId);
        var productVariants = await _productVariantRepository.GetListAsync(spec, cancellationToken);

        return _mapper.Map<List<GetByProductVariantsResponseDto>>(productVariants);
    }
}