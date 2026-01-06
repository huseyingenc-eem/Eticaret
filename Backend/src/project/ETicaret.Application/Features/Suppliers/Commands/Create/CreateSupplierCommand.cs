using AutoMapper;
using Core.Application.Abstractions.Repositories;
using Core.Application.Behaviors.Authorization;
using Core.Application.Behaviors.Caching;
using Core.Application.Behaviors.Transactional;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;

#region Create Supplier Command
[DefaultRoles("Admin")]
public class CreateSupplierCommand : IRequest<CreateSupplierResponseDto>, ITransactionalRequest ,ICacheRemoverRequest
{
    #region Properties
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

#region Create Supplier Command Handler
public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponseDto>
{
    #region Fields

    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    #endregion

    #region Constructor

    public CreateSupplierCommandHandler(ISupplierRepository supplierRepository,IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    #endregion

    #region Handler Implementation
    public async Task<CreateSupplierResponseDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        Supplier supplier = _mapper.Map<Supplier>(request);

        await _supplierRepository.AddAsync(supplier, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        CreateSupplierResponseDto response = _mapper.Map<CreateSupplierResponseDto>(supplier);
        response.Message = "Tedarikçi başarıyla oluşturuldu.";

        return response;
    }

    #endregion
}

#endregion