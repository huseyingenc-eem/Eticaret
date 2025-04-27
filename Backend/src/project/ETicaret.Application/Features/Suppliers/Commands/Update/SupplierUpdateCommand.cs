using AutoMapper;
using ETicaret.Application.Services.Repositories; // ISupplierRepository için
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;
using Core.Application.Pipelines.Authorization; // NotFoundException için

namespace ETicaret.Application.Features.Suppliers.Commands.Update;

public class SupplierUpdateCommand : IRequest<SupplierUpdateResponseDto> , IRoleExists
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }

    public string[] Roles => ["Admin"];

    
    public class SupplierUpdateCommandHandler : IRequestHandler<SupplierUpdateCommand, SupplierUpdateResponseDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public SupplierUpdateCommandHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<SupplierUpdateResponseDto> Handle(SupplierUpdateCommand request, CancellationToken cancellationToken)
        {
            Supplier? supplierToUpdate = await _supplierRepository.GetAsync(
                filter: s => s.Id == request.Id, 
                cancellationToken: cancellationToken);

            if (supplierToUpdate == null)
            {
                throw new NotFoundException($"Supplier with Id {request.Id} not found.");
            }

            _mapper.Map(request, supplierToUpdate);

            await _supplierRepository.UpdateAsync(supplierToUpdate, cancellationToken);

            // Cache temizleme eklenebilir (varsa)
            // await _redisService.RemoveDataAsync("suppliers");
            // await _redisService.RemoveDataAsync($"supplier:{request.Id}");

            SupplierUpdateResponseDto response = _mapper.Map<SupplierUpdateResponseDto>(supplierToUpdate);
            response.Message = "Tedarikçi başarıyla güncellendi.";
            return response;
        }
    }
}
