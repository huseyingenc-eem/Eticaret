using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using System;

namespace ETicaret.Application.Features.Products.Specifications;

/// <summary>
/// Belirtilen ID'ye sahip tek bir ürünü getirmek için kullanılan spesifikasyon.
/// </summary>
public class ProductByIdSpecification : Specification<Product>
{
    public ProductByIdSpecification(Guid id)
        : base(product => product.Id == id)
    {
    }
}