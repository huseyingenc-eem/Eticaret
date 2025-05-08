using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;

namespace ETicaret.Application.Features.Suppliers.Commands.Create;

public class SupplierAddCommand : IRequest<SupplierAddResponseDto>
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public class SupplierAddCommandHandler : IRequestHandler<SupplierAddCommand, SupplierAddResponseDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierAddCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<SupplierAddResponseDto> Handle(SupplierAddCommand request, CancellationToken cancellationToken)
        {
            Supplier supplier = _mapper.Map<Supplier>(request);

            Supplier addedSupplier = await _supplierRepository.AddAsync(supplier, cancellationToken);


            SupplierAddResponseDto response = _mapper.Map<SupplierAddResponseDto>(addedSupplier);
            response.Message = "Tedarikçi başarıyla eklendi.";
            return response;
        }
    }
}
