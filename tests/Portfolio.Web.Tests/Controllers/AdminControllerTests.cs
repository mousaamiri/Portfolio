using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models;
using Portfolio.Web.Services.Api;
using System.Security.Claims;

namespace Portfolio.Web.Tests.Controllers;

public class AdminControllerTests
{
    private readonly Mock<IAdminApiClient> _adminApi = new();
    private readonly Mock<IAuthenticationService> _authService = new();
    private readonly AdminController _sut;

    public AdminControllerTests()
    {
        _authService
            .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        _authService
            .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();
        services.AddSingleton(_authService.Object);

        var httpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };

        _sut = new AdminController(_adminApi.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>()),
            Url = Mock.Of<IUrlHelper>(u => u.IsLocalUrl(It.IsAny<string>()) == false)
        };
    }

    private void SetAuthenticated()
    {
        _sut.ControllerContext.HttpContext.User =
            new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "admin") }, "cookie"));
    }

    [Fact]
    public async Task Login_Post_ValidCredentials_SignsInAndRedirectsToDashboard()
    {
        _adminApi.Setup(a => a.LoginAsync("admin", "secret", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdminLoginResult("jwt-token", DateTime.UtcNow.AddDays(7)));

        var result = await _sut.Login(new AdminLoginModel { Username = "admin", Password = "secret" }, null, CancellationToken.None);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be("Index");
        _adminApi.Verify(a => a.LoginAsync("admin", "secret", It.IsAny<CancellationToken>()), Times.Once);
        _authService.Verify(a => a.SignInAsync(
            It.IsAny<HttpContext>(), "Cookies",
            It.Is<ClaimsPrincipal>(p => p.FindFirst("access_token")!.Value == "jwt-token"),
            It.IsAny<AuthenticationProperties>()), Times.Once);
    }

    [Fact]
    public async Task Login_Post_InvalidCredentials_ReturnsViewWithErrorAndDoesNotSignIn()
    {
        _adminApi.Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AdminLoginResult?)null);

        var result = await _sut.Login(new AdminLoginModel { Username = "admin", Password = "wrong" }, null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        _sut.ModelState.IsValid.Should().BeFalse();
        _authService.Verify(a => a.SignInAsync(
            It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Never);
    }

    [Fact]
    public async Task Login_Post_InvalidModelState_ReturnsViewWithoutCallingApi()
    {
        _sut.ModelState.AddModelError("Username", "Required");

        var result = await _sut.Login(new AdminLoginModel(), null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        _adminApi.Verify(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Login_Get_WhenAlreadyAuthenticated_RedirectsToDashboard()
    {
        SetAuthenticated();

        var result = _sut.Login();

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Index");
    }

    [Fact]
    public void Login_Get_WhenAnonymous_ReturnsView()
    {
        var result = _sut.Login();

        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Logout_Post_SignsOutAndRedirectsToLogin()
    {
        var result = await _sut.Logout();

        result.Should().BeOfType<RedirectToActionResult>()
            .Which.ActionName.Should().Be("Login");
        _authService.Verify(a => a.SignOutAsync(It.IsAny<HttpContext>(), "Cookies", It.IsAny<AuthenticationProperties>()), Times.Once);
    }
}
