/* ==========================================================================
   theme-switcher.js — Floating accent-color picker component
   --------------------------------------------------------------------------
   Injects a fixed palette button with a popover of color swatches.
   Persists the chosen accent in localStorage and applies it via CSS
   custom properties on :root.
   ========================================================================== */
(function () {
  'use strict';

  var STORAGE_KEY = 'portfolio-accent';

  var THEMES = [
    { name: 'amber',   color: '#f0a63b', soft: 'rgba(240,166,59,0.14)',  strong: '#ffc069'  },
    { name: 'fuchsia', color: '#d946ef', soft: 'rgba(217,70,239,0.14)',  strong: '#e879f9'  },
    { name: 'indigo',  color: '#5b6bff', soft: 'rgba(91,107,255,0.14)',  strong: '#818cf8'  },
    { name: 'pink',    color: '#ec4899', soft: 'rgba(236,72,153,0.14)',  strong: '#f472b6'  },
    { name: 'emerald', color: '#10d9a3', soft: 'rgba(16,217,163,0.14)', strong: '#34d399'  }
  ];

  var PALETTE_SVG =
    '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">' +
      '<circle cx="13.5" cy="6.5" r="0.5" fill="currentColor"/>' +
      '<circle cx="17.5" cy="10.5" r="0.5" fill="currentColor"/>' +
      '<circle cx="8.5" cy="7.5" r="0.5" fill="currentColor"/>' +
      '<circle cx="6.5" cy="12" r="0.5" fill="currentColor"/>' +
      '<path d="M12 2C6.5 2 2 6.5 2 12s4.5 10 10 10c.926 0 1.648-.746 1.648-1.688 0-.437-.18-.835-.437-1.125-.29-.289-.438-.652-.438-1.125a1.64 1.64 0 0 1 1.668-1.668h1.996c3.051 0 5.563-2.503 5.563-5.563C22 6.5 17.5 2 12 2Z"/>' +
    '</svg>';

  function getStored() {
    try { return localStorage.getItem(STORAGE_KEY); } catch (e) { return null; }
  }

  function setStored(name) {
    try { localStorage.setItem(STORAGE_KEY, name); } catch (e) { /* noop */ }
  }

  function findTheme(name) {
    for (var i = 0; i < THEMES.length; i++) {
      if (THEMES[i].name === name) return THEMES[i];
    }
    return THEMES[0];
  }

  function applyTheme(theme, animate) {
    var root = document.documentElement;

    if (animate) {
      root.classList.add('accent-transitioning');
      setTimeout(function () {
        root.classList.remove('accent-transitioning');
      }, 600);
    }

    root.style.setProperty('--accent', theme.color);
    root.style.setProperty('--accent-soft', theme.soft);
    root.style.setProperty('--accent-strong', theme.strong);
  }

  function buildUI() {
    var wrapper = document.createElement('div');
    wrapper.className = 'theme-switcher';
    wrapper.setAttribute('aria-label', 'Accent color picker');

    var btn = document.createElement('button');
    btn.className = 'theme-switcher-btn';
    btn.setAttribute('aria-expanded', 'false');
    btn.setAttribute('aria-haspopup', 'true');
    btn.setAttribute('title', 'Change accent color');
    btn.innerHTML = PALETTE_SVG;

    var popover = document.createElement('div');
    popover.className = 'theme-switcher-popover';
    popover.setAttribute('role', 'menu');

    var title = document.createElement('span');
    title.className = 'theme-switcher-popover-title';
    title.textContent = (window.i18n && window.i18n.lang() === 'fa')
      ? window.i18n.t('theme.accent_color', 'Accent Color')
      : 'Accent Color';

    var colorsRow = document.createElement('div');
    colorsRow.className = 'theme-switcher-colors';

    var currentName = getStored() || THEMES[0].name;

    THEMES.forEach(function (theme) {
      var swatch = document.createElement('button');
      swatch.className = 'theme-color-swatch';
      if (theme.name === currentName) swatch.classList.add('active');
      swatch.style.background = theme.color;
      swatch.setAttribute('role', 'menuitem');
      swatch.setAttribute('aria-label', theme.name + ' accent');
      swatch.setAttribute('data-accent', theme.name);
      colorsRow.appendChild(swatch);
    });

    popover.appendChild(title);
    popover.appendChild(colorsRow);
    wrapper.appendChild(btn);
    wrapper.appendChild(popover);

    btn.addEventListener('click', function (e) {
      e.stopPropagation();
      var isOpen = wrapper.classList.toggle('open');
      btn.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    });

    colorsRow.addEventListener('click', function (e) {
      var swatch = e.target.closest('.theme-color-swatch');
      if (!swatch) return;

      var name = swatch.getAttribute('data-accent');
      var theme = findTheme(name);

      var prev = colorsRow.querySelector('.active');
      if (prev) prev.classList.remove('active');
      swatch.classList.add('active');

      applyTheme(theme, true);
      setStored(name);

      wrapper.classList.remove('open');
      btn.setAttribute('aria-expanded', 'false');
    });

    document.addEventListener('click', function (e) {
      if (!wrapper.contains(e.target)) {
        wrapper.classList.remove('open');
        btn.setAttribute('aria-expanded', 'false');
      }
    });

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && wrapper.classList.contains('open')) {
        wrapper.classList.remove('open');
        btn.setAttribute('aria-expanded', 'false');
        btn.focus();
      }
    });

    document.body.appendChild(wrapper);
  }

  var stored = getStored();
  if (stored) {
    applyTheme(findTheme(stored), false);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', buildUI);
  } else {
    buildUI();
  }
})();
