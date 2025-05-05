using AutoMapper;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;
using MediatR;


namespace ETicaret.Application.Features.Suppliers.Queries.GetList;

// Sayfalama isteniyorsa IRequest<GetListResponse<GetListSupplierResponseDto>> ve PageRequest eklenir
public class GetListSupplierQuery : IRequest<List<GetListSupplierResponseDto>> // Şimdilik basit liste döndürelim
{
    // Sayfalama için: public PageRequest PageRequest { get; set; }

    // --- Handler ---
    public class GetListSupplierQueryHandler : IRequestHandler<GetListSupplierQuery, List<GetListSupplierResponseDto>> // Dönüş tipi GetListResponse<> olabilir
    {
        private readonly ISupplierRepository _supplierRepository; // Henüz oluşturulmadı
        private readonly IMapper _mapper;

        public GetListSupplierQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<List<GetListSupplierResponseDto>> Handle(GetListSupplierQuery request, CancellationToken cancellationToken)
        {
            // Sayfalama olmadan basit liste çekme:
            var suppliers = await _supplierRepository.GetListAsync(
                filter: s => s.IsActive,
                orderBy: q => q.OrderBy(s => s.Name),
                cancellationToken: cancellationToken
            );

            List<GetListSupplierResponseDto> response = _mapper.Map<List<GetListSupplierResponseDto>>(suppliers);
            return response;

            /* // Sayfalama ile liste çekme örneği:
            IPaginate<Supplier> suppliers = await _supplierRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                orderBy: q => q.OrderBy(s => s.Name),
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListSupplierResponseDto> response = _mapper.Map<GetListResponse<GetListSupplierResponseDto>>(suppliers);
            return response;
            */
        }
    }
}