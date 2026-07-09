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
    private readonly Mock<IAdminCrudClient> _crud = new();
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

        _sut = new AdminController(_adminApi.Object, _crud.Object)
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

    // ── Projects CRUD ──

    [Fact]
    public async Task Projects_ReturnsListFromApi()
    {
        _adminApi.Setup(a => a.GetProjectsAsync("en", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ProjectApiDto { Id = Guid.NewGuid(), Slug = "vitastic", Title = "Vitastic", IsPublished = true }]);

        var result = await _sut.Projects(CancellationToken.None) as ViewResult;
        var model = (List<Portfolio.Web.Models.Admin.ProjectListItem>)result!.Model!;

        model.Should().ContainSingle();
        model[0].Slug.Should().Be("vitastic");
        model[0].Title.Should().Be("Vitastic");
    }

    [Fact]
    public async Task ProjectCreate_Post_Valid_CreatesAndRedirects()
    {
        _adminApi.Setup(a => a.CreateProjectAsync(It.IsAny<CreateProjectApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var model = new Portfolio.Web.Models.Admin.ProjectFormModel
        {
            Slug = "new-proj", ThumbnailUrl = "t.jpg", TitleEn = "New"
        };

        var result = await _sut.ProjectCreate(model, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Projects");
        _adminApi.Verify(a => a.CreateProjectAsync(
            It.Is<CreateProjectApiRequest>(r => r.Slug == "new-proj" && r.Translations.Count == 1 && r.Translations[0].Title == "New"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProjectCreate_Post_ApiFailure_ReturnsFormWithError()
    {
        _adminApi.Setup(a => a.CreateProjectAsync(It.IsAny<CreateProjectApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var model = new Portfolio.Web.Models.Admin.ProjectFormModel { Slug = "dup", ThumbnailUrl = "t", TitleEn = "X" };

        var result = await _sut.ProjectCreate(model, CancellationToken.None);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.ViewName.Should().Be("ProjectForm");
        _sut.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task ProjectCreate_Post_InvalidModel_DoesNotCallApi()
    {
        _sut.ModelState.AddModelError("Slug", "Required");

        var result = await _sut.ProjectCreate(new Portfolio.Web.Models.Admin.ProjectFormModel(), CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        _adminApi.Verify(a => a.CreateProjectAsync(It.IsAny<CreateProjectApiRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProjectEdit_Get_BuildsBilingualForm()
    {
        var id = Guid.NewGuid();
        _adminApi.Setup(a => a.GetProjectBothLanguagesAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                new ProjectApiDto { Id = id, Slug = "vitastic", Title = "Vitastic", ShortDescription = "LMS" },
                new ProjectApiDto { Id = id, Slug = "vitastic", Title = "ویتاستیک", ShortDescription = "سامانه" }));

        var result = await _sut.ProjectEdit(id, CancellationToken.None) as ViewResult;
        var model = (Portfolio.Web.Models.Admin.ProjectFormModel)result!.Model!;

        model.TitleEn.Should().Be("Vitastic");
        model.TitleFa.Should().Be("ویتاستیک");
        model.IsEdit.Should().BeTrue();
    }

    [Fact]
    public async Task ProjectEdit_Get_NotFound_Returns404()
    {
        var id = Guid.NewGuid();
        _adminApi.Setup(a => a.GetProjectBothLanguagesAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(((ProjectApiDto?)null, (ProjectApiDto?)null));

        var result = await _sut.ProjectEdit(id, CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ProjectEdit_Post_Valid_UpdatesAndRedirects()
    {
        var id = Guid.NewGuid();
        _adminApi.Setup(a => a.UpdateProjectAsync(id, It.IsAny<UpdateProjectApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = new Portfolio.Web.Models.Admin.ProjectFormModel { Id = id, Slug = "s", ThumbnailUrl = "t", TitleEn = "Updated", IsActive = true };

        var result = await _sut.ProjectEdit(id, model, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Projects");
        _adminApi.Verify(a => a.UpdateProjectAsync(id, It.Is<UpdateProjectApiRequest>(r => r.Translations[0].Title == "Updated"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProjectDelete_Post_DeletesAndRedirects()
    {
        var id = Guid.NewGuid();
        _adminApi.Setup(a => a.DeleteProjectAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.ProjectDelete(id, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Projects");
        _adminApi.Verify(a => a.DeleteProjectAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Skills CRUD (generic client) ──

    [Fact]
    public async Task Skills_ReturnsListView()
    {
        _crud.Setup(c => c.ListAsync<ProjectApiDto>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
        _crud.Setup(c => c.ListAsync<Portfolio.Web.Services.Api.SkillApiDto>("skills", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Portfolio.Web.Services.Api.SkillApiDto { Id = Guid.NewGuid(), Name = "C#", Category = "Backend", Proficiency = 85 }]);

        var result = await _sut.Skills(CancellationToken.None) as ViewResult;

        result!.ViewName.Should().Be("_AdminList");
        var vm = (Portfolio.Web.Models.Admin.AdminListViewModel)result.Model!;
        vm.Rows.Should().ContainSingle();
        vm.Rows[0].Cells[0].Should().Be("C#");
    }

    [Fact]
    public async Task SkillCreate_Post_Valid_CreatesAndRedirects()
    {
        _crud.Setup(c => c.CreateAsync("skills", It.IsAny<Portfolio.Web.Services.Api.SkillApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var result = await _sut.SkillCreate(new Portfolio.Web.Models.Admin.SkillFormModel { IconUrl = "csharp", NameEn = "C#" }, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Skills");
        _crud.Verify(c => c.CreateAsync("skills",
            It.Is<Portfolio.Web.Services.Api.SkillApiRequest>(r => r.Translations[0].Name == "C#"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SkillCreate_Post_InvalidModel_DoesNotCallApi()
    {
        _sut.ModelState.AddModelError("NameEn", "Required");

        var result = await _sut.SkillCreate(new Portfolio.Web.Models.Admin.SkillFormModel(), CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        _crud.Verify(c => c.CreateAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SkillEdit_Get_NotFound_Returns404()
    {
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.SkillApiDto>("skills", It.IsAny<Guid>(), "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Portfolio.Web.Services.Api.SkillApiDto?)null);

        var result = await _sut.SkillEdit(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task SkillEdit_Get_BuildsBilingualForm()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.SkillApiDto>("skills", id, "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.SkillApiDto { Id = id, Name = "C#", Category = "Backend" });
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.SkillApiDto>("skills", id, "fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.SkillApiDto { Id = id, Name = "سی‌شارپ" });

        var result = await _sut.SkillEdit(id, CancellationToken.None) as ViewResult;
        var model = (Portfolio.Web.Models.Admin.SkillFormModel)result!.Model!;

        model.NameEn.Should().Be("C#");
        model.NameFa.Should().Be("سی‌شارپ");
    }

    [Fact]
    public async Task SkillDelete_Post_DeletesAndRedirects()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.DeleteAsync("skills", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.SkillDelete(id, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Skills");
        _crud.Verify(c => c.DeleteAsync("skills", id, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ── Simple entities (Faq/Interest/Timeline/Stat) via generic helpers ──

    [Fact]
    public async Task Faqs_ListView_MapsRows()
    {
        _crud.Setup(c => c.ListAsync<Portfolio.Web.Services.Api.FaqApiDto>("faqs", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Portfolio.Web.Services.Api.FaqApiDto { Id = Guid.NewGuid(), Question = "Remote?", Answer = "Yes" }]);

        var result = await _sut.Faqs(CancellationToken.None) as ViewResult;

        result!.ViewName.Should().Be("_AdminList");
        ((Portfolio.Web.Models.Admin.AdminListViewModel)result.Model!).Rows.Should().ContainSingle();
    }

    [Fact]
    public async Task FaqCreate_Post_Valid_PostsBilingualAndRedirects()
    {
        _crud.Setup(c => c.CreateAsync("faqs", It.IsAny<Portfolio.Web.Services.Api.FaqApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var result = await _sut.FaqCreate(
            new Portfolio.Web.Models.Admin.FaqFormModel { QuestionEn = "Q", AnswerEn = "A", QuestionFa = "س", AnswerFa = "ج" },
            CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Faqs");
        _crud.Verify(c => c.CreateAsync("faqs",
            It.Is<Portfolio.Web.Services.Api.FaqApiRequest>(r => r.Translations.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FaqCreate_Post_EnglishOnly_SendsSingleTranslation()
    {
        _crud.Setup(c => c.CreateAsync("faqs", It.IsAny<Portfolio.Web.Services.Api.FaqApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        await _sut.FaqCreate(new Portfolio.Web.Models.Admin.FaqFormModel { QuestionEn = "Q", AnswerEn = "A" }, CancellationToken.None);

        _crud.Verify(c => c.CreateAsync("faqs",
            It.Is<Portfolio.Web.Services.Api.FaqApiRequest>(r => r.Translations.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InterestEdit_Get_NotFound_Returns404()
    {
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.InterestApiDto>("interests", It.IsAny<Guid>(), "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Portfolio.Web.Services.Api.InterestApiDto?)null);

        var result = await _sut.InterestEdit(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task TimelineEdit_Get_BuildsBilingualForm()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.TimelineEntryApiDto>("timeline", id, "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.TimelineEntryApiDto { Id = id, Year = "2024", Title = "Built X" });
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.TimelineEntryApiDto>("timeline", id, "fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.TimelineEntryApiDto { Id = id, Year = "2024", Title = "ساخت X" });

        var result = await _sut.TimelineEdit(id, CancellationToken.None) as ViewResult;
        var model = (Portfolio.Web.Models.Admin.TimelineFormModel)result!.Model!;

        model.TitleEn.Should().Be("Built X");
        model.TitleFa.Should().Be("ساخت X");
    }

    [Fact]
    public async Task StatDelete_Post_DeletesAndRedirects()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.DeleteAsync("stats", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.StatDelete(id, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Stats");
    }

    [Fact]
    public async Task StatCreate_Post_InvalidModel_DoesNotCallApi()
    {
        _sut.ModelState.AddModelError("LabelEn", "Required");

        var result = await _sut.StatCreate(new Portfolio.Web.Models.Admin.StatFormModel(), CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        _crud.Verify(c => c.CreateAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // ── Experience / Education (date-based CV entities) ──

    [Fact]
    public async Task Experiences_ListView_MapsRows()
    {
        _crud.Setup(c => c.ListAsync<Portfolio.Web.Services.Api.ExperienceApiDto>("experiences", "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Portfolio.Web.Services.Api.ExperienceApiDto { Id = Guid.NewGuid(), CompanyName = "Acme", JobTitle = "Dev", StartDate = new DateTime(2022, 1, 1) }]);

        var result = await _sut.Experiences(CancellationToken.None) as ViewResult;

        result!.ViewName.Should().Be("_AdminList");
        ((Portfolio.Web.Models.Admin.AdminListViewModel)result.Model!).Rows.Should().ContainSingle();
    }

    [Fact]
    public async Task ExperienceCreate_Post_Valid_PostsTranslationAndRedirects()
    {
        _crud.Setup(c => c.CreateAsync("experiences", It.IsAny<Portfolio.Web.Services.Api.ExperienceApiRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        var result = await _sut.ExperienceCreate(
            new Portfolio.Web.Models.Admin.ExperienceFormModel { CompanyNameEn = "Acme", JobTitleEn = "Dev" }, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Experiences");
        _crud.Verify(c => c.CreateAsync("experiences",
            It.Is<Portfolio.Web.Services.Api.ExperienceApiRequest>(r => r.Translations[0].JobTitle == "Dev"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EducationEdit_Get_BuildsBilingualForm()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.EducationApiDto>("educations", id, "en", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.EducationApiDto { Id = id, InstitutionName = "MIT", Degree = "BSc", FieldOfStudy = "CS" });
        _crud.Setup(c => c.GetAsync<Portfolio.Web.Services.Api.EducationApiDto>("educations", id, "fa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Portfolio.Web.Services.Api.EducationApiDto { Id = id, InstitutionName = "ام‌آی‌تی", Degree = "کارشناسی", FieldOfStudy = "کامپیوتر" });

        var result = await _sut.EducationEdit(id, CancellationToken.None) as ViewResult;
        var model = (Portfolio.Web.Models.Admin.EducationFormModel)result!.Model!;

        model.DegreeEn.Should().Be("BSc");
        model.DegreeFa.Should().Be("کارشناسی");
    }

    [Fact]
    public async Task EducationDelete_Post_DeletesAndRedirects()
    {
        var id = Guid.NewGuid();
        _crud.Setup(c => c.DeleteAsync("educations", id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _sut.EducationDelete(id, CancellationToken.None);

        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Education");
    }
}
