using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Application.Services.Repositories;

using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Delete;

#region Delete Supplier Command

[DefaultRoles("Admin")]
public class DeleteSupplierCommand : IRequest<DeleteSupplierResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    #region Properties
    public Guid Id { get; set; }
    #endregion

    #region Cache Settings
    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    #endregion
}

#endregion

#region Delete Supplier Command Handler
public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, DeleteSupplierResponseDto>
{
    #region Fields

    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor
    public DeleteSupplierCommandHandler(ISupplierRepository supplierRepository,IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation
    public async Task<DeleteSupplierResponseDto> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var spec = new SupplierSpecifications.ById(request.Id);
        var supplier = (await _supplierRepository.GetAsync(spec, cancellationToken))!;

        await _supplierRepository.DeleteAsync(supplier, permanent: false, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        DeleteSupplierResponseDto response = _mapper.Map<DeleteSupplierResponseDto>(supplier);
        response.Message = "Tedarikçi başarıyla silindi.";

        return response;
    }

    #endregion
}

#endregion