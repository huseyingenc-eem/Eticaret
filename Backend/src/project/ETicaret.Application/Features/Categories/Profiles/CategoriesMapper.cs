using AutoMapper;
using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Update;
using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;
using ETicaret.Application.Features.Categories.Queries.GetChildCategories;
using ETicaret.Application.Features.Categories.Queries.GetParentCategories;


// Diğer using'ler...
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Profiles;

public class CategoriesMapper : Profile
{
    public CategoriesMapper()
    {
        // Command -> Entity Mappings
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<UpdateCategoryCommand, Category>();

        // Entity -> Query Response DTO Mappings
        CreateMap<Category, GetCategoryTreeResponseDto>();

        // GetCategoryWithProductsResponseDto için map'leme.
        // Dikkat: Kaynak Category'de Products yüklenmemişse (Include edilmemişse),
        // hedef DTO'daki Products listesi boş olacaktır veya AutoMapper hata verebilir (versiyona bağlı).
        // Null check eklemek gerekebilir veya Ignore() kullanılabilir şimdilik.
        CreateMap<Category, GetCategoryWithProductsResponseDto>()
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        CreateMap<Category, GetChildCategoriesResponseDto>();
        CreateMap<Category, GetParentCategoriesResponseDto>();

        // Entity -> Command Response DTO Mappings
        CreateMap<Category, CreateCategoryResponseDto>();
        CreateMap<Category, UpdateCategoryResponseDto>();

        CreateMap<Product, CategoryProductDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
    }
}