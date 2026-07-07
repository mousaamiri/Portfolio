/* ==========================================================================
   i18n.js — Language manager for EN/FA bilingual portfolio
   --------------------------------------------------------------------------
   Must be loaded AFTER translations.js.
   Provides: window.i18n.lang(), window.i18n.t(key), window.i18n.setLang(lang)
   ========================================================================== */
(function () {
  "use strict";

  var STORAGE_KEY = "portfolio-lang";
  var translations = (window.__translations && window.__translations.fa) || {};

  function getStored() {
    try { return localStorage.getItem(STORAGE_KEY); } catch (e) { return null; }
  }

  function setStored(lang) {
    try { localStorage.setItem(STORAGE_KEY, lang); } catch (e) { /* noop */ }
  }

  var currentLang = getStored() || "en";

  function t(key, fallback) {
    if (currentLang === "fa" && translations[key]) {
      return translations[key];
    }
    return fallback || key;
  }

  function applyDirection() {
    var html = document.documentElement;
    if (currentLang === "fa") {
      html.setAttribute("dir", "rtl");
      html.setAttribute("lang", "fa");
      document.body.classList.add("is-rtl");
      document.body.classList.remove("is-ltr");
    } else {
      html.setAttribute("dir", "ltr");
      html.setAttribute("lang", "en");
      document.body.classList.add("is-ltr");
      document.body.classList.remove("is-rtl");
    }
  }

  function applyFont() {
    /* Font switching is handled by rtl.css [dir="rtl"] selectors.
       No inline style override needed — that would clobber CSS specificity. */
  }

  function translateElements() {
    var els = document.querySelectorAll("[data-i18n]");
    els.forEach(function (el) {
      var key = el.getAttribute("data-i18n");
      if (currentLang === "fa" && translations[key]) {
        if (!el.hasAttribute("data-i18n-original")) {
          el.setAttribute("data-i18n-original", el.innerHTML);
        }
        el.innerHTML = translations[key];
      } else {
        var original = el.getAttribute("data-i18n-original");
        if (original !== null) {
          el.innerHTML = original;
        }
      }
    });

    var placeholderEls = document.querySelectorAll("[data-i18n-placeholder]");
    placeholderEls.forEach(function (el) {
      var key = el.getAttribute("data-i18n-placeholder");
      if (currentLang === "fa" && translations[key]) {
        if (!el.hasAttribute("data-i18n-placeholder-original")) {
          el.setAttribute("data-i18n-placeholder-original", el.placeholder);
        }
        el.placeholder = translations[key];
      } else {
        var original = el.getAttribute("data-i18n-placeholder-original");
        if (original !== null) {
          el.placeholder = original;
        }
      }
    });

    var titleEls = document.querySelectorAll("[data-i18n-title]");
    titleEls.forEach(function (el) {
      var key = el.getAttribute("data-i18n-title");
      if (currentLang === "fa" && translations[key]) {
        if (!el.hasAttribute("data-i18n-title-original")) {
          el.setAttribute("data-i18n-title-original", document.title);
        }
        document.title = translations[key];
      } else {
        var original = el.getAttribute("data-i18n-title-original");
        if (original !== null) {
          document.title = original;
        }
      }
    });
  }

  function rebuildComponents() {
    if (typeof window.__rebuildNavbar === "function") {
      window.__rebuildNavbar();
    }
    if (typeof window.__rebuildFooter === "function") {
      window.__rebuildFooter();
    }
    if (typeof window.__rebuildCommandPalette === "function") {
      window.__rebuildCommandPalette();
    }
    if (typeof window.__rebuildWorkProjects === "function") {
      window.__rebuildWorkProjects();
    }
  }

  function updateLangSwitcher() {
    var btns = document.querySelectorAll(".lang-switch-btn");
    btns.forEach(function (btn) {
      if (currentLang === "fa") {
        btn.textContent = "EN";
        btn.setAttribute("aria-label", "Switch to English");
      } else {
        btn.textContent = "فا";
        btn.setAttribute("aria-label", "تغییر زبان به فارسی");
      }
    });
  }

  function setLang(lang) {
    currentLang = lang;
    setStored(lang);
    applyDirection();
    applyFont();
    rebuildComponents();
    translateElements();
    updateLangSwitcher();

    window.dispatchEvent(new CustomEvent("langchange", { detail: { lang: lang } }));
  }

  function init() {
    applyDirection();
    applyFont();
  }

  init();

  window.i18n = {
    lang: function () { return currentLang; },
    t: t,
    setLang: setLang,
    translateElements: translateElements,
    updateLangSwitcher: updateLangSwitcher,
  };
})();
