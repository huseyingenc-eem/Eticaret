using AutoMapper;
using ETicaret.Application.Features.Products.Commands.Create;
using ETicaret.Application.Features.Products.Commands.Delete;
using ETicaret.Application.Features.Products.Commands.Update;
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
        //Crud İşlemleri
        CreateMap<ProductAddCommand, Product>();
        CreateMap<ProductUpdateCommand, Product>();
        CreateMap<ProductDeleteCommand, Product>();


        //Query İşlemleri
        CreateMap<Product,GetListProductResponseDto>();
        CreateMap<Product, GetDetailsProductResponseDto>();
        CreateMap<Product, GetByIdProductResponseDto>();
        CreateMap<Product, GetAllByCategoryIdProductResponseDto>();
        CreateMap<Product, GetListProductPriceRangeResponseDto>();
        CreateMap<Product, GetListProductNameContainsResponseDto>();

    }
}
