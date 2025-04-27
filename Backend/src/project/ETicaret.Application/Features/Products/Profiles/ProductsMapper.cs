using AutoMapper;
using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Update;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Queries.GetAllByCategoryId;
using ETicaret.Application.Features.Products.Queries.GetById;
using ETicaret.Application.Features.Products.Queries.GetDetails;
using ETicaret.Application.Features.Products.Queries.GetList;
using ETicaret.Application.Features.Products.Queries.GetListNameContains;
using ETicaret.Application.Features.Products.Queries.GetListPriceRange;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Products.Profiles;

public class ProductsMapper : Profile
{
    public ProductsMapper()
    {
        // Command -> Entity Mappings
        CreateMap<ProductAddCommand, Product>(); // Yeni alanlar otomatik maplenebilir veya .ForMember ile konfigüre edilebilir
        CreateMap<ProductUpdateCommand, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // Null değerleri maplemeyi atla (opsiyonel)

        // Entity -> DTO Mappings
        CreateMap<Product, GetListProductResponseDto>(); 

        CreateMap<Product, GetByIdProductResponseDto>(); 

        CreateMap<Product, GetDetailsProductResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)) // Category auto-include edildiyse çalışır
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name)); // Supplier auto-include edildiyse çalışır

        CreateMap<Product, GetAllByCategoryIdProductResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)); // Mevcut

        CreateMap<Product, GetListProductPriceRangeResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)); // Mevcut

        CreateMap<Product, GetListProductNameContainsResponseDto>()
             .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name)); // Mevcut

        // Entity -> Command Response DTO Mappings (Eklendi)
        CreateMap<Product, ProductAddResponseDto>();
        CreateMap<Product, ProductUpdateResponseDto>();
        CreateMap<Product, ProductDeleteResponseDto>();
    }
}