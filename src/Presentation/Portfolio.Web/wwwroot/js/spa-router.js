/* ==========================================================================
   spa-router.js — Production-grade client-side SPA navigation
   --------------------------------------------------------------------------
   Transforms the multi-page portfolio into a single-page experience.
   Header and footer load once; only <main id="page-content"> is swapped.

   Features:
     • AJAX navigation with History API (back / forward / refresh / deep link)
     • LRU page cache (20 entries)
     • View Transition API with CSS fallback
     • NProgress-style loading bar (GPU-accelerated)
     • Prefetch on hover + IntersectionObserver viewport prefetch
     • Scroll position save / restore on back / forward
     • SEO meta-tag propagation (title, OG, canonical, JSON-LD)
     • Accessibility: ARIA live-region announcements + focus management
     • AbortController request deduplication
     • Language & theme state preservation (no FOUC)
     • Graceful fallback to full reload on error
   ========================================================================== */
(function () {
  'use strict';

  if (window.location.protocol === 'file:') return;

  /* ======================== CONFIGURATION ======================== */

  var MAIN_SELECTOR = '#page-content';
  var CACHE_LIMIT   = 20;
  var HOVER_DELAY   = 65;
  var LEAVE_MS      = 150;
  var ENTER_MS      = 250;
  var TRICKLE_MS    = 220;
  var TRICKLE_SEED  = 0.08;

  var SHARED_SCRIPTS = [
    'i18n.js', 'navbar-component.js', 'text-reveal.js',
    'navbar.js', 'command-palette.js', 'footer-component.js', 'theme-switcher.js',
    'spa-router.js', 'site.js'
  ];

  var SHARED_CSS = [
    // MVC migration: shared shell stylesheets live in /css/ now. Everything
    // NOT in this list is treated as page-specific and swapped per navigation.
    'tokens.css', 'base.css', 'layout.css', 'components.css',
    'style.css', 'navbar.css', 'footer.css', 'rtl.css',
    'theme-switcher.css', 'spa-transitions.css'
  ];


  /* ======================== LRU CACHE ======================== */

  var cacheMap = new Map();

  function cacheGet(key) {
    if (!cacheMap.has(key)) return undefined;
    var val = cacheMap.get(key);
    cacheMap.delete(key);
    cacheMap.set(key, val);
    return val;
  }

  function cacheSet(key, val) {
    if (cacheMap.has(key)) cacheMap.delete(key);
    cacheMap.set(key, val);
    if (cacheMap.size > CACHE_LIMIT) {
      cacheMap.delete(cacheMap.keys().next().value);
    }
  }

  function cacheHas(key) {
    return cacheMap.has(key);
  }


  /* ======================== PROGRESS BAR ======================== */

  var progressEl     = null;
  var progressBarEl  = null;
  var progressValue  = 0;
  var progressTimer  = null;
  var progressActive = false;

  function progressCreate() {
    if (progressEl) return;
    progressEl = document.createElement('div');
    progressEl.className = 'spa-progress';
    progressEl.setAttribute('role', 'progressbar');
    progressEl.setAttribute('aria-valuemin', '0');
    progressEl.setAttribute('aria-valuemax', '100');
    progressBarEl = document.createElement('div');
    progressBarEl.className = 'spa-progress-bar';
    progressEl.appendChild(progressBarEl);
    document.body.appendChild(progressEl);
  }

  function progressSet(n) {
    progressValue = Math.max(0, Math.min(1, n));
    if (progressBarEl) {
      progressBarEl.style.transform = 'scaleX(' + progressValue + ')';
    }
    if (progressEl) {
      progressEl.setAttribute('aria-valuenow', Math.round(progressValue * 100));
    }
  }

  function progressInc() {
    var amount;
    if      (progressValue < 0.2)  amount = 0.08 + Math.random() * 0.04;
    else if (progressValue < 0.5)  amount = 0.03 + Math.random() * 0.03;
    else if (progressValue < 0.8)  amount = 0.01 + Math.random() * 0.02;
    else if (progressValue < 0.99) amount = 0.003;
    else return;
    progressSet(progressValue + amount);
  }

  function progressStart() {
    progressCreate();
    progressActive = true;
    progressSet(TRICKLE_SEED);
    progressEl.classList.add('active');
    clearInterval(progressTimer);
    progressTimer = setInterval(progressInc, TRICKLE_MS);
  }

  function progressDone() {
    if (!progressActive) return;
    clearInterval(progressTimer);
    progressTimer = null;
    progressSet(1);
    progressActive = false;
    setTimeout(function () {
      if (progressEl) progressEl.classList.remove('active');
      setTimeout(function () { progressSet(0); }, 400);
    }, 300);
  }

  function progressReset() {
    clearInterval(progressTimer);
    progressTimer = null;
    progressActive = false;
    if (progressEl) progressEl.classList.remove('active');
    progressSet(0);
  }


  /* ======================== ARIA ANNOUNCER ======================== */

  var announcerEl = null;

  function announceCreate() {
    if (announcerEl) return;
    announcerEl = document.createElement('div');
    announcerEl.className = 'spa-announcer';
    announcerEl.setAttribute('role', 'status');
    announcerEl.setAttribute('aria-live', 'polite');
    announcerEl.setAttribute('aria-atomic', 'true');
    document.body.appendChild(announcerEl);
  }

  function announce(message) {
    announceCreate();
    announcerEl.textContent = '';
    requestAnimationFrame(function () {
      announcerEl.textContent = message;
    });
  }


  /* ======================== SCROLL MANAGER ======================== */

  var scrollPositions = {};

  function scrollSave(url) {
    scrollPositions[url] = window.scrollY;
  }

  function scrollRestore(url) {
    var pos = scrollPositions[url];
    if (typeof pos === 'number') {
      requestAnimationFrame(function () { window.scrollTo(0, pos); });
    }
  }


  /* ======================== SEO MANAGER ======================== */

  function updateMeta(doc) {
    // Skip if the cached doc IS the live document (initial page)
    if (doc === document) return;

    var titleEl = doc.querySelector('title');
    if (titleEl) document.title = titleEl.textContent;

    var selectors = [
      'meta[name="description"]',
      'meta[name="keywords"]',
      'meta[property="og:title"]',
      'meta[property="og:description"]',
      'meta[property="og:url"]',
      'meta[property="og:image"]',
      'meta[property="og:type"]',
      'meta[name="twitter:title"]',
      'meta[name="twitter:description"]',
      'meta[name="twitter:image"]'
    ];

    selectors.forEach(function (sel) {
      var newTag = doc.querySelector(sel);
      var oldTag = document.querySelector(sel);
      if (newTag) {
        if (oldTag) {
          oldTag.setAttribute('content', newTag.getAttribute('content'));
        } else {
          document.head.appendChild(newTag.cloneNode(true));
        }
      } else if (oldTag) {
        oldTag.remove();
      }
    });

    var newCanonical = doc.querySelector('link[rel="canonical"]');
    var oldCanonical = document.querySelector('link[rel="canonical"]');
    if (newCanonical) {
      if (oldCanonical) {
        oldCanonical.setAttribute('href', newCanonical.getAttribute('href'));
      } else {
        document.head.appendChild(newCanonical.cloneNode(true));
      }
    } else if (oldCanonical) {
      oldCanonical.remove();
    }

    document.querySelectorAll('script[type="application/ld+json"]')
      .forEach(function (s) { s.remove(); });
    doc.querySelectorAll('script[type="application/ld+json"]')
      .forEach(function (s) { document.head.appendChild(s.cloneNode(true)); });
  }


  /* ======================== UTILITIES ======================== */

  function getFilename(path) {
    return path.split('/').pop().split('?')[0].split('#')[0];
  }

  function isSharedScript(src) {
    var name = getFilename(src);
    return SHARED_SCRIPTS.indexOf(name) !== -1 ||
           src.indexOf('cdn.jsdelivr.net') !== -1 ||
           src.indexOf('unpkg.com') !== -1;
  }

  function isSharedCSS(href) {
    var name = getFilename(href);
    return SHARED_CSS.indexOf(name) !== -1 ||
           href.indexOf('fonts.googleapis.com') !== -1 ||
           href.indexOf('cdn.jsdelivr.net') !== -1;
  }

  function isInternalLink(href) {
    if (!href) return false;
    if (href.charAt(0) === '#' || href.indexOf('javascript:') === 0) return false;
    if (href.indexOf('mailto:') === 0 || href.indexOf('tel:') === 0) return false;
    if (href.indexOf('http') === 0 && href.indexOf(window.location.host) === -1) return false;
    if (/\.(pdf|zip|png|jpg|jpeg|gif|svg|webp|ico|mp4|webm)$/i.test(href)) return false;
    // MVC migration: routes are extensionless (e.g. "/About"), not "*.html".
    // Accept same-origin app routes; exclude the admin area (its own shell)
    // and any explicit opt-out via data-no-spa.
    var path = resolveURL(href).replace(/^https?:\/\/[^/]+/, '');
    if (path.indexOf('/admin') === 0 || path.indexOf('/Admin') === 0) return false;
    return true;
  }

  function resolveURL(href) {
    var a = document.createElement('a');
    a.href = href;
    return a.href;
  }

  function cleanupGSAP() {
    if (typeof gsap !== 'undefined') {
      if (typeof ScrollTrigger !== 'undefined') {
        ScrollTrigger.getAll().forEach(function (t) { t.kill(); });
        ScrollTrigger.refresh();
      }
      gsap.globalTimeline.clear();
    }
  }

  function wait(ms) {
    return new Promise(function (resolve) { setTimeout(resolve, ms); });
  }

  function resetTransitionClasses(main) {
    main.classList.remove('spa-leave', 'spa-enter-prep', 'spa-enter');
    main.removeAttribute('aria-busy');
  }


  /* ======================== PAGE PARSER ======================== */

  function extractPageName(doc) {
    var scripts = doc.querySelectorAll('script');
    for (var i = 0; i < scripts.length; i++) {
      var text = scripts[i].textContent;
      var match = text.match(/currentPage\s*:\s*['"](\w+)['"]/);
      if (match) return match[1];
    }
    return 'home';
  }

  function parsePage(html, url) {
    var parser = new DOMParser();
    var doc = parser.parseFromString(html, 'text/html');

    var mainContent = doc.querySelector('main' + MAIN_SELECTOR);
    if (!mainContent) {
      var sections = doc.querySelectorAll(
        'body > section, body > div.closing-cta-block, body > .blog-modal-overlay'
      );
      if (sections.length === 0) return null;
      mainContent = document.createElement('main');
      mainContent.id = MAIN_SELECTOR.replace('#', '');
      sections.forEach(function (s) { mainContent.appendChild(s.cloneNode(true)); });
    }

    var baseURL = url.substring(0, url.lastIndexOf('/') + 1);

    var pageCSS = [];
    doc.querySelectorAll('link[rel="stylesheet"]').forEach(function (link) {
      var href = link.getAttribute('href');
      if (href && !isSharedCSS(href)) {
        if (href.indexOf('http') !== 0) href = new URL(href, baseURL).href;
        pageCSS.push(href);
      }
    });

    var pageScripts = [];
    var cdnScripts  = [];
    doc.querySelectorAll('script[src]').forEach(function (script) {
      var src = script.getAttribute('src');
      if (!src) return;
      if (src.indexOf('unpkg.com') !== -1 ||
          (src.indexOf('cdn.jsdelivr.net') !== -1 && src.indexOf('gsap') === -1)) {
        cdnScripts.push(src.indexOf('http') === 0 ? src : new URL(src, baseURL).href);
      } else if (!isSharedScript(src) && src.indexOf('gsap') === -1) {
        if (src.indexOf('http') !== 0) src = new URL(src, baseURL).href;
        pageScripts.push(src);
      }
    });

    var inlineScripts = [];
    doc.querySelectorAll('script:not([src])').forEach(function (script) {
      var text = script.textContent.trim();
      if (text &&
          text.indexOf('__navConfig') === -1 &&
          text.indexOf('i18n.translateElements') === -1 &&
          text.indexOf('portfolio-theme') === -1 &&
          text.indexOf('portfolio-lang') === -1) {
        inlineScripts.push(text);
      }
    });

    var title    = doc.querySelector('title') ? doc.querySelector('title').textContent : '';
    var pageName = extractPageName(doc);
    var bodyEl   = doc.querySelector('body');
    var i18nTitle = bodyEl ? (bodyEl.getAttribute('data-i18n-title') || '') : '';

    return {
      content:       mainContent,
      css:           pageCSS,
      scripts:       pageScripts,
      cdnScripts:    cdnScripts,
      inlineScripts: inlineScripts,
      title:         title,
      pageName:      pageName,
      i18nTitle:     i18nTitle,
      doc:           doc
    };
  }


  /* ======================== ASSET LOADER ======================== */

  var loadedPageCSS     = [];
  var loadedPageScripts = [];

  function loadCSS(href) {
    return new Promise(function (resolve) {
      if (document.querySelector('link[href="' + href + '"]')) {
        resolve(); return;
      }
      var link = document.createElement('link');
      link.rel  = 'stylesheet';
      link.href = href;
      link.onload  = resolve;
      link.onerror = resolve;
      document.head.appendChild(link);
      loadedPageCSS.push(link);
    });
  }

  function loadScript(src) {
    return new Promise(function (resolve) {
      if (document.querySelector('script[src="' + src + '"]')) {
        resolve(); return;
      }
      var script = document.createElement('script');
      script.src = src;
      script.onload  = resolve;
      script.onerror = resolve;
      document.body.appendChild(script);
      loadedPageScripts.push(script);
    });
  }

  function removeOldCSS(refs) {
    refs.forEach(function (link) {
      if (link.parentNode) link.parentNode.removeChild(link);
    });
  }

  function unloadPageScripts() {
    loadedPageScripts.forEach(function (script) {
      if (script.parentNode) script.parentNode.removeChild(script);
    });
    loadedPageScripts = [];
  }


  /* ======================== TRANSITION ENGINE ======================== */

  var supportsViewTransition = typeof document.startViewTransition === 'function';

  function transitionOut(main) {
    if (supportsViewTransition) return Promise.resolve();
    main.classList.add('spa-leave');
    return wait(LEAVE_MS);
  }

  function transitionIn(main) {
    if (supportsViewTransition) return Promise.resolve();
    main.classList.remove('spa-leave');
    main.classList.add('spa-enter-prep');
    void main.offsetHeight; // force reflow before adding enter class
    main.classList.add('spa-enter');
    return wait(ENTER_MS).then(function () {
      main.classList.remove('spa-enter-prep', 'spa-enter');
    });
  }


  /* ======================== CORE ROUTER ======================== */

  var activeController = null;
  var navigating       = false;

  function fetchHTML(url, signal) {
    return new Promise(function (resolve, reject) {
      var xhr = new XMLHttpRequest();
      xhr.open('GET', url, true);
      xhr.onload = function () {
        if (xhr.status === 0 || (xhr.status >= 200 && xhr.status < 300)) {
          resolve(xhr.responseText);
        } else {
          reject(new Error('HTTP ' + xhr.status));
        }
      };
      xhr.onerror = function () { reject(new Error('Network error')); };
      if (signal) {
        signal.addEventListener('abort', function () {
          xhr.abort();
        });
      }
      xhr.send();
    });
  }

  function fetchPage(url, signal) {
    var cached = cacheGet(url);
    if (cached) return Promise.resolve(cached);

    return fetchHTML(url, signal)
      .then(function (html) {
        var page = parsePage(html, url);
        if (!page) throw new Error('Parse failed');
        cacheSet(url, page);
        return page;
      });
  }

  function swapContent(page, main) {
    document.body.style.overflow = '';

    var staleOverlay = document.getElementById('blog-modal-overlay');
    if (staleOverlay) staleOverlay.remove();

    cleanupGSAP();
    unloadPageScripts();

    window.__navConfig = window.__navConfig || {};
    window.__navConfig.currentPage = page.pageName;

    document.querySelectorAll('[data-reveal-applied]').forEach(function (el) {
      el.removeAttribute('data-reveal-applied');
    });

    main.innerHTML = page.content.innerHTML;

    if (window.i18n) {
      window.i18n.translateElements();
      window.i18n.updateLangSwitcher();
    }

    document.title = page.title || document.title;
    if (page.i18nTitle) {
      document.body.setAttribute('data-i18n-title', page.i18nTitle);
    }

    updateMeta(page.doc);
  }

  function loadPageScriptsAndInline(page) {
    // CDN scripts: load only if not already present (e.g. Lucide)
    var cdnP = page.cdnScripts.map(function (src) {
      if (document.querySelector('script[src="' + src + '"]')) return Promise.resolve();
      return loadScript(src);
    });

    return Promise.all(cdnP).then(function () {
      // Page scripts: always re-execute by removing the old tag first
      var chain = Promise.resolve();
      page.scripts.forEach(function (src) {
        chain = chain.then(function () {
          var existing = document.querySelector('script[src="' + src + '"]');
          if (existing) existing.remove();
          return loadScript(src);
        });
      });
      return chain;
    }).then(function () {
      page.inlineScripts.forEach(function (code) {
        try { new Function(code)(); } catch (e) { /* skip */ }
      });
    });
  }

  function rebuildSharedComponents() {
    if (typeof window.__rebuildNavbar === 'function') window.__rebuildNavbar();
    if (typeof window.__rebuildFooter === 'function') window.__rebuildFooter();
    if (typeof window.__rebuildCommandPalette === 'function') window.__rebuildCommandPalette();

    if (window.i18n) {
      window.i18n.translateElements();
      window.i18n.updateLangSwitcher();
    }

    if (typeof lucide !== 'undefined' && lucide.createIcons) {
      lucide.createIcons();
    }
  }

  function navigateTo(url, isPush) {
    // Abort any in-flight navigation
    if (activeController) {
      try { activeController.abort(); } catch (e) { /* ignore */ }
      activeController = null;
    }

    var main = document.querySelector(MAIN_SELECTOR);
    if (!main) { window.location.href = url; return; }

    if (url === window.location.href) return;

    navigating = true;
    var controller = null;
    try { controller = new AbortController(); } catch (e) { /* older browser */ }
    activeController = controller;
    var signal = controller ? controller.signal : undefined;

    scrollSave(window.location.href);
    progressStart();
    main.setAttribute('aria-busy', 'true');

    // Snapshot old CSS links so we can remove them after the swap
    var oldCSS = loadedPageCSS.slice();
    loadedPageCSS = [];

    // Fetch page + transition out run in parallel
    var pagePromise  = fetchPage(url, signal);
    var leavePromise = transitionOut(main);

    var aborted = function () { return signal && signal.aborted; };

    Promise.all([pagePromise, leavePromise]).then(function (results) {
      if (aborted()) return;
      var page = results[0];

      // Load new CSS while old content is still hidden (opacity 0)
      return Promise.all(page.css.map(loadCSS)).then(function () {
        if (aborted()) return;

        function doSwap() {
          swapContent(page, main);
        }

        var swapDone;
        if (supportsViewTransition) {
          var vt = document.startViewTransition(doSwap);
          swapDone = vt.finished.catch(function () {
            // View transition can fail on rapid navigation — not fatal
          });
        } else {
          doSwap();
          swapDone = transitionIn(main);
        }

        return swapDone;

      }).then(function () {
        if (aborted()) return;

        // Remove old page CSS after transition completes
        removeOldCSS(oldCSS);

        return loadPageScriptsAndInline(page);

      }).then(function () {
        if (aborted()) return;

        rebuildSharedComponents();

        if (isPush !== false) {
          history.pushState({ spaURL: url }, '', url);
        }

        // Focus management
        main.removeAttribute('aria-busy');
        main.setAttribute('tabindex', '-1');
        main.focus({ preventScroll: true });
        main.removeAttribute('tabindex');

        // Scroll management
        if (isPush !== false) {
          window.scrollTo({ top: 0, left: 0 });
        } else {
          scrollRestore(url);
        }

        announce(document.title);
        progressDone();
        navigating = false;
        activeController = null;

        prefetchObserve();
      });

    }).catch(function (err) {
      if (err && err.name === 'AbortError') {
        resetTransitionClasses(main);
        progressReset();
        navigating = false;
        return;
      }
      // Unrecoverable — fall back to full page load
      progressReset();
      resetTransitionClasses(main);
      window.location.href = url;
    });
  }


  /* ======================== PREFETCH ENGINE ======================== */

  var prefetchedURLs   = new Set();
  var hoverTimer       = null;
  var prefetchObserver = null;
  var MAX_PREFETCH     = 10;
  var prefetchCount    = 0;

  function prefetchURL(url) {
    if (prefetchedURLs.has(url) || cacheHas(url) || prefetchCount >= MAX_PREFETCH) return;
    prefetchedURLs.add(url);
    prefetchCount++;

    fetchHTML(url)
      .then(function (html) {
        var page = parsePage(html, url);
        if (page) cacheSet(url, page);
      })
      .catch(function () {
        prefetchedURLs.delete(url);
        prefetchCount--;
      });
  }

  function getPrefetchURL(link) {
    var href = link.getAttribute('href');
    if (!href || !isInternalLink(href)) return null;
    if (link.getAttribute('target') === '_blank') return null;
    if (link.hasAttribute('download')) return null;
    if (link.hasAttribute('data-no-prefetch')) return null;
    return resolveURL(href);
  }

  function onLinkHoverIn(e) {
    var link = e.target.closest('a');
    if (!link) return;
    var url = getPrefetchURL(link);
    if (!url || prefetchedURLs.has(url) || cacheHas(url)) return;
    clearTimeout(hoverTimer);
    hoverTimer = setTimeout(function () { prefetchURL(url); }, HOVER_DELAY);
  }

  function onLinkHoverOut() {
    clearTimeout(hoverTimer);
  }

  function onTouchStart(e) {
    var link = e.target.closest('a');
    if (!link) return;
    var url = getPrefetchURL(link);
    if (url) prefetchURL(url);
  }

  function prefetchObserve() {
    if (!('IntersectionObserver' in window)) return;
    if (prefetchObserver) prefetchObserver.disconnect();

    prefetchObserver = new IntersectionObserver(function (entries) {
      entries.forEach(function (entry) {
        if (!entry.isIntersecting) return;
        var url = getPrefetchURL(entry.target);
        if (!url) return;
        if ('requestIdleCallback' in window) {
          requestIdleCallback(function () { prefetchURL(url); });
        } else {
          setTimeout(function () { prefetchURL(url); }, 200);
        }
        prefetchObserver.unobserve(entry.target);
      });
    }, { rootMargin: '200px' });

    document.querySelectorAll('a[href]').forEach(function (link) {
      var url = getPrefetchURL(link);
      if (url && !prefetchedURLs.has(url) && !cacheHas(url)) {
        prefetchObserver.observe(link);
      }
    });
  }


  /* ======================== EVENT HANDLERS ======================== */

  document.addEventListener('click', function (e) {
    var link = e.target.closest('a');
    if (!link) return;

    var href = link.getAttribute('href');
    if (!href || !isInternalLink(href)) return;
    if (e.ctrlKey || e.metaKey || e.shiftKey || e.altKey) return;
    if (link.getAttribute('target') === '_blank') return;
    if (link.hasAttribute('download')) return;

    var resolvedURL = resolveURL(href);
    if (resolvedURL === window.location.href) { e.preventDefault(); return; }

    e.preventDefault();

    // Close mobile menu if open
    var mobilePanel = document.querySelector('.navbar-mobile-panel');
    if (mobilePanel && mobilePanel.classList.contains('open')) {
      var hamburger = document.querySelector('.navbar-hamburger');
      if (hamburger) hamburger.click();
    }

    navigateTo(resolvedURL, true);
  }, { passive: false });

  window.addEventListener('popstate', function () {
    navigateTo(window.location.href, false);
  });

  document.addEventListener('mouseenter', onLinkHoverIn, { passive: true, capture: true });
  document.addEventListener('mouseleave', onLinkHoverOut, { passive: true, capture: true });
  document.addEventListener('touchstart', onTouchStart, { passive: true });


  /* ======================== INITIALIZATION ======================== */

  history.replaceState({ spaURL: window.location.href }, '', window.location.href);

  progressCreate();
  announceCreate();

  // Cache the initial page so back-navigation is instant
  (function () {
    var main = document.querySelector(MAIN_SELECTOR);
    if (!main) return;
    var pageName = (window.__navConfig && window.__navConfig.currentPage) || 'home';
    var bodyEl   = document.querySelector('body');

    // Collect page-specific scripts from the live document
    var initialScripts = [];
    var initialCDN     = [];
    document.querySelectorAll('script[src]').forEach(function (script) {
      var src = script.getAttribute('src');
      if (!src) return;
      var abs = resolveURL(src);
      if (src.indexOf('unpkg.com') !== -1 ||
          (src.indexOf('cdn.jsdelivr.net') !== -1 && src.indexOf('gsap') === -1)) {
        initialCDN.push(abs);
      } else if (!isSharedScript(src) && src.indexOf('gsap') === -1) {
        initialScripts.push(abs);
      }
    });

    cacheSet(window.location.href, {
      content:       main.cloneNode(true),
      css:           [],
      scripts:       initialScripts,
      cdnScripts:    initialCDN,
      inlineScripts: [],
      title:         document.title,
      pageName:      pageName,
      i18nTitle:     bodyEl ? (bodyEl.getAttribute('data-i18n-title') || '') : '',
      doc:           document
    });
  })();

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', prefetchObserve);
  } else {
    prefetchObserve();
  }

  window.__spaNavigate = navigateTo;

})();
