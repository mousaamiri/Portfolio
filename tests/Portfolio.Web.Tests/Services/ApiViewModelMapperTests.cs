using FluentAssertions;
using Portfolio.Web.Services.Api;

namespace Portfolio.Web.Tests.Services;

public class ApiViewModelMapperTests
{
    [Fact]
    public void ToViewModel_Project_MapsThumbnailAndPreviewAndSubtitle()
    {
        var dto = new ProjectApiDto
        {
            Title = "GitGlance",
            ShortDescription = "GitHub insights",
            Description = "A dashboard.",
            Technologies = "C#, .NET, Blazor",
            ThumbnailUrl = "thumb.png",
            PreviewUrl = "https://demo",
            SourceCodeUrl = "https://github.com/x",
            DisplayOrder = 3
        };

        var vm = ApiViewModelMapper.ToViewModel(dto);

        vm.Title.Should().Be("GitGlance");
        vm.ImageUrl.Should().Be("thumb.png");
        vm.DemoUrl.Should().Be("https://demo");
        vm.GithubUrl.Should().Be("https://github.com/x");
        vm.DisplayId.Should().Be(3);
        vm.SubtitleEn.Should().Be("GitHub insights");
        vm.Techs.Should().HaveCount(3);
        vm.Techs[0].Name.Should().Be("C#");
    }

    [Fact]
    public void ToViewModel_Project_WithoutSource_UsesHashGithubUrl()
    {
        var dto = new ProjectApiDto { Title = "Client work", SourceCodeUrl = null };

        var vm = ApiViewModelMapper.ToViewModel(dto);

        vm.GithubUrl.Should().Be("#");
        vm.Techs.Should().BeEmpty();
    }

    [Fact]
    public void ToViewModel_Skill_MapsIconUrlOntoIconClass()
    {
        var dto = new SkillApiDto { Name = "Docker", Category = "Tools", Proficiency = 82, IconUrl = "docker.svg" };

        var vm = ApiViewModelMapper.ToViewModel(dto);

        vm.Name.Should().Be("Docker");
        vm.Category.Should().Be("Tools");
        vm.Proficiency.Should().Be(82);
        vm.IconClass.Should().Be("docker.svg");
    }

    [Fact]
    public void ToViewModel_Education_DerivesScoreFromGpa()
    {
        var dto = new EducationApiDto { InstitutionName = "Uni", Degree = "BSc", FieldOfStudy = "CS", Gpa = 18.2 };

        var vm = ApiViewModelMapper.ToViewModel(dto);

        vm.InstitutionName.Should().Be("Uni");
        vm.Score.Should().Be("18.2");
    }
}
