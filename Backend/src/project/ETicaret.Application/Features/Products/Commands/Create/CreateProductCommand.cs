using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Create;

[DefaultRoles("Admin")]
public class CreateProductCommand : IRequest<CreateProductResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Guid? SupplierId { get; set; }
    public bool IsActive { get; set; } = true;

    #region Cache Settings
    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;
    #endregion
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponseDto>
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = unitOfWork.GetRepository<Product, Guid>();
        _mapper = mapper;
    }

    public async Task<CreateProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        Product product = _mapper.Map<Product>(request);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        CreateProductResponseDto response = _mapper.Map<CreateProductResponseDto>(product);
        response.Message = "Ürün başarıyla oluşturuldu.";

        return response;
    }
}