using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using static Core.Application.Abstractions.Specifications.CommonSpecifications;

namespace ETicaret.Application.Features.ShoppingCarts.Specifications;

/// <summary>
/// Shopping Cart ve Cart Items için karmaşık specifications
/// Handler ve Query'ler tarafından kullanılan ileri seviye sorgu mantığı
/// </summary>
public static class ShoppingCartSpecifications
{
    #region Basic Specifications - Handler'lar için gerekli

    public class ById : ByIdSpecification<ShoppingCart, Guid>
    {
        public ById(Guid id) : base(id) { }
    }

    public class ByUserId : Specification<ShoppingCart>
    {
        public ByUserId(string userId) : base(cart => cart.UserId == userId) { }
    }

    #endregion

    #region Complex Query Specifications

    /// <summary>
    /// Kullanıcının sepetini tam detaylı olarak getiren specification
    /// Cart item'ları, ürün bilgileri ve stok durumlarıyla birlikte
    /// </summary>
    public class UserCartWithFullDetails : Specification<ShoppingCart>
    {
        public UserCartWithFullDetails(string userId)
            : base(cart => cart.UserId == userId)
        {
            AddInclude(sc => sc.Items);
            AddInclude("Items.ProductVariant");
            AddInclude("Items.ProductVariant.Product");
            AddInclude("Items.ProductVariant.Product.Category");
            AddInclude("Items.ProductVariant.Product.Images");
            AddOrderBy(sc => sc.CreatedTime);
        }
    }

    /// <summary>
    /// Sepet item'larını fiyat hesaplama için getiren specification
    /// Discount'lar ve güncel fiyatlarla birlikte
    /// </summary>
    public class CartItemsForPricingCalculation : Specification<CardItem>
    {
        public CartItemsForPricingCalculation(string userId)
            : base(item => item.ShoppingCart.UserId == userId)
        {
            AddInclude(ci => ci.ProductVariant);
            AddInclude("ProductVariant.Product");
            AddInclude("ProductVariant.Product.DiscountProducts");
            AddInclude("ProductVariant.Product.DiscountProducts.Discount");
            AddOrderBy(ci => ci.CreatedTime);
        }
    }

    /// <summary>
    /// Stok kontrolü için cart item'ları
    /// </summary>
    public class CartItemsWithStockValidation : Specification<CardItem>
    {
        public CartItemsWithStockValidation(string userId)
            : base(item => item.ShoppingCart.UserId == userId && item.ProductVariant.IsActive)
        {
            AddInclude(ci => ci.ProductVariant);
            AddInclude("ProductVariant.Product");
        }
    }

    /// <summary>
    /// Checkout için gerekli tüm bilgilerle cart
    /// </summary>
    public class CartForCheckout : Specification<ShoppingCart>
    {
        public CartForCheckout(string userId)
            : base(cart => cart.UserId == userId && cart.Items.Any())
        {
            AddInclude(sc => sc.Items.Where(i => i.ProductVariant.IsActive && i.ProductVariant.UnitsInStock >= i.Quantity));
            AddInclude("Items.ProductVariant");
            AddInclude("Items.ProductVariant.Product");
            AddInclude(sc => sc.User);
            AddInclude("User.Addresses");
        }
    }

    /// <summary>
    /// Abandoned cart analizi için
    /// </summary>
    public class AbandonedCartsDetailed : Specification<ShoppingCart>
    {
        public AbandonedCartsDetailed(int daysAgo = 7, decimal minTotal = 0)
            : base(cart => cart.Items.Any() &&
                          (cart.UpdateTime ?? cart.CreatedTime) <= DateTime.UtcNow.AddDays(-daysAgo) &&
                          cart.Items.Sum(i => i.Quantity * i.PriceAtAddition) >= minTotal)
        {
            AddInclude(sc => sc.User);
            AddInclude(sc => sc.Items);
            AddInclude("Items.ProductVariant");
            AddOrderByDescending(sc => sc.UpdateTime ?? sc.CreatedTime);
        }
    }

    #endregion

    #region Analytics & Reporting Specifications

    /// <summary>
    /// Cart analytics için gelişmiş rapor
    /// </summary>
    public class CartAnalyticsReport : Specification<ShoppingCart>
    {
        public CartAnalyticsReport(DateTime fromDate, DateTime toDate)
            : base(cart => cart.CreatedTime >= fromDate && cart.CreatedTime <= toDate)
        {
            AddInclude(sc => sc.Items);
            AddInclude("Items.ProductVariant");
            AddInclude("Items.ProductVariant.Product");
            AddInclude("Items.ProductVariant.Product.Category");
            AddOrderByDescending(sc => sc.CreatedTime);
        }
    }

    /// <summary>
    /// En popüler sepet ürünleri
    /// </summary>
    public class PopularCartProducts : Specification<CardItem>
    {
        public PopularCartProducts(DateTime fromDate, DateTime toDate, int topCount = 10)
            : base(item => item.CreatedTime >= fromDate && item.CreatedTime <= toDate)
        {
            AddInclude(ci => ci.ProductVariant);
            AddInclude("ProductVariant.Product");
            AddInclude("ProductVariant.Product.Category");
            ApplyPaging(0, topCount);
        }
    }

    #endregion
}