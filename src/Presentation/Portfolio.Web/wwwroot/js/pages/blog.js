/* ==========================================================================
   blog.js — Blog page logic
   --------------------------------------------------------------------------
   Renders blog cards, category filters, search, and full-screen article
   modal with reading-progress bar.
   ========================================================================== */

(function () {
  "use strict";

  /* ====================================================================
     BLOG POST DATA
     ==================================================================== */
  var posts = [
    {
      id: 1,
      title: "Java 21 Virtual Threads: Infinite Scaling Meets Database Bottlenecks",
      excerpt: "Virtual threads solve the thread-per-request scaling limit, but they shift the bottleneck directly to your relational database connection pool. Here is why it happens and how to fix it.",
      category: "Java",
      date: "2026-05-26",
      readTime: 3,
      content:
        '<p>With the release of Java 21, <strong>Virtual Threads (Project Loom)</strong> finally became a stable feature. For years, the standard Java web server model was "one thread per request." Because standard OS threads are heavy (consuming ~1MB of memory each), a typical Spring Boot application would cap out at a couple of hundred concurrent threads before running out of memory.</p>' +
        '<p>Virtual threads flip this script. They are lightweight, managed by the JVM, and you can comfortably spin up millions of them on a standard machine.</p>' +
        '<pre><code>// Spinning up 10,000 virtual threads is now trivial\ntry (var executor = Executors.newVirtualThreadPerTaskExecutor()) {\n    IntStream.range(0, 10_000).forEach(i -> {\n        executor.submit(() -> {\n            // Do some blocking I/O operation\n            performDatabaseQuery();\n        });\n    });\n}</code></pre>' +
        '<p>While this is a massive win for high-throughput applications, it creates a hidden, terrifying problem for your database.</p>' +
        '<h2>The Headache: Connection Pool Exhaustion</h2>' +
        '<p>When your web server could only handle 200 concurrent requests, your database connection pool (like <strong>HikariCP</strong>) only needed to handle a maximum of 200 connections.</p>' +
        '<p>But what happens when you enable virtual threads and suddenly 10,000 requests hit your server at the exact same time?</p>' +
        '<ol>' +
          '<li><strong>10,000 virtual threads</strong> are instantly created to handle the incoming requests.</li>' +
          '<li>All 10,000 threads execute your business logic and eventually ask HikariCP for a database connection.</li>' +
          '<li>Your database (e.g., PostgreSQL or MySQL) is physically incapable of handling 10,000 active connections without crashing.</li>' +
          '<li>HikariCP limits the active connections (usually defaulting to 10 or 20).</li>' +
          '<li>The remaining 9,980 virtual threads block, waiting in a queue for a connection to become available.</li>' +
        '</ol>' +
        '<p>Suddenly, your "infinitely scalable" virtual threads are bottlenecked by a tiny pipe leading to your database. If those queries take even slightly too long, HikariCP will throw a <code>ConnectionTimeoutException</code>, and your application will start dropping thousands of requests.</p>' +
        '<h2>How to Fix the Database Bottleneck</h2>' +
        '<p>Virtual threads shift the bottleneck from the <strong>compute layer</strong> to the <strong>data layer</strong>. Here is how you handle it:</p>' +
        '<h3>1. Implement Rate Limiting (Semaphores)</h3>' +
        '<p>Since you can no longer rely on Tomcat\'s thread pool to limit concurrency, you must limit it yourself. You can use a <code>Semaphore</code> to restrict how many virtual threads are allowed to execute database-heavy logic simultaneously.</p>' +
        '<pre><code>// Limit DB concurrency to 50\nprivate final Semaphore dbThrottle = new Semaphore(50);\n\npublic UserData getUser(String id) throws InterruptedException {\n    dbThrottle.acquire();\n    try {\n        return repository.findById(id);\n    } finally {\n        dbThrottle.release();\n    }\n}</code></pre>' +
        '<h3>2. Use a Database Connection Proxy</h3>' +
        '<p>If you are using PostgreSQL, you should run <strong>PgBouncer</strong> or <strong>Pgpool-II</strong> in front of your database. These proxies manage thousands of incoming connections from your virtual threads and multiplex them onto a smaller number of actual physical database connections.</p>' +
        '<h3>3. Caching (Redis / Memcached)</h3>' +
        '<p>If you are spinning up thousands of threads, odds are many of them are asking for the exact same data. Introduce a caching layer (like Redis) before the database. Virtual threads are perfect for Redis because they can block on the network call to the cache without consuming heavy OS resources.</p>' +
        '<h2>Conclusion</h2>' +
        '<p>Virtual threads are a paradigm shift for Java backend engineering. They eliminate the need for complex reactive programming (like WebFlux/RxJava) while providing massive throughput. Just remember: your web server might scale to infinity now, but your relational database still lives in the physical world.</p>',
    },
    {
      id: 2,
      title: "Virtual Threads vs. OS Threads in Java 21: A Deep Dive",
      excerpt: "Understand the fundamental architectural differences between OS-managed platform threads and JVM-managed virtual threads, and when to use each in your Java applications.",
      category: "Java",
      date: "2026-05-26",
      readTime: 3,
      content:
        '<p>Java 21 introduced virtual threads as a stable feature, but many developers still confuse them with platform (OS) threads. Understanding the distinction is critical for making the right concurrency decisions.</p>' +
        '<h2>Platform Threads: The Old Model</h2>' +
        '<p>A platform thread is a thin wrapper around an OS thread. The operating system kernel schedules it, allocates ~1MB of stack memory, and manages its lifecycle. Creating thousands of them is expensive and impractical.</p>' +
        '<pre><code>// Platform thread — one OS thread per task\nThread thread = new Thread(() -> {\n    // Heavy OS resource allocation\n    processRequest();\n});\nthread.start();</code></pre>' +
        '<p>This model served Java well for two decades, but it creates a hard ceiling: the number of concurrent requests your server handles is limited by how many OS threads you can afford.</p>' +
        '<h2>Virtual Threads: The New Model</h2>' +
        '<p>Virtual threads are managed entirely by the JVM. They are mounted onto a pool of <strong>carrier threads</strong> (platform threads), and the JVM handles scheduling. When a virtual thread blocks on I/O, the JVM unmounts it from the carrier, freeing the carrier for other work.</p>' +
        '<pre><code>// Virtual thread — lightweight, JVM-managed\nThread.startVirtualThread(() -> {\n    // Uses almost no OS resources when blocking\n    processRequest();\n});</code></pre>' +
        '<h2>Key Differences</h2>' +
        '<p><strong>Memory:</strong> A platform thread uses ~1MB of stack. A virtual thread starts at a few hundred bytes and grows on demand.</p>' +
        '<p><strong>Scheduling:</strong> OS threads are preemptively scheduled by the kernel. Virtual threads are cooperatively scheduled by the JVM — they yield when they hit a blocking operation.</p>' +
        '<p><strong>Scalability:</strong> You can run millions of virtual threads on a single JVM. Try that with platform threads and your process gets killed by the OOM killer.</p>' +
        '<h2>When NOT to Use Virtual Threads</h2>' +
        '<p>Virtual threads are not a universal replacement. CPU-bound tasks (heavy computation, number crunching) do not benefit from virtual threads because they never yield. For these, stick with a fixed-size platform thread pool sized to your CPU core count.</p>' +
        '<p>Also beware of <code>synchronized</code> blocks: they pin the virtual thread to its carrier, negating the scalability benefit. Prefer <code>ReentrantLock</code> instead.</p>' +
        '<h2>The Bottom Line</h2>' +
        '<p>Use virtual threads for I/O-bound work (HTTP calls, database queries, file reads). Use platform threads for CPU-bound work. The two models complement each other — they are not competitors.</p>',
    },
    {
      id: 3,
      title: "Java Upgrades Are Architectural Events",
      excerpt: "Upgrading Java is never just about the JVM. It exposes architectural weaknesses, hidden coupling, and long-standing assumptions.",
      category: "Java",
      date: "2026-03-29",
      readTime: 1,
      content:
        '<p>When teams talk about "upgrading Java," they usually mean bumping the version in a Dockerfile and fixing compile errors. The reality is far more brutal.</p>' +
        '<p>A Java upgrade forces you to confront every implicit assumption your codebase has accumulated. Internal APIs you relied on get removed. Reflection hacks that bypassed module boundaries stop working. Libraries that haven\'t been maintained in years suddenly fail at startup.</p>' +
        '<h2>The Real Cost</h2>' +
        '<p>The compilation errors are the easy part. The hard part is the behavioral changes: garbage collection tuning that worked on Java 11 produces completely different pause patterns on Java 21. Security defaults that were permissive become restrictive. Serialization formats shift subtly.</p>' +
        '<p>Every one of these is an <strong>architectural event</strong> — it reveals where your system was tightly coupled to implementation details rather than abstractions.</p>' +
        '<h2>What Upgrades Actually Test</h2>' +
        '<p>A Java upgrade is the best integration test you never wrote. It tests whether your dependency graph is maintainable, whether your build system is reproducible, and whether your team understands the difference between "it works" and "it works correctly."</p>' +
        '<p>If a Java upgrade terrifies your team, that fear is diagnostic. It means your architecture has accumulated technical debt that was invisible during normal development but becomes undeniable when the runtime shifts beneath you.</p>',
    },
    {
      id: 4,
      title: "When Microservices Become an Organizational Bug",
      excerpt: "Microservices don\'t fail at scale — teams do. A blunt look at how service boundaries expose complexity, slow delivery, and hide responsibility.",
      category: "Architecture",
      date: "2025-12-06",
      readTime: 2,
      content:
        '<p>The promise of microservices was independence: teams deploy independently, scale independently, fail independently. The reality at most organizations is the exact opposite.</p>' +
        '<h2>The Coordination Tax</h2>' +
        '<p>When you split a monolith into 40 services, you don\'t eliminate complexity — you distribute it. Every feature that touches more than one service now requires cross-team coordination, shared release schedules, and distributed debugging.</p>' +
        '<p>The monolith had one deploy pipeline. Now you have 40, each with its own CI/CD, monitoring, alerting, and on-call rotation. The operational overhead alone can consume an entire platform team.</p>' +
        '<h2>Service Boundaries Mirror Org Charts</h2>' +
        '<p>Conway\'s Law isn\'t a suggestion — it\'s a force of nature. Your service boundaries will eventually mirror your org chart, regardless of what your architecture diagram says. If two services need to change in lockstep for every feature, they aren\'t independent services. They\'re a distributed monolith with a network hop.</p>' +
        '<h2>When Microservices Work</h2>' +
        '<p>Microservices work when the boundaries are genuinely independent: different data, different release cadences, different scaling characteristics. A payment service that handles transactions is a legitimate microservice. A "user-preferences service" that every other service calls on every request is just a shared library with latency.</p>' +
        '<h2>The Honest Question</h2>' +
        '<p>Before splitting a monolith, ask: "Are we solving a technical scaling problem, or an organizational communication problem?" If it\'s the latter, no amount of Kubernetes will fix it.</p>',
    },
    {
      id: 5,
      title: "Code Coverage Is Not Test Quality",
      excerpt: "High coverage numbers often hide fragile systems. This post explains why meaningful tests look like in real backend services and why branches matter more than percentages.",
      category: "Quality",
      date: "2025-12-05",
      readTime: 2,
      content:
        '<p>Your CI pipeline reports 85% code coverage. Your team celebrates. Your production system crashes on a Saturday at 2 AM because a null check was missing in a code path that was technically "covered."</p>' +
        '<h2>What Coverage Actually Measures</h2>' +
        '<p>Code coverage measures which lines of code were <em>executed</em> during your test suite. It does not measure whether the behavior was <em>verified</em>. A test that calls a function and ignores the return value "covers" that code. It tests nothing.</p>' +
        '<pre><code>// This "test" gives you coverage with zero verification\n@Test\nvoid testGetUser() {\n    userService.getUser("123"); // No assertion\n}</code></pre>' +
        '<h2>Branch Coverage vs. Line Coverage</h2>' +
        '<p>Line coverage is almost meaningless. Branch coverage is slightly better — it tells you whether both sides of every conditional were exercised. But even branch coverage misses critical scenarios: what happens when the database is down? When the input is malformed? When the response is empty?</p>' +
        '<h2>What Matters Instead</h2>' +
        '<p>Focus on <strong>behavior coverage</strong>: for each business rule, is there a test that verifies the correct outcome and at least one test that verifies the error path? This is harder to measure automatically, which is exactly why teams fall back to line coverage percentages.</p>' +
        '<p>The best test suites I\'ve seen have moderate coverage numbers (60-75%) but every test is intentional, readable, and catches real bugs. The worst test suites I\'ve seen have 95% coverage and break every time you refactor a method signature.</p>',
    },
    {
      id: 6,
      title: "Cognitive Complexity Is a Smell, Not a Metric",
      excerpt: "Static analysis gives numbers. Production gives pain. This article explains why cognitive complexity always predicts expensive thinking.",
      category: "Code Quality",
      date: "2025-11-22",
      readTime: 2,
      content:
        '<p>SonarQube flags your method with a cognitive complexity score of 23. Your team\'s threshold is 15. Someone refactors the method by extracting three helper functions. The score drops to 8. The code is now <em>harder</em> to understand.</p>' +
        '<h2>The Problem with the Metric</h2>' +
        '<p>Cognitive complexity scores count nesting depth, boolean operators, and control flow breaks. These correlate with difficulty, but they don\'t <em>cause</em> difficulty. A deeply nested loop that processes a well-understood data structure might be perfectly readable. A flat sequence of ten method calls with side effects might be incomprehensible.</p>' +
        '<h2>Complexity as a Smell</h2>' +
        '<p>Treat cognitive complexity like a code smell: it tells you <em>where</em> to look, not <em>what</em> to do. A high score says "this method might be doing too much." The correct response is to read it and decide whether it\'s genuinely confusing or just algorithmically dense.</p>' +
        '<h2>The Real Measure</h2>' +
        '<p>The true test of cognitive complexity is this: can a new team member read this code and predict what it does without running it? If yes, the score is irrelevant. If no, no amount of extracting methods will fix the underlying problem — unclear intent.</p>' +
        '<p>Write code for the next reader, not for the linter. If the linter is happy but the next developer is confused, you optimized for the wrong audience.</p>',
    },
    {
      id: 7,
      title: "Why Most Backend APIs Are Designed Backwards",
      excerpt: "APIs often expose databases instead of intent. This post explains how backend engineers leak internal models, and how to design APIs that survive change.",
      category: "API Design",
      date: "2025-11-12",
      readTime: 2,
      content:
        '<p>Most backend APIs start the same way: a developer looks at the database schema and creates one REST endpoint per table. <code>GET /users</code>, <code>GET /orders</code>, <code>GET /products</code>. CRUD operations map directly to SQL operations. The API is "done" in a day.</p>' +
        '<p>Six months later, the frontend team needs a single endpoint that returns a user\'s recent orders with product thumbnails. There\'s no endpoint for that. So they make three API calls and stitch the data together in the browser.</p>' +
        '<h2>The Backwards Design</h2>' +
        '<p>Designing APIs from the database up is backwards. The database schema reflects storage concerns — normalization, indexing, referential integrity. The API should reflect <strong>use cases</strong> — what does the client need to accomplish?</p>' +
        '<h2>Intent-Driven API Design</h2>' +
        '<p>Start with the client\'s screen, not the database\'s schema. Ask: "What action is the user performing, and what data do they need to complete it?" Design endpoints around those actions.</p>' +
        '<pre><code>// Database-driven (backwards)\nGET /users/{id}\nGET /orders?userId={id}\nGET /products?ids={id1},{id2}\n\n// Intent-driven (forwards)\nGET /users/{id}/dashboard\n// Returns user info + recent orders + recommended products</code></pre>' +
        '<h2>Why This Matters</h2>' +
        '<p>Database-driven APIs are brittle. When you refactor the database schema (split a table, merge two tables, add a column), every client breaks. Intent-driven APIs are resilient — the endpoint contract stays the same even when the underlying storage changes completely.</p>' +
        '<p>Your API is a contract with the outside world. Your database is an implementation detail. Don\'t let the detail dictate the contract.</p>',
    },
    {
      id: 8,
      title: "Spring Boot Magic Ends at the First Incident",
      excerpt: "Auto-configuration helps — until it hides behavior. This post explains where Spring Boot shines, and where core Spring knowledge becomes mandatory.",
      category: "Spring",
      date: "2025-11-10",
      readTime: 2,
      content:
        '<p>Spring Boot is remarkable. You add <code>spring-boot-starter-web</code> to your dependencies, write a controller, hit run, and you have a fully functional web server. No XML, no boilerplate, no manual bean wiring.</p>' +
        '<p>Then production happens.</p>' +
        '<h2>When the Magic Breaks</h2>' +
        '<p>Auto-configuration works until it doesn\'t. When your application starts with 200 beans and you need to understand why one specific bean is being injected instead of another, the magic becomes an obstacle. You need to understand <code>@ConditionalOnMissingBean</code>, <code>@ConditionalOnProperty</code>, and the entire auto-configuration resolution order.</p>' +
        '<pre><code>// Why is MyCustomDataSource being ignored?\n// Because HikariAutoConfiguration already created one.\n// And its @ConditionalOnMissingBean(DataSource.class)\n// ran before your @Configuration class loaded.</code></pre>' +
        '<h2>The Spring Boot Contract</h2>' +
        '<p>Spring Boot gives you sensible defaults. In exchange, you accept that debugging those defaults requires understanding core Spring: the bean lifecycle, the application context hierarchy, property resolution order, and profile-based configuration.</p>' +
        '<h2>What to Learn First</h2>' +
        '<p>Before you need it in a crisis, understand these three things: how <code>@ComponentScan</code> discovers beans, how <code>@Autowired</code> resolves ambiguity (by type, then by qualifier, then by name), and how profiles and property sources layer on top of each other.</p>' +
        '<p>Spring Boot is a productivity tool, not a knowledge replacement. The moment your application does something unexpected, you\'re debugging Spring, not Spring Boot.</p>',
    },
    {
      id: 9,
      title: "JPA Is Not Slow — Your Mental Model Is",
      excerpt: "JPA doesn\'t randomly hit the database. It does exactly what you told it to do. This post fixes common ORM misconceptions at code level.",
      category: "Java",
      date: "2025-10-14",
      readTime: 2,
      content:
        '<p>Every few months, a blog post goes viral claiming JPA/Hibernate is slow and you should use raw SQL instead. The author usually demonstrates a query that generates 47 SQL statements for a simple list page. "See? JPA is broken."</p>' +
        '<p>No. JPA did exactly what the code asked it to do. The problem is always the developer\'s mental model.</p>' +
        '<h2>The N+1 Problem Is Not a JPA Problem</h2>' +
        '<p>The N+1 query problem happens when you load a list of entities and then lazily access a relationship on each one. JPA fires one query for the list and N queries for the relationships. This is not a bug — it\'s the documented behavior of <code>FetchType.LAZY</code>.</p>' +
        '<pre><code>// This generates N+1 queries — and it SHOULD\nList&lt;Order&gt; orders = orderRepository.findAll();\norders.forEach(o -> System.out.println(o.getCustomer().getName()));\n\n// Fix: use a JOIN FETCH\n@Query("SELECT o FROM Order o JOIN FETCH o.customer")\nList&lt;Order&gt; findAllWithCustomer();</code></pre>' +
        '<h2>Eager Loading Is Not the Fix</h2>' +
        '<p>Setting everything to <code>FetchType.EAGER</code> is worse than N+1. Now every query loads the entire object graph regardless of whether you need it. A simple "get order status" query now joins five tables.</p>' +
        '<h2>The Right Mental Model</h2>' +
        '<p>Think of JPA entities as <strong>persistence boundaries</strong>, not domain objects. Each entity manages its own lifecycle with the database. When you need data from multiple entities in a single operation, write a query that fetches exactly what you need — <code>JOIN FETCH</code>, projections, or native queries.</p>' +
        '<p>JPA is not slow. Unintentional SQL generation is slow. And that\'s always a code problem, not a framework problem.</p>',
    },
    {
      id: 10,
      title: "Why Java 8 Still Runs Half the World",
      excerpt: "New LTS versions exist — yet Java 8 refuses to die. This post explains enterprise risk, tooling inertia, and why upgrades are rarely just technical.",
      category: "Java",
      date: "2025-10-03",
      readTime: 2,
      content:
        '<p>Java 21 has been out for over a year. Java 17 has been out for four. Yet the most widely deployed Java version in production today is still Java 8, released in 2014.</p>' +
        '<p>This isn\'t laziness. It\'s rational risk management — and understanding why is more important than judging it.</p>' +
        '<h2>The Enterprise Risk Equation</h2>' +
        '<p>For a bank running 2,000 microservices on Java 8, upgrading to Java 21 means: revalidating every dependency, retraining the CI pipeline, recertifying for compliance, regression testing across all services, and coordinating the rollout across dozens of teams.</p>' +
        '<p>The cost of upgrading is concrete, measurable, and immediate. The benefit of upgrading (better GC, virtual threads, pattern matching) is abstract and deferred. In enterprise decision-making, concrete costs always outweigh abstract benefits.</p>' +
        '<h2>Tooling Inertia</h2>' +
        '<p>Java 8 has the most mature ecosystem of any Java version. Every library, framework, and tool works with it. Every developer knows it. Every StackOverflow answer targets it. Moving to a newer version means navigating module system changes (<code>--add-opens</code>), deprecated API removals, and behavioral differences that only surface under load.</p>' +
        '<h2>The Honest Path Forward</h2>' +
        '<p>If you\'re starting a new project, use the latest LTS. If you\'re maintaining an existing system on Java 8, the upgrade should be planned as a multi-quarter initiative with dedicated engineering time — not a "we\'ll do it when we have bandwidth" afterthought. The longer you wait, the more painful it gets.</p>',
    },
    {
      id: 11,
      title: "Annotations Are Not Magic",
      excerpt: "@Component doesn\'t register anything by itself. This article walks through what Spring actually does — from scanning to bean instantiation.",
      category: "Spring Core",
      date: "2025-09-19",
      readTime: 2,
      content:
        '<p>Put <code>@Component</code> on a class. Spring "magically" creates an instance and injects it where needed. But what actually happens between annotation and running code?</p>' +
        '<h2>Step 1: Classpath Scanning</h2>' +
        '<p>When your application starts, <code>@ComponentScan</code> (triggered by <code>@SpringBootApplication</code>) walks the classpath looking for classes annotated with <code>@Component</code>, <code>@Service</code>, <code>@Repository</code>, or <code>@Controller</code>. It reads the bytecode (via ASM, not reflection) to find these annotations without loading the classes.</p>' +
        '<h2>Step 2: Bean Definition Registration</h2>' +
        '<p>For each annotated class, Spring creates a <code>BeanDefinition</code> — a metadata object describing the class, its scope, its dependencies, and its initialization method. No instances are created yet. This is just a blueprint.</p>' +
        '<h2>Step 3: Bean Instantiation and Wiring</h2>' +
        '<p>The <code>ApplicationContext</code> iterates through all <code>BeanDefinition</code>s and instantiates them in dependency order. If Bean A depends on Bean B, Bean B is created first. Constructor injection, field injection, and setter injection all happen during this phase.</p>' +
        '<pre><code>// What you write\n@Service\npublic class OrderService {\n    private final OrderRepository repo;\n    public OrderService(OrderRepository repo) {\n        this.repo = repo;\n    }\n}\n\n// What Spring does (simplified)\nOrderRepository repo = context.getBean(OrderRepository.class);\nOrderService service = new OrderService(repo);\ncontext.registerSingleton("orderService", service);</code></pre>' +
        '<h2>Why This Matters</h2>' +
        '<p>When you understand this pipeline, debugging becomes tractable. "Bean not found" means the component wasn\'t scanned — check your package structure. "Ambiguous dependency" means multiple beans match — add a <code>@Qualifier</code>. "Circular dependency" means two beans need each other at construction time — restructure one to use setter injection.</p>' +
        '<p>Annotations are not magic. They are metadata that a well-defined framework pipeline reads and acts on. Understanding the pipeline makes you a Spring developer, not just a Spring user.</p>',
    },
    {
      id: 12,
      title: "Why 90% Coverage Still Means Zero Confidence",
      excerpt: "Coverage measures execution, not correctness. This post explains why high-coverage systems still fear releases, and how to test behavior instead of lines.",
      category: "Testing",
      date: "2025-09-05",
      readTime: 2,
      content:
        '<p>Your team achieved 90% code coverage. Your manager is happy. Your QA team is still finding bugs in production every sprint. What went wrong?</p>' +
        '<h2>The Coverage Illusion</h2>' +
        '<p>Coverage tells you which lines of code were <em>executed</em> during tests. It says nothing about what was <em>verified</em>. A test that calls a method without asserting anything gives you coverage with zero confidence. A test that only checks the happy path gives you coverage while leaving every error path untested.</p>' +
        '<h2>The Real Problem: Assertion-Free Tests</h2>' +
        '<pre><code>// 100% coverage, 0% confidence\n@Test\nvoid testProcessOrder() {\n    Order order = new Order("item-1", 2);\n    orderService.process(order);\n    // No assertions — we just ran the code\n}\n\n// Lower coverage, actual confidence\n@Test\nvoid testProcessOrder_appliesDiscount() {\n    Order order = new Order("item-1", 10);\n    OrderResult result = orderService.process(order);\n    assertEquals(new BigDecimal("90.00"), result.getTotal());\n    assertTrue(result.isDiscountApplied());\n}</code></pre>' +
        '<h2>Behavior Over Lines</h2>' +
        '<p>Stop measuring line coverage. Start measuring <strong>behavior coverage</strong>: for every business rule, is there at least one test that verifies the correct behavior and one that verifies the failure mode?</p>' +
        '<p>A system with 60% line coverage but 100% behavior coverage on critical paths is infinitely more reliable than a system with 95% line coverage and no meaningful assertions.</p>' +
        '<p>Coverage is a compass, not a destination. Use it to find untested areas, then write tests that actually verify something.</p>',
    },
    {
      id: 13,
      title: "Mocking Everything Is a Design Smell",
      excerpt: "If every dependency needs mocking, the design is already lying. This post explains when mocks help — and when they hurt.",
      category: "Testing",
      date: "2025-08-21",
      readTime: 2,
      content:
        '<p>You\'re writing a test for a service method. It has six dependencies. You mock all six. Your test is now 40 lines of <code>when(...).thenReturn(...)</code> and 5 lines of actual logic. The test passes. You feel nothing.</p>' +
        '<h2>What Mocking Actually Tests</h2>' +
        '<p>When you mock everything, your test verifies that your code calls the right methods on the right mocks in the right order. It does not verify that the system works. Change the implementation (same behavior, different method calls), and the test breaks. This is the hallmark of a brittle test.</p>' +
        '<h2>When Mocks Are Appropriate</h2>' +
        '<p>Mock external boundaries: HTTP clients, message queues, third-party APIs. These are systems you don\'t control and can\'t run in a test environment reliably. Everything else — your own repositories, services, utilities — should use real implementations or in-memory fakes.</p>' +
        '<pre><code>// Over-mocked: tests implementation, not behavior\n@Mock UserRepository userRepo;\n@Mock EmailService emailService;\n@Mock AuditLogger auditLogger;\n\n// Better: use real implementations where possible\n@Autowired UserRepository userRepo;  // H2 in-memory DB\n@Mock EmailService emailService;      // External system — mock it\n// AuditLogger writes to the same DB — let it run</code></pre>' +
        '<h2>The Design Smell</h2>' +
        '<p>If a class requires six mocks to test, the class has too many responsibilities. The mocking pain is a signal — it\'s telling you to refactor, not to write more <code>when().thenReturn()</code> chains.</p>' +
        '<p>The best unit test is one where you construct the object with real dependencies, call a method, and assert the result. If you can\'t do that, the problem isn\'t your test — it\'s your design.</p>',
    },
    {
      id: 14,
      title: "DDoS Is Not a Network Problem",
      excerpt: "Most DDoS discussions stop at bandwidth. This post explains why application asymmetry and state are the real vulnerabilities.",
      category: "Systems",
      date: "2025-08-07",
      readTime: 2,
      content:
        '<p>When people hear "DDoS," they think of bandwidth. Massive botnets flooding a server with traffic until the pipe is full. While volumetric attacks exist, the most devastating DDoS attacks target something far more subtle: <strong>application asymmetry</strong>.</p>' +
        '<h2>The Asymmetry Problem</h2>' +
        '<p>Consider a search endpoint. The attacker sends a single HTTP request: <code>GET /search?q=a</code>. Your server receives this request, parses it, queries a database, sorts results, serializes the response, and sends it back. The attacker\'s cost: one packet. Your cost: CPU cycles, database connections, memory allocation, and network bandwidth.</p>' +
        '<p>This is application-layer DDoS. The attacker doesn\'t need to overwhelm your bandwidth — they need to overwhelm your <em>compute</em>.</p>' +
        '<h2>State Is the Real Target</h2>' +
        '<p>Every connection your server maintains is a piece of state: TCP connections, TLS sessions, HTTP/2 streams, WebSocket connections. Each one consumes memory and file descriptors. An attacker who opens thousands of connections and sends data slowly (Slowloris attack) can exhaust your server without sending significant bandwidth.</p>' +
        '<h2>Defense Is Layered</h2>' +
        '<p>Rate limiting at the application layer is necessary but not sufficient. You need:</p>' +
        '<ol>' +
          '<li><strong>Edge protection</strong> (Cloudflare, AWS Shield) to absorb volumetric attacks</li>' +
          '<li><strong>Connection limits</strong> per IP at the load balancer</li>' +
          '<li><strong>Request timeouts</strong> to kill slow connections</li>' +
          '<li><strong>Application-level rate limiting</strong> based on user identity, not just IP</li>' +
        '</ol>' +
        '<p>The best defense is designing your application to be <strong>symmetric</strong>: if a request is cheap to send, make it cheap to process. If processing is expensive, require authentication first.</p>',
    },
    {
      id: 15,
      title: "Being a Senior Engineer Is About Saying No",
      excerpt: "Senior engineers don\'t write more code. They prevent unnecessary code from existing. This post explains why judgment beats speed.",
      category: "Career",
      date: "2025-07-15",
      readTime: 3,
      content:
        '<p>Junior engineers are measured by what they build. Senior engineers are measured by what they prevent.</p>' +
        '<p>This sounds counterintuitive, but it\'s the most important shift in an engineering career. The ability to say "no" — to a feature, to a technology choice, to a premature abstraction — is the highest-leverage skill a senior engineer develops.</p>' +
        '<h2>The Cost of Yes</h2>' +
        '<p>Every line of code is a liability. It must be read, understood, tested, maintained, debugged, and eventually rewritten. Saying "yes" to a feature means saying "yes" to all of that ongoing cost. A senior engineer asks: "Is the value of this feature worth its lifetime maintenance cost?"</p>' +
        '<h2>What "No" Looks Like</h2>' +
        '<p>"We don\'t need a microservices architecture for a team of four."</p>' +
        '<p>"We don\'t need a custom ORM when JPA solves this problem."</p>' +
        '<p>"We don\'t need to support three database vendors when we\'ll only ever use PostgreSQL."</p>' +
        '<p>"We don\'t need real-time updates for data that changes once a day."</p>' +
        '<p>Each of these "no"s saves weeks to months of engineering time. Not by writing code faster, but by not writing code at all.</p>' +
        '<h2>Judgment Over Speed</h2>' +
        '<p>A junior engineer can write a caching layer in two days. A senior engineer asks whether caching is needed at all, and saves two days of implementation, plus the ongoing cost of cache invalidation bugs, stale data, and operational complexity.</p>' +
        '<p>Speed without judgment produces technical debt. Judgment without speed produces nothing. The senior engineer\'s job is to apply both — and know when each one matters more.</p>' +
        '<h2>The Hardest Part</h2>' +
        '<p>Saying "no" requires confidence, context, and political skill. It means disagreeing with stakeholders who want features, with peers who want to try new technologies, and with yourself when you\'re excited about an elegant solution to a problem that doesn\'t exist.</p>' +
        '<p>The best code is the code that was never written.</p>',
    },
    {
      id: 16,
      title: "Clean Code Alone Doesn\'t Survive Production",
      excerpt: "Clean code is readable. Production code is negotiable under pressure. This post explores the gap between theory and reality.",
      category: "Thinking",
      date: "2025-07-04",
      readTime: 2,
      content:
        '<p>"Clean code" has become a religion. Developers debate naming conventions, method lengths, and abstraction layers with the fervor of theological disputes. Meanwhile, production systems are held together by code that violates every "clean code" principle — and they work.</p>' +
        '<h2>The Gap Between Theory and Reality</h2>' +
        '<p>In theory, every function should do one thing. In practice, the function that handles your payment webhook does twelve things because that\'s what the business requires: validate the signature, parse the payload, check for duplicates, update the order, notify the warehouse, send a confirmation email, update analytics, and handle six different error cases.</p>' +
        '<p>You could extract twelve helper functions. You\'d have "clean code." You\'d also have a flow that\'s impossible to understand without jumping between thirteen files.</p>' +
        '<h2>Readability vs. Navigability</h2>' +
        '<p>Clean code optimizes for readability of individual functions. Production code should optimize for navigability of entire flows. When an incident happens at 3 AM, the on-call engineer needs to trace a request from entry to exit without opening twenty files. Sometimes, a long function with clear comments is more navigable than a short function that delegates to six abstractions.</p>' +
        '<h2>The Balance</h2>' +
        '<p>Write clean code by default. But don\'t sacrifice clarity of the whole for elegance of the parts. The goal isn\'t beautiful code — it\'s code that the next developer can understand, debug, and modify under pressure.</p>' +
        '<p>Production doesn\'t care about your method length. It cares about whether the on-call engineer can find and fix the bug before the SLA expires.</p>',
    },
  ];

  /* ====================================================================
     CATEGORIES (derived from data)
     ==================================================================== */
  var categories = ["All"];
  posts.forEach(function (p) {
    if (categories.indexOf(p.category) === -1) categories.push(p.category);
  });

  /* ====================================================================
     DOM REFERENCES
     ==================================================================== */
  var gridEl = document.getElementById("blog-grid");
  var filtersEl = document.getElementById("blog-filters");
  var searchEl = document.getElementById("blog-search");

  var activeCategory = "All";

  /* ====================================================================
     HELPERS
     ==================================================================== */
  function formatDate(dateStr) {
    var d = new Date(dateStr + "T00:00:00");
    var days = ["SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT"];
    var months = ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"];
    return days[d.getDay()] + " " + months[d.getMonth()] + " " + String(d.getDate()).padStart(2, "0") + " " + d.getFullYear();
  }

  function formatModalDate(dateStr) {
    var d = new Date(dateStr + "T00:00:00");
    return String(d.getMonth() + 1).padStart(2, "0") + "/" + String(d.getDate()).padStart(2, "0") + "/" + d.getFullYear();
  }

  function escapeHtml(str) {
    var div = document.createElement("div");
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
  }

  /* ====================================================================
     RENDER FILTERS
     ==================================================================== */
  function renderFilters() {
    filtersEl.innerHTML = categories
      .map(function (cat) {
        var isActive = cat === activeCategory;
        return (
          '<button class="blog-filter-btn' +
          (isActive ? " is-active" : "") +
          '" role="tab" aria-selected="' +
          isActive +
          '" data-category="' +
          escapeHtml(cat) +
          '">' +
          escapeHtml(cat) +
          "</button>"
        );
      })
      .join("");
  }

  /* ====================================================================
     RENDER GRID
     ==================================================================== */
  function getFilteredPosts() {
    var query = (searchEl.value || "").toLowerCase().trim();
    return posts.filter(function (p) {
      var matchCategory = activeCategory === "All" || p.category === activeCategory;
      var matchSearch =
        !query ||
        p.title.toLowerCase().indexOf(query) !== -1 ||
        p.excerpt.toLowerCase().indexOf(query) !== -1 ||
        p.category.toLowerCase().indexOf(query) !== -1;
      return matchCategory && matchSearch;
    });
  }

  function renderGrid() {
    var filtered = getFilteredPosts();

    if (filtered.length === 0) {
      gridEl.innerHTML =
        '<div class="blog-empty">No articles match your search.</div>';
      return;
    }

    gridEl.innerHTML = filtered
      .map(function (p) {
        return (
          '<article class="blog-card" data-post-id="' + p.id + '" tabindex="0">' +
            '<div class="blog-card-top">' +
              '<span class="blog-card-meta">' + formatDate(p.date) + '</span>' +
              '<span class="blog-card-meta blog-card-meta--sep">&middot;</span>' +
              '<span class="blog-card-meta">' + p.readTime + ' MIN READ</span>' +
              '<span class="blog-card-tag">' + escapeHtml(p.category.toUpperCase()) + '</span>' +
            '</div>' +
            '<h3 class="blog-card-title">' + escapeHtml(p.title) + '</h3>' +
            '<p class="blog-card-excerpt">' + escapeHtml(p.excerpt) + '</p>' +
            '<span class="blog-card-link">Read <span class="blog-card-arrow">&rarr;</span></span>' +
          '</article>'
        );
      })
      .join("");
  }

  /* ====================================================================
     MODAL
     ==================================================================== */
  var isModalOpen = false;
  var escHandler = null;

  var MODAL_HTML =
    '<div class="blog-modal" role="dialog" aria-modal="true" aria-labelledby="blog-modal-title">' +
      '<div class="blog-modal-header">' +
        '<div class="blog-modal-header-left">' +
          '<span class="blog-modal-tag" id="blog-modal-tag"></span>' +
          '<span class="blog-modal-date" id="blog-modal-date"></span>' +
        '</div>' +
        '<button class="blog-modal-close" id="blog-modal-close" aria-label="Close article" type="button">' +
          '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
            '<line x1="18" y1="6" x2="6" y2="18"/>' +
            '<line x1="6" y1="6" x2="18" y2="18"/>' +
          '</svg>' +
        '</button>' +
      '</div>' +
      '<div class="blog-modal-progress-track">' +
        '<div class="blog-modal-progress-bar" id="blog-modal-progress"></div>' +
      '</div>' +
      '<div class="blog-modal-body" id="blog-modal-body">' +
        '<h1 class="blog-modal-title" id="blog-modal-title"></h1>' +
        '<div class="blog-modal-content" id="blog-modal-content"></div>' +
      '</div>' +
    '</div>';

  function openModal(postId) {
    var post = posts.find(function (p) { return p.id === postId; });
    if (!post || isModalOpen) return;

    var overlay = document.createElement("div");
    overlay.className = "blog-modal-overlay is-open";
    overlay.id = "blog-modal-overlay";
    overlay.setAttribute("aria-hidden", "false");
    overlay.innerHTML = MODAL_HTML;

    overlay.querySelector("#blog-modal-tag").textContent = post.category.toUpperCase();
    overlay.querySelector("#blog-modal-date").textContent = formatModalDate(post.date);
    overlay.querySelector("#blog-modal-title").textContent = post.title;
    overlay.querySelector("#blog-modal-content").innerHTML = post.content;

    document.body.appendChild(overlay);
    document.body.style.overflow = "hidden";
    isModalOpen = true;

    var body = overlay.querySelector("#blog-modal-body");
    var bar = overlay.querySelector("#blog-modal-progress");
    body.scrollTop = 0;

    body.addEventListener("scroll", function () {
      requestAnimationFrame(function () {
        var scrollTop = body.scrollTop;
        var scrollable = body.scrollHeight - body.clientHeight;
        var pct = scrollable > 0 ? (scrollTop / scrollable) * 100 : 0;
        bar.style.width = pct + "%";
      });
    }, { passive: true });

    overlay.querySelector("#blog-modal-close").addEventListener("click", closeModal);
    overlay.addEventListener("click", function (e) {
      if (e.target === overlay) closeModal();
    });

    escHandler = function (e) {
      if (e.key === "Escape") closeModal();
    };
    document.addEventListener("keydown", escHandler);
  }

  function closeModal() {
    if (!isModalOpen) return;
    isModalOpen = false;

    var overlay = document.getElementById("blog-modal-overlay");
    if (overlay && overlay.parentNode) {
      overlay.parentNode.removeChild(overlay);
    }
    document.body.style.overflow = "";

    if (escHandler) {
      document.removeEventListener("keydown", escHandler);
      escHandler = null;
    }
  }

  /* ====================================================================
     EVENT LISTENERS
     ==================================================================== */

  // Filter clicks
  filtersEl.addEventListener("click", function (e) {
    var btn = e.target.closest(".blog-filter-btn");
    if (!btn) return;
    activeCategory = btn.getAttribute("data-category");
    renderFilters();
    renderGrid();
  });

  // Search
  searchEl.addEventListener("input", function () {
    renderGrid();
  });

  // Card clicks — open modal
  gridEl.addEventListener("click", function (e) {
    var card = e.target.closest(".blog-card");
    if (!card) return;
    var postId = parseInt(card.getAttribute("data-post-id"), 10);
    openModal(postId);
  });

  // Card keyboard — Enter/Space opens modal
  gridEl.addEventListener("keydown", function (e) {
    if (e.key !== "Enter" && e.key !== " ") return;
    var card = e.target.closest(".blog-card");
    if (!card) return;
    e.preventDefault();
    var postId = parseInt(card.getAttribute("data-post-id"), 10);
    openModal(postId);
  });

  /* ====================================================================
     INIT
     ==================================================================== */
  renderFilters();
  renderGrid();
})();

/* ==========================================================================
   Text reveals (uses revealText from text-reveal.js)
   ========================================================================== */
(function () {
  "use strict";

  if (typeof window.revealText !== "function") return;

  var REVEALS = [
    [".blog-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
  ];

  REVEALS.forEach(function (entry) {
    window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
  });
})();
