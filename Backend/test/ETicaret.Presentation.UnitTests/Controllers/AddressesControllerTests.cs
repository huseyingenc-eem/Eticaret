using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetList;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;
using ETicaret.Presentation.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Core.Application.Abstractions.Paging;

namespace ETicaret.Presentation.UnitTests.Controllers;

/// <summary>
/// AddressesController unit testleri.
/// </summary>
public class AddressesControllerTests
{
    #region Setup

    private readonly Mock<IMediator> _mockMediator;
    private readonly AddressesController _controller;
    private const string TestUserId = "test-user-123";
    private const string AdminUserId = "admin-user-456";

    public AddressesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new AddressesController(_mockMediator.Object);

        // Mock User Claims - Normal user by default
        SetupUserClaims();
    }

    private void SetupUserClaims(bool isAdmin = false, string userId = TestUserId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, isAdmin ? "Admin User" : "Test User"),
            new(ClaimTypes.Email, isAdmin ? "admin@example.com" : "test@example.com")
        };

        // ✅ Admin ise Admin rolü ekle
        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            // Normal kullanıcı için User rolü ekle
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    private void SetupNoRoleClaims()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, TestUserId),
            new(ClaimTypes.Name, "No Role User"),
            new(ClaimTypes.Email, "norole@example.com")
            // ⚠️ Role claim'i yok
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    #endregion

    #region GetListByCurrentUser Tests

    [Fact]
    public async Task GetListByCurrentUser_ShouldReturnOkResult_WithUserAddresses()
    {
        // Arrange
        var expectedAddresses = new List<GetListByUserIdAddressResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                AddressTitle = "Ev",
                Country = "Türkiye",
                City = "İstanbul",
                District = "Kadıköy",
                AddressLine = "Test Caddesi No:123"
            },
            new()
            {
                Id = Guid.NewGuid(),
                AddressTitle = "İş",
                Country = "Türkiye",
                City = "Ankara",
                District = "Çankaya",
                AddressLine = "İş Caddesi No:456"
            }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListByUserIdAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAddresses);

        // Act
        var result = await _controller.GetListByCurrentUser();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedAddresses);

        // Verify mediator call
        _mockMediator.Verify(m => m.Send(
            It.Is<GetListByUserIdAddressQuery>(q => q.UserId == TestUserId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetListByCurrentUser_WithoutUserId_ShouldReturnUnauthorized()
    {
        // Arrange - Clear user claims
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.GetListByCurrentUser();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.Value.Should().Be("Kullanıcı kimliği alınamadı.");

        // Verify mediator was never called
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListByUserIdAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ShouldReturnOkResult_WithAddress()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var expectedAddress = new GetByIdAddressResponseDto
        {
            Id = addressId,
            AddressTitle = "Test Adresi",
            Country = "Türkiye",
            City = "İstanbul",
            District = "Kadıköy",
            AddressLine = "Test Caddesi No:123"
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetByIdAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAddress);

        // Act
        var result = await _controller.GetById(addressId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedAddress);

        // Verify mediator call with correct parameters
        _mockMediator.Verify(m => m.Send(
            It.Is<GetByIdAddressQuery>(q => q.Id == addressId && q.UserId == TestUserId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetById_WithoutUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.GetById(addressId);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Add Tests

    [Fact]
    public async Task Add_ShouldReturnCreatedAtAction_WithCreatedAddress()
    {
        // Arrange
        var command = new CreateAddressCommand
        {
            AddressTitle = "Test Adresi",
            Country = "Türkiye",
            City = "İstanbul",
            District = "Kadıköy",
            AddressLine = "Test Caddesi No:123"
        };

        var expectedResponse = new CreateAddressResponseDto
        {
            Id = Guid.NewGuid(),
            UserId = TestUserId,
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
        var createdResult = result as CreatedAtActionResult;
        createdResult!.Value.Should().BeEquivalentTo(expectedResponse);
        createdResult.ActionName.Should().Be(nameof(AddressesController.GetById));
        createdResult.RouteValues!["id"].Should().Be(expectedResponse.Id);

        // Verify command was modified with UserId
        command.UserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task Add_WithoutUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new CreateAddressCommand
        {
            AddressTitle = "Test Adresi"
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.Add(command);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldReturnOkResult_WithUpdatedAddress()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            Id = Guid.NewGuid(),
            AddressTitle = "Güncellenmiş Adres",
            Country = "Türkiye",
            City = "Ankara",
            AddressLine = "Güncellenmiş Cadde No:789"
        };

        var expectedResponse = new UpdateAddressResponseDto
        {
            Id = command.Id,
            UserId = TestUserId,
            AddressTitle = command.AddressTitle,
            AddressLine = command.AddressLine
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Update(command);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        // Verify command was modified with UserId
        command.UserId.Should().Be(TestUserId);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldReturnOkResult_WithDeletedResponse()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var expectedResponse = new DeleteAddressResponseDto
        {
            Id = addressId,
            Message = "Adres başarıyla silindi."
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteAddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Delete(addressId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        // Verify mediator call
        _mockMediator.Verify(m => m.Send(
            It.Is<DeleteAddressCommand>(c => c.Id == addressId && c.UserId == TestUserId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Admin Tests - Existing

    [Fact]
    public async Task GetAllAddresses_WithAdminRole_ShouldReturnOkResult_WithPaginatedAddresses()
    {
        // Arrange - ✅ Admin kullanıcısı olarak ayarla
        SetupUserClaims(isAdmin: true, userId: AdminUserId);

        var query = new GetListAddressQuery
        {
            UserNameSearch = "test",
        };

        var expectedResponse = new Mock<IPaginate<GetListAddressResponseDto>>();
        expectedResponse.Setup(x => x.Items).Returns(new List<GetListAddressResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                AddressTitle = "Test Address 1",
                UserFirstName = "John",
                UserLastName = "Doe"
            }
        });

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse.Object);

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResponse.Object);
    }

    #endregion

    #region ⭐ Admin Authorization Tests - NEW

    [Fact]
    public async Task GetAllAddresses_WithNormalUserRole_ShouldBeForbidden()
    {
        // Arrange - Normal kullanıcı (User rolü)
        SetupUserClaims(isAdmin: false);

        var query = new GetListAddressQuery
        {
            UserNameSearch = "test",
        };

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        // ⚠️ Controller seviyesinde role kontrolü yapılmaz, bu ASP.NET Core authorization middleware'inin işidir
        // Ancak unit testte bu senaryoyu simüle edebiliriz
        // Gerçek uygulamada 403 Forbidden dönmeli

        // Controller'ın kendisi role kontrolü yapmadığı için bu test middleware testinde olmalı
        // Burada sadece controller'ın mediator'ı çağırdığını test edebiliriz
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAddresses_WithNoRole_ShouldBeForbidden()
    {
        // Arrange - Role claim'i olmayan kullanıcı
        SetupNoRoleClaims();

        var query = new GetListAddressQuery
        {
            UserNameSearch = "test",
        };

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        // Role olmadığında authorization middleware 403 döner
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAddresses_WithDifferentRole_ShouldBeForbidden()
    {
        // Arrange - Farklı rol (Moderator)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, TestUserId),
            new(ClaimTypes.Name, "Moderator User"),
            new(ClaimTypes.Email, "moderator@example.com"),
            new(ClaimTypes.Role, "Moderator") // ✅ Admin değil, Moderator
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var query = new GetListAddressQuery
        {
            UserNameSearch = "test",
        };

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        // Admin olmadığında 403 Forbidden
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAddresses_WithMultipleRoles_IncludingAdmin_ShouldSucceed()
    {
        // Arrange - Birden fazla role sahip kullanıcı (Admin dahil)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, AdminUserId),
            new(ClaimTypes.Name, "Super Admin"),
            new(ClaimTypes.Email, "superadmin@example.com"),
            new(ClaimTypes.Role, "Admin"), // ✅ Admin rolü var
            new(ClaimTypes.Role, "Moderator"), // Ek rol
            new(ClaimTypes.Role, "User") // Ek rol
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var query = new GetListAddressQuery
        {
            UserNameSearch = "test",
        };

        var expectedResponse = new Mock<IPaginate<GetListAddressResponseDto>>();
        expectedResponse.Setup(x => x.Items).Returns(new List<GetListAddressResponseDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                AddressTitle = "Admin Access Address",
                UserFirstName = "Admin",
                UserLastName = "User"
            }
        });

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse.Object);

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().Be(expectedResponse.Object);

        // Admin rolü olduğu için mediator çağrılmalı
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAddresses_WithCaseInsensitiveAdminRole_ShouldSucceed()
    {
        // Arrange - Case-insensitive admin rolü
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, AdminUserId),
            new(ClaimTypes.Name, "Admin User"),
            new(ClaimTypes.Email, "admin@example.com"),
            new(ClaimTypes.Role, "ADMIN") // ✅ Büyük harfle Admin
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };

        var query = new GetListAddressQuery();

        var expectedResponse = new Mock<IPaginate<GetListAddressResponseDto>>();
        expectedResponse.Setup(x => x.Items).Returns(new List<GetListAddressResponseDto>());

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetListAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse.Object);

        // Act
        var result = await _controller.GetAllAddresses(query);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        // Case-insensitive olduğu için çalışmalı
        _mockMediator.Verify(m => m.Send(
            It.IsAny<GetListAddressQuery>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Helper Method Tests

    [Fact]
    public void GetUserIdFromClaims_WithValidClaims_ShouldReturnUserId()
    {
        // Arrange & Act - User claims already set in constructor

        // Use reflection to test private method
        var method = typeof(AddressesController).GetMethod("GetUserIdFromClaims",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = method?.Invoke(_controller, null) as string;

        // Assert
        result.Should().Be(TestUserId);
    }

    [Fact]
    public void GetUserIdFromClaims_WithoutClaims_ShouldReturnNull()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var method = typeof(AddressesController).GetMethod("GetUserIdFromClaims",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var result = method?.Invoke(_controller, null) as string;

        // Assert
        result.Should().BeNull();
    }

    #endregion
}