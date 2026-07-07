/* ==========================================================================
   data.js — In-memory bilingual mock data + CRUD helpers
   ========================================================================== */

var AdminData = (function () {
  "use strict";

  var _id = 100;
  function nextId() { return String(++_id); }

  /* ---------- PROJECTS ---------- */
  var projects = [
    {
      id: "1", title_en: "Portfolio Website", title_fa: "وب‌سایت پورتفولیو",
      subtitle_en: "Personal portfolio with dark/light theme", subtitle_fa: "پورتفولیو شخصی با تم تیره/روشن",
      description_en: "A fully responsive portfolio website built with vanilla HTML, CSS and JavaScript featuring theme switching and bilingual support.",
      description_fa: "یک وب‌سایت پورتفولیو کاملاً ریسپانسیو ساخته شده با HTML، CSS و جاوااسکریپت خالص با قابلیت تغییر تم و پشتیبانی دو زبانه.",
      techs: ["HTML", "CSS", "JavaScript"], demoUrl: "https://example.com", githubUrl: "https://github.com/mousaamiri/portfolio", status: "published"
    },
    {
      id: "2", title_en: "E-Commerce Platform", title_fa: "پلتفرم فروشگاهی",
      subtitle_en: "Full-stack online store", subtitle_fa: "فروشگاه آنلاین فول‌استک",
      description_en: "A complete e-commerce solution with product management, cart, checkout and payment integration.",
      description_fa: "یک راه‌حل کامل تجارت الکترونیک با مدیریت محصول، سبد خرید، تسویه‌حساب و یکپارچه‌سازی پرداخت.",
      techs: ["React", "Node.js", "MongoDB", "Stripe"], demoUrl: "", githubUrl: "https://github.com/mousaamiri/ecommerce", status: "published"
    },
    {
      id: "3", title_en: "Task Manager App", title_fa: "اپلیکیشن مدیریت وظایف",
      subtitle_en: "Kanban-style task management", subtitle_fa: "مدیریت وظایف به سبک کانبان",
      description_en: "A drag-and-drop task manager with boards, lists and cards inspired by Trello.",
      description_fa: "یک مدیر وظایف کشیدن و رها کردن با بردها، لیست‌ها و کارت‌ها الهام‌گرفته از ترلو.",
      techs: ["Vue.js", "Firebase", "Tailwind CSS"], demoUrl: "https://example.com/tasks", githubUrl: "", status: "published"
    },
    {
      id: "4", title_en: "Weather Dashboard", title_fa: "داشبورد آب‌وهوا",
      subtitle_en: "Real-time weather data visualization", subtitle_fa: "تجسم داده‌های آب‌وهوای بلادرنگ",
      description_en: "A dashboard showing current weather, forecasts and historical data with interactive charts.",
      description_fa: "داشبوردی که آب‌وهوای فعلی، پیش‌بینی‌ها و داده‌های تاریخی را با نمودارهای تعاملی نشان می‌دهد.",
      techs: ["TypeScript", "D3.js", "OpenWeather API"], demoUrl: "", githubUrl: "", status: "draft"
    }
  ];

  /* ---------- ARTICLES ---------- */
  var articles = [
    {
      id: "1", title_en: "Getting Started with CSS Grid", title_fa: "شروع کار با CSS Grid",
      excerpt_en: "Learn the fundamentals of CSS Grid layout and build complex responsive layouts with ease.",
      excerpt_fa: "اصول چیدمان CSS Grid را بیاموزید و به‌راحتی چیدمان‌های ریسپانسیو پیچیده بسازید.",
      body_en: "CSS Grid is a powerful layout system available in CSS...",
      body_fa: "CSS Grid یک سیستم چیدمان قدرتمند در CSS است...",
      category: "CSS", tags: ["css", "layout", "grid"], publishDate: "2025-03-15", readTime: 8, status: "published"
    },
    {
      id: "2", title_en: "JavaScript Async Patterns", title_fa: "الگوهای ناهمزمان جاوااسکریپت",
      excerpt_en: "Deep dive into callbacks, promises, and async/await patterns in modern JavaScript.",
      excerpt_fa: "بررسی عمیق callback‌ها، promise‌ها و الگوهای async/await در جاوااسکریپت مدرن.",
      body_en: "Understanding asynchronous JavaScript is essential...",
      body_fa: "درک جاوااسکریپت ناهمزمان ضروری است...",
      category: "JavaScript", tags: ["javascript", "async", "promises"], publishDate: "2025-04-02", readTime: 12, status: "published"
    },
    {
      id: "3", title_en: "Building Accessible Forms", title_fa: "ساخت فرم‌های دسترس‌پذیر",
      excerpt_en: "A comprehensive guide to creating web forms that work for everyone.",
      excerpt_fa: "راهنمای جامع ساخت فرم‌های وب که برای همه کار می‌کند.",
      body_en: "Accessibility in forms is often overlooked...",
      body_fa: "دسترس‌پذیری در فرم‌ها اغلب نادیده گرفته می‌شود...",
      category: "Accessibility", tags: ["a11y", "html", "forms"], publishDate: "2025-01-20", readTime: 10, status: "draft"
    }
  ];

  /* ---------- EXPERIENCES ---------- */
  var experiences = [
    {
      id: "1", jobTitle_en: "Senior Frontend Developer", jobTitle_fa: "توسعه‌دهنده ارشد فرانت‌اند",
      company_en: "Tech Solutions Inc.", company_fa: "شرکت فناوری راهکارها",
      description_en: "Led the frontend team in building a large-scale SPA using React and TypeScript. Mentored junior developers and established coding standards.",
      description_fa: "رهبری تیم فرانت‌اند در ساخت یک SPA در مقیاس بزرگ با استفاده از React و TypeScript. آموزش توسعه‌دهندگان جونیور و تعیین استانداردهای کدنویسی.",
      startDate: "2023-01", endDate: "", isCurrent: true
    },
    {
      id: "2", jobTitle_en: "Frontend Developer", jobTitle_fa: "توسعه‌دهنده فرانت‌اند",
      company_en: "Creative Agency", company_fa: "آژانس خلاقیت",
      description_en: "Developed responsive websites and web applications for various clients using modern web technologies.",
      description_fa: "توسعه وب‌سایت‌ها و برنامه‌های وب ریسپانسیو برای مشتریان مختلف با استفاده از فناوری‌های مدرن وب.",
      startDate: "2021-06", endDate: "2022-12", isCurrent: false
    },
    {
      id: "3", jobTitle_en: "Junior Web Developer", jobTitle_fa: "توسعه‌دهنده وب جونیور",
      company_en: "Startup Hub", company_fa: "مرکز استارتاپ",
      description_en: "Built and maintained company websites. Collaborated with designers to implement pixel-perfect UI components.",
      description_fa: "ساخت و نگهداری وب‌سایت‌های شرکت. همکاری با طراحان برای پیاده‌سازی کامپوننت‌های UI پیکسل‌پرفکت.",
      startDate: "2020-01", endDate: "2021-05", isCurrent: false
    }
  ];

  /* ---------- EDUCATION ---------- */
  var education = [
    {
      id: "1", institution_en: "University of Tehran", institution_fa: "دانشگاه تهران",
      degree_en: "Bachelor of Computer Engineering", degree_fa: "کارشناسی مهندسی کامپیوتر",
      description_en: "Focused on software engineering and web technologies. Dean's list multiple semesters.",
      description_fa: "تمرکز بر مهندسی نرم‌افزار و فناوری‌های وب. لیست برترین‌ها در چندین ترم.",
      startYear: "2016", endYear: "2020", score: "18.5/20"
    },
    {
      id: "2", institution_en: "Sharif University of Technology", institution_fa: "دانشگاه صنعتی شریف",
      degree_en: "Master of Software Engineering", degree_fa: "کارشناسی ارشد مهندسی نرم‌افزار",
      description_en: "Research on web performance optimization and progressive web applications.",
      description_fa: "تحقیق در زمینه بهینه‌سازی عملکرد وب و برنامه‌های وب پیش‌رونده.",
      startYear: "2020", endYear: "2023", score: "19/20"
    }
  ];

  /* ---------- SKILLS ---------- */
  var skills = [
    { id: "1", name_en: "JavaScript", name_fa: "جاوااسکریپت", level: 95, category: "Frontend" },
    { id: "2", name_en: "TypeScript", name_fa: "تایپ‌اسکریپت", level: 88, category: "Frontend" },
    { id: "3", name_en: "React", name_fa: "ری‌اکت", level: 90, category: "Frontend" },
    { id: "4", name_en: "Vue.js", name_fa: "ویو جی‌اس", level: 80, category: "Frontend" },
    { id: "5", name_en: "CSS / Sass", name_fa: "سی‌اس‌اس / ساس", level: 92, category: "Frontend" },
    { id: "6", name_en: "Node.js", name_fa: "نود جی‌اس", level: 85, category: "Backend" },
    { id: "7", name_en: "Python", name_fa: "پایتون", level: 75, category: "Backend" },
    { id: "8", name_en: "MongoDB", name_fa: "مونگو‌دی‌بی", level: 78, category: "Backend" },
    { id: "9", name_en: "Git", name_fa: "گیت", level: 90, category: "Tools" },
    { id: "10", name_en: "Figma", name_fa: "فیگما", level: 70, category: "Design" }
  ];

  /* ---------- TESTIMONIALS ---------- */
  var testimonials = [
    {
      id: "1", name_en: "Ali Rezaei", name_fa: "علی رضایی",
      role_en: "CTO at Tech Solutions", role_fa: "مدیر فنی شرکت فناوری راهکارها",
      quote_en: "An exceptional developer who consistently delivers high-quality work. His attention to detail and problem-solving skills are outstanding.",
      quote_fa: "یک توسعه‌دهنده استثنایی که به‌طور مداوم کار باکیفیت ارائه می‌دهد. توجه او به جزئیات و مهارت‌های حل مسئله برجسته است."
    },
    {
      id: "2", name_en: "Sara Mohammadi", name_fa: "سارا محمدی",
      role_en: "Product Manager at Creative Agency", role_fa: "مدیر محصول در آژانس خلاقیت",
      quote_en: "Working with Mousa was a great experience. He understood our requirements quickly and delivered beyond expectations.",
      quote_fa: "کار با موسی تجربه‌ای عالی بود. او نیازمندی‌های ما را سریع درک کرد و فراتر از انتظارات عمل کرد."
    },
    {
      id: "3", name_en: "Mehdi Karimi", name_fa: "مهدی کریمی",
      role_en: "Lead Designer at Digital Studio", role_fa: "طراح ارشد در استودیو دیجیتال",
      quote_en: "Mousa has an excellent eye for design implementation. He translates mockups into pixel-perfect interfaces effortlessly.",
      quote_fa: "موسی چشم عالی برای پیاده‌سازی طراحی دارد. او ماکاپ‌ها را بدون زحمت به رابط‌های پیکسل‌پرفکت تبدیل می‌کند."
    }
  ];

  /* ---------- MESSAGES ---------- */
  var messages = [
    {
      id: "1", name: "John Smith", email: "john@example.com",
      subject: "Project Collaboration", message: "Hi, I'd like to discuss a potential project collaboration. Are you available for a call this week?",
      interest: "collaboration", date: "2025-04-15", isRead: false
    },
    {
      id: "2", name: "Emily Chen", email: "emily@startup.io",
      subject: "Job Opportunity", message: "We are looking for a senior frontend developer to join our team. Your portfolio is impressive and we'd love to chat.",
      interest: "hiring", date: "2025-04-10", isRead: true
    },
    {
      id: "3", name: "Ahmad Hosseini", email: "ahmad@company.ir",
      subject: "Website Redesign", message: "Our company needs a complete website redesign. Could you provide a quote and timeline?",
      interest: "project", date: "2025-04-08", isRead: false
    },
    {
      id: "4", name: "Lisa Park", email: "lisa@design.co",
      subject: "Great Portfolio!", message: "Just wanted to say your portfolio is beautifully designed. The dark mode implementation is particularly impressive!",
      interest: "feedback", date: "2025-03-28", isRead: true
    }
  ];

  /* =========================================================
     CRUD HELPERS (factory)
     ========================================================= */

  function createStore(arr) {
    return {
      getAll: function (search, searchKeys) {
        if (!search) return arr.slice();
        var q = search.toLowerCase();
        return arr.filter(function (item) {
          return searchKeys.some(function (k) {
            return item[k] && String(item[k]).toLowerCase().indexOf(q) !== -1;
          });
        });
      },
      getById: function (id) {
        for (var i = 0; i < arr.length; i++) { if (arr[i].id === String(id)) return arr[i]; }
        return null;
      },
      add: function (data) {
        var item = Object.assign({}, data, { id: nextId() });
        arr.push(item);
        return item;
      },
      update: function (id, data) {
        for (var i = 0; i < arr.length; i++) {
          if (arr[i].id === String(id)) { Object.assign(arr[i], data); return arr[i]; }
        }
        return null;
      },
      remove: function (id) {
        for (var i = 0; i < arr.length; i++) {
          if (arr[i].id === String(id)) { arr.splice(i, 1); return true; }
        }
        return false;
      },
      count: function () { return arr.length; },
      raw: function () { return arr; }
    };
  }

  return {
    Projects: createStore(projects),
    Articles: createStore(articles),
    Experiences: createStore(experiences),
    Education: createStore(education),
    Skills: createStore(skills),
    Testimonials: createStore(testimonials),
    Messages: createStore(messages),
    unreadMessages: function () {
      return messages.filter(function (m) { return !m.isRead; }).length;
    }
  };
})();
