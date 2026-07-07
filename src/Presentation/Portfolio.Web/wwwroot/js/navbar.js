/* ==========================================================================
   navbar.js — Floating pill navbar logic (multi-page version)
   --------------------------------------------------------------------------
   Handles: sliding active-pill highlight, page-based active detection,
   IntersectionObserver scroll tracking for in-page sections, scroll-based
   "scrolled" state, More dropdown, theme toggle, hamburger / mobile panel.

   Reads window.__navConfig.currentPage to set the initial active link.
   Exposes window.__initNavbarBehavior for re-init after i18n rebuild.
   ========================================================================== */

(function () {
  "use strict";

  var THEME_KEY = "portfolio-theme";

  function getPreferredTheme() {
    try {
      var stored = localStorage.getItem(THEME_KEY);
      if (stored) return stored;
    } catch (e) {}
    return window.matchMedia("(prefers-color-scheme: light)").matches
      ? "light"
      : "dark";
  }

  function applyTheme(theme) {
    document.documentElement.setAttribute("data-theme", theme);
    try { localStorage.setItem(THEME_KEY, theme); } catch (e) {}
  }

  function toggleTheme() {
    var current =
      document.documentElement.getAttribute("data-theme") || "dark";
    applyTheme(current === "dark" ? "light" : "dark");
  }

  if (!document.documentElement.getAttribute("data-theme")) {
    applyTheme(getPreferredTheme());
  }

  function initNavbarBehavior() {
    var config = window.__navConfig || {};
    var currentPage = config.currentPage || "home";

    var navbar = document.getElementById("navbar");
    if (!navbar) return;

    var navLinks = navbar.querySelectorAll("[data-nav-link]");
    var activePillEl = navbar.querySelector(".navbar-active-pill");
    var navContainer = navbar.querySelector(".navbar-nav");
    var moreBtn = navbar.querySelector(".navbar-more-btn");
    var dropdown = navbar.querySelector(".navbar-dropdown");
    var themeToggle = navbar.querySelector(".navbar-theme-toggle");
    var hamburger = navbar.querySelector(".navbar-hamburger");
    var mobilePanel = navbar.querySelector(".navbar-mobile-panel");
    var mobileThemeToggle = navbar.querySelector(".navbar-mobile-theme-toggle");
    var mobileLinks = navbar.querySelectorAll("[data-mobile-link]");

    var currentActiveLink = null;

    function moveActivePill(link) {
      if (!link || !activePillEl || !navContainer) return;
      var navRect = navContainer.getBoundingClientRect();
      var linkRect = link.getBoundingClientRect();
      activePillEl.style.left = linkRect.left - navRect.left + "px";
      activePillEl.style.width = linkRect.width + "px";
    }

    function setActiveLink(link) {
      if (link === currentActiveLink) return;
      navLinks.forEach(function (l) { l.classList.remove("active"); });
      link.classList.add("active");
      currentActiveLink = link;
      moveActivePill(link);
      if (!activePillEl.classList.contains("visible")) {
        activePillEl.classList.add("visible");
      }
      updateMobileActive(link);
    }

    function updateMobileActive(link) {
      var page = link.getAttribute("data-page");
      mobileLinks.forEach(function (ml) {
        ml.classList.toggle("active", ml.getAttribute("data-page") === page);
      });
    }

    function initActivePill() {
      var activeLink = navbar.querySelector('[data-nav-link][data-page="' + currentPage + '"]');
      if (!activeLink) activeLink = navbar.querySelector("[data-nav-link]");
      if (activeLink) setActiveLink(activeLink);
    }

    navLinks.forEach(function (link) {
      link.addEventListener("click", function (e) {
        var href = link.getAttribute("href");
        if (!href || href === "#") { e.preventDefault(); return; }
        if (href.charAt(0) === "#") setActiveLink(link);
      });
    });

    var mobileOpen = false;

    function closeMobilePanel() {
      if (!hamburger || !mobilePanel) return;
      mobileOpen = false;
      hamburger.setAttribute("aria-expanded", "false");
      mobilePanel.setAttribute("aria-hidden", "true");
      mobilePanel.classList.remove("open");
      document.body.style.overflow = "";
      setTimeout(function () {
        if (!mobileOpen) mobilePanel.style.display = "none";
      }, 350);
    }

    mobileLinks.forEach(function (link) {
      link.addEventListener("click", function () {
        closeMobilePanel();
        var href = link.getAttribute("href");
        if (href && href.charAt(0) === "#" && href !== "#") {
          var matchingNavLink = navbar.querySelector('[data-nav-link][data-page="' + link.getAttribute("data-page") + '"]');
          if (matchingNavLink) setActiveLink(matchingNavLink);
        }
      });
    });

    navbar.classList.toggle("scrolled", window.scrollY > 50);

    var scrollTicking = false;
    function onScroll() {
      if (!scrollTicking) {
        requestAnimationFrame(function () {
          navbar.classList.toggle("scrolled", window.scrollY > 50);
          scrollTicking = false;
        });
        scrollTicking = true;
      }
    }
    window.addEventListener("scroll", onScroll, { passive: true });

    var resizeTimer;
    window.addEventListener("resize", function () {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function () {
        if (currentActiveLink) moveActivePill(currentActiveLink);
      }, 100);
    });

    if (moreBtn) {
      moreBtn.addEventListener("click", function (e) {
        e.stopPropagation();
        var isOpen = moreBtn.getAttribute("aria-expanded") === "true";
        moreBtn.setAttribute("aria-expanded", isOpen ? "false" : "true");
        if (dropdown) dropdown.classList.toggle("open", !isOpen);
      });
    }

    document.addEventListener("click", function (e) {
      if (dropdown && dropdown.classList.contains("open")) {
        if (!dropdown.contains(e.target) && e.target !== moreBtn) {
          if (moreBtn) moreBtn.setAttribute("aria-expanded", "false");
          dropdown.classList.remove("open");
        }
      }
    });

    if (themeToggle) themeToggle.addEventListener("click", toggleTheme);
    if (mobileThemeToggle) mobileThemeToggle.addEventListener("click", toggleTheme);

    function openMobilePanel() {
      if (!hamburger || !mobilePanel) return;
      mobileOpen = true;
      hamburger.setAttribute("aria-expanded", "true");
      mobilePanel.setAttribute("aria-hidden", "false");
      mobilePanel.style.display = "flex";
      requestAnimationFrame(function () {
        requestAnimationFrame(function () {
          mobilePanel.classList.add("open");
        });
      });
      document.body.style.overflow = "hidden";
    }

    if (hamburger) {
      hamburger.addEventListener("click", function () {
        if (mobileOpen) closeMobilePanel();
        else openMobilePanel();
      });
    }

    function doInit() {
      initActivePill();
      onScroll();
    }

    if (document.fonts && document.fonts.ready) {
      document.fonts.ready.then(doInit);
    } else {
      doInit();
    }
  }

  initNavbarBehavior();
  window.__initNavbarBehavior = initNavbarBehavior;
})();
