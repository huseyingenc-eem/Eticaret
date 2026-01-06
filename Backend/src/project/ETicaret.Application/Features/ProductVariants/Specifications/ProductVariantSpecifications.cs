using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.ProductVariants.Specifications;

/// <summary>
/// ProductVariant entity'si için karmaşık specifications
/// Handler ve Query'ler tarafından kullanılan ileri seviye sorgu mantığı
/// </summary>
public static class ProductVariantSpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    /// <summary>
    /// ID'ye göre product variant getirme - Handler'larda kullanılır
    /// Rules zaten existence kontrolü yaptı, Handler sadece get yapar
    /// </summary>
    public class ById : ByIdSpecification<ProductVariant, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    /// <summary>
    /// Product variant'ı tüm detaylarıyla getiren specification
    /// Update ve Delete handler'larda kullanılır
    /// </summary>
    public class ByIdWithDetails : Specification<ProductVariant>
    {
        public ByIdWithDetails(Guid id) : base(pv => pv.Id == id)
        {
            AddInclude(pv => pv.Product);
            AddInclude(pv => pv.CartItems);
            AddInclude(pv => pv.OrderItems);
            AddInclude(pv => pv.WishlistItems);
        }
    }

    #endregion

    #region Query Specifications

    /// <summary>
    /// Belirli bir ürüne ait aktif varyantları getiren specification
    /// </summary>
    public class ActiveByProduct : Specification<ProductVariant>
    {
        public ActiveByProduct(Guid productId)
            : base(pv => pv.ProductId == productId && pv.IsActive)
        {
            AddInclude(pv => pv.Product);
            AddOrderBy(pv => pv.Price);
        }
    }

    /// <summary>
    /// Stok durumuna göre varyantları getiren specification
    /// </summary>
    public class ByStockStatus : Specification<ProductVariant>
    {
        public ByStockStatus(bool inStock = true, int threshold = 0)
            : base(pv => pv.IsActive &&
                        (inStock ? pv.UnitsInStock > threshold : pv.UnitsInStock <= threshold))
        {
            AddInclude(pv => pv.Product);
            AddOrderBy(pv => pv.UnitsInStock);
        }
    }

    /// <summary>
    /// Fiyat aralığına göre varyantları getiren specification
    /// </summary>
    public class ByPriceRange : Specification<ProductVariant>
    {
        public ByPriceRange(decimal minPrice, decimal maxPrice, bool onlyActive = true)
            : base(pv => pv.Price >= minPrice &&
                        pv.Price <= maxPrice &&
                        (!onlyActive || pv.IsActive))
        {
            AddInclude(pv => pv.Product);
            AddOrderBy(pv => pv.Price);
        }
    }

    /// <summary>
    /// Admin paneli için sayfalanmış variant listesi
    /// </summary>
    public class AdminPagedAndFiltered : Specification<ProductVariant>
    {
        public AdminPagedAndFiltered(int pageIndex, int pageSize,
            string? skuFilter = null,
            Guid? productIdFilter = null,
            bool? isActiveFilter = null,
            bool? onlyLowStock = null,
            int lowStockThreshold = 10)
            : base(pv =>
                (string.IsNullOrEmpty(skuFilter) || pv.Sku.Contains(skuFilter)) &&
                (!productIdFilter.HasValue || pv.ProductId == productIdFilter.Value) &&
                (isActiveFilter == null || pv.IsActive == isActiveFilter) &&
                (!onlyLowStock.HasValue || !onlyLowStock.Value || pv.UnitsInStock <= lowStockThreshold))
        {
            AddInclude(pv => pv.Product);
            AddOrderByDescending(pv => pv.CreatedTime);
            ApplyPaging(pageIndex * pageSize, pageSize);
        }
    }


    #endregion

    #region Analytics Specifications

    /// <summary>
    /// En çok satan varyantları getiren specification
    /// </summary>
    public class BestSelling : Specification<ProductVariant>
    {
        public BestSelling(DateTime fromDate, DateTime toDate, int topCount = 10)
            : base(pv => pv.IsActive &&
                        pv.OrderItems.Any(oi => oi.Order.OrderDate >= fromDate &&
                                              oi.Order.OrderDate <= toDate))
        {
            AddInclude(pv => pv.Product);
            AddInclude(pv => pv.OrderItems);
            AddOrderByDescending(pv => pv.OrderItems
                .Where(oi => oi.Order.OrderDate >= fromDate && oi.Order.OrderDate <= toDate)
                .Sum(oi => oi.Quantity));
            ApplyPaging(0, topCount);
        }
    }

    /// <summary>
    /// Düşük stoklu varyantları getiren specification
    /// </summary>
    public class LowStock : Specification<ProductVariant>
    {
        public LowStock(int threshold = 10)
            : base(pv => pv.IsActive && pv.UnitsInStock <= threshold && pv.UnitsInStock > 0)
        {
            AddInclude(pv => pv.Product);
            AddOrderBy(pv => pv.UnitsInStock);
        }
    }

    #endregion
}