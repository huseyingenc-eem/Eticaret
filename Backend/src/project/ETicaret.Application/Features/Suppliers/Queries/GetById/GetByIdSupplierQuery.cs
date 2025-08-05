using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Queries.GetById;

/// <summary>
/// Belirtilen ID'ye sahip tedarikçiyi getiren sorgu. Önbellek desteği ile yüksek performans sağlar.
/// </summary>
public class GetByIdSupplierQuery : IRequest<GetByIdSupplierResponseDto>, 
    ICachableRequest,
    IPublicRequest
{
    public Guid Id { get; set; }

    public bool BypassCache { get; set; }
    public string CacheKey => $"supplier:{Id}";
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    public TimeSpan? SlidingExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow => TimeSpan.FromHours(1);
}

public class GetByIdSupplierQueryHandler : IRequestHandler<GetByIdSupplierQuery, GetByIdSupplierResponseDto>
{
    private readonly IRepository<Supplier, Guid> _supplierRepository;
    private readonly IMapper _mapper;

    public GetByIdSupplierQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _supplierRepository = unitOfWork.GetRepository<Supplier, Guid>();
        _mapper = mapper;
    }

    public async Task<GetByIdSupplierResponseDto> Handle(GetByIdSupplierQuery request, CancellationToken cancellationToken)
    {
        var spec = new SupplierSpecifications.ById(request.Id);
        Supplier? supplier = await _supplierRepository.GetAsync(spec, cancellationToken) ?? 
            throw new NotFoundException(
                message: $"Supplier with ID {request.Id} was not found.",
                userFriendlyMessage: "Aranan tedarikçi bulunamadı.",
                errorCode: "SUPPLIER_NOT_FOUND");

        return _mapper.Map<GetByIdSupplierResponseDto>(supplier);
    }
}