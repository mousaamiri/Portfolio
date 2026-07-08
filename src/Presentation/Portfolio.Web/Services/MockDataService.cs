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
        Journey = GetJourney(),
        Footprint = GetFootprint(),
        Interests = GetInterests(),
        Endorsements = GetEndorsements()
        // Skills + Education now come from Portfolio.API (see AboutController).
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
