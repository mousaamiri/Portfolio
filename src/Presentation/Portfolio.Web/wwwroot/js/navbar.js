/* ==========================================================================
   navbar.js — Floating pill navbar logic (multi-page version)
   --------------------------------------------------------------------------
   Handles: sliding active-pill highlight, page-based active detection,
   IntersectionObserver scroll tracking for in-page sections, scroll-based
   "scrolled" state, More dropdown, theme toggle, hamburger / mobile panel.

    Reads window.__navConfig.currentPage to set the initial active link.
    Exposes window.__initNavbarBehavior for re-init (idempotent: listeners bind
    once) and window.__setActiveNavPage for the SPA router to sync the active
    link + sliding pill after a navigation without rebuilding the navbar.
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

  // Idempotency: the navbar DOM is built once and never rebuilt during SPA
  // navigation (the router only syncs the active link). Re-running this init on
  // every navigation would stack duplicate window/document listeners —
  // scroll/resize handlers that run on every frame and a render loop that gets
  // progressively heavier ("gets worse after a few clicks"). behaviorBound
  // guarantees listeners attach exactly once; later calls only refresh derived
  // state (active pill + "scrolled" class).
  var behaviorBound = false;

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
    var mobileClose = navbar.querySelector(".navbar-mobile-close");
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
      setContactActive(false);
    }

    function updateMobileActive(link) {
      var page = link.getAttribute("data-page");
      mobileLinks.forEach(function (ml) {
        ml.classList.toggle("active", ml.getAttribute("data-page") === page);
      });
    }

    function setContactActive(on) {
      var contactBtn = navbar.querySelector(".navbar-contact-btn");
      var mobileContact = navbar.querySelector(".navbar-mobile-contact");
      if (contactBtn) contactBtn.classList.toggle("active", on);
      if (mobileContact) mobileContact.classList.toggle("active", on);
    }

    function setActivePage(page) {
      var activeLink = page
        ? navbar.querySelector('[data-nav-link][data-page="' + page + '"]')
        : null;

      if (activeLink) {
        setActiveLink(activeLink);
      } else {
        // The current page isn't part of the pill nav (e.g. the standalone
        // Contact CTA). Clear the highlight instead of falling back to the
        // first link, which would wrongly light up Home.
        navLinks.forEach(function (l) { l.classList.remove("active"); });
        mobileLinks.forEach(function (ml) { ml.classList.remove("active"); });
        currentActiveLink = null;
        if (activePillEl) activePillEl.classList.remove("visible");
        setContactActive(page === "contact");
      }
    }

    function initActivePill() {
      setActivePage(currentPage);
    }

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

    var resizeTimer;
    function onResize() {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function () {
        if (currentActiveLink) moveActivePill(currentActiveLink);
      }, 100);
    }

    function onDropdownClick(e) {
      if (dropdown && dropdown.classList.contains("open")) {
        if (!dropdown.contains(e.target) && e.target !== moreBtn) {
          if (moreBtn) moreBtn.setAttribute("aria-expanded", "false");
          dropdown.classList.remove("open");
        }
      }
    }

    function onKeydown(e) {
      if (mobileOpen && (e.key === "Escape" || e.key === "Esc")) {
        closeMobilePanel();
      }
    }

    // Bind listeners exactly once (see behaviorBound above). Everything below is
    // idempotent by construction, so a re-init after a later navigation is a
    // cheap state refresh rather than a full rewire.
    if (!behaviorBound) {
      navLinks.forEach(function (link) {
        link.addEventListener("click", function (e) {
          var href = link.getAttribute("href");
          if (!href || href === "#") { e.preventDefault(); return; }
          if (href.charAt(0) === "#") setActiveLink(link);
        });
      });

      mobileLinks.forEach(function (link) {
        link.addEventListener("click", function () {
          closeMobilePanel();
          var href = link.getAttribute("href");
          if (href && href.charAt(0) === "#" && href !== "#") {
            setActivePage(link.getAttribute("data-page"));
          }
        });
      });

      window.addEventListener("scroll", onScroll, { passive: true });
      window.addEventListener("resize", onResize);

      if (moreBtn) {
        moreBtn.addEventListener("click", function (e) {
          e.stopPropagation();
          var isOpen = moreBtn.getAttribute("aria-expanded") === "true";
          moreBtn.setAttribute("aria-expanded", isOpen ? "false" : "true");
          if (dropdown) dropdown.classList.toggle("open", !isOpen);
        });
      }

      document.addEventListener("click", onDropdownClick);

      if (themeToggle) themeToggle.addEventListener("click", toggleTheme);
      if (mobileThemeToggle) mobileThemeToggle.addEventListener("click", toggleTheme);

      if (hamburger) {
        hamburger.addEventListener("click", function () {
          if (mobileOpen) closeMobilePanel();
          else openMobilePanel();
        });
      }

      if (mobileClose) {
        mobileClose.addEventListener("click", closeMobilePanel);
      }

      document.addEventListener("keydown", onKeydown);

      behaviorBound = true;

      // SPA entry point: lets the router sync the active link and reposition the
      // sliding pill after a navigation, without rewriting navbar.innerHTML.
      // Closed over the (stable) first-init DOM queries.
      window.__setActiveNavPage = function (page) {
        setActivePage(page);
      };
    }

    navbar.classList.toggle("scrolled", window.scrollY > 50);

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
