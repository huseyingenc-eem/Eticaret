using ETicaret.Domain.Entities;

namespace ETicaret.Domain.UnitTests.Helpers;

/// <summary>
/// Test verilerini oluşturmak için yardımcı sınıf.
/// </summary>
public static class TestDataHelper
{
    #region Address Test Data

    public static Address CreateValidAddress(string? userId = null)
    {
        return new Address
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            AddressTitle = "Test Adresi",
            Country = "Türkiye",
            City = "İstanbul",
            District = "Kadıköy",
            AddressLine = "Test Caddesi No:123",
            ZipCode = "34710",
            PhoneNumber = "+905551234567",
            IsDefaultShipping = false,
            IsDefaultBilling = false
        };
    }

    public static Address CreateDefaultShippingAddress(string userId)
    {
        var address = CreateValidAddress(userId);
        address.IsDefaultShipping = true;
        address.AddressTitle = "Varsayılan Kargo Adresi";
        return address;
    }

    public static Address CreateDefaultBillingAddress(string userId)
    {
        var address = CreateValidAddress(userId);
        address.IsDefaultBilling = true;
        address.AddressTitle = "Varsayılan Fatura Adresi";
        return address;
    }

    #endregion

    #region Category Test Data

    public static Category CreateValidCategory(string name = "Test Kategori", int? parentId = null)
    {
        return new Category
        {
            Name = name,
            Description = $"{name} açıklaması",
            IsActive = true,
            ParentId = parentId
        };
    }

    public static Category CreateRootCategory(string name = "Ana Kategori")
    {
        return CreateValidCategory(name, parentId: null);
    }

    public static Category CreateChildCategory(string name = "Alt Kategori", int parentId = 1)
    {
        return CreateValidCategory(name, parentId);
    }

    #endregion

    #region Product Test Data

    public static Product CreateValidProduct(string name = "Test Ürün", int categoryId = 1)
    {
        return new Product
        {
            Name = name,
            Description = $"{name} açıklaması",
            CategoryId = categoryId,
            IsActive = true
        };
    }

    public static Product CreateProductWithSupplier(string name = "Test Ürün", int categoryId = 1, Guid? supplierId = null)
    {
        var product = CreateValidProduct(name, categoryId);
        product.SupplierId = supplierId ?? Guid.NewGuid();
        return product;
    }

    #endregion

    #region Complex Test Scenarios

    public static (Category parent, Category child1, Category child2) CreateCategoryHierarchy()
    {
        var parent = CreateRootCategory("Elektronik");
        var child1 = CreateChildCategory("Bilgisayar", 1);
        var child2 = CreateChildCategory("Telefon", 1);

        parent.Children.Add(child1);
        parent.Children.Add(child2);

        return (parent, child1, child2);
    }

    public static (Product product, ProductVariant variant1, ProductVariant variant2) CreateProductWithVariants()
    {
        var product = CreateValidProduct("iPhone 15");

        var variant1 = new ProductVariant
        {
            Sku = "IPH15-128-BLK",
            Price = 999.99m,
            UnitsInStock = 10,
            IsActive = true
        };

        var variant2 = new ProductVariant
        {
            Sku = "IPH15-256-WHT",
            Price = 1099.99m,
            UnitsInStock = 5,
            IsActive = true
        };

        product.Variants.Add(variant1);
        product.Variants.Add(variant2);

        return (product, variant1, variant2);
    }

    #endregion
}