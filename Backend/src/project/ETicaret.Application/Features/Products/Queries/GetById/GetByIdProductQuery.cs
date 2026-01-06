using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Features.Products.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Queries.GetById;

public class GetByIdProductQuery : IRequest<GetByIdProductResponseDto>,
    ICachableRequest,
    IPublicRequest
{
    public Guid Id { get; set; }

    #region Cache Settings
    public bool BypassCache { get; set; }
    public string CacheKey => $"product-detail_{Id}";
    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(2);
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(6);
    #endregion
}

public class GetByIdProductQueryHandler : IRequestHandler<GetByIdProductQuery, GetByIdProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetByIdProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetByIdProductResponseDto> Handle(GetByIdProductQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new BusinessException(
                message: "Product ID cannot be empty.",
                userFriendlyMessage: "Geçersiz ürün ID'si.",
                errorCode: "INVALID_PRODUCT_ID"
            );
        }

        var spec = new ProductSpecifications.ByIdWithDetails(request.Id);
        var product = await _productRepository.GetAsync(spec, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(
                message: $"Product with ID {request.Id} not found.",
                userFriendlyMessage: "Belirtilen ürün bulunamadı.",
                errorCode: ProductConstants.ErrorCodes.ProductNotFound
            );
        }

        return _mapper.Map<GetByIdProductResponseDto>(product);
    }
}