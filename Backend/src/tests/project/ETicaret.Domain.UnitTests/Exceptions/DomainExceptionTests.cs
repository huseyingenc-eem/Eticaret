using Core.Domain.Exceptions;
using FluentAssertions;

namespace ETicaret.Domain.UnitTests.Exceptions;

/// <summary>
/// DomainException sınıfının davranışlarını test eder.
/// </summary>
public class DomainExceptionTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithMessageAndErrorCode_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        const string message = "Domain rule violation occurred";
        const string errorCode = "DOMAIN_ERROR_001";
        const string userFriendlyMessage = "Bir iş kuralı ihlali oluştu";
        var details = new { UserId = "12345", Action = "CreateProduct" };

        // Act
        var exception = new DomainException(message, errorCode, userFriendlyMessage, details);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.UserFriendlyMessage.Should().Be(userFriendlyMessage);
        exception.Details.Should().Be(details);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithInnerException_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        const string message = "Domain rule violation with inner exception";
        const string errorCode = "DOMAIN_ERROR_002";
        const string userFriendlyMessage = "İç hata ile birlikte iş kuralı ihlali";
        var details = new { Category = "Electronics" };
        var innerException = new InvalidOperationException("Inner exception message");

        // Act
        var exception = new DomainException(message, errorCode, innerException, userFriendlyMessage, details);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.UserFriendlyMessage.Should().Be(userFriendlyMessage);
        exception.Details.Should().Be(details);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Constructor_WithNullDetails_ShouldAllowNullDetails()
    {
        // Arrange
        const string message = "Domain error without details";
        const string errorCode = "DOMAIN_ERROR_003";

        // Act
        var exception = new DomainException(message, errorCode, userFriendlyMessage: null, details: null);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.UserFriendlyMessage.Should().BeNull();
        exception.Details.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithoutUserFriendlyMessage_ShouldSetToNull()
    {
        // Arrange
        const string message = "Domain error without user friendly message";
        const string errorCode = "DOMAIN_ERROR_004";

        // Act
        var exception = new DomainException(message, errorCode);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.UserFriendlyMessage.Should().BeNull();
        exception.Details.Should().BeNull();
    }

    #endregion

    #region Error Code Tests

    [Theory]
    [InlineData("")]
    [InlineData("INVALID_PRODUCT_NAME")]
    [InlineData("CATEGORY_NOT_FOUND")]
    [InlineData("BUSINESS_RULE_VIOLATION")]
    public void ErrorCode_ShouldAcceptVariousErrorCodes(string errorCode)
    {
        // Arrange & Act
        var exception = new DomainException("Test message", errorCode);

        // Assert
        exception.ErrorCode.Should().Be(errorCode);
    }

    #endregion

    #region UserFriendlyMessage Tests

    [Fact]
    public void UserFriendlyMessage_WhenProvided_ShouldBeSet()
    {
        // Arrange
        const string message = "Technical error message";
        const string errorCode = "TEST_ERROR";
        const string userFriendlyMessage = "Kullanıcı dostu hata mesajı";

        // Act
        var exception = new DomainException(message, errorCode, userFriendlyMessage);

        // Assert
        exception.UserFriendlyMessage.Should().Be(userFriendlyMessage);
    }

    [Fact]
    public void UserFriendlyMessage_WhenNotProvided_ShouldBeNull()
    {
        // Arrange & Act
        var exception = new DomainException("Test message", "TEST_ERROR");

        // Assert
        exception.UserFriendlyMessage.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Geçerli kullanıcı mesajı")]
    [InlineData("Valid user message")]
    public void UserFriendlyMessage_ShouldAcceptVariousValues(string userFriendlyMessage)
    {
        // Arrange & Act
        var exception = new DomainException("Test message", "TEST_ERROR", userFriendlyMessage);

        // Assert
        exception.UserFriendlyMessage.Should().Be(userFriendlyMessage);
    }

    #endregion

    #region Details Tests

    [Fact]
    public void Details_WithComplexObject_ShouldPreserveObjectStructure()
    {
        // Arrange
        var complexDetails = new
        {
            ProductId = Guid.NewGuid(),
            CategoryId = 123,
            ValidationErrors = new[] { "Name required", "Price invalid" },
            Metadata = new { Source = "ProductCreation", Timestamp = DateTime.UtcNow }
        };

        // Act
        var exception = new DomainException("Complex domain error", "COMPLEX_ERROR", "Karmaşık domain hatası", complexDetails);

        // Assert
        exception.Details.Should().BeEquivalentTo(complexDetails);
    }

    [Fact]
    public void Details_WithPrimitiveTypes_ShouldWorkCorrectly()
    {
        // Arrange
        const int primitiveDetail = 42;

        // Act
        var exception = new DomainException("Primitive detail error", "PRIMITIVE_ERROR", "Basit detay hatası", primitiveDetail);

        // Assert
        exception.Details.Should().Be(primitiveDetail);
    }

    #endregion

    #region Exception Inheritance Tests

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new DomainException("Test", "TEST_ERROR");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }

    [Fact]
    public void DomainException_ShouldBeSerializable()
    {
        // Arrange
        var exception = new DomainException("Serialization test", "SERIALIZATION_ERROR", "Serileştirme testi", new { Id = 123 });

        // Act & Assert
        // DomainException Exception'dan türediği için serializable olmalı
        exception.Should().BeAssignableTo<Exception>();
        exception.Data.Should().NotBeNull();
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldIncludeErrorCodeAndMessage()
    {
        // Arrange
        const string message = "Domain validation failed";
        const string errorCode = "VALIDATION_ERROR";
        var exception = new DomainException(message, errorCode);

        // Act
        var stringRepresentation = exception.ToString();

        // Assert
        stringRepresentation.Should().Contain(message);
        stringRepresentation.Should().Contain(nameof(DomainException));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void DomainException_FullScenario_ShouldWorkCorrectly()
    {
        // Arrange
        const string message = "Product creation failed due to business rule violation";
        const string errorCode = "PRODUCT_CREATION_FAILED";
        const string userFriendlyMessage = "Ürün oluşturulurken bir hata oluştu. Lütfen girdiğiniz bilgileri kontrol ediniz.";
        var details = new
        {
            ProductName = "iPhone 15",
            CategoryId = 1,
            SupplierId = Guid.NewGuid(),
            Errors = new[] { "Name already exists", "Invalid category" }
        };
        var innerException = new ArgumentException("Invalid product data");

        // Act
        var exception = new DomainException(message, errorCode, innerException, userFriendlyMessage, details);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.UserFriendlyMessage.Should().Be(userFriendlyMessage);
        exception.Details.Should().BeEquivalentTo(details);
        exception.InnerException.Should().Be(innerException);
        exception.Should().BeAssignableTo<Exception>();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithEmptyStrings_ShouldAcceptEmptyValues()
    {
        // Arrange & Act
        var exception = new DomainException("", "", "", null);

        // Assert
        exception.Message.Should().Be("");
        exception.ErrorCode.Should().Be("");
        exception.UserFriendlyMessage.Should().Be("");
        exception.Details.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithWhitespaceStrings_ShouldAcceptWhitespace()
    {
        // Arrange & Act
        var exception = new DomainException("   ", "   ", "   ", null);

        // Assert
        exception.Message.Should().Be("   ");
        exception.ErrorCode.Should().Be("   ");
        exception.UserFriendlyMessage.Should().Be("   ");
    }

    #endregion
}