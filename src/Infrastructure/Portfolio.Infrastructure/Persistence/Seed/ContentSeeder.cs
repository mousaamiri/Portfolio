using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the owner's real portfolio content (from PERSONAL_INFO.md) into the
/// database. Each content area is guarded by an existence check, so this is
/// idempotent and safe to run on every startup. Areas the owner left as TODO
/// (Timeline years, FAQ answers, Experience/Education/Articles/Testimonials) are
/// intentionally NOT seeded — an empty section is preferred over invented data.
/// </summary>
public class ContentSeeder(AppDbContext context, ILogger<ContentSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedProfileAsync(cancellationToken);
        await SeedSkillsAsync(cancellationToken);
        await SeedProjectsAsync(cancellationToken);
        await SeedInterestsAsync(cancellationToken);
        await SeedStatsAsync(cancellationToken);
        logger.LogInformation("Content seed check complete.");
    }

    private async Task SeedProfileAsync(CancellationToken ct)
    {
        if (await context.Profiles.AnyAsync(ct)) return;

        var profile = new Profile
        {
            Id = Guid.NewGuid(),
            Email = "mousa.amiri.dev@gmail.com",
            GitHubUrl = "https://github.com/mousaamiri",
            WebsiteUrl = "https://mousaamiri.ir",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        profile.Translations.Add(new ProfileTranslation
        {
            Id = Guid.NewGuid(), ProfileId = profile.Id, Language = Language.En,
            FullName = "Mousa Amiri Motlaq",
            JobTitle = "Backend Developer — .NET / C#",
            Tagline = "PRESS CMD+K FOR COMMANDS",
            Bio = "Backend developer focused on .NET and C#, working with Clean Architecture, " +
                  "Domain-Driven Design, and CQRS. Nine years of hands-on, self-directed engineering " +
                  "across Java, WordPress, and the .NET ecosystem, now concentrated on building " +
                  "maintainable, well-structured backend systems."
        });
        profile.Translations.Add(new ProfileTranslation
        {
            Id = Guid.NewGuid(), ProfileId = profile.Id, Language = Language.Fa,
            FullName = "موسی امیری مطلق",
            JobTitle = "برنامه‌نویس بک‌اند — دات‌نت / سی‌شارپ",
            Tagline = "PRESS CMD+K FOR COMMANDS",
            Bio = "برنامه‌نویس بک‌اند با تمرکز بر دات‌نت و سی‌شارپ، با تجربه در Clean Architecture، " +
                  "Domain-Driven Design و CQRS. نه سال تجربه‌ی عملی و خودآموز در جاوا، وردپرس و " +
                  "اکوسیستم دات‌نت، که اکنون بر ساخت سیستم‌های بک‌اند قابل‌نگهداری و ساختاریافته متمرکز شده است."
        });

        await context.Profiles.AddAsync(profile, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded profile.");
    }

    private async Task SeedSkillsAsync(CancellationToken ct)
    {
        if (await context.Skills.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (nameEn, nameFa, category, level, icon) in RealSkills)
        {
            var skill = new Skill
            {
                Id = Guid.NewGuid(), IconUrl = icon, Proficiency = level,
                DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            skill.Translations.Add(new SkillTranslation
            {
                Id = Guid.NewGuid(), SkillId = skill.Id, Language = Language.En, Name = nameEn, Category = category
            });
            skill.Translations.Add(new SkillTranslation
            {
                Id = Guid.NewGuid(), SkillId = skill.Id, Language = Language.Fa, Name = nameFa, Category = category
            });
            await context.Skills.AddAsync(skill, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} skills.", RealSkills.Length);
    }

    private async Task SeedProjectsAsync(CancellationToken ct)
    {
        if (await context.Projects.AnyAsync(ct)) return;

        foreach (var p in RealProjects)
            await context.Projects.AddAsync(p(), ct);

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} projects.", RealProjects.Length);
    }

    private async Task SeedInterestsAsync(CancellationToken ct)
    {
        if (await context.Interests.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (labelEn, labelFa, icon) in RealInterests)
        {
            var interest = new Interest
            {
                Id = Guid.NewGuid(), Icon = icon, DisplayOrder = order++,
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            interest.Translations.Add(new InterestTranslation
            {
                Id = Guid.NewGuid(), InterestId = interest.Id, Language = Language.En, Label = labelEn
            });
            interest.Translations.Add(new InterestTranslation
            {
                Id = Guid.NewGuid(), InterestId = interest.Id, Language = Language.Fa, Label = labelFa
            });
            await context.Interests.AddAsync(interest, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} interests.", RealInterests.Length);
    }

    private async Task SeedStatsAsync(CancellationToken ct)
    {
        if (await context.StatCounters.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (labelEn, labelFa, target, suffix, icon) in RealStats)
        {
            var stat = new StatCounter
            {
                Id = Guid.NewGuid(), Icon = icon, CountTarget = target, Suffix = suffix,
                DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            stat.Translations.Add(new StatCounterTranslation
            {
                Id = Guid.NewGuid(), StatCounterId = stat.Id, Language = Language.En, Label = labelEn
            });
            stat.Translations.Add(new StatCounterTranslation
            {
                Id = Guid.NewGuid(), StatCounterId = stat.Id, Language = Language.Fa, Label = labelFa
            });
            await context.StatCounters.AddAsync(stat, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} stat counters.", RealStats.Length);
    }

    // ── Real content (PERSONAL_INFO.md) ──

    private static readonly (string En, string Fa, string Category, int Level, string Icon)[] RealSkills =
    [
        ("Razor Views / MVC", "نمای Razor / MVC", "Frontend", 65, "dotnetcore"),
        ("Vanilla JS", "جاوااسکریپت خالص", "Frontend", 60, "javascript"),
        ("HTML / CSS (SCSS)", "اچ‌تی‌ام‌ال / سی‌اس‌اس", "Frontend", 65, "sass"),
        ("C# / .NET", "سی‌شارپ / دات‌نت", "Backend", 85, "csharp"),
        ("ASP.NET Core (Web API, MVC)", "ای‌اس‌پی‌دات‌نت‌کور", "Backend", 80, "dotnetcore"),
        ("Clean Architecture", "معماری تمیز", "Backend", 80, ""),
        ("Domain-Driven Design (DDD)", "طراحی دامنه‌محور", "Backend", 75, ""),
        ("CQRS", "سی‌کیو‌آر‌اس", "Backend", 75, ""),
        ("Repository Pattern / Unit of Work", "الگوی ریپازیتوری", "Backend", 80, ""),
        ("Outbox Pattern", "الگوی Outbox", "Backend", 65, ""),
        ("JWT Auth / Refresh Token rotation", "احراز هویت JWT", "Backend", 70, ""),
        ("TDD (xUnit, FluentAssertions, Moq)", "تست‌محور توسعه", "Backend", 70, ""),
        ("MVVM (WPF)", "ام‌وی‌وی‌ام", "Backend", 75, ""),
        ("PostgreSQL", "پستگرس‌کیوال", "Database", 70, "postgresql"),
        ("SQL Server", "اس‌کیو‌ال سرور", "Database", 70, "microsoftsqlserver"),
        ("Entity Framework Core", "ای‌اف کور", "Database", 80, "dotnetcore"),
        ("Docker / Docker Compose", "داکر", "Tools", 65, "docker"),
        ("Git / GitHub", "گیت", "Tools", 75, "git"),
        ("WPF / MahApps.Metro", "دبلیو‌پی‌اف", "Tools", 70, "dot-net"),
    ];

    private static readonly (string En, string Fa, string Icon)[] RealInterests =
    [
        ("Open Source", "متن‌باز", "git-branch"),
        ("Architecture & Design Patterns", "معماری و الگوهای طراحی", "layout-template"),
        ("Persian-language Dev Tooling", "ابزار توسعه‌ی فارسی", "languages"),
    ];

    private static readonly (string En, string Fa, long Target, string? Suffix, string Icon)[] RealStats =
    [
        ("Years of self-directed engineering", "سال مهندسی خودآموز", 9, null, "calendar"),
        ("Public projects", "پروژه‌ی عمومی", 3, null, "folder-git-2"),
    ];

    private static readonly Func<Project>[] RealProjects =
    [
        () => BuildProject(
            slug: "vitastic", order: 0, github: "https://github.com/mousaamiri/Vitastic", demo: null,
            titleEn: "Vitastic", titleFa: "ویتاستیک",
            subtitleEn: "Online Learning Platform", subtitleFa: "پلتفرم فروش دوره‌های آموزشی آنلاین",
            descEn: "An e-learning backend built around tactical DDD — bounded contexts, aggregates, and " +
                    "eventual consistency via the Outbox Pattern. Follows a four-layer DDD structure (Domain, " +
                    "Application, Infrastructure, Presentation) with an ASP.NET Core MVC frontend consuming the API over HTTP.",
            descFa: "بک‌اندی برای فروش دوره‌های آنلاین با Domain-Driven Design تاکتیکی — bounded contextها، " +
                    "aggregateها و سازگاری نهایی از طریق الگوی Outbox. ساختار چهار لایه‌ای DDD (Domain، Application، " +
                    "Infrastructure، Presentation) با فرانت‌اند MVC که از طریق HTTP با API ارتباط برقرار می‌کند.",
            tech: "ASP.NET Core, PostgreSQL, Clean Architecture, DDD, CQRS, Outbox Pattern, Docker"),

        () => BuildProject(
            slug: "wazhechin", order: 1, github: "https://github.com/mousaamiri/Wazhechin", demo: null,
            titleEn: "Wazhechin", titleFa: "واژه‌چین",
            subtitleEn: "Persian Library Lending Management System", subtitleFa: "سیستم مدیریت امانت کتاب‌خانه",
            descEn: "A WPF desktop application for library book-lending management, built to demonstrate a " +
                    "professional MVVM implementation. Full book/author/category/member management, a loan/return " +
                    "system, a statistics dashboard, runtime theme switching (MahApps.Metro), and a custom " +
                    "change-tracking Wrapper system (ModelWrapper<T>) with full test coverage on the ViewModel layer.",
            descFa: "یک اپلیکیشن دسکتاپ ویندوزی برای مدیریت امانت کتاب‌خانه، با هدف نمایش یک پیاده‌سازی حرفه‌ای از " +
                    "معماری MVVM. شامل مدیریت کامل کتاب‌ها، نویسندگان، دسته‌بندی‌ها و اعضا، سیستم امانت و استرداد، " +
                    "داشبورد آماری، تغییر تم در زمان اجرا (MahApps.Metro) و سیستم Wrapper سفارشی برای ردیابی تغییرات.",
            tech: "WPF, MVVM, SQL Server, Entity Framework Core, CommunityToolkit.Mvvm, MahApps.Metro, TDD, Repository Pattern"),

        () => BuildProject(
            slug: "portfolio", order: 2, github: "https://github.com/mousaamiri/Portfolio", demo: "https://mousaamiri.ir",
            titleEn: "Portfolio", titleFa: "پرتفولیو",
            subtitleEn: "Personal Portfolio Website", subtitleFa: "وب‌سایت شخصی",
            descEn: "A bilingual (Persian/English) personal portfolio with an admin panel, built with ASP.NET Core " +
                    "using Clean Architecture (Domain, Application, Infrastructure, API). JWT-based admin " +
                    "authentication with rate-limited login, EF Core migrations applied at startup, and a public/admin controller split.",
            descFa: "یک وب‌سایت شخصی دوزبانه (فارسی/انگلیسی) با پنل مدیریت، ساخته‌شده با ASP.NET Core و معماری Clean " +
                    "Architecture (Domain، Application، Infrastructure، API). احراز هویت ادمین مبتنی بر JWT با محدودیت " +
                    "نرخ ورود، اجرای Migrationهای EF Core در زمان استارت‌آپ، و تفکیک کنترلرهای عمومی و ادمین.",
            tech: "ASP.NET Core, .NET 10, Entity Framework Core, Clean Architecture, JWT Auth, xUnit, FluentAssertions, Moq"),
    ];

    private static Project BuildProject(
        string slug, int order, string github, string? demo,
        string titleEn, string titleFa, string subtitleEn, string subtitleFa,
        string descEn, string descFa, string tech)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Slug = slug,
            ThumbnailUrl = $"/images/projects/{slug}.jpg",
            SourceCodeUrl = github,
            PreviewUrl = demo,
            IsPublished = true,
            IsFeatured = order == 0,
            DisplayOrder = order,
            StartedAt = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = project.Id, Language = Language.En,
            Title = titleEn, ShortDescription = subtitleEn, Description = descEn, Technologies = tech
        });
        project.Translations.Add(new ProjectTranslation
        {
            Id = Guid.NewGuid(), ProjectId = project.Id, Language = Language.Fa,
            Title = titleFa, ShortDescription = subtitleFa, Description = descFa, Technologies = tech
        });
        return project;
    }
}
