/* ==========================================================================
   i18n.js — server-backed language runtime (shim)
   --------------------------------------------------------------------------
   Language is now resolved SERVER-SIDE from the `portfolio-lang` cookie: the
   Razor layout renders <html lang/dir> and injects window.__lang plus the
   DB-sourced UI-chrome map window.__ui. This file preserves the historical
   window.i18n API so the client-built components (navbar/footer/command
   palette/theme switcher) keep working unchanged:
     - lang()            -> the server-resolved language
     - t(key, fallback)  -> window.__ui[key] || fallback
     - setLang(lang)     -> full server round-trip via /set-language (persists
                            the cookie, reloads, re-renders in that language)
   translateElements()/updateLangSwitcher() remain for callers but element text
   is already server-rendered, so they are effectively no-ops (the lang button
   label is still refreshed for safety).
   ========================================================================== */
(function () {
  "use strict";

  var ui = window.__ui || {};
  var currentLang = window.__lang || "en";

  function lang() { return currentLang; }

  function t(key, fallback) {
    if (currentLang !== "en" && ui[key]) return ui[key];
    return fallback || key;
  }

  function setLang(newLang) {
    // Server round-trip: persist the cookie and reload so both dynamic content
    // and UI chrome render in the chosen language.
    var returnUrl = window.location.pathname + window.location.search;
    window.location.href =
      "/set-language?lang=" + encodeURIComponent(newLang) +
      "&returnUrl=" + encodeURIComponent(returnUrl);
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

  function translateElements() { /* no-op: text is server-rendered */ }

  window.i18n = {
    lang: lang,
    t: t,
    setLang: setLang,
    translateElements: translateElements,
    updateLangSwitcher: updateLangSwitcher,
  };
})();
