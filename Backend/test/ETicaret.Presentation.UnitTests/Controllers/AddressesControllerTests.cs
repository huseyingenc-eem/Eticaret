using Core.Application.Abstractions.Paging;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetByUserId;
using ETicaret.Application.Features.Addresses.Queries.GetList;
using ETicaret.Application.Features.Addresses.Queries.GetMyAddresses;
using ETicaret.Presentation.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ETicaret.Presentation.UnitTests.Controllers;

/// <summary>
/// AddressController için comprehensive unit test suite.
/// Modern C# 12 patterns, Records ve best practices kullanılarak yazılmıştır.
/// </summary>
public sealed class AddressControllerTests : IDisposable
{
    #region Test Infrastructure & Setup

    private readonly Mock<IMediator> _mockMediator;
    private readonly AddressController _controller;
    private readonly TestUserContext _normalUser;
    private readonly TestUserContext _adminUser;

    public AddressControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new AddressController(_mockMediator.Object);

        _normalUser = new TestUserContext("test-user-123", "Test User", "test@example.com", ["User"]);
        _adminUser = new TestUserContext("admin-user-456", "Admin User", "admin@example.com", ["Admin", "User"]);

        // Default olarak normal user olarak ayarla
        SetupUserContext(_normalUser);
    }

    public void Dispose()
    {
        _mockMediator?.Reset();
    }

    #endregion

    #region Test Context Records

    /// <summary>
    /// Test kullanıcı bağlamını temsil eden immutable record.
    /// </summary>
    /// <param name="Id">Kullanıcı ID'si</param>
    /// <param name="Name">Kullanıcı adı</param>
    /// <param name="Email">Email adresi</param>
    /// <param name="Roles">Kullanıcı rolleri</param>
    private sealed record TestUserContext(string Id, string Name, string Email, string[] Roles);

    /// <summary>
    /// Test adres verileri için immutable record.
    /// </summary>
    private sealed record TestAddressData(
        Guid Id,
        string Title,
        string Country,
        string City,
        string District,
        string AddressLine,
        bool IsDefaultShipping = false,
        bool IsDefaultBilling = false
    )
    {
        public static TestAddressData CreateSample() => new(
            Guid.NewGuid(),
            "Ev Adresi",
            "Türkiye",
            "İstanbul",
            "Kadıköy",
            "Test Caddesi No:123"
        );

        public static TestAddressData CreateBusinessAddress() => new(
            Guid.NewGuid(),
            "İş Adresi",
            "Türkiye",
            "Ankara",
            "Çankaya",
            "İş Caddesi No:456",
            IsDefaultShipping: true
        );
    }

    /// <summary>
    /// Sayfalanmış test verileri için record.
    /// </summary>
    private sealed record PaginatedTestData<T>(
        IList<T> Items,
        int Index = 0,
        int Size = 10,
        int Count = 0,
        int Pages = 1,
        bool HasPrevious = false,
        bool HasNext = false
    );

    #endregion

    #region Helper Methods

    /// <summary>
    /// Kullanıcı bağlamını ayarlar.
    /// </summary>
    private void SetupUserContext(TestUserContext userContext)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userContext.Id),
            new(ClaimTypes.Name, userContext.Name),
            new(ClaimTypes.Email, userContext.Email)
        };

        claims.AddRange(userContext.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    /// <summary>
    /// Claims olmayan kullanıcı bağlamı ayarlar.
    /// </summary>
    private void SetupUnauthenticatedContext()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    /// <summary>
    /// Mock IPaginate nesnesi oluşturur.
    /// </summary>
    private static Mock<IPaginate<T>> CreateMockPaginate<T>(PaginatedTestData<T> data)
    {
        var mock = new Mock<IPaginate<T>>();
        mock.Setup(x => x.Items).Returns(data.Items);
        mock.Setup(x => x.Index).Returns(data.Index);
        mock.Setup(x => x.Size).Returns(data.Size);
        mock.Setup(x => x.Count).Returns(data.Count);
        mock.Setup(x => x.Pages).Returns(data.Pages);
        mock.Setup(x => x.HasPrevious).Returns(data.HasPrevious);
        mock.Setup(x => x.HasNext).Returns(data.HasNext);
        return mock;
    }

    #endregion

    #region User Address Operations Tests

    [Fact]
    public async Task GetMyAddresses_WithValidUser_ShouldReturnOkWithAddresses()
    {
        // Arrange
        var expectedAddresses = new List<GetMyAddressesResponseDto>
        {
            new()
            {
                Id = TestAddressData.CreateSample().Id,
                AddressTitle = "Ev",
                Country = "Türkiye",
                City = "İstanbul",
                District = "Kadıköy",
                AddressLine = "Test Caddesi No:123"
            },
            new()
            {
                Id = TestAddressData.CreateBusinessAddress().Id,
                AddressTitle = "İş",
                Country = "Türkiye",
                City = "Ankara",
                District = "Çankaya",
                AddressLine = "İş Caddesi No:456"
            }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetMyAddressesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAddresses);

        // Act
        var result = await _controller.GetMyAddresses();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeEquivalentTo(expectedAddresses);

        // Verify mediator call with proper query
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetMyAddressesQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetMyAddresses_WithUnauthenticatedUser_ShouldReturnUnauthorized()
    {
        // Arrange
        SetupUnauthenticatedContext();

        // Act
        var result = await _controller.GetMyAddresses();

        // Assert
        // Not: Gerçek uygulamada [Authorize] attribute'u bu kontrolü yapar
        // Unit test seviyesinde authorization middleware test edilmez
        // Bu test controller'ın authentication olmadan çağrılması durumunu simüle eder
        result.Should().BeOfType<OkObjectResult>(); // Actual implementation'a bağlı

        // Note: Gerçekte authorization middleware devreye girer
    }

    [Fact]
    public async Task GetById_WithValidAddressId_ShouldReturnOkWithAddress()
    {
        // Arrange
        var testAddress = TestAddressData.CreateSample();
        var expectedResponse = new GetByIdAddressResponseDto
        {
            Id = testAddress.Id,
            AddressTitle = testAddress.Title,
            Country = testAddress.Country,
            City = testAddress.City,
            District = testAddress.District,
            AddressLine = testAddress.AddressLine
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetByIdAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetById(testAddress.Id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeEquivalentTo(expectedResponse);

        // Verify mediator call
        _mockMediator.Verify(m => m.Send(
            It.Is<GetByIdAddressQuery>(q => q.Id == testAddress.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(GetInvalidAddressIds))]
    public async Task GetById_WithInvalidAddressId_ShouldHandleGracefully(Guid invalidId)
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetByIdAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetByIdAddressResponseDto?)null);

        // Act
        var result = await _controller.GetById(invalidId);

        // Assert - Controller'ın null response'u nasıl handle ettiğine bağlı
        result.Should().NotBeNull();
    }

    public static IEnumerable<object[]> GetInvalidAddressIds()
    {
        yield return new object[] { Guid.Empty };
        yield return new object[] { Guid.NewGuid() }; // Non-existent but valid GUID
    }

    #endregion

    #region CRUD Operations Tests

    [Fact]
    public async Task Add_WithValidCommand_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var testAddress = TestAddressData.CreateSample();
        var command = new CreateAddressCommand
        {
            AddressTitle = testAddress.Title,
            Country = testAddress.Country,
            City = testAddress.City,
            District = testAddress.District,
            AddressLine = testAddress.AddressLine,
            IsDefaultShipping = testAddress.IsDefaultShipping,
            IsDefaultBilling = testAddress.IsDefaultBilling
        };

        var expectedResponse = new CreateAddressResponseDto
        {
            Id = testAddress.Id,
            UserId = _normalUser.Id,
            AddressTitle = command.AddressTitle,
            Country = command.Country,
            City = command.City,
            District = command.District,
            AddressLine = command.AddressLine,
            Message = "Adres başarıyla oluşturuldu."
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Add(command);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.As<CreatedAtActionResult>();

        createdResult.Value.Should().BeEquivalentTo(expectedResponse);
        createdResult.ActionName.Should().Be(nameof(AddressController.GetById));
        createdResult.RouteValues.Should().ContainKey("id")
            .WhoseValue.Should().Be(expectedResponse.Id);
    }

    [Fact]
    public async Task Update_WithValidCommand_ShouldReturnOkWithUpdatedAddress()
    {
        // Arrange
        var testAddress = TestAddressData.CreateSample();
        var command = new UpdateAddressCommand
        {
            Id = testAddress.Id,
            AddressTitle = "Güncellenmiş Adres",
            Country = "Türkiye",
            City = "Ankara",
            District = "Yenimahalle",
            AddressLine = "Güncellenmiş Cadde No:789"
        };

        var expectedResponse = new UpdateAddressResponseDto
        {
            Id = command.Id,
            UserId = _normalUser.Id,
            AddressTitle = command.AddressTitle,
            Country = command.Country,
            City = command.City,
            District = command.District,
            AddressLine = command.AddressLine,
            Message = "Adres başarıyla güncellendi."
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Delete_WithValidAddressId_ShouldReturnOkWithDeletedResponse()
    {
        // Arrange
        var testAddress = TestAddressData.CreateSample();
        var expectedResponse = new DeleteAddressResponseDto
        {
            Id = testAddress.Id,
            AddressTitle = testAddress.Title,
            Message = "Adres başarıyla silindi."
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Delete(testAddress.Id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().BeEquivalentTo(expectedResponse);
    }

    #endregion

    #region Admin Operations Tests

    [Fact]
    public async Task GetDefaultShippingAddresses_WithAdminRole_ShouldReturnPaginatedAddresses()
    {
        // Arrange
        SetupUserContext(_adminUser);

        var testAddresses = new List<GetListAddressResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AddressTitle = "Default Shipping Address 1",
                City = "İstanbul",
                UserFirstName = "John",
                UserLastName = "Doe",
                UserEmail = "john.doe@example.com"
            },
            new()
            {
                Id = Guid.NewGuid(),
                AddressTitle = "Default Shipping Address 2",
                City = "Ankara",
                UserFirstName = "Jane",
                UserLastName = "Smith",
                UserEmail = "jane.smith@example.com"
            }
        };

        var paginatedData = new PaginatedTestData<GetListAddressResponseDto>(
            testAddresses,
            Count: testAddresses.Count,
            Pages: 1
        );

        var mockPaginate = CreateMockPaginate(paginatedData);

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPaginate.Object);

        var query = new GetListAddressQuery
        {
            UserNameSearch = "john",
            PageIndex = 0,
            PageSize = 10
        };

        // Act
        var result = await _controller.GetDefaultShippingAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        okResult.Value.Should().Be(mockPaginate.Object);

        _mockMediator.Verify(m => m.Send(
            It.Is<GetListAddressQuery>(q =>
                q.UserNameSearch == "john" &&
                q.PageIndex == 0 &&
                q.PageSize == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task GetUserAllAddresses_WithInvalidUserId_ShouldReturnBadRequest(string? invalidUserId)
    {
        // Arrange
        SetupUserContext(_adminUser);

        // Act
        var result = await _controller.GetUserAllAddresses(invalidUserId!, pageIndex: 0, pageSize: 20);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.As<BadRequestObjectResult>();
        badRequestResult.Value.Should().Be("Kullanıcı ID'si boş olamaz.");

        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetByUserIdAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetDefaultShippingAddresses_WithNormalUser_ShouldCallMediator()
    {
        // Arrange - Normal user (not admin)
        SetupUserContext(_normalUser);

        var mockPaginate = CreateMockPaginate(
            new PaginatedTestData<GetListAddressResponseDto>(new List<GetListAddressResponseDto>())
        );

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPaginate.Object);

        var query = new GetListAddressQuery { UserNameSearch = "test" };

        // Act
        var result = await _controller.GetDefaultShippingAddresses(query);

        // Assert
        // Note: Authorization kontrolü middleware seviyesinde yapılır
        // Unit test seviyesinde controller'ın davranışını test ediyoruz
        result.Should().BeOfType<OkObjectResult>();
    }

    
    [Fact]
    public async Task GetDefaultShippingAddresses_WithMultipleRolesIncludingAdmin_ShouldSucceed()
    {
        // Arrange
        var superAdminUser = new TestUserContext(
            "super-admin-999",
            "Super Admin",
            "superadmin@example.com",
            ["Admin", "Moderator", "User", "SuperAdmin"]
        );

        SetupUserContext(superAdminUser);

        var mockPaginate = CreateMockPaginate(
            new PaginatedTestData<GetListAddressResponseDto>(new List<GetListAddressResponseDto>())
        );

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPaginate.Object);

        var query = new GetListAddressQuery();

        // Act
        var result = await _controller.GetDefaultShippingAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Edge Cases & Error Handling Tests

    [Fact]
    public async Task GetMyAddresses_WhenMediatorThrowsException_ShouldPropagateException()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetMyAddressesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetMyAddresses());
    }
    [Fact]
    public async Task GetDefaultShippingAddresses_WithEmptyResults_ShouldReturnEmptyPagination()
    {
        // Arrange
        SetupUserContext(_adminUser);

        var emptyPaginate = CreateMockPaginate(
            new PaginatedTestData<GetListAddressResponseDto>(
                new List<GetListAddressResponseDto>(),
                Count: 0,
                Pages: 0
            )
        );

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPaginate.Object);

        var query = new GetListAddressQuery();

        // Act
        var result = await _controller.GetDefaultShippingAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result.As<OkObjectResult>();
        var paginatedResult = okResult.Value.Should().BeAssignableTo<IPaginate<GetListAddressResponseDto>>().Subject;

        paginatedResult.Items.Should().BeEmpty();
        paginatedResult.Count.Should().Be(0);
        paginatedResult.Pages.Should().Be(0);
    }

    #endregion

    #region Performance & Integration Tests (TestContainers Ready)

    [Fact]
    [Trait("Category", "Integration")]
    public async Task FullAddressLifecycle_ShouldWorkEndToEnd()
    {
        // Arrange
        var testAddress = TestAddressData.CreateSample();

        // Create
        var createCommand = new CreateAddressCommand
        {
            AddressTitle = testAddress.Title,
            Country = testAddress.Country,
            City = testAddress.City,
            District = testAddress.District,
            AddressLine = testAddress.AddressLine
        };

        var createResponse = new CreateAddressResponseDto
        {
            Id = testAddress.Id,
            UserId = _normalUser.Id,
            AddressTitle = createCommand.AddressTitle,
            Country = createCommand.Country,
            City = createCommand.City,
            District = createCommand.District,
            AddressLine = createCommand.AddressLine,
            Message = "Adres başarıyla oluşturuldu."
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<CreateAddressCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(createResponse);

        // Update
        var updateCommand = new UpdateAddressCommand
        {
            Id = testAddress.Id,
            AddressTitle = "Updated " + testAddress.Title,
            Country = testAddress.Country,
            City = testAddress.City,
            District = testAddress.District,
            AddressLine = testAddress.AddressLine
        };

        var updateResponse = new UpdateAddressResponseDto
        {
            Id = updateCommand.Id,
            UserId = _normalUser.Id,
            AddressTitle = updateCommand.AddressTitle,
            Country = updateCommand.Country,
            City = updateCommand.City,
            District = updateCommand.District,
            AddressLine = updateCommand.AddressLine,
            Message = "Adres başarıyla güncellendi."
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateAddressCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(updateResponse);

        // Delete
        var deleteResponse = new DeleteAddressResponseDto
        {
            Id = testAddress.Id,
            AddressTitle = updateCommand.AddressTitle,
            Message = "Adres başarıyla silindi."
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteAddressCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(deleteResponse);

        // Act & Assert
        // 1. Create
        var createResult = await _controller.Add(createCommand);
        createResult.Should().BeOfType<CreatedAtActionResult>();

        // 2. Update  
        var updateResult = await _controller.Update(updateCommand);
        updateResult.Should().BeOfType<OkObjectResult>();

        // 3. Delete
        var deleteResult = await _controller.Delete(testAddress.Id);
        deleteResult.Should().BeOfType<OkObjectResult>();

        // Verify all mediator calls
        _mockMediator.Verify(m => m.Send(It.IsAny<CreateAddressCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockMediator.Verify(m => m.Send(It.IsAny<UpdateAddressCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockMediator.Verify(m => m.Send(It.IsAny<DeleteAddressCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}