using Portfolio.Web.Models.ViewModels;

namespace Portfolio.Web.Services;

public static class MockDataService
{
    public static HomeViewModel GetHomeViewModel() => new()
    {
        // Hero identity — from the static site (design source of truth).
        FullName = "Mousa Amiri Motlagh",
        JobTitle = "Backend Engineer",
        Bio = "I build backend systems, design APIs, and work with cloud infrastructure. " +
              "Focused on Java, Spring Boot, and production-grade architecture. I care " +
              "about clean design, performance, and building systems that are reliable " +
              "in real-world environments.",

        // Hero structured config (resume links, socials, "learning" popover).
        ResumeUrlEn = "/resumes/resume-en.pdf",
        ResumeUrlFa = "/resumes/resume-fa.pdf",
        GitHubUrl = "https://github.com/mousaamiri/",
        InstagramUrl = "https://instagram.com",
        LinkedInUrl = "https://linkedin.com",
        LearningTitle = "Learning Langchain",
        LearningDesc = "Exploring LLM orchestration, chains, agents, and tool integration with LangChain framework.",
        LearningDate = "Jul 6, 2026"

        // All Home lists (Projects/Skills/Experiences/Educations) now come from
        // Portfolio.API via HomeController; their mock lists were retired in D2-D4.
        // GetHomeViewModel supplies only the hero copy (TODO(E4): move to Profile).
    };

    // Projects are served by Portfolio.API (Home + Work). The static six-project
    // mock list and GetWorkProjects() were removed in D2.

    // Articles are served by Portfolio.API (Blog page). The mock article-metadata
    // list was retired in E1b. (blog.js still owns the client-side grid + reading
    // modal from its own hardcoded post array — see BlogController TODO.)

    // Skills are served by Portfolio.API (About page). The mock skill list was
    // retired in D3.

    // Professional history is served by Portfolio.API (Experience page). The
    // legacy mock experience list was retired in D4.

    // Education is served by Portfolio.API (About page + admin). The mock
    // education list was retired in D3.

    // ── About page aggregate (9 sections) ──
    public static AboutViewModel GetAboutViewModel() => new()
    {
        RoleValue = "Software Engineer",
        ExperienceValue = "3+ Years",
        DegreeValue = "B.S. Computer Science",
        PortraitUrl = "/images/about-portrait.jpg",
        PortraitAlt = "Mousa — portrait photo"
        // Skills, Education, Journey, Interests, Footprint and Endorsements now come
        // from Portfolio.API (see AboutController). Only the hero stat badges above
        // remain mock-backed.
    };

    // Journey/timeline is served by Portfolio.API (About page). The mock journey
    // list was retired in E6.

    // Footprint stats are served by Portfolio.API (About page). The mock footprint
    // list was retired in E8.

    // Interests are served by Portfolio.API (About page). The mock interest list
    // was retired in E7.

    // Endorsements are served by Portfolio.API (About page). The mock (fictional
    // "Acme Systems") endorsement list was retired in E2 — the section stays empty
    // until real testimonials exist, per PERSONAL_INFO.md.

    // ── Experience page aggregate ──
    // Metrics / Principles / Proficiency come from Portfolio.API and Summary from the
    // Profile entity (assigned in ExperienceController). The static site's CV-style
    // Education, single-role block and Stack were fabricated template data
    // ("Jaya Vignesh / TCS") with no real equivalent, so they are intentionally left
    // empty — the view hides those sections when there's nothing real to show.
    public static ExperiencePageViewModel GetExperiencePageViewModel() => new();

    // ── Contact page ──
    // Structured contact config only. The FAQ list is served by Portfolio.API
    // (retired here in E5); contact info stays mocked pending Profile coverage.
    public static ContactViewModel GetContactViewModel() => new()
    {
        Email = "hello@jayavignesh.dev",
        Phone = "+91 00000 00000",
        GitHubUrl = "https://github.com/jayavignesh",
        LinkedInUrl = "https://linkedin.com/in/jayavignesh",
        InstagramUrl = "https://instagram.com/jayavignesh"
    };
}
