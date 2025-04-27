using AutoMapper;
using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;
// Diğer using'ler...
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Profiles;

public class CategoriesMapper : Profile
{
    public CategoriesMapper()
    {
        // Command -> Entity Mappings
        CreateMap<CategoryAddCommand, Category>();
        CreateMap<CategoryUpdateCommand, Category>();

        // Entity -> Query Response DTO Mappings
        CreateMap<Category, GetCategoryTreeResponseDto>();

        // GetCategoryWithProductsResponseDto için map'leme.
        // Dikkat: Kaynak Category'de Products yüklenmemişse (Include edilmemişse),
        // hedef DTO'daki Products listesi boş olacaktır veya AutoMapper hata verebilir (versiyona bağlı).
        // Null check eklemek gerekebilir veya Ignore() kullanılabilir şimdilik.
        CreateMap<Category, GetCategoryWithProductsResponseDto>()
            .ForMember(dest => dest.Products, opt => opt.Ignore());


        // Entity -> Command Response DTO Mappings
        CreateMap<Category, CategoryAddResponseDto>();
        CreateMap<Category, CategoryUpdateResponseDto>();


        // Product -> CategoryProductDto Mapping
        // Bu map'leme tanımı ileride lazım olabilir diye kalabilir.
        CreateMap<Product, CategoryProductDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
    }
}