using ETicaret.Domain.Entities;
using FluentAssertions;

namespace ETicaret.Domain.UnitTests.Entities;

/// <summary>
/// Address entity'sinin davranışlarını test eder.
/// </summary>
public class AddressTests
{
    #region Constructor Tests

    [Fact]
    public void Address_Constructor_ShouldSetCreatedTimeAutomatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddMilliseconds(-100);

        // Act
        var address = new Address();
        var afterCreation = DateTime.UtcNow.AddMilliseconds(100);

        // Assert
        address.CreatedTime.Should().BeAfter(beforeCreation);
        address.CreatedTime.Should().BeBefore(afterCreation);
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Address_Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var address = new Address();
        const string userId = "user123";
        const string addressTitle = "Ev";
        const string country = "Türkiye";
        const string city = "İstanbul";
        const string district = "Kadıköy";
        const string addressLine = "Test Caddesi No:123";
        const string zipCode = "34710";
        const string phoneNumber = "+905551234567";

        // Act
        address.UserId = userId;
        address.AddressTitle = addressTitle;
        address.Country = country;
        address.City = city;
        address.District = district;
        address.AddressLine = addressLine;
        address.ZipCode = zipCode;
        address.PhoneNumber = phoneNumber;
        address.IsDefaultShipping = true;
        address.IsDefaultBilling = false;

        // Assert
        address.UserId.Should().Be(userId);
        address.AddressTitle.Should().Be(addressTitle);
        address.Country.Should().Be(country);
        address.City.Should().Be(city);
        address.District.Should().Be(district);
        address.AddressLine.Should().Be(addressLine);
        address.ZipCode.Should().Be(zipCode);
        address.PhoneNumber.Should().Be(phoneNumber);
        address.IsDefaultShipping.Should().BeTrue();
        address.IsDefaultBilling.Should().BeFalse();
    }

    [Fact]
    public void Address_OptionalProperties_ShouldAcceptNullValues()
    {
        // Arrange & Act
        var address = new Address
        {
            UserId = null,
            ZipCode = null,
            PhoneNumber = null
        };

        // Assert
        address.UserId.Should().BeNull();
        address.ZipCode.Should().BeNull();
        address.PhoneNumber.Should().BeNull();
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Valid Title")]
    public void AddressTitle_ShouldAcceptVariousValues(string title)
    {
        // Arrange & Act
        var address = new Address { AddressTitle = title };

        // Assert
        address.AddressTitle.Should().Be(title);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("34710")]
    [InlineData("TR-34710")]
    [InlineData(null)]
    public void ZipCode_ShouldAcceptVariousFormats(string? zipCode)
    {
        // Arrange & Act
        var address = new Address { ZipCode = zipCode };

        // Assert
        address.ZipCode.Should().Be(zipCode);
    }

    [Theory]
    [InlineData("+905551234567")]
    [InlineData("05551234567")]
    [InlineData("555-123-4567")]
    [InlineData(null)]
    public void PhoneNumber_ShouldAcceptVariousFormats(string? phoneNumber)
    {
        // Arrange & Act
        var address = new Address { PhoneNumber = phoneNumber };

        // Assert
        address.PhoneNumber.Should().Be(phoneNumber);
    }

    #endregion

    #region Default Address Tests

    [Fact]
    public void Address_CanBeBothDefaultShippingAndBilling()
    {
        // Arrange & Act
        var address = new Address
        {
            IsDefaultShipping = true,
            IsDefaultBilling = true
        };

        // Assert
        address.IsDefaultShipping.Should().BeTrue();
        address.IsDefaultBilling.Should().BeTrue();
    }

    [Fact]
    public void Address_CanBeNeitherDefaultShippingNorBilling()
    {
        // Arrange & Act
        var address = new Address
        {
            IsDefaultShipping = false,
            IsDefaultBilling = false
        };

        // Assert
        address.IsDefaultShipping.Should().BeFalse();
        address.IsDefaultBilling.Should().BeFalse();
    }

    [Fact]
    public void Address_DefaultValues_ShouldBeFalse()
    {
        // Arrange & Act
        var address = new Address();

        // Assert
        address.IsDefaultShipping.Should().BeFalse();
        address.IsDefaultBilling.Should().BeFalse();
    }

    #endregion

    #region Entity Tests

    [Fact]
    public void Address_Equality_ShouldBeBasedOnId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var address1 = new Address();
        var address2 = new Address();

        // Reflection ile Id'yi set etmek (test amaçlı)
        typeof(Core.Domain.Entities.Entity<Guid>)
            .GetProperty("Id")!
            .SetValue(address1, id);
        typeof(Core.Domain.Entities.Entity<Guid>)
            .GetProperty("Id")!
            .SetValue(address2, id);

        // Act & Assert
        address1.Should().Be(address2); // Entity base sınıfında Id'ye göre equality
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void Address_CompleteAddressScenario_ShouldWorkCorrectly()
    {
        // Arrange
        var creationTime = DateTime.UtcNow;

        // Act
        var address = new Address
        {
            AddressTitle = "İş Yeri",
            Country = "Türkiye",
            City = "Ankara",
            District = "Çankaya",
            AddressLine = "Atatürk Bulvarı No:123 Kat:5 Daire:15",
            ZipCode = "06420",
            PhoneNumber = "+905551234567",
            IsDefaultShipping = false,
            IsDefaultBilling = true
        };

        // Assert
        address.AddressTitle.Should().Be("İş Yeri");
        address.Country.Should().Be("Türkiye");
        address.City.Should().Be("Ankara");
        address.District.Should().Be("Çankaya");
        address.AddressLine.Should().Be("Atatürk Bulvarı No:123 Kat:5 Daire:15");
        address.ZipCode.Should().Be("06420");
        address.PhoneNumber.Should().Be("+905551234567");
        address.IsDefaultShipping.Should().BeFalse();
        address.IsDefaultBilling.Should().BeTrue();
        address.CreatedTime.Should().BeAfter(creationTime.AddMinutes(-1));
    }

    #endregion
}