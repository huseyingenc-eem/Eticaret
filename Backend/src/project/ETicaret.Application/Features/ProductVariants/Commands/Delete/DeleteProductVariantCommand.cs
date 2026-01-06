using AutoMapper;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.ProductVariants.Constants;
using ETicaret.Application.Features.ProductVariants.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.ProductVariants.Commands.Delete;

[DefaultRoles("Admin")]
public class DeleteProductVariantCommand : IRequest<DeleteProductVariantResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    public Guid Id { get; set; }
    public bool IsHardDelete { get; set; } = false;

    #region Cache Settings
    public string? CacheKey => null;
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductVariantConstants.ProductVariantsCacheGroup;
    #endregion
}

public class DeleteProductVariantCommandHandler : IRequestHandler<DeleteProductVariantCommand, DeleteProductVariantResponseDto>
{
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IMapper _mapper;
    public DeleteProductVariantCommandHandler(IProductVariantRepository productVariantRepository, IMapper mapper)
    {
        _productVariantRepository = productVariantRepository ;
        _mapper = mapper;
    }

    public async Task<DeleteProductVariantResponseDto> Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
    {
        var spec = new ProductVariantSpecifications.ById(request.Id);
        var productVariant = (await _productVariantRepository.GetAsync(spec, cancellationToken))!;

        await _productVariantRepository.DeleteAsync(productVariant, permanent: request.IsHardDelete, cancellationToken);

        var response = _mapper.Map<DeleteProductVariantResponseDto>(productVariant);
        response.Message = request.IsHardDelete
            ? "Ürün varyantı kalıcı olarak silindi."
            : "Ürün varyantı geçici olarak devre dışı bırakıldı.";

        return response;
    }
}