using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Features.ProductVariants.Specifications;
using ETicaret.Application.Services.Repositories;

using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Commands.Update;

[DefaultRoles("Admin")]
public class UpdateProductVariantCommand : IRequest<UpdateProductVariantResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? VariantImageUrl { get; set; }
    public bool IsActive { get; set; }
    public string? AttributeDescription { get; set; }

    #region Cache Settings
    public string? CacheKey => null;
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;
    #endregion
}

public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand, UpdateProductVariantResponseDto>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;

    public UpdateProductVariantCommandHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductVariantResponseDto> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var spec = new ProductVariantSpecifications.ById(request.Id);
        var productVariant = (await _productVariantRepository.GetAsync(spec, cancellationToken))!;

        _mapper.Map(request, productVariant);

        await _productVariantRepository.UpdateAsync(productVariant, cancellationToken);

        var response = _mapper.Map<UpdateProductVariantResponseDto>(productVariant);
        response.Message = "Ürün varyantı başarıyla güncellendi.";
        return response;
    }
}