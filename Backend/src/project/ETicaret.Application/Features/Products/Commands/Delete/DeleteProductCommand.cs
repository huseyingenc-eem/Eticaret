using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Products.Constants;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Products.Commands.Delete;

[DefaultRoles("Admin")]
public class DeleteProductCommand : IRequest<DeleteProductResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    public Guid Id { get; set; }

    #region Cache Settings
    public string? CacheKey => $"product:{Id}";
    public bool BypassCache => false;
    public string? CacheGroupKey => ProductConstants.ProductsCacheGroup;
    #endregion
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponseDto>
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _productRepository = unitOfWork.GetRepository<Product, Guid>();
        _mapper = mapper;
    }

    public async Task<DeleteProductResponseDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.DeleteByIdAsync(request.Id, permanent: false, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return new DeleteProductResponseDto
        {
            Id = request.Id,
            Message = "Ürün başarıyla silindi."
        };
    }
}