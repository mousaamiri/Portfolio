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
        PortraitAlt = "Mousa — portrait photo",
        Footprint = GetFootprint(),
        Endorsements = GetEndorsements()
        // Skills + Education + Journey + Interests now come from Portfolio.API (see AboutController).
    };

    // Journey/timeline is served by Portfolio.API (About page). The mock journey
    // list was retired in E6.

    private static List<StatCounterViewModel> GetFootprint() =>
    [
        new() { Icon = "folder-check", CountTarget = 48, Label = "Projects Completed", LabelKey = "about.footprint_projects" },
        new() { Icon = "code-2", CountTarget = 120000, Suffix = "+", Label = "Lines of Code", LabelKey = "about.footprint_lines" },
        new() { Icon = "git-commit-horizontal", CountTarget = 1500, Suffix = "+", Label = "Commits", LabelKey = "about.footprint_commits" },
        new() { Icon = "calendar", CountTarget = 3, Suffix = "+", Label = "Years Experience", LabelKey = "about.footprint_years" },
        new() { Icon = "layers", CountTarget = 25, Suffix = "+", Label = "Technologies Used", LabelKey = "about.footprint_techs" },
        new() { Icon = "coffee", CountTarget = 999, Suffix = "+", Label = "Cups of Coffee", LabelKey = "about.footprint_coffee" },
        new() { Icon = "badge-check", CountTarget = 6, Label = "Certifications", LabelKey = "about.footprint_certs",
                HasTip = true, TipAriaLabel = "What counts as a certification",
                TipText = "PLACEHOLDER: includes vendor cloud certs and one internal engineering certification track." },
        new() { Icon = "upload-cloud", CountTarget = 150, Suffix = "+", Label = "Deployments", LabelKey = "about.footprint_deploys" }
    ];

    // Interests are served by Portfolio.API (About page). The mock interest list
    // was retired in E7.

    private static List<EndorsementViewModel> GetEndorsements() =>
    [
        new() { Initials = "JD", AvatarColor = "var(--accent)",
                QuoteKey = "about.endorsement_1", NameKey = "about.endorsement_1_name", RoleKey = "about.endorsement_1_role",
                Quote = "“One of the most reliable engineers I've worked with — hands things off knowing they'll be handled properly, no follow-up needed.”",
                Name = "Jane Doe", Role = "Engineering Manager, Acme Systems" },
        new() { Initials = "MS", AvatarColor = "#ec4899",
                QuoteKey = "about.endorsement_2", NameKey = "about.endorsement_2_name", RoleKey = "about.endorsement_2_role",
                Quote = "“Explains complex systems in a way that actually makes sense. Turned a confusing migration into something the whole team understood.”",
                Name = "Mark Smith", Role = "Senior Product Manager, Acme Systems" },
        new() { Initials = "AL", AvatarColor = "#3b82f6",
                QuoteKey = "about.endorsement_3", NameKey = "about.endorsement_3_name", RoleKey = "about.endorsement_3_role",
                Quote = "“Great instincts for when to move fast and when to slow down. That balance is rare and it shows in the quality of the work.”",
                Name = "Amara Lee", Role = "Tech Lead, Acme Systems" },
        new() { Initials = "RK", AvatarColor = "#22c55e",
                QuoteKey = "about.endorsement_4", NameKey = "about.endorsement_4_name", RoleKey = "about.endorsement_4_role",
                Quote = "“Mentored two juniors on my team this year. Patient, clear, and genuinely invested in people growing, not just shipping.”",
                Name = "Ravi Kumar", Role = "Director of Engineering, Acme Systems" }
    ];

    // ── Experience page aggregate ──
    // NOTE (Step 5): the Experience "Technical Proficiency Matrix" is NOT the
    // same data as the About skills grid (it has no percentages — just titled
    // name lists), so it is modeled separately rather than reusing SkillViewModel.
    public static ExperiencePageViewModel GetExperiencePageViewModel() => new()
    {
        Metrics =
        [
            new() { Value = "99.9%", Color = "pink", TagKey = "exp.uptime", Tag = "SERVICE UPTIME",
                    DescKey = "exp.uptime_desc", Desc = "Ensured high availability for mission-critical enterprise backend services." },
            new() { Value = "~40%", Color = "amber", TagKey = "exp.latency", Tag = "API LATENCY",
                    DescKey = "exp.latency_desc", Desc = "Optimized database access patterns and query performance across microservices." },
            new() { Value = "85%+", Color = "green", TagKey = "exp.coverage", Tag = "CODE COVERAGE",
                    DescKey = "exp.coverage_desc", Desc = "Maintained rigorous testing standards using JUnit and Mockito for reliability." },
            new() { Value = "10x", Color = "amber", TagKey = "exp.scale", Tag = "SCALE CAPACITY",
                    DescKey = "exp.scale_desc", Desc = "Architected components to handle exponential increases in concurrent processing." }
        ],
        Principles =
        [
            new() { TitleKey = "exp.principle_1_title", Title = "Scale-First Architecture",
                    DescKey = "exp.principle_1_desc", Desc = "Designing systems that are born to grow, focusing on horizontal scalability and decoupling." },
            new() { TitleKey = "exp.principle_2_title", Title = "Performance Obsessed",
                    DescKey = "exp.principle_2_desc", Desc = "Every millisecond counts. Prioritizing low-latency execution and efficient resource utilization." },
            new() { TitleKey = "exp.principle_3_title", Title = "Security by Design",
                    DescKey = "exp.principle_3_desc", Desc = "Integration of authentication, authorization, and data protection at the core of every service." },
            new() { TitleKey = "exp.principle_4_title", Title = "Craft & Maintainability",
                    DescKey = "exp.principle_4_desc", Desc = "Writing self-documenting code that is as easy to read as it is to extend." }
        ],
        SummaryName = "Jaya Vignesh",
        SummaryText = "Backend engineer focused on designing and evolving Java-based distributed systems. " +
                      "Experienced in building scalable RESTful APIs, modernizing legacy codebases, and " +
                      "driving performance improvements across enterprise microservice architectures.",
        Education =
        [
            new()
            {
                TitleKey = "exp.edu_btech", Title = "Bachelor of Technology (B.Tech) – Computer Science",
                Years = "2019 – 2023",
                InstitutionKey = "exp.edu_btech_inst", Institution = "Sri Manakula Vinayagar Engineering College, Puducherry",
                Score = "CGPA: 7.76",
                Courses =
                [
                    new() { Key = "exp.edu_courses_ds", Text = "DATA STRUCTURES & ALGORITHMS" },
                    new() { Key = "exp.edu_courses_dbms", Text = "DATABASE MANAGEMENT SYSTEMS" },
                    new() { Key = "exp.edu_courses_os", Text = "OPERATING SYSTEMS" },
                    new() { Key = "exp.edu_courses_cn", Text = "COMPUTER NETWORKS" },
                    new() { Key = "exp.edu_courses_web", Text = "WEB DEVELOPMENT" },
                    new() { Key = "exp.edu_courses_cloud", Text = "CLOUD COMPUTING" },
                    new() { Key = "exp.edu_courses_ml", Text = "MACHINE LEARNING" }
                ]
            },
            new()
            {
                TitleKey = "exp.edu_hsc", Title = "Higher Secondary (HSC) – Computer Science",
                Years = "2018 – 2019",
                InstitutionKey = "exp.edu_hsc_inst", Institution = "Amalorpavam Higher Secondary School",
                Score = "Score: 65.1%"
            },
            new()
            {
                TitleKey = "exp.edu_sslc", Title = "Secondary School (SSLC)",
                Years = "2016 – 2017",
                InstitutionKey = "exp.edu_sslc_inst", Institution = "Amalorpavam Higher Secondary School",
                Score = "Score: 89.2%", IsLast = true
            }
        ],
        JobTitle = "Software Engineer (Backend)",
        JobDate = "2024 – Present",
        JobCompany = "Tata Consultancy Services (TCS), India",
        JobBullets =
        [
            new() { Key = "exp.job_1", Text = "Designed, developed, and maintained <span class=\"highlight highlight--amber\">RESTful backend services</span> using <span class=\"highlight highlight--amber\">Java</span> and <span class=\"highlight highlight--amber\">JAX-RS</span>, powering core business workflows within a large-scale enterprise microservice architecture." },
            new() { Key = "exp.job_2", Text = "Spearheaded a major <span class=\"highlight highlight--pink\">Java 8 to Java 21 migration</span>, leveraging virtual threads, pattern matching, and modern APIs to significantly boost runtime performance and developer productivity." },
            new() { Key = "exp.job_3", Text = "Completely rewrote legacy test suites from scratch using modern <span class=\"highlight highlight--amber\">JUnit</span> and <span class=\"highlight highlight--amber\">Mockito</span> frameworks, achieving over 85% code coverage and reducing regression escape rate." },
            new() { Key = "exp.job_4", Text = "Resolved critical <span class=\"highlight highlight--pink\">database bottlenecks</span> by rewriting inefficient queries and restructuring ORM access patterns, resulting in approximately 40% reduction in average API latency." },
            new() { Key = "exp.job_5", Text = "Mentored junior and fellow developers through structured <span class=\"highlight highlight--amber\">code reviews</span>, pair programming sessions, and knowledge-sharing workshops on backend best practices." },
            new() { Key = "exp.job_6", Text = "Proactively monitored and supported <span class=\"highlight highlight--amber\">production backend services</span>, rapidly diagnosing and resolving incidents to uphold 99.9% service uptime SLAs." },
            new() { Key = "exp.job_7", Text = "Refined <span class=\"highlight highlight--amber\">API payloads</span> and contracts through iterative schema optimization, reducing response sizes and improving frontend integration efficiency." },
            new() { Key = "exp.job_8", Text = "Assisted in refactoring <span class=\"highlight highlight--pink\">legacy backend components</span> toward a cleaner, more modular service-oriented architecture, improving code maintainability and team velocity." },
            new() { Key = "exp.job_9", Text = "Worked within secure enterprise environments, adhering to strict <span class=\"highlight highlight--amber\">security protocols</span> and participating in internal audits and compliance reviews." }
        ],
        Stack =
        [
            "JAVA", "MYSQL", "GIT", "POSTMAN", "VS CODE", "ECLIPSE", "BASH",
            "JENKINS", "FILEZILLA", "SPRING BOOT", "MAVEN", "PUTTY", "GITLAB",
            "RED HAT ENTERPRISE LINUX"
        ],
        Proficiency =
        [
            new() { Color = "amber", TitleKey = "exp.mastery", Title = "MASTERY (CORE STACK)",
                    Items = ["Java 21", "Spring Boot", "MySQL", "RESTful APIs", "Microservices"] },
            new() { Color = "pink", TitleKey = "exp.ecosystem", Title = "ECOSYSTEM & TOOLS",
                    Items = ["Git", "Docker", "Jenkins", "Maven", "Postman", "Linux", "JUnit"] },
            new() { Color = "purple", TitleKey = "exp.explorations", Title = "CURRENT EXPLORATIONS",
                    Items = ["System Design", "Cloud Native", "Distributed Systems", "Rust", "LLM Integration"] }
        ]
    };

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
