using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;
using Core.CrossCuttingConcerns.Exceptions;

namespace ETicaret.Application.Features.Suppliers.Queries.GetById;

public class GetByIdSupplierQuery : IRequest<GetByIdSupplierResponseDto>
{
    public int Id { get; set; }

    public class GetByIdSupplierQueryHandler : IRequestHandler<GetByIdSupplierQuery, GetByIdSupplierResponseDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetByIdSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<GetByIdSupplierResponseDto> Handle(GetByIdSupplierQuery request, CancellationToken cancellationToken)
        {
            Supplier? supplier = await _supplierRepository.GetAsync(
                                        filter: s => s.Id == request.Id,
                                        cancellationToken: cancellationToken);

            if (supplier == null)
            {
                throw new NotFoundException($"Supplier with Id {request.Id} not found.");
            }

            

            GetByIdSupplierResponseDto response = _mapper.Map<GetByIdSupplierResponseDto>(supplier);
            return response;
        }
    }
}