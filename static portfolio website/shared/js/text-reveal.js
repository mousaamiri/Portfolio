/* ==========================================================================
   text-reveal.js — Reusable text animation utilities
   --------------------------------------------------------------------------
   Exposes two functions on window:
     1. typewriterEffect(el, options)
     2. revealText(target, options)

   REQUIRES: gsap.min.js, ScrollTrigger.min.js, SplitText.min.js (GSAP 3.13+)
   ========================================================================== */

(function (window) {
  "use strict";

  if (typeof window.gsap !== "undefined") {
    if (window.ScrollTrigger) gsap.registerPlugin(ScrollTrigger);
    if (window.SplitText) gsap.registerPlugin(SplitText);
  }

  function prefersReducedMotion() {
    return window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  }

  function typewriterEffect(el, options) {
    var opts = Object.assign({ charDelay: 35, onComplete: null }, options || {});
    if (!el) return;

    var fullText = el.textContent;

    if (prefersReducedMotion()) {
      el.textContent = fullText;
      if (opts.onComplete) opts.onComplete();
      return;
    }

    el.textContent = "";
    var i = 0;

    function typeNext() {
      if (i < fullText.length) {
        el.textContent += fullText.charAt(i);
        i += 1;
        setTimeout(typeNext, opts.charDelay);
      } else if (opts.onComplete) {
        opts.onComplete();
      }
    }

    typeNext();
  }

  function revealText(target, options) {
    var opts = Object.assign(
      {
        type: "chars",
        stagger: 0.025,
        blurAmount: 6,
        duration: 0.5,
        delay: 0,
        yOffset: 4,
        ease: "power2.out",
        scroll: false,
      },
      options || {}
    );

    var els =
      typeof target === "string"
        ? Array.prototype.slice.call(document.querySelectorAll(target))
        : [target];

    els.forEach(function (el) {
      if (!el) return;

      if (el.dataset.revealApplied === "true") return;
      el.dataset.revealApplied = "true";

      el.style.visibility = "visible";

      var gsapReady =
        typeof window.gsap !== "undefined" && typeof window.SplitText !== "undefined";

      if (prefersReducedMotion() || !gsapReady) {
        el.style.filter = "blur(0px)";
        el.style.opacity = "1";
        el.style.transform = "none";
        return;
      }

      var isRTL = document.documentElement.getAttribute("dir") === "rtl";
      var splitType = isRTL ? "words" : opts.type;
      var split = SplitText.create(el, { type: splitType });
      var units = splitType === "words" ? split.words : split.chars;

      gsap.set(units, {
        filter: "blur(" + opts.blurAmount + "px)",
        opacity: 0,
        y: opts.yOffset,
      });

      var tweenVars = {
        filter: "blur(0px)",
        opacity: 1,
        y: 0,
        duration: opts.duration,
        ease: opts.ease,
        stagger: opts.stagger,
        delay: opts.delay,
      };

      if (opts.scroll) {
        tweenVars.scrollTrigger = {
          trigger: el,
          start: "top 85%",
          once: true,
        };
      }

      gsap.to(units, tweenVars);
    });
  }

  window.typewriterEffect = typewriterEffect;
  window.revealText = revealText;
})(window);
