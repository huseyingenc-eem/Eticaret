using ETicaret.Domain.Entities;
using FluentAssertions;

namespace ETicaret.Domain.UnitTests.Entities;

/// <summary>
/// Category entity'sinin davranışlarını test eder.
/// </summary>
public class CategoryTests
{
    #region Constructor Tests

    [Fact]
    public void Category_Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Name.Should().Be(string.Empty);
        category.Description.Should().BeNull();
        category.IsActive.Should().BeTrue(); // Default değer true
        category.ParentId.Should().BeNull();
        category.Parent.Should().BeNull();
        category.Children.Should().NotBeNull();
        category.Products.Should().NotBeNull();
        category.DiscountCategories.Should().NotBeNull();
    }

    #endregion

    #region Property Tests

    [Theory]
    [InlineData("Elektronik")]
    [InlineData("Giyim")]
    [InlineData("Ev & Yaşam")]
    [InlineData("Spor & Outdoor")]
    public void Category_Name_ShouldAcceptValidNames(string name)
    {
        // Arrange & Act
        var category = new Category { Name = name };

        // Assert
        category.Name.Should().Be(name);
    }

    [Theory]
    [InlineData("Elektronik ürünleri kategorisi")]
    [InlineData("")]
    [InlineData(null)]
    public void Category_Description_ShouldAcceptVariousValues(string? description)
    {
        // Arrange & Act
        var category = new Category { Description = description };

        // Assert
        category.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Category_IsActive_ShouldBeSettable(bool isActive)
    {
        // Arrange & Act
        var category = new Category { IsActive = isActive };

        // Assert
        category.IsActive.Should().Be(isActive);
    }

    #endregion

    #region Hierarchy Tests

    [Fact]
    public void Category_ParentId_ShouldAcceptNullForRootCategories()
    {
        // Arrange & Act
        var rootCategory = new Category
        {
            Name = "Ana Kategori",
            ParentId = null
        };

        // Assert
        rootCategory.ParentId.Should().BeNull();
        rootCategory.Parent.Should().BeNull();
    }

    [Fact]
    public void Category_ParentId_ShouldAcceptValidParentId()
    {
        // Arrange
        const int parentId = 1;

        // Act
        var childCategory = new Category
        {
            Name = "Alt Kategori",
            ParentId = parentId
        };

        // Assert
        childCategory.ParentId.Should().Be(parentId);
    }

    [Fact]
    public void Category_Children_ShouldBeInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Children.Should().NotBeNull();
        category.Children.Should().BeEmpty();
        category.Children.Should().BeAssignableTo<ICollection<Category>>();
    }

    [Fact]
    public void Category_Children_ShouldAllowAddingChildCategories()
    {
        // Arrange
        var parentCategory = new Category { Name = "Ana Kategori" };
        var childCategory1 = new Category { Name = "Alt Kategori 1", ParentId = 1 };
        var childCategory2 = new Category { Name = "Alt Kategori 2", ParentId = 1 };

        // Act
        parentCategory.Children.Add(childCategory1);
        parentCategory.Children.Add(childCategory2);

        // Assert
        parentCategory.Children.Should().HaveCount(2);
        parentCategory.Children.Should().Contain(childCategory1);
        parentCategory.Children.Should().Contain(childCategory2);
    }

    #endregion

    #region Product Association Tests

    [Fact]
    public void Category_Products_ShouldBeInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Products.Should().NotBeNull();
        category.Products.Should().BeEmpty();
        category.Products.Should().BeAssignableTo<ICollection<Product>>();
    }

    [Fact]
    public void Category_Products_ShouldAllowAddingProducts()
    {
        // Arrange
        var category = new Category { Name = "Elektronik" };
        var product1 = new Product { Name = "Laptop", CategoryId = 1 };
        var product2 = new Product { Name = "Telefon", CategoryId = 1 };

        // Act
        category.Products.Add(product1);
        category.Products.Add(product2);

        // Assert
        category.Products.Should().HaveCount(2);
        category.Products.Should().Contain(product1);
        category.Products.Should().Contain(product2);
    }

    #endregion

    #region Entity Tests

    [Fact]
    public void Category_ShouldInheritFromEntityWithInt()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Should().BeAssignableTo<Core.Domain.Entities.Entity<int>>();
    }

    [Fact]
    public void Category_CreatedTime_ShouldBeSetAutomatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddMilliseconds(-100);

        // Act
        var category = new Category();
        var afterCreation = DateTime.UtcNow.AddMilliseconds(100);

        // Assert
        category.CreatedTime.Should().BeAfter(beforeCreation);
        category.CreatedTime.Should().BeBefore(afterCreation);
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void Category_HierarchicalStructure_ShouldWorkCorrectly()
    {
        // Arrange
        var electronics = new Category
        {
            Name = "Elektronik",
            Description = "Elektronik ürünler",
            IsActive = true
        };

        var computers = new Category
        {
            Name = "Bilgisayar",
            Description = "Bilgisayar ve aksesuarları",
            ParentId = 1,
            IsActive = true
        };

        var laptops = new Category
        {
            Name = "Laptop",
            Description = "Taşınabilir bilgisayarlar",
            ParentId = 2,
            IsActive = true
        };

        // Act
        electronics.Children.Add(computers);
        computers.Children.Add(laptops);

        // Assert - Electronics (Root)
        electronics.ParentId.Should().BeNull();
        electronics.Children.Should().HaveCount(1);
        electronics.Children.Should().Contain(computers);

        // Assert - Computers (Middle)
        computers.ParentId.Should().Be(1);
        computers.Children.Should().HaveCount(1);
        computers.Children.Should().Contain(laptops);

        // Assert - Laptops (Leaf)
        laptops.ParentId.Should().Be(2);
        laptops.Children.Should().BeEmpty();
    }

    [Fact]
    public void Category_WithProducts_ShouldMaintainRelationship()
    {
        // Arrange
        var category = new Category
        {
            Name = "Laptop",
            Description = "Taşınabilir bilgisayarlar",
            IsActive = true
        };

        var product1 = new Product
        {
            Name = "MacBook Pro",
            Description = "Apple laptop",
            CategoryId = 1,
            IsActive = true
        };

        var product2 = new Product
        {
            Name = "Dell XPS",
            Description = "Dell laptop",
            CategoryId = 1,
            IsActive = true
        };

        // Act
        category.Products.Add(product1);
        category.Products.Add(product2);

        // Assert
        category.Products.Should().HaveCount(2);
        category.Products.Should().Contain(product1);
        category.Products.Should().Contain(product2);

        // Products should reference the category
        product1.CategoryId.Should().Be(1);
        product2.CategoryId.Should().Be(1);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Category_EmptyName_ShouldBeAllowed()
    {
        // Arrange & Act
        var category = new Category { Name = string.Empty };

        // Assert
        category.Name.Should().Be(string.Empty);
    }

    [Fact]
    public void Category_LongDescription_ShouldBeAccepted()
    {
        // Arrange
        var longDescription = new string('A', 1000); // 1000 karakterlik açıklama

        // Act
        var category = new Category { Description = longDescription };

        // Assert
        category.Description.Should().Be(longDescription);
        category.Description.Should().HaveLength(1000);
    }

    #endregion
}