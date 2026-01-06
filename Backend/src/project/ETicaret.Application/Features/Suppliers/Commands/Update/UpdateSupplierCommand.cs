using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Features.Suppliers.Specifications;
using ETicaret.Application.Services.Repositories;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Update;

#region Update Supplier Command

[DefaultRoles("Admin")]
public class UpdateSupplierCommand : IRequest<UpdateSupplierResponseDto>,
    ICacheRemoverRequest,
    ITransactionalRequest
{
    #region Properties
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion

    #region Cache Settings
    public string? CacheKey { get; }
    public bool BypassCache => false;
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;
    #endregion
}

#endregion

#region Update Supplier Command Handler

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponseDto>
{
    #region Fields

    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor
    public UpdateSupplierCommandHandler(ISupplierRepository supplierRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation

    public async Task<UpdateSupplierResponseDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var spec = new SupplierSpecifications.ById(request.Id);
        var supplier = (await _supplierRepository.GetAsync(spec, cancellationToken))!;

        _mapper.Map(request, supplier);

        await _supplierRepository.UpdateAsync(supplier, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        UpdateSupplierResponseDto response = _mapper.Map<UpdateSupplierResponseDto>(supplier);
        response.Message = "Tedarikçi başarıyla güncellendi.";

        return response;
    }

    #endregion
}

#endregion