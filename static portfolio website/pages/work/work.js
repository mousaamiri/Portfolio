/* ==========================================================================
   work.js — Project showcase interactive logic
   --------------------------------------------------------------------------
   Renders the project list, handles selection, and updates the detail panel.
   Optional GSAP fade transition on project switch.
   ========================================================================== */

(function () {
  "use strict";

  /* ---- Project Data ---- */
  var projects = [
    {
      id: 1,
      name: "ResumeIQ Platform",
      subtitle: "AI Resume Analysis & ATS Intelligence Platform",
      description:
        "An enterprise-grade resume analysis platform that evaluates candidate resumes against job descriptions using Google Gemini AI. Features automated PDF parsing, ATS compatibility scoring, skill gap identification, actionable improvement recommendations, secure JWT authentication with eager auto-renewal, and role-based access control.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "PostgreSQL", color: "#3b82f6" },
        { name: "React", color: "#3b82f6" },
        { name: "TypeScript", color: "#3b82f6" },
        { name: "Tailwind CSS", color: "#14b8a6" },
        { name: "Gemini AI", color: "#3b82f6" },
        { name: "Apache PDFBox", color: "#ec4899" },
        { name: "JWT Auth", color: "#22c55e" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
    {
      id: 2,
      name: "GitGlance",
      subtitle: "Ultimate Github Profile Visualizer",
      description:
        "A comprehensive GitHub profile visualization tool that provides detailed analytics, contribution graphs, repository insights, and developer statistics in a beautiful dashboard interface.",
      techs: [
        { name: "React", color: "#3b82f6" },
        { name: "TypeScript", color: "#3b82f6" },
        { name: "GitHub API", color: "#f2f1ec" },
        { name: "Tailwind CSS", color: "#14b8a6" },
        { name: "Chart.js", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 3,
      name: "Spring AI RAG",
      subtitle: "Production-Ready AI RAG System",
      description:
        "A production-ready Retrieval-Augmented Generation system built with Spring AI, featuring document ingestion, vector storage, semantic search, and context-aware AI responses for enterprise applications.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "Spring AI", color: "#22c55e" },
        { name: "PostgreSQL", color: "#3b82f6" },
        { name: "pgvector", color: "#a855f7" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
    {
      id: 4,
      name: "SyncBoard",
      subtitle: "Real-time Collaborative Clipboard",
      description:
        "A real-time collaborative clipboard application enabling seamless text and data sharing across devices with WebSocket-powered synchronization and secure room-based access.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "WebSocket", color: "#a855f7" },
        { name: "React", color: "#3b82f6" },
        { name: "Redis", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 5,
      name: "Movie Recommendation System",
      subtitle: "Content-Based ML Recommender",
      description:
        "A machine learning-powered movie recommendation engine using content-based filtering with TF-IDF vectorization and cosine similarity for personalized movie suggestions.",
      techs: [
        { name: "Python", color: "#f0a63b" },
        { name: "scikit-learn", color: "#3b82f6" },
        { name: "Pandas", color: "#a855f7" },
        { name: "Flask", color: "#22c55e" },
        { name: "TMDB API", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 6,
      name: "Order Management Service",
      subtitle: "Secure Spring Boot Backend API",
      description:
        "A secure and scalable order management backend service with RESTful APIs, JWT authentication, role-based authorization, and comprehensive order lifecycle management.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "Spring Security", color: "#22c55e" },
        { name: "MySQL", color: "#3b82f6" },
        { name: "JWT", color: "#f0a63b" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
  ];

  /* ---- Persian Project Data ---- */
  var projectsFa = [
    {
      id: 1,
      name: "پلتفرم ResumeIQ",
      subtitle: "تحلیل هوشمند رزومه و سازگاری با سامانه‌های ATS",
      description: "یک پلتفرم سازمانی برای تحلیل رزومه که با بهره‌گیری از هوش مصنوعی Google Gemini، رزومه‌ی متقاضیان را با شرح شغل مقایسه و ارزیابی می‌کند. امکاناتی مانند استخراج خودکار اطلاعات از PDF، امتیازدهی سازگاری با ATS، شناسایی شکاف مهارتی، پیشنهادهای عملی بهبود، احراز هویت امن با JWT و کنترل دسترسی مبتنی بر نقش را فراهم می‌آورد.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "PostgreSQL", color: "#3b82f6" },
        { name: "React", color: "#3b82f6" },
        { name: "TypeScript", color: "#3b82f6" },
        { name: "Tailwind CSS", color: "#14b8a6" },
        { name: "Gemini AI", color: "#3b82f6" },
        { name: "Apache PDFBox", color: "#ec4899" },
        { name: "JWT Auth", color: "#22c55e" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
    {
      id: 2,
      name: "GitGlance",
      subtitle: "ابزار جامع مصورسازی پروفایل گیت‌هاب",
      description: "ابزاری کامل برای مصورسازی پروفایل گیت‌هاب که تحلیل‌های جزئی، نمودار مشارکت‌ها، بینش مخازن و آمار توسعه‌دهنده را در قالب داشبوردی زیبا و کاربردی ارائه می‌دهد.",
      techs: [
        { name: "React", color: "#3b82f6" },
        { name: "TypeScript", color: "#3b82f6" },
        { name: "GitHub API", color: "#f2f1ec" },
        { name: "Tailwind CSS", color: "#14b8a6" },
        { name: "Chart.js", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 3,
      name: "Spring AI RAG",
      subtitle: "سامانه هوش مصنوعی RAG آماده‌ی تولید",
      description: "یک سامانه‌ی بازیابی-افزوده‌ی تولیدی (RAG) آماده‌ی بهره‌برداری که با Spring AI ساخته شده است. قابلیت‌هایی شامل دریافت و پردازش اسناد، ذخیره‌سازی بُرداری، جستجوی معنایی و تولید پاسخ‌های هوشمند متناسب با زمینه را برای کاربردهای سازمانی فراهم می‌کند.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "Spring AI", color: "#22c55e" },
        { name: "PostgreSQL", color: "#3b82f6" },
        { name: "pgvector", color: "#a855f7" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
    {
      id: 4,
      name: "SyncBoard",
      subtitle: "کلیپ‌بورد مشارکتی لحظه‌ای",
      description: "برنامه‌ای برای اشتراک‌گذاری لحظه‌ای متن و داده میان دستگاه‌های مختلف، با هم‌زمان‌سازی مبتنی بر WebSocket و دسترسی امن از طریق سیستم اتاق‌های خصوصی.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "WebSocket", color: "#a855f7" },
        { name: "React", color: "#3b82f6" },
        { name: "Redis", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 5,
      name: "سامانه پیشنهاد فیلم",
      subtitle: "پیشنهاددهنده‌ی هوشمند مبتنی بر محتوا",
      description: "موتور پیشنهاد فیلم که از یادگیری ماشین و فیلترسازی مبتنی بر محتوا بهره می‌برد. با استفاده از بُرداری‌سازی TF-IDF و شباهت کسینوسی، پیشنهادهای شخصی‌سازی‌شده‌ای به کاربر ارائه می‌دهد.",
      techs: [
        { name: "Python", color: "#f0a63b" },
        { name: "scikit-learn", color: "#3b82f6" },
        { name: "Pandas", color: "#a855f7" },
        { name: "Flask", color: "#22c55e" },
        { name: "TMDB API", color: "#ec4899" },
      ],
      github: "#",
    },
    {
      id: 6,
      name: "سرویس مدیریت سفارش",
      subtitle: "رابط برنامه‌نویسی امن با Spring Boot",
      description: "سرویس بک‌اند امن و مقیاس‌پذیر برای مدیریت سفارش‌ها، مجهز به رابط‌های RESTful، احراز هویت JWT، سطوح دسترسی مبتنی بر نقش و مدیریت کامل چرخه‌ی عمر سفارش.",
      techs: [
        { name: "Java 21", color: "#f0a63b" },
        { name: "Spring Boot", color: "#22c55e" },
        { name: "Spring Security", color: "#22c55e" },
        { name: "MySQL", color: "#3b82f6" },
        { name: "JWT", color: "#f0a63b" },
        { name: "Docker", color: "#3b82f6" },
      ],
      github: "#",
    },
  ];

  /* ---- Get active projects based on language ---- */
  function getActiveProjects() {
    if (window.i18n && window.i18n.lang() === "fa") return projectsFa;
    return projects;
  }

  /* ---- DOM references ---- */
  var listEl = document.querySelector(".work-list");
  var detailInner = document.querySelector(".work-detail-inner");
  var detailNumber = document.getElementById("detail-number");
  var detailTitle = document.getElementById("detail-title");
  var detailSubtitle = document.getElementById("detail-subtitle");
  var detailDesc = document.getElementById("detail-desc");
  var detailTechs = document.getElementById("detail-techs");
  var detailGithub = document.getElementById("detail-github");

  /* ---- Modal DOM references ---- */
  var modalOverlay = document.getElementById("work-modal-overlay");
  var modalCloseBtn = document.getElementById("work-modal-close");
  var modalNumber = document.getElementById("modal-number");
  var modalTitle = document.getElementById("modal-title");
  var modalSubtitle = document.getElementById("modal-subtitle");
  var modalDesc = document.getElementById("modal-desc");
  var modalTechs = document.getElementById("modal-techs");
  var modalGithub = document.getElementById("modal-github");
  var mobileBreakpoint = window.matchMedia("(max-width: 860px)");

  var activeIndex = 0;
  var isTransitioning = false;

  /* ---- Helpers ---- */

  function padNumber(n) {
    return n < 10 ? "0" + n : "" + n;
  }

  /* Build tech pill HTML */
  function buildTechPills(techs) {
    return techs
      .map(function (t) {
        return (
          '<span class="work-tech-pill">' +
          '<span class="work-tech-dot" style="background:' + t.color + '"></span>' +
          t.name +
          "</span>"
        );
      })
      .join("");
  }

  /* ---- Render project list (left side) ---- */

  function renderList() {
    var html = "";
    getActiveProjects().forEach(function (p, i) {
      html +=
        '<button class="work-list-item' +
        (i === 0 ? " is-active" : "") +
        '" role="tab" aria-selected="' +
        (i === 0 ? "true" : "false") +
        '" data-index="' +
        i +
        '">' +
        '<span class="work-list-number">' +
        padNumber(p.id) +
        "</span>" +
        '<span class="work-list-text">' +
        '<span class="work-list-name">' +
        p.name +
        "</span>" +
        '<span class="work-list-sub">' +
        p.subtitle +
        "</span>" +
        "</span>" +
        "</button>";
    });
    listEl.innerHTML = html;
  }

  /* ---- Update detail panel (right side) ---- */

  function updateDetail(index) {
    var p = getActiveProjects()[index];
    detailNumber.textContent = padNumber(p.id);
    detailTitle.textContent = p.name;
    detailSubtitle.textContent = p.subtitle;
    detailDesc.textContent = p.description;
    detailTechs.innerHTML = buildTechPills(p.techs);
    detailGithub.href = p.github;
  }

  /* ---- Mobile modal helpers ---- */

  function openModal(index) {
    var p = getActiveProjects()[index];
    modalNumber.textContent = padNumber(p.id);
    modalTitle.textContent = p.name;
    modalSubtitle.textContent = p.subtitle;
    modalDesc.textContent = p.description;
    modalTechs.innerHTML = buildTechPills(p.techs);
    modalGithub.href = p.github;

    var githubSpan = modalGithub.querySelector("[data-i18n]");
    if (githubSpan && window.i18n && window.i18n.lang() === "fa") {
      githubSpan.textContent = window.i18n.t("work.view_github", "View GitHub");
    }

    modalOverlay.classList.add("is-open");
    document.body.style.overflow = "hidden";
    modalCloseBtn.focus();
  }

  function closeModal() {
    modalOverlay.classList.remove("is-open");
    document.body.style.overflow = "";
  }

  if (modalCloseBtn) {
    modalCloseBtn.addEventListener("click", closeModal);
  }

  if (modalOverlay) {
    modalOverlay.addEventListener("click", function (e) {
      if (e.target === modalOverlay) closeModal();
    });
  }

  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape" && modalOverlay && modalOverlay.classList.contains("is-open")) {
      closeModal();
    }
  });

  /* ---- Select a project with optional GSAP fade ---- */

  function selectProject(index) {
    if (index === activeIndex || isTransitioning) return;

    /* Update list highlight */
    var items = listEl.querySelectorAll(".work-list-item");
    items[activeIndex].classList.remove("is-active");
    items[activeIndex].setAttribute("aria-selected", "false");
    items[index].classList.add("is-active");
    items[index].setAttribute("aria-selected", "true");

    activeIndex = index;

    /* Animate with GSAP if available, otherwise instant swap */
    if (typeof gsap !== "undefined") {
      isTransitioning = true;
      gsap.to(detailInner, {
        opacity: 0,
        y: 8,
        duration: 0.2,
        ease: "power2.in",
        onComplete: function () {
          updateDetail(index);
          gsap.to(detailInner, {
            opacity: 1,
            y: 0,
            duration: 0.3,
            ease: "power2.out",
            onComplete: function () {
              isTransitioning = false;
            },
          });
        },
      });
    } else {
      /* CSS-only fallback */
      detailInner.classList.add("is-fading");
      setTimeout(function () {
        updateDetail(index);
        detailInner.classList.remove("is-fading");
      }, 250);
    }
  }

  /* ---- Event delegation for project list clicks ---- */

  listEl.addEventListener("click", function (e) {
    var btn = e.target.closest(".work-list-item");
    if (!btn) return;
    var idx = parseInt(btn.getAttribute("data-index"), 10);
    if (mobileBreakpoint.matches) {
      openModal(idx);
    } else {
      selectProject(idx);
    }
  });

  /* Keyboard navigation — arrow keys to move between projects */
  listEl.addEventListener("keydown", function (e) {
    if (e.key === "ArrowDown" || e.key === "ArrowRight") {
      e.preventDefault();
      var next = (activeIndex + 1) % getActiveProjects().length;
      selectProject(next);
      listEl.querySelectorAll(".work-list-item")[next].focus();
    } else if (e.key === "ArrowUp" || e.key === "ArrowLeft") {
      e.preventDefault();
      var prev = (activeIndex - 1 + getActiveProjects().length) % getActiveProjects().length;
      selectProject(prev);
      listEl.querySelectorAll(".work-list-item")[prev].focus();
    }
  });

  /* ---- Initialize ---- */

  renderList();
  updateDetail(0);

  /* ---- Expose rebuild for i18n language switching ---- */
  window.__rebuildWorkProjects = function () {
    activeIndex = 0;
    renderList();
    updateDetail(0);
    // Update the View GitHub link text
    var githubLink = document.querySelector('.work-detail-github');
    if (githubLink) {
      var span = githubLink.querySelector('[data-i18n]');
      if (span && window.i18n && window.i18n.lang() === 'fa') {
        span.textContent = window.i18n.t('work.view_github', 'View GitHub');
      }
    }
  };
})();

/* ==========================================================================
   Text reveals (uses revealText from text-reveal.js)
   ========================================================================== */
(function () {
  "use strict";

  if (typeof window.revealText !== "function") return;

  var REVEALS = [
    [".work-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
    [".work-detail-title", { type: "chars", stagger: 0.025, blurAmount: 10, duration: 0.5 }],
    [".work-detail-subtitle", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".work-detail-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
    [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    [".closing-cta-card-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.45 }],
  ];

  REVEALS.forEach(function (entry) {
    window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
  });
})();
