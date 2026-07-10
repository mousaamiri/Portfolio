using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Portfolio.Domain.Entities.Articles;
using Portfolio.Domain.Entities.Experiences;
using Portfolio.Domain.Entities.Faqs;
using Portfolio.Domain.Entities.ImpactMetrics;
using Portfolio.Domain.Entities.Interests;
using Portfolio.Domain.Entities.Principles;
using Portfolio.Domain.Entities.Proficiencies;
using Portfolio.Domain.Entities.Profiles;
using Portfolio.Domain.Entities.Projects;
using Portfolio.Domain.Entities.Skills;
using Portfolio.Domain.Entities.Stats;
using Portfolio.Domain.Entities.Timeline;
using Portfolio.Domain.Entities.UiTranslations;
using Portfolio.Domain.Enums;
using Portfolio.Infrastructure.Data;

namespace Portfolio.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the owner's real portfolio content (from PERSONAL_INFO.md) into the
/// database. Each content area is guarded by an existence check, so this is
/// idempotent and safe to run on every startup. Two areas are intentionally NOT
/// seeded — Education (no real degree to list) and Testimonials (no real client
/// feedback yet); an empty section is more credible than invented data.
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
        await SeedTimelineAsync(cancellationToken);
        await SeedFaqsAsync(cancellationToken);
        await SeedExperiencesAsync(cancellationToken);
        await SeedImpactMetricsAsync(cancellationToken);
        await SeedPrinciplesAsync(cancellationToken);
        await SeedProficienciesAsync(cancellationToken);
        await SeedArticlesAsync(cancellationToken);
        await SeedUiTranslationsAsync(cancellationToken);
        logger.LogInformation("Content seed check complete.");
    }

    private async Task SeedUiTranslationsAsync(CancellationToken ct)
    {
        // Idempotent top-up per language: insert only the (Key, Language) pairs
        // not already in the DB. Safe on a fresh DB (inserts all) and on an
        // existing one (adds only newly-introduced keys/languages), so new seed
        // entries — including the English rows that make chrome admin-editable —
        // land on restart without duplicating existing rows.
        var added = 0;
        added += await TopUpLanguageAsync(Language.En, UiTranslationSeedData.En, ct);
        added += await TopUpLanguageAsync(Language.Fa, UiTranslationSeedData.Fa, ct);

        if (added == 0) return;

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} UI translations.", added);
    }

    private async Task<int> TopUpLanguageAsync(
        Language language, (string Key, string Value)[] seed, CancellationToken ct)
    {
        var existing = new HashSet<string>(await context.UiTranslations
            .Where(t => t.Language == language)
            .Select(t => t.Key)
            .ToListAsync(ct));

        var added = 0;
        foreach (var (key, value) in seed)
        {
            if (existing.Contains(key)) continue;

            await context.UiTranslations.AddAsync(new UiTranslation
            {
                Id = Guid.NewGuid(),
                Key = key,
                Language = language,
                Value = value,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, ct);
            added++;
        }

        return added;
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

    private async Task SeedTimelineAsync(CancellationToken ct)
    {
        if (await context.TimelineEntries.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (year, icon, titleEn, titleFa, descEn, descFa) in RealTimeline)
        {
            var entry = new TimelineEntry
            {
                Id = Guid.NewGuid(), Year = year, Icon = icon, DisplayOrder = order++,
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            entry.Translations.Add(new TimelineEntryTranslation
            {
                Id = Guid.NewGuid(), TimelineEntryId = entry.Id, Language = Language.En, Title = titleEn, Description = descEn
            });
            entry.Translations.Add(new TimelineEntryTranslation
            {
                Id = Guid.NewGuid(), TimelineEntryId = entry.Id, Language = Language.Fa, Title = titleFa, Description = descFa
            });
            await context.TimelineEntries.AddAsync(entry, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} timeline entries.", RealTimeline.Length);
    }

    private async Task SeedFaqsAsync(CancellationToken ct)
    {
        if (await context.Faqs.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (qEn, aEn, qFa, aFa) in RealFaqs)
        {
            var faq = new Faq { Id = Guid.NewGuid(), DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow };
            faq.Translations.Add(new FaqTranslation
            {
                Id = Guid.NewGuid(), FaqId = faq.Id, Language = Language.En, Question = qEn, Answer = aEn
            });
            faq.Translations.Add(new FaqTranslation
            {
                Id = Guid.NewGuid(), FaqId = faq.Id, Language = Language.Fa, Question = qFa, Answer = aFa
            });
            await context.Faqs.AddAsync(faq, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} FAQs.", RealFaqs.Length);
    }

    private async Task SeedExperiencesAsync(CancellationToken ct)
    {
        if (await context.Experiences.AnyAsync(ct)) return;

        // One honest entry: no professional software employer yet, so this frames the
        // nine years of self-directed engineering (alongside teaching) as the track
        // record. No invented company or school name.
        var exp = new Experience
        {
            Id = Guid.NewGuid(),
            CompanyUrl = "https://github.com/mousaamiri",
            StartDate = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        exp.Translations.Add(new ExperienceTranslation
        {
            Id = Guid.NewGuid(), ExperienceId = exp.Id, Language = Language.En,
            CompanyName = "Independent / Self-directed",
            JobTitle = "Backend Developer — .NET / C#",
            Location = "Tehran, Iran",
            Description = "Nine years of self-directed engineering alongside full-time teaching: designing and " +
                          "building backend systems in C# and .NET — ASP.NET Core Web APIs, a WPF/MVVM desktop " +
                          "application, and Clean Architecture / DDD / CQRS projects, developed test-first with xUnit."
        });
        exp.Translations.Add(new ExperienceTranslation
        {
            Id = Guid.NewGuid(), ExperienceId = exp.Id, Language = Language.Fa,
            CompanyName = "مستقل / خودآموز",
            JobTitle = "برنامه‌نویس بک‌اند — دات‌نت / سی‌شارپ",
            Location = "تهران، ایران",
            Description = "نه سال مهندسی خودآموز در کنار تدریس تمام‌وقت: طراحی و ساخت سیستم‌های بک‌اند با سی‌شارپ و " +
                          "دات‌نت — Web APIهای ASP.NET Core، یک اپلیکیشن دسکتاپ WPF/MVVM و پروژه‌های Clean Architecture / " +
                          "DDD / CQRS، توسعه‌یافته به‌صورت تست‌محور با xUnit."
        });

        await context.Experiences.AddAsync(exp, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded 1 experience entry.");
    }

    private async Task SeedImpactMetricsAsync(CancellationToken ct)
    {
        if (await context.ImpactMetrics.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (value, color, tagEn, descEn, tagFa, descFa) in RealMetrics)
        {
            var metric = new ImpactMetric
            {
                Id = Guid.NewGuid(), Value = value, Color = color, DisplayOrder = order++,
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            metric.Translations.Add(new ImpactMetricTranslation
            {
                Id = Guid.NewGuid(), ImpactMetricId = metric.Id, Language = Language.En, Tag = tagEn, Description = descEn
            });
            metric.Translations.Add(new ImpactMetricTranslation
            {
                Id = Guid.NewGuid(), ImpactMetricId = metric.Id, Language = Language.Fa, Tag = tagFa, Description = descFa
            });
            await context.ImpactMetrics.AddAsync(metric, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} impact metrics.", RealMetrics.Length);
    }

    private async Task SeedPrinciplesAsync(CancellationToken ct)
    {
        if (await context.Principles.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (titleEn, descEn, titleFa, descFa) in RealPrinciples)
        {
            var principle = new Principle { Id = Guid.NewGuid(), DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow };
            principle.Translations.Add(new PrincipleTranslation
            {
                Id = Guid.NewGuid(), PrincipleId = principle.Id, Language = Language.En, Title = titleEn, Description = descEn
            });
            principle.Translations.Add(new PrincipleTranslation
            {
                Id = Guid.NewGuid(), PrincipleId = principle.Id, Language = Language.Fa, Title = titleFa, Description = descFa
            });
            await context.Principles.AddAsync(principle, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} principles.", RealPrinciples.Length);
    }

    private async Task SeedProficienciesAsync(CancellationToken ct)
    {
        if (await context.ProficiencyGroups.AnyAsync(ct)) return;

        var order = 0;
        foreach (var (color, titleEn, itemsEn, titleFa, itemsFa) in RealProficiencies)
        {
            var group = new ProficiencyGroup
            {
                Id = Guid.NewGuid(), Color = color, DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            group.Translations.Add(new ProficiencyGroupTranslation
            {
                Id = Guid.NewGuid(), ProficiencyGroupId = group.Id, Language = Language.En, Title = titleEn, Items = itemsEn
            });
            group.Translations.Add(new ProficiencyGroupTranslation
            {
                Id = Guid.NewGuid(), ProficiencyGroupId = group.Id, Language = Language.Fa, Title = titleFa, Items = itemsFa
            });
            await context.ProficiencyGroups.AddAsync(group, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} proficiency groups.", RealProficiencies.Length);
    }

    private async Task SeedArticlesAsync(CancellationToken ct)
    {
        if (await context.Articles.AnyAsync(ct)) return;

        var order = 0;
        foreach (var a in RealArticles)
        {
            var article = new Article
            {
                Id = Guid.NewGuid(), Slug = a.Slug, Category = a.Category, Tags = a.Tags,
                PublishDate = a.PublishDate, ReadTimeMinutes = a.ReadMinutes,
                IsPublished = true, DisplayOrder = order++, IsActive = true, CreatedAt = DateTime.UtcNow
            };
            article.Translations.Add(new ArticleTranslation
            {
                Id = Guid.NewGuid(), ArticleId = article.Id, Language = Language.En,
                Title = a.TitleEn, Excerpt = a.ExcerptEn, Body = a.BodyEn
            });
            article.Translations.Add(new ArticleTranslation
            {
                Id = Guid.NewGuid(), ArticleId = article.Id, Language = Language.Fa,
                Title = a.TitleFa, Excerpt = a.ExcerptFa, Body = a.BodyFa
            });
            await context.Articles.AddAsync(article, ct);
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} articles.", RealArticles.Length);
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

    // Technical milestones only (owner's preference — no personal/life story).
    private static readonly (string Year, string Icon, string TitleEn, string TitleFa, string DescEn, string DescFa)[] RealTimeline =
    [
        ("2016", "code", "Started self-teaching software development", "آغاز یادگیری خودآموز برنامه‌نویسی",
            "Began learning programming alongside teaching — starting with Java and WordPress before moving toward structured backend engineering.",
            "شروع یادگیری برنامه‌نویسی در کنار تدریس — ابتدا با جاوا و وردپرس، پیش از حرکت به سمت مهندسی بک‌اند ساختاریافته."),
        ("2021", "layers", "Focused on .NET & Clean Architecture", "تمرکز بر دات‌نت و معماری تمیز",
            "Shifted focus to C#, ASP.NET Core, and architectural patterns — Clean Architecture, Domain-Driven Design, and CQRS.",
            "تغییر تمرکز به سی‌شارپ، ASP.NET Core و الگوهای معماری — Clean Architecture، طراحی دامنه‌محور و CQRS."),
        ("2023", "monitor", "Built Wazhechin", "ساخت واژه‌چین",
            "A WPF desktop application with a full MVVM implementation, a custom change-tracking wrapper, and test coverage on the ViewModel layer.",
            "یک اپلیکیشن دسکتاپ WPF با پیاده‌سازی کامل MVVM، سیستم Wrapper سفارشی برای ردیابی تغییرات و پوشش تست روی لایه ViewModel."),
        ("2024", "server", "Built Vitastic", "ساخت ویتاستیک",
            "A DDD/CQRS e-learning backend — bounded contexts, aggregates, and eventual consistency via the Outbox Pattern on ASP.NET Core.",
            "بک‌اند آموزش آنلاین با DDD/CQRS — bounded contextها، aggregateها و سازگاری نهایی از طریق الگوی Outbox روی ASP.NET Core."),
        ("2025", "globe", "Built this portfolio", "ساخت همین پرتفولیو",
            "A bilingual portfolio on a Clean Architecture backend with a real JWT-secured admin panel and full server-rendered CRUD.",
            "یک پرتفولیوی دوزبانه روی بک‌اند Clean Architecture با پنل ادمین واقعی و امن با JWT و CRUD کامل سمت سرور."),
    ];

    private static readonly (string QEn, string AEn, string QFa, string AFa)[] RealFaqs =
    [
        ("Are you available for freelance or remote work?",
            "Yes — I'm open to remote and freelance backend work, especially projects built in .NET / C#.",
            "آیا برای کار فریلنس یا دورکاری در دسترس هستید؟",
            "بله — برای کارهای بک‌اند به‌صورت دورکاری و فریلنس آماده‌ام، به‌ویژه پروژه‌هایی که با دات‌نت / سی‌شارپ ساخته می‌شوند."),
        ("What kind of projects do you take on?",
            "Backend systems in .NET and C# — Web APIs, and applications where Clean Architecture, DDD, or CQRS are a good fit.",
            "چه نوع پروژه‌هایی را می‌پذیرید؟",
            "سیستم‌های بک‌اند با دات‌نت و سی‌شارپ — Web APIها و اپلیکیشن‌هایی که Clean Architecture، DDD یا CQRS برایشان مناسب است."),
        ("What's your typical response time?",
            "I usually reply within 24 hours.",
            "معمولاً چه زمانی پاسخ می‌دهید؟",
            "معمولاً ظرف ۲۴ ساعت پاسخ می‌دهم."),
    ];

    // Experience-page "impact metrics" — honest, verifiable figures (no invented KPIs).
    private static readonly (string Value, string Color, string TagEn, string DescEn, string TagFa, string DescFa)[] RealMetrics =
    [
        ("9", "amber", "Years", "Self-directed engineering alongside full-time teaching.",
            "سال", "مهندسی خودآموز در کنار تدریس تمام‌وقت."),
        ("3", "green", "Projects", "Public projects spanning Web API, DDD, and WPF/MVVM.",
            "پروژه", "پروژه‌های عمومی در حوزه‌ی Web API، DDD و WPF/MVVM."),
        ("100%", "pink", "Test-first", "Core layers built test-first with xUnit, FluentAssertions, and Moq.",
            "تست‌محور", "لایه‌های اصلی به‌صورت تست‌محور با xUnit، FluentAssertions و Moq ساخته شده‌اند."),
    ];

    private static readonly (string TitleEn, string DescEn, string TitleFa, string DescFa)[] RealPrinciples =
    [
        ("Architecture first",
            "Every project starts from Clean Architecture and DDD boundaries before a single feature is written.",
            "معماری در وهله‌ی اول",
            "هر پروژه پیش از نوشتن حتی یک قابلیت، از مرزهای Clean Architecture و DDD آغاز می‌شود."),
        ("Test-driven by default",
            "Behaviour is pinned with tests first — the domain and application layers are covered before the UI exists.",
            "تست‌محور به‌صورت پیش‌فرض",
            "رفتار ابتدا با تست تثبیت می‌شود — لایه‌های دامنه و اپلیکیشن پیش از وجود رابط کاربری پوشش داده می‌شوند."),
        ("Explicit over clever",
            "Readable, boring code that the next developer can follow beats clever code that only I understand.",
            "شفافیت به‌جای زیرکی",
            "کد خوانا و ساده که توسعه‌دهنده‌ی بعدی بتواند دنبال کند، بر کد زیرکانه‌ای که فقط خودم می‌فهمم ارجح است."),
    ];

    // Proficiency matrix groups; Items are comma-separated (split in the Web mapper).
    private static readonly (string Color, string TitleEn, string ItemsEn, string TitleFa, string ItemsFa)[] RealProficiencies =
    [
        ("amber", "Core Stack", "C#, .NET, ASP.NET Core, Entity Framework Core",
            "استک اصلی", "سی‌شارپ, دات‌نت, ASP.NET Core, Entity Framework Core"),
        ("pink", "Architecture", "Clean Architecture, DDD, CQRS, Repository / Unit of Work, Outbox Pattern",
            "معماری", "Clean Architecture, DDD, CQRS, Repository / Unit of Work, الگوی Outbox"),
        ("purple", "Tooling & Data", "PostgreSQL, SQL Server, Docker, Git, xUnit, WPF / MVVM",
            "ابزار و داده", "PostgreSQL, SQL Server, Docker, Git, xUnit, WPF / MVVM"),
    ];

    private sealed record ArticleSeed(
        string Slug, string Category, string Tags, DateTime PublishDate, int ReadMinutes,
        string TitleEn, string ExcerptEn, string BodyEn,
        string TitleFa, string ExcerptFa, string BodyFa);

    private static readonly ArticleSeed[] RealArticles =
    [
        new("clean-architecture-aspnet-core", "Architecture", ".NET, Clean Architecture, ASP.NET Core",
            new DateTime(2025, 11, 3, 0, 0, 0, DateTimeKind.Utc), 8,
            "Clean Architecture in ASP.NET Core: Boundaries that Scale",
            "Why I start every .NET project from the dependency rule, and how the Domain / Application / Infrastructure / Presentation split keeps a codebase changeable as it grows.",
            "Clean Architecture is less about folders and more about the direction of dependencies. In this write-up I walk through how I structure an ASP.NET Core solution so that the Domain layer knows nothing about EF Core, the Application layer orchestrates use-cases behind interfaces, and Infrastructure plugs in at the edges. The payoff is a codebase where swapping a database or a web framework never reaches the business rules.",
            "معماری تمیز در ASP.NET Core: مرزهایی که مقیاس می‌گیرند",
            "چرا هر پروژه‌ی دات‌نت را از قانون وابستگی آغاز می‌کنم، و چگونه تفکیک Domain / Application / Infrastructure / Presentation کدبیس را با رشد آن قابل‌تغییر نگه می‌دارد.",
            "معماری تمیز کمتر درباره‌ی پوشه‌ها و بیشتر درباره‌ی جهت وابستگی‌هاست. در این نوشته توضیح می‌دهم چگونه یک راه‌حل ASP.NET Core را طوری ساختار می‌دهم که لایه‌ی Domain چیزی درباره‌ی EF Core نداند، لایه‌ی Application موارد کاربرد را پشت اینترفیس‌ها هماهنگ کند و Infrastructure در لبه‌ها متصل شود. نتیجه کدبیسی است که تعویض پایگاه‌داده یا فریم‌ورک وب هرگز به قواعد کسب‌وکار نمی‌رسد."),

        new("outbox-pattern-eventual-consistency", "Patterns", ".NET, DDD, Messaging, EF Core",
            new DateTime(2025, 12, 1, 0, 0, 0, DateTimeKind.Utc), 7,
            "The Outbox Pattern: Reliable Events Without Distributed Transactions",
            "How Vitastic publishes domain events atomically with its database writes — no two-phase commit, no lost messages.",
            "When a use-case both changes state and needs to publish an event, doing them in two separate calls invites the classic dual-write problem. The Outbox Pattern solves it by writing the event into the same transaction as the state change, then relaying it asynchronously. I cover the EF Core mechanics, the relay worker, and idempotent consumers.",
            "الگوی Outbox: رویدادهای قابل‌اعتماد بدون تراکنش توزیع‌شده",
            "چگونه ویتاستیک رویدادهای دامنه را به‌صورت اتمیک همراه با نوشتن در پایگاه‌داده منتشر می‌کند — بدون two-phase commit و بدون گم‌شدن پیام.",
            "وقتی یک مورد کاربرد هم وضعیت را تغییر می‌دهد و هم باید رویدادی منتشر کند، انجام این دو در دو فراخوانی جدا مشکل کلاسیک dual-write را پدید می‌آورد. الگوی Outbox با نوشتن رویداد در همان تراکنشِ تغییر وضعیت و سپس ارسال ناهمگام آن، این را حل می‌کند. مکانیک EF Core، کارگر ارسال و مصرف‌کننده‌های idempotent را پوشش می‌دهم."),

        new("tdd-dotnet-xunit", "Testing", ".NET, TDD, xUnit, Testing",
            new DateTime(2026, 1, 12, 0, 0, 0, DateTimeKind.Utc), 6,
            "Test-Driven .NET: A Practical xUnit Workflow",
            "The red-green-refactor loop I actually use — and why testing the domain against a relational provider caught a bug the in-memory provider hid.",
            "TDD gets abstract fast, so this is the concrete version: how I structure xUnit projects, when I reach for FluentAssertions and Moq, and a real story where an in-memory database happily passed a test that SQL Server rejected. The lesson — test your persistence-facing code against a provider that behaves like production.",
            "دات‌نت تست‌محور: یک روال عملی با xUnit",
            "حلقه‌ی red-green-refactor که واقعاً استفاده می‌کنم — و چرا تست دامنه روی یک provider رابطه‌ای باگی را گرفت که provider درون‌حافظه‌ای پنهان کرده بود.",
            "TDD زود انتزاعی می‌شود، پس این نسخه‌ی عینی آن است: چگونه پروژه‌های xUnit را ساختار می‌دهم، کِی سراغ FluentAssertions و Moq می‌روم، و ماجرایی واقعی که پایگاه‌داده‌ی درون‌حافظه‌ای تستی را با خوشحالی پاس کرد که SQL Server ردش می‌کرد. درس — کدِ روبه‌پایداری را روی provider ای تست کنید که مثل محیط تولید رفتار کند."),
    ];
}
