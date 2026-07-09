/* ==========================================================================
   shared.js — Reusable admin components: sidebar, CRUD framework, modal,
   table, search, toast, bilingual form helpers
   ========================================================================== */

/* ================= UTILITIES ================= */

var AdminUtil = {
  escapeHtml: function (str) {
    if (!str) return "";
    return String(str).replace(/&/g,"&amp;").replace(/</g,"&lt;").replace(/>/g,"&gt;").replace(/"/g,"&quot;");
  },
  truncate: function (str, n) {
    if (!str) return "";
    return str.length > n ? str.substring(0, n) + "…" : str;
  },
  debounce: function (fn, ms) {
    var t;
    return function () {
      var ctx = this, args = arguments;
      clearTimeout(t);
      t = setTimeout(function () { fn.apply(ctx, args); }, ms);
    };
  }
};

/* ================= TOAST ================= */

var AdminToast = (function () {
  function ensure() {
    var c = document.getElementById("toastContainer");
    if (c) return c;
    c = document.createElement("div");
    c.id = "toastContainer";
    c.className = "admin-toast-container";
    document.body.appendChild(c);
    return c;
  }
  return {
    show: function (msg, type) {
      var c = ensure();
      var icon = type === "success"
        ? '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><path d="m9 11 3 3L22 4"/></svg>'
        : '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>';
      var el = document.createElement("div");
      el.className = "admin-toast admin-toast--" + (type || "success");
      el.innerHTML = icon + '<span>' + AdminUtil.escapeHtml(msg) + '</span>';
      c.appendChild(el);
      setTimeout(function () {
        el.classList.add("removing");
        setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 250);
      }, 3000);
    }
  };
})();

/* ================= MODAL ================= */

var AdminModal = (function () {
  var _overlay, _content;

  function getOverlay() {
    if (_overlay) return _overlay;
    _overlay = document.getElementById("modalOverlay");
    _content = document.getElementById("modalContent");
    if (!_overlay) {
      _overlay = document.createElement("div");
      _overlay.id = "modalOverlay";
      _overlay.className = "admin-modal-overlay";
      _content = document.createElement("div");
      _content.id = "modalContent";
      _content.className = "admin-modal";
      _overlay.appendChild(_content);
      document.body.appendChild(_overlay);
    }
    _overlay.addEventListener("click", function (e) {
      if (e.target === _overlay) AdminModal.close();
    });
    return _overlay;
  }

  return {
    open: function (html, opts) {
      var ov = getOverlay();
      _content.className = "admin-modal" + (opts && opts.small ? " admin-modal--sm" : "");
      _content.innerHTML = html;
      ov.classList.add("is-open");
      document.body.style.overflow = "hidden";
      if (window.lucide) lucide.createIcons({ nodes: [_content] });
      var closeBtn = _content.querySelector(".admin-modal-close");
      if (closeBtn) closeBtn.addEventListener("click", AdminModal.close);
      var cancelBtns = _content.querySelectorAll("[data-modal-cancel]");
      cancelBtns.forEach(function (b) { b.addEventListener("click", AdminModal.close); });
    },
    close: function () {
      if (_overlay) {
        _overlay.classList.remove("is-open");
        document.body.style.overflow = "";
      }
    },
    confirmDelete: function (itemName, onConfirm) {
      var html = '';
      html += '<div class="admin-modal-header"><h3 class="admin-modal-title">Delete Confirmation</h3>';
      html += '<button class="admin-modal-close"><i data-lucide="x"></i></button></div>';
      html += '<div class="admin-modal-body">';
      html += '<div class="admin-modal-icon admin-modal-icon--danger"><i data-lucide="trash-2"></i></div>';
      html += '<p class="admin-modal-msg">Are you sure you want to delete <strong>' + AdminUtil.escapeHtml(itemName) + '</strong>? This action cannot be undone.</p>';
      html += '</div>';
      html += '<div class="admin-modal-footer">';
      html += '<button class="admin-btn admin-btn--ghost" data-modal-cancel>Cancel</button>';
      html += '<button class="admin-btn admin-btn--danger" id="confirmDeleteBtn"><i data-lucide="trash-2"></i> Delete</button>';
      html += '</div>';
      AdminModal.open(html, { small: true });
      document.getElementById("confirmDeleteBtn").addEventListener("click", function () {
        AdminModal.close();
        onConfirm();
      });
    }
  };
})();

/* ================= SIDEBAR ================= */

