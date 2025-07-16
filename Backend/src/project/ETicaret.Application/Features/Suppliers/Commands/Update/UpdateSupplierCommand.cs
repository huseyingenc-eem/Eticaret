using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.Shared.Exceptions;
using Core.Application.Pipelines.Transactional;
using Core.Application.Pipelines.Caching;
using ETicaret.Application.Features.Suppliers.Constants;

namespace ETicaret.Application.Features.Suppliers.Commands.Update;

public class UpdateSupplierCommand : IRequest<UpdateSupplierResponseDto>, ITransactionalRequest, ICacheRemoverRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }

    public string? CacheKey => $"supplier:{Id}";

    public bool ByPassCache => false;

    public string? CacheGroupKey => SupplierConstants.SuppliersCacheGroup;

    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateSupplierCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UpdateSupplierResponseDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            Supplier? supplierToUpdate = await _unitOfWork.SupplierRepository.GetAsync(
                filter: s => s.Id == request.Id,
                cancellationToken: cancellationToken);

            if (supplierToUpdate == null)
            {
                throw new NotFoundException($"Supplier with Id {request.Id} not found.");
            }

            _mapper.Map(request, supplierToUpdate);

            await _unitOfWork.SupplierRepository.UpdateAsync(supplierToUpdate, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            UpdateSupplierResponseDto response = _mapper.Map<UpdateSupplierResponseDto>(supplierToUpdate);
            response.Message = "Tedarikçi başarıyla güncellendi.";
            return response;
        }
    }
}
