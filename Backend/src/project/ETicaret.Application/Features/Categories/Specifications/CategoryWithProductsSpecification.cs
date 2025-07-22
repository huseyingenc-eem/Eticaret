using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Specifications;

/// <summary>
/// Belirtilen ID'ye sahip tek bir kategoriyi, ilişkili ürünleriyle birlikte getiren spesifikasyon.
/// </summary>
public class CategoryWithProductsSpecification : Specification<Category>
{
    public CategoryWithProductsSpecification(int id)
        : base(category => category.Id == id)
    {
        // 1. İlişkili Ürünler koleksiyonunu sorguya dahil et (Eager Loading).
        AddInclude(c => c.Products);
    }
}