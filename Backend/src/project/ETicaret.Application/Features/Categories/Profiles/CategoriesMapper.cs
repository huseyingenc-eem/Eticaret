using AutoMapper;
using ETicaret.Application.Features.Categories.Commands.Create;
using ETicaret.Application.Features.Categories.Commands.Update;

using ETicaret.Application.Features.Categories.Queries.GetCategoryTree;
using ETicaret.Application.Features.Categories.Queries.GetCategoryWithProducts;
using ETicaret.Application.Features.Categories.Queries.GetChildCategories;
using ETicaret.Application.Features.Categories.Queries.GetParentCategories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Profiles;

public class CategoriesMapper : Profile
{
    public CategoriesMapper()
    {
        CreateMap<CategoryAddCommand, Category>();
        CreateMap<CategoryUpdateCommand, Category>();
        CreateMap<CategoryDeleteCommand, Category>();

        CreateMap<Category, GetCategoryTreeResponseDto>();
        CreateMap<Category, GetCategoryWithProductsResponseDto>();
        CreateMap<Category, GetChildCategoriesResponseDto>();
        CreateMap<Category, GetParentCategoriesResponseDto>();
    }
}
