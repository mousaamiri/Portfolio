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
        LearningDate = "Jul 6, 2026",

        Projects = GetProjects(),
        Skills = GetSkills(),
        Experiences = GetExperiences(),
        Educations = GetEducations()
    };

    // Projects — the six real projects from the static Work page. Shared by the
    // Home model (test only needs non-empty) and the Work master/detail showcase.
    // Legacy fields (Title/Technologies/...) are kept populated so _ProjectCard
    // still compiles; Work uses the bilingual + colored-tech fields.
    private static List<ProjectViewModel> GetProjects() =>
    [
        new()
        {
            DisplayId = 1,
            NameEn = "ResumeIQ Platform", NameFa = "پلتفرم ResumeIQ",
            SubtitleEn = "AI Resume Analysis & ATS Intelligence Platform",
            SubtitleFa = "تحلیل هوشمند رزومه و سازگاری با سامانه‌های ATS",
            DescriptionEn = "An enterprise-grade resume analysis platform that evaluates candidate resumes against job descriptions using Google Gemini AI. Features automated PDF parsing, ATS compatibility scoring, skill gap identification, actionable improvement recommendations, secure JWT authentication with eager auto-renewal, and role-based access control.",
            DescriptionFa = "یک پلتفرم سازمانی برای تحلیل رزومه که با بهره‌گیری از هوش مصنوعی Google Gemini، رزومه‌ی متقاضیان را با شرح شغل مقایسه و ارزیابی می‌کند. امکاناتی مانند استخراج خودکار اطلاعات از PDF، امتیازدهی سازگاری با ATS، شناسایی شکاف مهارتی، پیشنهادهای عملی بهبود، احراز هویت امن با JWT و کنترل دسترسی مبتنی بر نقش را فراهم می‌آورد.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "Java 21", Color = "#f0a63b" }, new() { Name = "Spring Boot", Color = "#22c55e" },
                new() { Name = "PostgreSQL", Color = "#3b82f6" }, new() { Name = "React", Color = "#3b82f6" },
                new() { Name = "TypeScript", Color = "#3b82f6" }, new() { Name = "Tailwind CSS", Color = "#14b8a6" },
                new() { Name = "Gemini AI", Color = "#3b82f6" }, new() { Name = "Apache PDFBox", Color = "#ec4899" },
                new() { Name = "JWT Auth", Color = "#22c55e" }, new() { Name = "Docker", Color = "#3b82f6" }
            ],
            Title = "ResumeIQ Platform",
            Description = "AI Resume Analysis & ATS Intelligence Platform",
            Technologies = "Java 21, Spring Boot, PostgreSQL, React, TypeScript, Tailwind CSS, Gemini AI, Apache PDFBox, JWT Auth, Docker",
            SourceCodeUrl = "#"
        },
        new()
        {
            DisplayId = 2,
            NameEn = "GitGlance", NameFa = "GitGlance",
            SubtitleEn = "Ultimate Github Profile Visualizer",
            SubtitleFa = "ابزار جامع مصورسازی پروفایل گیت‌هاب",
            DescriptionEn = "A comprehensive GitHub profile visualization tool that provides detailed analytics, contribution graphs, repository insights, and developer statistics in a beautiful dashboard interface.",
            DescriptionFa = "ابزاری کامل برای مصورسازی پروفایل گیت‌هاب که تحلیل‌های جزئی، نمودار مشارکت‌ها، بینش مخازن و آمار توسعه‌دهنده را در قالب داشبوردی زیبا و کاربردی ارائه می‌دهد.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "React", Color = "#3b82f6" }, new() { Name = "TypeScript", Color = "#3b82f6" },
                new() { Name = "GitHub API", Color = "#f2f1ec" }, new() { Name = "Tailwind CSS", Color = "#14b8a6" },
                new() { Name = "Chart.js", Color = "#ec4899" }
            ],
            Title = "GitGlance", Description = "Ultimate Github Profile Visualizer",
            Technologies = "React, TypeScript, GitHub API, Tailwind CSS, Chart.js", SourceCodeUrl = "#"
        },
        new()
        {
            DisplayId = 3,
            NameEn = "Spring AI RAG", NameFa = "Spring AI RAG",
            SubtitleEn = "Production-Ready AI RAG System",
            SubtitleFa = "سامانه هوش مصنوعی RAG آماده‌ی تولید",
            DescriptionEn = "A production-ready Retrieval-Augmented Generation system built with Spring AI, featuring document ingestion, vector storage, semantic search, and context-aware AI responses for enterprise applications.",
            DescriptionFa = "یک سامانه‌ی بازیابی-افزوده‌ی تولیدی (RAG) آماده‌ی بهره‌برداری که با Spring AI ساخته شده است. قابلیت‌هایی شامل دریافت و پردازش اسناد، ذخیره‌سازی بُرداری، جستجوی معنایی و تولید پاسخ‌های هوشمند متناسب با زمینه را برای کاربردهای سازمانی فراهم می‌کند.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "Java 21", Color = "#f0a63b" }, new() { Name = "Spring Boot", Color = "#22c55e" },
                new() { Name = "Spring AI", Color = "#22c55e" }, new() { Name = "PostgreSQL", Color = "#3b82f6" },
                new() { Name = "pgvector", Color = "#a855f7" }, new() { Name = "Docker", Color = "#3b82f6" }
            ],
            Title = "Spring AI RAG", Description = "Production-Ready AI RAG System",
            Technologies = "Java 21, Spring Boot, Spring AI, PostgreSQL, pgvector, Docker", SourceCodeUrl = "#"
        },
        new()
        {
            DisplayId = 4,
            NameEn = "SyncBoard", NameFa = "SyncBoard",
            SubtitleEn = "Real-time Collaborative Clipboard",
            SubtitleFa = "کلیپ‌بورد مشارکتی لحظه‌ای",
            DescriptionEn = "A real-time collaborative clipboard application enabling seamless text and data sharing across devices with WebSocket-powered synchronization and secure room-based access.",
            DescriptionFa = "برنامه‌ای برای اشتراک‌گذاری لحظه‌ای متن و داده میان دستگاه‌های مختلف، با هم‌زمان‌سازی مبتنی بر WebSocket و دسترسی امن از طریق سیستم اتاق‌های خصوصی.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "Java 21", Color = "#f0a63b" }, new() { Name = "Spring Boot", Color = "#22c55e" },
                new() { Name = "WebSocket", Color = "#a855f7" }, new() { Name = "React", Color = "#3b82f6" },
                new() { Name = "Redis", Color = "#ec4899" }
            ],
            Title = "SyncBoard", Description = "Real-time Collaborative Clipboard",
            Technologies = "Java 21, Spring Boot, WebSocket, React, Redis", SourceCodeUrl = "#"
        },
        new()
        {
            DisplayId = 5,
            NameEn = "Movie Recommendation System", NameFa = "سامانه پیشنهاد فیلم",
            SubtitleEn = "Content-Based ML Recommender",
            SubtitleFa = "پیشنهاددهنده‌ی هوشمند مبتنی بر محتوا",
            DescriptionEn = "A machine learning-powered movie recommendation engine using content-based filtering with TF-IDF vectorization and cosine similarity for personalized movie suggestions.",
            DescriptionFa = "موتور پیشنهاد فیلم که از یادگیری ماشین و فیلترسازی مبتنی بر محتوا بهره می‌برد. با استفاده از بُرداری‌سازی TF-IDF و شباهت کسینوسی، پیشنهادهای شخصی‌سازی‌شده‌ای به کاربر ارائه می‌دهد.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "Python", Color = "#f0a63b" }, new() { Name = "scikit-learn", Color = "#3b82f6" },
                new() { Name = "Pandas", Color = "#a855f7" }, new() { Name = "Flask", Color = "#22c55e" },
                new() { Name = "TMDB API", Color = "#ec4899" }
            ],
            Title = "Movie Recommendation System", Description = "Content-Based ML Recommender",
            Technologies = "Python, scikit-learn, Pandas, Flask, TMDB API", SourceCodeUrl = "#"
        },
        new()
        {
            DisplayId = 6,
            NameEn = "Order Management Service", NameFa = "سرویس مدیریت سفارش",
            SubtitleEn = "Secure Spring Boot Backend API",
            SubtitleFa = "رابط برنامه‌نویسی امن با Spring Boot",
            DescriptionEn = "A secure and scalable order management backend service with RESTful APIs, JWT authentication, role-based authorization, and comprehensive order lifecycle management.",
            DescriptionFa = "سرویس بک‌اند امن و مقیاس‌پذیر برای مدیریت سفارش‌ها، مجهز به رابط‌های RESTful، احراز هویت JWT، سطوح دسترسی مبتنی بر نقش و مدیریت کامل چرخه‌ی عمر سفارش.",
            GithubUrl = "#",
            Techs =
            [
                new() { Name = "Java 21", Color = "#f0a63b" }, new() { Name = "Spring Boot", Color = "#22c55e" },
                new() { Name = "Spring Security", Color = "#22c55e" }, new() { Name = "MySQL", Color = "#3b82f6" },
                new() { Name = "JWT", Color = "#f0a63b" }, new() { Name = "Docker", Color = "#3b82f6" }
            ],
            Title = "Order Management Service", Description = "Secure Spring Boot Backend API",
            Technologies = "Java 21, Spring Boot, Spring Security, MySQL, JWT, Docker", SourceCodeUrl = "#"
        }
    ];

    // Work page uses the same six-project list as the master/detail showcase.
    public static List<ProjectViewModel> GetWorkProjects() => GetProjects();

    // Blog — article metadata (title/excerpt/category/date/read-time). Mirrors
    // the 16 posts in blog.js, which still owns the full HTML article bodies and
    // the reading modal. English-only (the static Blog has no FA / data-i18n).
    public static List<BlogPostViewModel> GetBlogPosts() =>
    [
        new() { Id = 1, Category = "Java", Date = new DateTime(2026, 5, 26), ReadTime = 3,
                Title = "Java 21 Virtual Threads: Infinite Scaling Meets Database Bottlenecks",
                Excerpt = "Virtual threads solve the thread-per-request scaling limit, but they shift the bottleneck directly to your relational database connection pool. Here is why it happens and how to fix it." },
        new() { Id = 2, Category = "Java", Date = new DateTime(2026, 5, 26), ReadTime = 3,
                Title = "Virtual Threads vs. OS Threads in Java 21: A Deep Dive",
                Excerpt = "Understand the fundamental architectural differences between OS-managed platform threads and JVM-managed virtual threads, and when to use each in your Java applications." },
        new() { Id = 3, Category = "Java", Date = new DateTime(2026, 3, 29), ReadTime = 1,
                Title = "Java Upgrades Are Architectural Events",
                Excerpt = "Upgrading Java is never just about the JVM. It exposes architectural weaknesses, hidden coupling, and long-standing assumptions." },
        new() { Id = 4, Category = "Architecture", Date = new DateTime(2025, 12, 6), ReadTime = 2,
                Title = "When Microservices Become an Organizational Bug",
                Excerpt = "Microservices don't fail at scale — teams do. A blunt look at how service boundaries expose complexity, slow delivery, and hide responsibility." },
        new() { Id = 5, Category = "Quality", Date = new DateTime(2025, 12, 5), ReadTime = 2,
                Title = "Code Coverage Is Not Test Quality",
                Excerpt = "High coverage numbers often hide fragile systems. This post explains why meaningful tests look like in real backend services and why branches matter more than percentages." },
        new() { Id = 6, Category = "Code Quality", Date = new DateTime(2025, 11, 22), ReadTime = 2,
                Title = "Cognitive Complexity Is a Smell, Not a Metric",
                Excerpt = "Static analysis gives numbers. Production gives pain. This article explains why cognitive complexity always predicts expensive thinking." },
        new() { Id = 7, Category = "API Design", Date = new DateTime(2025, 11, 12), ReadTime = 2,
                Title = "Why Most Backend APIs Are Designed Backwards",
                Excerpt = "APIs often expose databases instead of intent. This post explains how backend engineers leak internal models, and how to design APIs that survive change." },
        new() { Id = 8, Category = "Spring", Date = new DateTime(2025, 11, 10), ReadTime = 2,
                Title = "Spring Boot Magic Ends at the First Incident",
                Excerpt = "Auto-configuration helps — until it hides behavior. This post explains where Spring Boot shines, and where core Spring knowledge becomes mandatory." },
        new() { Id = 9, Category = "Java", Date = new DateTime(2025, 10, 14), ReadTime = 2,
                Title = "JPA Is Not Slow — Your Mental Model Is",
                Excerpt = "JPA doesn't randomly hit the database. It does exactly what you told it to do. This post fixes common ORM misconceptions at code level." },
        new() { Id = 10, Category = "Java", Date = new DateTime(2025, 10, 3), ReadTime = 2,
                Title = "Why Java 8 Still Runs Half the World",
                Excerpt = "New LTS versions exist — yet Java 8 refuses to die. This post explains enterprise risk, tooling inertia, and why upgrades are rarely just technical." },
        new() { Id = 11, Category = "Spring Core", Date = new DateTime(2025, 9, 19), ReadTime = 2,
                Title = "Annotations Are Not Magic",
                Excerpt = "@Component doesn't register anything by itself. This article walks through what Spring actually does — from scanning to bean instantiation." },
        new() { Id = 12, Category = "Testing", Date = new DateTime(2025, 9, 5), ReadTime = 2,
                Title = "Why 90% Coverage Still Means Zero Confidence",
                Excerpt = "Coverage measures execution, not correctness. This post explains why high-coverage systems still fear releases, and how to test behavior instead of lines." },
        new() { Id = 13, Category = "Testing", Date = new DateTime(2025, 8, 21), ReadTime = 2,
                Title = "Mocking Everything Is a Design Smell",
                Excerpt = "If every dependency needs mocking, the design is already lying. This post explains when mocks help — and when they hurt." },
        new() { Id = 14, Category = "Systems", Date = new DateTime(2025, 8, 7), ReadTime = 2,
                Title = "DDoS Is Not a Network Problem",
                Excerpt = "Most DDoS discussions stop at bandwidth. This post explains why application asymmetry and state are the real vulnerabilities." },
        new() { Id = 15, Category = "Career", Date = new DateTime(2025, 7, 15), ReadTime = 3,
                Title = "Being a Senior Engineer Is About Saying No",
                Excerpt = "Senior engineers don't write more code. They prevent unnecessary code from existing. This post explains why judgment beats speed." },
        new() { Id = 16, Category = "Thinking", Date = new DateTime(2025, 7, 4), ReadTime = 2,
                Title = "Clean Code Alone Doesn't Survive Production",
                Excerpt = "Clean code is readable. Production code is negotiable under pressure. This post explores the gap between theory and reality." }
    ];

    // Skills — shared data (migration decision #5): the About "Skills" grid and
    // the Experience "Technical Proficiency Matrix" render the SAME list two ways.
    // Values ported from the static About page (4 categories × 4 skills).
    private static List<SkillViewModel> GetSkills() =>
    [
        new() { Name = "Java", Category = "Backend", Proficiency = 90 },
        new() { Name = "Node.js", Category = "Backend", Proficiency = 85 },
        new() { Name = "Python", Category = "Backend", Proficiency = 80 },
        new() { Name = "Spring Boot", Category = "Backend", Proficiency = 88 },
        new() { Name = "React", Category = "Frontend", Proficiency = 90 },
        new() { Name = "JavaScript", Category = "Frontend", Proficiency = 92 },
        new() { Name = "HTML / CSS", Category = "Frontend", Proficiency = 95 },
        new() { Name = "TypeScript", Category = "Frontend", Proficiency = 78 },
        new() { Name = "PostgreSQL", Category = "Database", Proficiency = 85 },
        new() { Name = "MongoDB", Category = "Database", Proficiency = 75 },
        new() { Name = "MySQL", Category = "Database", Proficiency = 80 },
        new() { Name = "Redis", Category = "Database", Proficiency = 70 },
        new() { Name = "Git", Category = "Tools", Proficiency = 95 },
        new() { Name = "Docker", Category = "Tools", Proficiency = 82 },
        new() { Name = "AWS", Category = "Tools", Proficiency = 78 },
        new() { Name = "Linux", Category = "Tools", Proficiency = 85 }
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

    // Education — public About section (migration decision #6) + admin panel,
    // one shared list. Shapes mirror the admin `Education` entity (institution,
    // degree, description, start/end year, score).
    private static List<EducationViewModel> GetEducations() =>
    [
        new()
        {
            InstitutionName = "State University",
            Degree = "B.S.",
            FieldOfStudy = "Computer Science",
            Description = "Focused on distributed systems and algorithms; the foundation for thinking like an engineer beyond \"it compiles\".",
            StartDate = new DateTime(2016, 9, 1),
            EndDate = new DateTime(2019, 6, 1),
            Gpa = 18.2,
            Score = "18.2 / 20"
        },
        new()
        {
            InstitutionName = "Cloud Institute",
            Degree = "Professional Certification",
            FieldOfStudy = "Cloud Architecture",
            Description = "Formalized years of hands-on infrastructure work with a professional cloud architecture certification.",
            StartDate = new DateTime(2022, 1, 1),
            EndDate = new DateTime(2022, 6, 1),
            Score = "Pass"
        }
    ];

    // ── About page aggregate (9 sections) ──
    public static AboutViewModel GetAboutViewModel() => new()
    {
        RoleValue = "Software Engineer",
        ExperienceValue = "3+ Years",
        DegreeValue = "B.S. Computer Science",
        PortraitUrl = "/images/about-portrait.jpg",
        PortraitAlt = "Mousa — portrait photo",
        Journey = GetJourney(),
        Footprint = GetFootprint(),
        Skills = GetSkills(),
        Interests = GetInterests(),
        Endorsements = GetEndorsements(),
        Education = GetEducations()
    };

    private static List<TimelineEntryViewModel> GetJourney() =>
    [
        new() { Year = "2016", Icon = "graduation-cap", TitleKey = "about.journey_1_title", DescKey = "about.journey_1_desc",
                Title = "Started Computer Science Degree",
                Description = "Enrolled at State University, drawn in by a first-year course that turned out to be the start of everything." },
        new() { Year = "2018", Icon = "briefcase", TitleKey = "about.journey_2_title", DescKey = "about.journey_2_desc",
                Title = "First Software Internship",
                Description = "Joined a small team as an intern, shipping real features to real users for the first time." },
        new() { Year = "2019", Icon = "award", TitleKey = "about.journey_3_title", DescKey = "about.journey_3_desc",
                Title = "Graduated & First Full-Time Role",
                Description = "Graduated with a B.S. in Computer Science and joined Acme Systems as a Backend Developer." },
        new() { Year = "2021", Icon = "rocket", TitleKey = "about.journey_4_title", DescKey = "about.journey_4_desc",
                Title = "Led First Major Project Launch",
                Description = "Owned the migration from a legacy monolith to a set of independently deployable services." },
        new() { Year = "2022", Icon = "badge-check", TitleKey = "about.journey_5_title", DescKey = "about.journey_5_desc",
                Title = "Earned Cloud Certification",
                Description = "Formalized years of hands-on infrastructure work with a professional cloud architecture certification." },
        new() { Year = "2024", Icon = "trending-up", TitleKey = "about.journey_6_title", DescKey = "about.journey_6_desc",
                Title = "Promoted to Senior Engineer",
                Description = "Now focused on cloud architecture and mentoring the next round of engineers joining the team." }
    ];

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

    private static List<InterestViewModel> GetInterests() =>
    [
        new() { Icon = "code", Label = "Coding", LabelKey = "about.interest_coding" },
        new() { Icon = "music", Label = "Music", LabelKey = "about.interest_music" },
        new() { Icon = "gamepad-2", Label = "Gaming", LabelKey = "about.interest_gaming" },
        new() { Icon = "bug", Label = "Debugging", LabelKey = "about.interest_debugging" },
        new() { Icon = "book-open", Label = "Reading", LabelKey = "about.interest_reading" },
        new() { Icon = "camera", Label = "Photography", LabelKey = "about.interest_photography" },
        new() { Icon = "mountain", Label = "Hiking", LabelKey = "about.interest_hiking" },
        new() { Icon = "puzzle", Label = "Chess", LabelKey = "about.interest_chess" },
        new() { Icon = "chef-hat", Label = "Cooking", LabelKey = "about.interest_cooking" },
        new() { Icon = "plane", Label = "Travel", LabelKey = "about.interest_travel" },
        new() { Icon = "clapperboard", Label = "Movies", LabelKey = "about.interest_movies" },
        new() { Icon = "dumbbell", Label = "Fitness", LabelKey = "about.interest_fitness" }
    ];

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
    // Structured contact config + FAQ list. The form itself stays a client-side
    // simulation (contact.js); no server submission in this phase.
    public static ContactViewModel GetContactViewModel() => new()
    {
        Email = "hello@jayavignesh.dev",
        Phone = "+91 00000 00000",
        GitHubUrl = "https://github.com/jayavignesh",
        LinkedInUrl = "https://linkedin.com/in/jayavignesh",
        InstagramUrl = "https://instagram.com/jayavignesh",
        Faqs =
        [
            new() { QuestionKey = "contact.faq_1_q", AnswerKey = "contact.faq_1_a",
                    Question = "What services do you offer?",
                    Answer = "I specialize in full-stack development, system architecture, backend engineering with Java/Spring Boot, cloud infrastructure (AWS, Azure, GCP), and building scalable distributed systems. I also consult on performance optimization and DevOps practices." },
            new() { QuestionKey = "contact.faq_2_q", AnswerKey = "contact.faq_2_a",
                    Question = "Are you available for freelance work?",
                    Answer = "Yes, I'm open to freelance and contract opportunities. I prefer projects where I can make a meaningful impact, whether it's building something from scratch or optimizing existing systems. Feel free to reach out with your project details." },
            new() { QuestionKey = "contact.faq_3_q", AnswerKey = "contact.faq_3_a",
                    Question = "What is your preferred tech stack?",
                    Answer = "My primary stack includes Java, Spring Boot, React, Python, and cloud services (AWS/Azure/GCP). For databases, I work with PostgreSQL, MongoDB, and Redis. I also have experience with Docker, Kubernetes, and CI/CD pipelines." },
            new() { QuestionKey = "contact.faq_4_q", AnswerKey = "contact.faq_4_a",
                    Question = "Do you work remotely?",
                    Answer = "Absolutely. I've been working remotely for several years and am comfortable with async communication, distributed teams, and remote collaboration tools. I'm flexible with time zones and can adjust my schedule for overlap with your team." }
        ]
    };
}