var AdminSidebar = {
  NAV: [
    // MVC migration: hrefs point at AdminController routes (was *.html).
    { page: "index", label: "Dashboard", icon: "layout-dashboard", href: "/Admin" },
    { page: "profile", label: "Profile", icon: "user-circle", href: "/Admin/Profile" },
    { page: "projects", label: "Projects", icon: "folder-kanban", href: "/Admin/Projects" },
    { page: "articles", label: "Articles", icon: "file-text", href: "/Admin/Articles" },
    { page: "experiences", label: "Experiences", icon: "briefcase", href: "/Admin/Experiences" },
    { page: "education", label: "Education", icon: "graduation-cap", href: "/Admin/Education" },
    { page: "metrics", label: "Impact Metrics", icon: "trending-up", href: "/Admin/Metrics" },
    { page: "principles", label: "Principles", icon: "compass", href: "/Admin/Principles" },
    { page: "proficiencies", label: "Proficiencies", icon: "layers", href: "/Admin/Proficiencies" },
    { page: "skills", label: "Skills", icon: "star", href: "/Admin/Skills" },
    { page: "timeline", label: "Timeline", icon: "milestone", href: "/Admin/Timeline" },
    { page: "interests", label: "Interests", icon: "heart", href: "/Admin/Interests" },
    { page: "stats", label: "Stats", icon: "bar-chart-3", href: "/Admin/Stats" },
    { page: "faqs", label: "FAQ", icon: "help-circle", href: "/Admin/Faqs" },
    { page: "testimonials", label: "Testimonials", icon: "quote", href: "/Admin/Testimonials" },
    { page: "messages", label: "Messages", icon: "mail", href: "/Admin/Messages", badge: true }
  ],

  init: function (activePage) {
    var sidebar = document.getElementById("adminSidebar");
    if (!sidebar) return;

    var navHtml = '';
    AdminSidebar.NAV.forEach(function (item) {
      var active = item.page === activePage ? " is-active" : "";
      var badge = "";
      navHtml += '<a href="' + item.href + '" class="admin-nav-link' + active + '">';
      navHtml += '<i data-lucide="' + item.icon + '" class="admin-nav-icon"></i>';
      navHtml += '<span>' + item.label + '</span>' + badge + '</a>';
    });

    sidebar.innerHTML = '<div class="admin-sidebar-header">' +
      '<img id="sidebarLogo" src="/images/logo-en-dark.svg" alt="Logo" class="admin-sidebar-logo" />' +
      '<span class="admin-sidebar-brand">Admin</span></div>' +
      '<nav class="admin-nav">' + navHtml + '</nav>' +
      '<div class="admin-sidebar-footer">' +
      '<a href="/Admin/ChangePassword" class="admin-nav-link admin-nav-link--subtle">' +
      '<i data-lucide="key-round" class="admin-nav-icon"></i><span>Change Password</span></a>' +
      '<a href="/" class="admin-nav-link admin-nav-link--subtle" target="_blank">' +
      '<i data-lucide="external-link" class="admin-nav-icon"></i><span>View Site</span></a>' +
      '<button class="admin-nav-link admin-nav-link--subtle" id="themeToggleBtn">' +
      '<span id="themeIconLight" class="admin-theme-icon"><i data-lucide="sun" class="admin-nav-icon"></i></span>' +
      '<span id="themeIconDark" class="admin-theme-icon"><i data-lucide="moon" class="admin-nav-icon"></i></span>' +
      '<span id="themeToggleText">Light Mode</span></button>' +
      '<button class="admin-nav-link admin-nav-link--subtle" id="logoutBtn">' +
      '<i data-lucide="log-out" class="admin-nav-icon"></i><span>Logout</span></button></div>';

    // Theme toggle
    AdminSidebar._initTheme();
    // Mobile hamburger
    AdminSidebar._initMobile();
    // Logout
    var logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
      logoutBtn.addEventListener("click", function () {
        // Real server-side logout — submit the antiforgery-protected form that
        // clears the admin auth cookie (see _AdminLayout).
        var lf = document.getElementById("logoutForm");
        if (lf) { lf.submit(); }
        else { window.location.href = "/Admin/Login"; }
      });
    }
  },

  _initTheme: function () {
    var btn = document.getElementById("themeToggleBtn");
    if (!btn) return;
    function update() {
      var isDark = document.documentElement.getAttribute("data-theme") !== "light";
      document.getElementById("themeIconLight").style.display = isDark ? "inline-flex" : "none";
      document.getElementById("themeIconDark").style.display = isDark ? "none" : "inline-flex";
      document.getElementById("themeToggleText").textContent = isDark ? "Light Mode" : "Dark Mode";
      var logo = document.getElementById("sidebarLogo");
      if (logo) logo.src = "/images/logo-en-" + (isDark ? "dark" : "light") + ".svg";
    }
    update();
    btn.addEventListener("click", function () {
      var cur = document.documentElement.getAttribute("data-theme") || "dark";
      var next = cur === "light" ? "dark" : "light";
      document.documentElement.setAttribute("data-theme", next);
      try { localStorage.setItem("portfolio-theme", next); } catch (e) {}
      update();
    });
  },

  _initMobile: function () {
    var hamburger = document.getElementById("hamburgerBtn");
    var sidebar = document.getElementById("adminSidebar");
    var overlay = document.getElementById("sidebarOverlay");
    if (!hamburger || !sidebar) return;
    function close() {
      sidebar.classList.remove("is-open");
      if (overlay) overlay.classList.remove("is-visible");
    }
    hamburger.addEventListener("click", function () {
      sidebar.classList.toggle("is-open");
      if (overlay) overlay.classList.toggle("is-visible");
    });
    if (overlay) overlay.addEventListener("click", close);
    sidebar.addEventListener("click", function (e) {
      if (e.target.closest(".admin-nav-link") && window.innerWidth < 768) close();
    });
  }

// The old client-side AdminCRUD table/modal framework (and its in-memory
// AdminData store) was retired in Phase 3 — every admin screen is now a
// server-rendered Razor view backed by the authenticated api/admin/* endpoints.
