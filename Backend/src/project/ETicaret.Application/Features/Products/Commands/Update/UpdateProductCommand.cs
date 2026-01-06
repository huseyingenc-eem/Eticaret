using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Application.Features.Products.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Update;

[DefaultRoles("Admin")]
public class UpdateProductCommand : IRequest<UpdateProductResponseDto>,
    ITransactionalRequest,
    ICacheRemoverRequest
{
    #region Komut Parametreleri
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryID { get; set; }
    public Guid SupplierID { get; set; }
    public bool IsActive { get; set; }
    #endregion

    #region Önbellek Ayarları
    public string CacheKey => $"product:{Id}";
    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;
    public bool BypassCache { get; set; }
    #endregion
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<UpdateProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var spec = new ProductSpecifications.ById(request.Id);
        var product = (await _productRepository.GetAsync(spec, cancellationToken))!;

        _mapper.Map(request, product);

        await _productRepository.UpdateAsync(product, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        UpdateProductResponseDto response = _mapper.Map<UpdateProductResponseDto>(product);
        response.Message = "Ürün başarıyla güncellendi.";

        return response;
    }
}