using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Commands.Create;

[DefaultRoles("Admin")]
public class CreateProductVariantCommand : IRequest<CreateProductVariantResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? VariantImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AttributeDescription { get; set; }

    #region Cache Settings
    public string? CacheKey => null;
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;
    #endregion
}

public class CreateProductVariantCommandHandler : IRequestHandler<CreateProductVariantCommand, CreateProductVariantResponseDto>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;

    public CreateProductVariantCommandHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductVariantResponseDto> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var productVariant = _mapper.Map<ProductVariant>(request);

        await _productVariantRepository.AddAsync(productVariant, cancellationToken);

        var response = _mapper.Map<CreateProductVariantResponseDto>(productVariant);
        response.Message = "Ürün varyantı başarıyla oluşturuldu.";
        return response;
    }
}