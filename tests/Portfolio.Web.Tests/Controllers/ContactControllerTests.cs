using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class ContactControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly ContactController _sut;

    public ContactControllerTests()
    {
        _sut = new ContactController(_api.Object);
    }

    [Fact]
    public async Task Index_ReturnsContactViewModel()
    {
        _api.Setup(a => a.GetFaqsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;

        result!.Model.Should().BeOfType<ContactViewModel>();
    }

    [Fact]
    public async Task Index_PopulatesFaqsFromApi()
    {
        _api.Setup(a => a.GetFaqsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new FaqApiDto { Question = "Remote?", Answer = "Yes" }]);

        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        var model = (ContactViewModel)result!.Model!;

        model.Faqs.Should().ContainSingle();
        model.Faqs[0].Question.Should().Be("Remote?");
        model.Faqs[0].Answer.Should().Be("Yes");
    }

    [Fact]
    public async Task Submit_ValidForm_ForwardsToApiAndReturnsSuccess()
    {
        _api.Setup(a => a.SendMessageAsync(It.IsAny<ContactMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var form = new ContactFormModel
        {
            Name = "Jane", Email = "jane@example.com", Message = "Hello", Interest = "freelance"
        };

        var result = await _sut.Submit(form, CancellationToken.None) as JsonResult;

        result.Should().NotBeNull();
        result!.Value!.GetType().GetProperty("success")!.GetValue(result.Value).Should().Be(true);
        _api.Verify(a => a.SendMessageAsync(
            It.Is<ContactMessageRequest>(r => r.Name == "Jane" && r.Body == "Hello" && r.Interest == "freelance"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Submit_InvalidForm_ReturnsBadRequestWithoutCallingApi()
    {
        _sut.ModelState.AddModelError("Email", "Required");

        var result = await _sut.Submit(new ContactFormModel(), CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
        _api.Verify(a => a.SendMessageAsync(It.IsAny<ContactMessageRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Submit_ApiFailure_ReturnsSuccessFalse()
    {
        _api.Setup(a => a.SendMessageAsync(It.IsAny<ContactMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var form = new ContactFormModel { Name = "Jane", Email = "jane@example.com", Message = "Hello" };

        var result = await _sut.Submit(form, CancellationToken.None) as JsonResult;

        result!.Value!.GetType().GetProperty("success")!.GetValue(result.Value).Should().Be(false);
    }
}
