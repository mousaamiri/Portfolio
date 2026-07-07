/* ==========================================================================
   footer-component.js — Shared footer builder for multi-page portfolio
   --------------------------------------------------------------------------
   Generates footer + scroll-progress button and injects into the page.
   Reads window.__navConfig for basePath. Supports i18n via window.i18n.
   ========================================================================== */

(function () {
  "use strict";

  function buildFooter() {
    var config = window.__navConfig || {};
    var bp = config.basePath || "../../";

    function t(key, fallback) {
      if (window.i18n) return window.i18n.t(key, fallback);
      return fallback;
    }

    // MVC migration: same routes object written by Razor _Layout.
    var pages = config.routes || {
      home: bp + "pages/home/home.html",
      about: bp + "pages/about/about.html",
      experience: bp + "pages/experience/experience.html",
      work: bp + "pages/work/work.html",
      blog: bp + "pages/blog/blog.html",
      contact: bp + "pages/contact/contact.html",
    };

    var existingFooter = document.querySelector(".site-footer");
    if (existingFooter) existingFooter.remove();

    var existingScrollBtn = document.querySelector(".scroll-progress-btn");
    if (existingScrollBtn) existingScrollBtn.remove();

    var footer = document.createElement("footer");
    footer.className = "site-footer";

    footer.innerHTML =
      '<div class="footer-inner">' +
        '<div class="footer-top">' +
          '<div class="footer-brand">' +
            '<a href="' + pages.home + '" class="footer-logo">mousa <span>.dev</span></a>' +
            '<span class="footer-credit">' + t("footer.developed_with", "Developed with ❤ by") + ' <a href="' + pages.about + '">Mousa Amiri Motlagh</a></span>' +
          '</div>' +
          '<nav class="footer-nav" aria-label="Footer navigation">' +
            '<a href="' + pages.home + '" class="footer-nav-link">' + t("footer.home", "Home") + '</a>' +
            '<a href="' + pages.about + '" class="footer-nav-link">' + t("footer.about", "About") + '</a>' +
            '<a href="' + pages.experience + '" class="footer-nav-link">' + t("footer.experience", "Experience") + '</a>' +
            '<a href="' + pages.work + '" class="footer-nav-link">' + t("footer.work", "Work") + '</a>' +
            '<a href="' + pages.blog + '" class="footer-nav-link">' + t("footer.blog", "Blog") + '</a>' +
            '<a href="' + pages.contact + '" class="footer-nav-link">' + t("footer.contact", "Contact") + '</a>' +
          '</nav>' +
          '<div class="footer-social">' +
            '<a href="#" class="footer-social-link" aria-label="LinkedIn"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 8a6 6 0 0 1 6 6v7h-4v-7a2 2 0 0 0-2-2 2 2 0 0 0-2 2v7h-4v-7a6 6 0 0 1 6-6z"/><rect width="4" height="12" x="2" y="9"/><circle cx="4" cy="4" r="2"/></svg></a>' +
            '<a href="#" class="footer-social-link" aria-label="Instagram"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="20" height="20" x="2" y="2" rx="5" ry="5"/><path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"/><line x1="17.5" y1="6.5" x2="17.51" y2="6.5"/></svg></a>' +
            '<a href="#" class="footer-social-link" aria-label="Telegram"><svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M11.944 0A12 12 0 0 0 0 12a12 12 0 0 0 12 12 12 12 0 0 0 12-12A12 12 0 0 0 12 0a12 12 0 0 0-.056 0zm4.962 7.224c.1-.002.321.023.465.14a.506.506 0 0 1 .171.325c.016.093.036.306.02.472-.18 1.898-.962 6.502-1.36 8.627-.168.9-.499 1.201-.82 1.23-.696.065-1.225-.46-1.9-.902-1.056-.693-1.653-1.124-2.678-1.8-1.185-.78-.417-1.21.258-1.91.177-.184 3.247-2.977 3.307-3.23.007-.032.014-.15-.056-.212s-.174-.041-.249-.024c-.106.024-1.793 1.14-5.061 3.345-.48.33-.913.49-1.302.48-.428-.008-1.252-.241-1.865-.44-.752-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.83-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635z"/></svg></a>' +
            '<a href="https://github.com/mousaamiri/" class="footer-social-link" aria-label="GitHub"><svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M15 22v-4a4.8 4.8 0 0 0-1-3.5c3 0 6-2 6-5.5.08-1.25-.27-2.48-1-3.5.28-1.15.28-2.35 0-3.5 0 0-1 0-3 1.5-2.64-.5-5.36-.5-8 0C6 2 5 2 5 2c-.3 1.15-.3 2.35 0 3.5A5.403 5.403 0 0 0 4 9c0 3.5 3 5.5 6 5.5-.39.49-.68 1.05-.85 1.65-.17.6-.22 1.23-.15 1.85v4"/><path d="M9 18c-4.51 2-5-2-7-2"/></svg></a>' +
            '<a href="#" class="footer-social-link" aria-label="X (Twitter)"><svg viewBox="0 0 24 24" fill="currentColor" width="18" height="18"><path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"/></svg></a>' +
          '</div>' +
        '</div>' +
        '<div class="footer-bottom">' +
          '<span>' + t("footer.copyright", "© 2026 Mousa Amiri Motlagh") + '</span>' +
          '<span class="footer-bottom-sep">·</span>' +
          '<a href="#">' + t("footer.privacy", "Privacy") + '</a>' +
          '<span class="footer-bottom-sep">·</span>' +
          '<a href="#">' + t("footer.terms", "Terms") + '</a>' +
          '<span class="footer-bottom-sep">·</span>' +
          '<span class="footer-live-dot">' + t("footer.viewing_now", "1 viewing now") + '</span>' +
        '</div>' +
      '</div>';

    document.body.appendChild(footer);

    var scrollBtn = document.createElement("button");
    scrollBtn.className = "scroll-progress-btn";
    scrollBtn.setAttribute("aria-label", "Scroll to top");
    scrollBtn.textContent = "0%";
    document.body.appendChild(scrollBtn);

    scrollBtn.addEventListener("click", function () {
      window.scrollTo({ top: 0, behavior: "smooth" });
    });

    var ticking = false;
    window.addEventListener("scroll", function () {
      if (!ticking) {
        requestAnimationFrame(function () {
          var scrollTop = window.scrollY;
          var docHeight = document.documentElement.scrollHeight - window.innerHeight;
          var pct = docHeight > 0 ? Math.round((scrollTop / docHeight) * 100) : 0;
          scrollBtn.textContent = pct + "%";
          ticking = false;
        });
        ticking = true;
      }
    }, { passive: true });
  }

  buildFooter();

  window.__rebuildFooter = function () {
    buildFooter();
  };
})();
