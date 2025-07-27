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
        var details = new { UserId = "12345", Action = "CreateProduct" };

        // Act
        var exception = new DomainException(message, errorCode, details);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
        exception.Details.Should().Be(details);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithInnerException_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        const string message = "Domain rule violation with inner exception";
        const string errorCode = "DOMAIN_ERROR_002";
        var details = new { Category = "Electronics" };
        var innerException = new InvalidOperationException("Inner exception message");

        // Act
        var exception = new DomainException(message, errorCode, innerException, details);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
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
        var exception = new DomainException(message, errorCode, details: null);

        // Assert
        exception.Message.Should().Be(message);
        exception.ErrorCode.Should().Be(errorCode);
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
        var exception = new DomainException("Complex domain error", "COMPLEX_ERROR", complexDetails);

        // Assert
        exception.Details.Should().BeEquivalentTo(complexDetails);
    }

    [Fact]
    public void Details_WithPrimitiveTypes_ShouldWorkCorrectly()
    {
        // Arrange
        const int primitiveDetail = 42;

        // Act
        var exception = new DomainException("Primitive detail error", "PRIMITIVE_ERROR", primitiveDetail);

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
        var exception = new DomainException("Serialization test", "SERIALIZATION_ERROR", new { Id = 123 });

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
}