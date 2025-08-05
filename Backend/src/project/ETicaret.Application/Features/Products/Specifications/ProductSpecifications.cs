using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.Products.Specifications;

/// <summary>
/// Product entity'si için specification library. Query'ler ve business rule'lar için kullanılır.
/// </summary>
public static class ProductSpecifications
{
    #region Basic Specifications

    public class ById : ByIdSpecification<Product, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    public class ByIdWithDetails : Specification<Product>
    {
        public ByIdWithDetails(Guid id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Supplier);
            AddInclude(p => p.Variants);
            AddInclude(p => p.Images);
        }
    }

    public class Active : ActiveEntitiesSpecification<Product>
    {
        public Active()
        {
            AddOrderBy(p => p.Name);
        }
    }

    #endregion

    #region Business Rule Specifications

    public class ByName : Specification<Product>
    {
        public ByName(string name) : base(p => p.Name.ToLower() == name.ToLower()) { }
    }

    public class ByNameExcludingId : Specification<Product>
    {
        public ByNameExcludingId(Guid excludeId, string name)
            : base(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower()) { }
    }

    public class HasVariants : Specification<Product>
    {
        public HasVariants(Guid productId) : base(p => p.Id == productId && p.Variants.Any())
        {
            AddInclude(p => p.Variants);
        }
    }

    public class InUse : Specification<Product>
    {
        public InUse(Guid productId) : base(p => p.Id == productId &&
            (p.Variants.Any(v => v.OrderItems.Any() || v.CartItems.Any() || v.WishlistItems.Any())))
        {
            AddInclude(p => p.Variants.Where(v => v.OrderItems.Any() || v.CartItems.Any() || v.WishlistItems.Any()));
        }
    }

    #endregion

    #region Query Specifications

    public class PagedAndFiltered : Specification<Product>
    {
        public PagedAndFiltered(int pageIndex, int pageSize, string? nameSearch = null,
            int? categoryId = null, Guid? supplierId = null, bool onlyActive = true)
            : base(product =>
                (!onlyActive || product.IsActive) &&
                (string.IsNullOrEmpty(nameSearch) || product.Name.Contains(nameSearch)) &&
                (!categoryId.HasValue || product.CategoryId == categoryId.Value) &&
                (!supplierId.HasValue || product.SupplierId == supplierId.Value))
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Supplier);
            AddInclude(p => p.Variants);
            AddOrderByDescending(p => p.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }

    public class ByCategory : Specification<Product>
    {
        public ByCategory(int categoryId, bool onlyActive = true)
            : base(p => p.CategoryId == categoryId && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Name);
        }
    }

    public class BySupplier : Specification<Product>
    {
        public BySupplier(Guid supplierId, bool onlyActive = true)
            : base(p => p.SupplierId == supplierId && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Supplier);
            AddOrderBy(p => p.Name);
        }
    }

    public class RecentlyCreated : Specification<Product>
    {
        public RecentlyCreated(int days = 7, bool onlyActive = true)
            : base(p => p.CreatedTime >= DateTime.UtcNow.AddDays(-days) && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Category);
            AddInclude(p => p.Supplier);
            AddOrderByDescending(p => p.CreatedTime);
        }
    }

    public class WithLowStock : Specification<Product>
    {
        public WithLowStock(int threshold = 10, bool onlyActive = true)
            : base(p => p.Variants.Sum(v => v.UnitsInStock) <= threshold && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Variants);
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Variants.Sum(v => v.UnitsInStock));
        }
    }

    public class SearchByName : Specification<Product>
    {
        public SearchByName(string searchTerm, bool onlyActive = true)
            : base(p => p.Name.Contains(searchTerm) && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Name);
        }
    }

    #endregion

    #region Advanced Query Specifications

    public class WithPriceRange : Specification<Product>
    {
        public WithPriceRange(decimal minPrice, decimal maxPrice, bool onlyActive = true)
            : base(p => p.Variants.Any(v => v.Price >= minPrice && v.Price <= maxPrice) && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Variants);
            AddInclude(p => p.Category);
            AddOrderBy(p => p.Variants.Min(v => v.Price));
        }
    }

    public class PopularProducts : Specification<Product>
    {
        public PopularProducts(int topCount = 10, bool onlyActive = true)
            : base(p => (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Variants);
            AddInclude(p => p.Category);
            AddOrderByDescending(p => p.Variants.Sum(v => v.OrderItems.Count()));
            ApplyPaging(0, topCount);
        }
    }

    public class FeaturedProducts : Specification<Product>
    {
        public FeaturedProducts(bool onlyActive = true)
            : base(p => p.Reviews.Average(r => r.Rating) >= 4.0 && (!onlyActive || p.IsActive))
        {
            AddInclude(p => p.Reviews);
            AddInclude(p => p.Category);
            AddOrderByDescending(p => p.Reviews.Average(r => r.Rating));
        }
    }

    #endregion
}