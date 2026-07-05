using Portfolio.Web.Models.ViewModels;

namespace Portfolio.Web.Services;

public static class MockDataService
{
    public static HomeViewModel GetHomeViewModel() => new()
    {
        FullName = "Mousa Amiri",
        JobTitle = "Full-Stack .NET Developer",
        Bio = "Passionate software engineer with expertise in building modern web applications using ASP.NET Core, Clean Architecture, and cloud technologies. I love crafting clean, maintainable code and delivering exceptional user experiences.",
        Projects = GetProjects(),
        Skills = GetSkills(),
        Experiences = GetExperiences(),
        Educations = GetEducations()
    };

    private static List<ProjectViewModel> GetProjects() =>
    [
        new()
        {
            Title = "E-Commerce Platform",
            Description = "A full-featured online store built with ASP.NET Core, featuring product management, shopping cart, payment integration, and order tracking.",
            Technologies = "ASP.NET Core, Entity Framework, SQL Server, Redis, Docker",
            ImageUrl = "https://placehold.co/600x400/2563eb/ffffff?text=E-Commerce",
            DemoUrl = "https://example.com/ecommerce",
            SourceCodeUrl = "https://github.com/example/ecommerce"
        },
        new()
        {
            Title = "Task Management App",
            Description = "A collaborative task management application with real-time updates, team workspaces, and Kanban board functionality.",
            Technologies = "ASP.NET Core, SignalR, React, PostgreSQL",
            ImageUrl = "https://placehold.co/600x400/8b5cf6/ffffff?text=Task+Manager",
            DemoUrl = "https://example.com/taskmanager",
            SourceCodeUrl = "https://github.com/example/taskmanager"
        },
        new()
        {
            Title = "Weather Dashboard",
            Description = "A responsive weather dashboard that displays real-time weather data, forecasts, and interactive maps using external APIs.",
            Technologies = "ASP.NET Core, Blazor, Chart.js, OpenWeatherMap API",
            ImageUrl = "https://placehold.co/600x400/0ea5e9/ffffff?text=Weather+App",
            DemoUrl = "https://example.com/weather"
        }
    ];

    private static List<SkillViewModel> GetSkills() =>
    [
        new() { Name = "C#", Category = "Backend", Proficiency = 95, IconClass = "skill-icon-csharp" },
        new() { Name = "ASP.NET Core", Category = "Backend", Proficiency = 90, IconClass = "skill-icon-dotnet" },
        new() { Name = "SQL Server", Category = "Backend", Proficiency = 85, IconClass = "skill-icon-sql" },
        new() { Name = "HTML / CSS", Category = "Frontend", Proficiency = 88, IconClass = "skill-icon-html" },
        new() { Name = "JavaScript", Category = "Frontend", Proficiency = 80, IconClass = "skill-icon-js" },
        new() { Name = "React", Category = "Frontend", Proficiency = 75, IconClass = "skill-icon-react" }
    ];

    private static List<ExperienceViewModel> GetExperiences() =>
    [
        new()
        {
            CompanyName = "Tech Solutions Inc.",
            JobTitle = "Senior .NET Developer",
            Description = "Led development of enterprise web applications using ASP.NET Core and microservices architecture. Mentored junior developers and established coding standards.",
            Location = "Tehran, Iran",
            StartDate = new DateTime(2022, 3, 1),
            EndDate = null
        },
        new()
        {
            CompanyName = "Digital Innovations Co.",
            JobTitle = ".NET Developer",
            Description = "Developed and maintained multiple web applications using ASP.NET MVC and Web API. Implemented CI/CD pipelines and improved deployment processes.",
            Location = "Tehran, Iran",
            StartDate = new DateTime(2019, 6, 1),
            EndDate = new DateTime(2022, 2, 28)
        }
    ];

    private static List<EducationViewModel> GetEducations() =>
    [
        new()
        {
            InstitutionName = "University of Tehran",
            Degree = "Master of Science",
            FieldOfStudy = "Software Engineering",
            Description = "Focused on software architecture, distributed systems, and cloud computing.",
            StartDate = new DateTime(2017, 9, 1),
            EndDate = new DateTime(2019, 6, 1),
            Gpa = 18.2
        },
        new()
        {
            InstitutionName = "Amirkabir University of Technology",
            Degree = "Bachelor of Science",
            FieldOfStudy = "Computer Engineering",
            Description = "Studied fundamentals of computer science, algorithms, and data structures.",
            StartDate = new DateTime(2013, 9, 1),
            EndDate = new DateTime(2017, 6, 1),
            Gpa = 17.5
        }
    ];
}
