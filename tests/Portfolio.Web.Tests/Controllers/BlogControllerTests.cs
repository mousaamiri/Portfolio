using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portfolio.Web.Controllers;
using Portfolio.Web.Models.ViewModels;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Controllers;

public class BlogControllerTests
{
    private readonly Mock<IPortfolioApiClient> _api = new();
    private readonly BlogController _sut;

    public BlogControllerTests()
    {
        _sut = new BlogController(_api.Object);
    }

    private async Task<List<BlogPostViewModel>> InvokeAsync()
    {
        var result = await _sut.Index(null, CancellationToken.None) as ViewResult;
        return (List<BlogPostViewModel>)result!.Model!;
    }

    [Fact]
    public async Task Index_PostsComeFromApi()
    {
        _api.Setup(a => a.GetArticlesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new ArticleApiDto
            {
                Slug = "virtual-threads", Title = "Virtual Threads", Excerpt = "x",
                Category = "Java", PublishDate = new DateTime(2026, 5, 26), ReadTimeMinutes = 3,
                Body = "<p>full body</p>"
            }]);

        var model = await InvokeAsync();

        model.Should().ContainSingle();
        model[0].Id.Should().Be("virtual-threads");
        model[0].Title.Should().Be("Virtual Threads");
        model[0].Category.Should().Be("Java");
        model[0].ReadTime.Should().Be(3);
        model[0].Body.Should().Be("<p>full body</p>");
        _api.Verify(a => a.GetArticlesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Index_WhenApiEmpty_ReturnsEmptyGrid()
    {
        _api.Setup(a => a.GetArticlesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var model = await InvokeAsync();

        model.Should().BeEmpty();
    }
}
