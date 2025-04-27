using AutoMapper;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;
using ETicaret.Application.Features.Suppliers.Queries.GetById;
using ETicaret.Application.Features.Suppliers.Queries.GetList;
// using ETicaret.Application.Features.Suppliers.Queries.GetSupplierWithProducts; // İleride gerekirse
using ETicaret.Domain.Entities;
// using Core.Persistence.Paging; // Sayfalama kullanılıyorsa GetListResponse map'lemek için

namespace ETicaret.Application.Features.Suppliers.Profiles;

public class SuppliersMapper : Profile
{
    public SuppliersMapper()
    {
        // Command -> Entity Mappings
        CreateMap<SupplierAddCommand, Supplier>();
        CreateMap<SupplierUpdateCommand, Supplier>();

        // Entity -> Query Response DTO Mappings
        CreateMap<Supplier, GetListSupplierResponseDto>();
        CreateMap<Supplier, GetByIdSupplierResponseDto>();
        // CreateMap<Supplier, GetSupplierWithProductsResponseDto>();

        // Entity -> Command Response DTO Mappings
        CreateMap<Supplier, SupplierAddResponseDto>();
        CreateMap<Supplier, SupplierUpdateResponseDto>();

        // Product -> SupplierProductDto Mapping (İleride GetSupplierWithProducts için)
        // CreateMap<Product, SupplierProductDto>();

        /* // Sayfalama kullanılıyorsa IPaginate -> GetListResponse mapping'i
        CreateMap<IPaginate<Supplier>, GetListResponse<GetListSupplierResponseDto>>().ReverseMap();
        */
    }
}