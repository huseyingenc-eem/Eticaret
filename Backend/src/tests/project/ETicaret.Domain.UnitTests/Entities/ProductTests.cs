using ETicaret.Domain.Entities;
using FluentAssertions;

namespace ETicaret.Domain.UnitTests.Entities;

/// <summary>
/// Product entity'sinin davranışlarını test eder.
/// </summary>
public class ProductTests
{
    #region Constructor Tests

    [Fact]
    public void Product_Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        product.Name.Should().Be(string.Empty);
        product.Description.Should().BeNull();
        product.CategoryId.Should().Be(0);
        product.SupplierId.Should().BeNull();
        product.IsActive.Should().BeTrue(); // Default true
        product.Variants.Should().NotBeNull().And.BeEmpty();
        product.Images.Should().NotBeNull().And.BeEmpty();
        product.Reviews.Should().NotBeNull().And.BeEmpty();
        product.DiscountProducts.Should().NotBeNull().And.BeEmpty();
    }

    #endregion

    #region Property Tests

    [Theory]
    [InlineData("iPhone 15 Pro")]
    [InlineData("Samsung Galaxy S24")]
    [InlineData("MacBook Pro M3")]
    [InlineData("")]
    public void Product_Name_ShouldAcceptVariousValues(string name)
    {
        // Arrange & Act
        var product = new Product { Name = name };

        // Assert
        product.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("En yeni iPhone modeli")]
    [InlineData("")]
    [InlineData(null)]
    public void Product_Description_ShouldAcceptVariousValues(string? description)
    {
        // Arrange & Act
        var product = new Product { Description = description };

        // Assert
        product.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(0)] // Geçersiz ama entity seviyesinde kontrol edilmez
    public void Product_CategoryId_ShouldAcceptVariousValues(int categoryId)
    {
        // Arrange & Act
        var product = new Product { CategoryId = categoryId };

        // Assert
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Product_SupplierId_ShouldAcceptNullValue()
    {
        // Arrange & Act
        var product = new Product { SupplierId = null };

        // Assert
        product.SupplierId.Should().BeNull();
    }

    [Fact]
    public void Product_SupplierId_ShouldAcceptValidGuid()
    {
        // Arrange
        var supplierId = Guid.NewGuid();

        // Act
        var product = new Product { SupplierId = supplierId };

        // Assert
        product.SupplierId.Should().Be(supplierId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Product_IsActive_ShouldBeSettable(bool isActive)
    {
        // Arrange & Act
        var product = new Product { IsActive = isActive };

        // Assert
        product.IsActive.Should().Be(isActive);
    }

    #endregion

    #region Navigation Property Tests

    [Fact]
    public void Product_Category_ShouldBeNavigationProperty()
    {
        // Arrange
        var category = new Category { Name = "Elektronik" };
        var product = new Product
        {
            Name = "iPhone",
            CategoryId = 1,
            Category = category
        };

        // Act & Assert
        product.Category.Should().Be(category);
        product.CategoryId.Should().Be(1);
    }

    [Fact]
    public void Product_Supplier_ShouldBeNavigationProperty()
    {
        // Arrange
        var supplier = new Supplier { CompanyName = "Apple Inc." };
        var supplierId = Guid.NewGuid();
        var product = new Product
        {
            Name = "iPhone",
            SupplierId = supplierId,
            Supplier = supplier
        };

        // Act & Assert
        product.Supplier.Should().Be(supplier);
        product.SupplierId.Should().Be(supplierId);
    }

    #endregion

    #region Collection Tests

    [Fact]
    public void Product_Variants_ShouldAllowAddingVariants()
    {
        // Arrange
        var product = new Product { Name = "iPhone 15" };
        var variant1 = new ProductVariant
        {
            Sku = "IPH15-128-BLK",
            Price = 999.99m,
            UnitsInStock = 10
        };
        var variant2 = new ProductVariant
        {
            Sku = "IPH15-256-WHT",
            Price = 1099.99m,
            UnitsInStock = 5
        };

        // Act
        product.Variants.Add(variant1);
        product.Variants.Add(variant2);

        // Assert
        product.Variants.Should().HaveCount(2);
        product.Variants.Should().Contain(variant1);
        product.Variants.Should().Contain(variant2);
    }

    [Fact]
    public void Product_Images_ShouldAllowAddingImages()
    {
        // Arrange
        var product = new Product { Name = "iPhone 15" };
        var image1 = new ProductImage
        {
            ImageUrl = "https://example.com/iphone1.jpg",
            IsMain = true
        };
        var image2 = new ProductImage
        {
            ImageUrl = "https://example.com/iphone2.jpg",
            IsMain = false
        };

        // Act
        product.Images.Add(image1);
        product.Images.Add(image2);

        // Assert
        product.Images.Should().HaveCount(2);
        product.Images.Should().Contain(image1);
        product.Images.Should().Contain(image2);
        product.Images.Where(i => i.IsMain).Should().HaveCount(1);
    }

    [Fact]
    public void Product_Reviews_ShouldAllowAddingReviews()
    {
        // Arrange
        var product = new Product { Name = "iPhone 15" };
        var review1 = new Review
        {
            Rating = 5,
            Title = "Mükemmel telefon",
            Comment = "Çok beğendim"
        };
        var review2 = new Review
        {
            Rating = 4,
            Title = "İyi ama pahalı",
            Comment = "Kaliteli ama fiyatı yüksek"
        };

        // Act
        product.Reviews.Add(review1);
        product.Reviews.Add(review2);

        // Assert
        product.Reviews.Should().HaveCount(2);
        product.Reviews.Should().Contain(review1);
        product.Reviews.Should().Contain(review2);
    }

    #endregion

    #region Entity Tests

    [Fact]
    public void Product_CreatedTime_ShouldBeSetAutomatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddMilliseconds(-100);

        // Act
        var product = new Product();
        var afterCreation = DateTime.UtcNow.AddMilliseconds(100);

        // Assert
        product.CreatedTime.Should().BeAfter(beforeCreation);
        product.CreatedTime.Should().BeBefore(afterCreation);
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void Product_CompleteProductScenario_ShouldWorkCorrectly()
    {
        // Arrange
        var category = new Category { Name = "Smartphones" };
        var supplier = new Supplier { CompanyName = "Apple Inc." };

        // Act
        var product = new Product
        {
            Name = "iPhone 15 Pro",
            Description = "Apple'ın en yeni flagship telefonu",
            CategoryId = 1,
            Category = category,
            Supplier = supplier,
            IsActive = true
        };

        // Variants ekle
        product.Variants.Add(new ProductVariant
        {
            Sku = "IPH15P-128-TITAN",
            Price = 1199.99m,
            UnitsInStock = 50,
            IsActive = true
        });

        // Images ekle
        product.Images.Add(new ProductImage
        {
            ImageUrl = "https://example.com/iphone15pro-main.jpg",
            IsMain = true
        });

        // Reviews ekle
        product.Reviews.Add(new Review
        {
            Rating = 5,
            Title = "Harika telefon",
            Comment = "Kamera kalitesi mükemmel"
        });

        // Assert
        product.Name.Should().Be("iPhone 15 Pro");
        product.Description.Should().Be("Apple'ın en yeni flagship telefonu");
        product.CategoryId.Should().Be(1);
        product.Category.Should().Be(category);
        product.Supplier.Should().Be(supplier);
        product.IsActive.Should().BeTrue();
        product.Variants.Should().HaveCount(1);
        product.Images.Should().HaveCount(1);
        product.Reviews.Should().HaveCount(1);
        product.CreatedTime.Should().BeBefore(DateTime.UtcNow);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Product_WithoutCategory_ShouldAllowZeroCategoryId()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "Orphan Product",
            CategoryId = 0 // Geçersiz ama entity seviyesinde izin veriliyor
        };

        // Assert
        product.CategoryId.Should().Be(0);
        product.Category.Should().BeNull();
    }

    [Fact]
    public void Product_WithoutSupplier_ShouldAllowNullSupplierId()
    {
        // Arrange & Act
        var product = new Product
        {
            Name = "No Supplier Product",
            SupplierId = null
        };

        // Assert
        product.SupplierId.Should().BeNull();
        product.Supplier.Should().BeNull();
    }

    #endregion
}