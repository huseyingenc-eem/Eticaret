using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Transactional;
using ETicaret.Application.Features.Suppliers.Constants;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;

public class CreateSupplierCommand : IRequest<CreateSupplierResponseDto>, ITransactionalRequest ,ICacheRemoverRequest
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public string? CacheKey => null;
    public bool ByPassCache => false; 
    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CreateSupplierResponseDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            Supplier supplier = _mapper.Map<Supplier>(request);

            Supplier addedSupplier = await _unitOfWork.SupplierRepository.AddAsync(supplier, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var response = _mapper.Map<CreateSupplierResponseDto>(addedSupplier);
            response.Message = "Tedarikçi başarıyla eklendi.";
            return response;
        }
    }
}
