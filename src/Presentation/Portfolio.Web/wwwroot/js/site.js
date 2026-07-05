(function () {
    'use strict';

    var THEME_KEY = 'portfolio-theme';
    var LANG_KEY = 'portfolio-lang';

    function initTheme() {
        var saved = localStorage.getItem(THEME_KEY);
        if (saved === 'dark' || (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
            document.documentElement.setAttribute('data-theme', 'dark');
        }
    }

    function toggleTheme() {
        var html = document.documentElement;
        var isDark = html.getAttribute('data-theme') === 'dark';
        if (isDark) {
            html.removeAttribute('data-theme');
            localStorage.setItem(THEME_KEY, 'light');
        } else {
            html.setAttribute('data-theme', 'dark');
            localStorage.setItem(THEME_KEY, 'dark');
        }
        updateThemeIcon();
    }

    function updateThemeIcon() {
        var btn = document.getElementById('theme-toggle');
        if (!btn) return;
        var isDark = document.documentElement.getAttribute('data-theme') === 'dark';
        btn.innerHTML = isDark
            ? '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="5"/><line x1="12" y1="1" x2="12" y2="3"/><line x1="12" y1="21" x2="12" y2="23"/><line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/><line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/><line x1="1" y1="12" x2="3" y2="12"/><line x1="21" y1="12" x2="23" y2="12"/><line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/><line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/></svg>'
            : '<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/></svg>';
        btn.setAttribute('aria-label', isDark ? 'Switch to light mode' : 'Switch to dark mode');
    }

    function toggleLang() {
        var html = document.documentElement;
        var isRtl = html.getAttribute('dir') === 'rtl';
        if (isRtl) {
            html.setAttribute('dir', 'ltr');
            html.setAttribute('lang', 'en');
            localStorage.setItem(LANG_KEY, 'en');
        } else {
            html.setAttribute('dir', 'rtl');
            html.setAttribute('lang', 'fa');
            localStorage.setItem(LANG_KEY, 'fa');
        }
        updateLangButton();
    }

    function updateLangButton() {
        var btn = document.getElementById('lang-toggle');
        if (!btn) return;
        var lang = document.documentElement.getAttribute('lang') || 'en';
        btn.textContent = lang === 'fa' ? 'EN' : 'FA';
        btn.setAttribute('aria-label', lang === 'fa' ? 'Switch to English' : 'تغییر به فارسی');
    }

    function initLang() {
        var saved = localStorage.getItem(LANG_KEY);
        if (saved === 'fa') {
            document.documentElement.setAttribute('dir', 'rtl');
            document.documentElement.setAttribute('lang', 'fa');
        }
    }

    function initSmoothScroll() {
        document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
            anchor.addEventListener('click', function (e) {
                var targetId = this.getAttribute('href');
                if (targetId === '#') return;
                var target = document.querySelector(targetId);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth', block: 'start' });
                }
            });
        });
    }

    initTheme();
    initLang();

    document.addEventListener('DOMContentLoaded', function () {
        updateThemeIcon();
        updateLangButton();
        initSmoothScroll();

        var themeBtn = document.getElementById('theme-toggle');
        if (themeBtn) themeBtn.addEventListener('click', toggleTheme);

        var langBtn = document.getElementById('lang-toggle');
        if (langBtn) langBtn.addEventListener('click', toggleLang);
    });
})();
