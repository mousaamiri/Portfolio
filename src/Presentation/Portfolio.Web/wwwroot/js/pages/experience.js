/* ==========================================================================
   experience.js — Scroll-triggered fade-in animations
   --------------------------------------------------------------------------
   Requires GSAP + ScrollTrigger loaded before this script.
   Also wires up revealText() from text-reveal.js for text elements.
   ========================================================================== */

(function () {
  "use strict";

  /* ---- Guard: skip if user prefers reduced motion ---- */
  var prefersReducedMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
  ).matches;

  if (prefersReducedMotion) return;

  /* ---- Wait for GSAP ---- */
  if (typeof gsap === "undefined" || typeof ScrollTrigger === "undefined") return;

  gsap.registerPlugin(ScrollTrigger);

  /* ================================================================
     1. Fade-in sections on scroll
     ================================================================ */

  var fadeTargets = [
    ".exp-impact-row",
    ".exp-main-row",
    ".exp-proficiency-block",
    ".closing-cta-block"
  ];

  fadeTargets.forEach(function (selector) {
    var el = document.querySelector(selector);
    if (!el) return;

    gsap.from(el, {
      opacity: 0,
      y: 40,
      duration: 0.8,
      ease: "power2.out",
      scrollTrigger: {
        trigger: el,
        start: "top 85%",
        once: true
      }
    });
  });

  /* ================================================================
     2. Stagger metric cards
     ================================================================ */

  var metricCards = document.querySelectorAll(".exp-metric-card");
  if (metricCards.length) {
    gsap.from(metricCards, {
      opacity: 0,
      y: 30,
      duration: 0.6,
      stagger: 0.1,
      ease: "power2.out",
      scrollTrigger: {
        trigger: ".exp-metrics-grid",
        start: "top 85%",
        once: true
      }
    });
  }

  /* ================================================================
     3. Stagger proficiency cards
     ================================================================ */

  var profCards = document.querySelectorAll(".exp-proficiency-card");
  if (profCards.length) {
    gsap.from(profCards, {
      opacity: 0,
      y: 30,
      duration: 0.6,
      stagger: 0.12,
      ease: "power2.out",
      scrollTrigger: {
        trigger: ".exp-proficiency-grid",
        start: "top 85%",
        once: true
      }
    });
  }

  /* ================================================================
     4. Stagger job list items
     ================================================================ */

  var jobItems = document.querySelectorAll(".exp-job-item");
  if (jobItems.length) {
    gsap.from(jobItems, {
      opacity: 0,
      x: -20,
      duration: 0.5,
      stagger: 0.08,
      ease: "power2.out",
      scrollTrigger: {
        trigger: ".exp-job-list",
        start: "top 85%",
        once: true
      }
    });
  }

  /* ================================================================
     5. Stagger education entries
     ================================================================ */

  var eduEntries = document.querySelectorAll(".exp-edu-entry");
  if (eduEntries.length) {
    gsap.from(eduEntries, {
      opacity: 0,
      y: 20,
      duration: 0.5,
      stagger: 0.15,
      ease: "power2.out",
      scrollTrigger: {
        trigger: ".exp-edu-timeline",
        start: "top 85%",
        once: true
      }
    });
  }

  /* ================================================================
     6. Stagger stack pills
     ================================================================ */

  var stackPills = document.querySelectorAll(".exp-stack-pill");
  if (stackPills.length) {
    gsap.from(stackPills, {
      opacity: 0,
      scale: 0.85,
      duration: 0.35,
      stagger: 0.03,
      ease: "back.out(1.5)",
      scrollTrigger: {
        trigger: ".exp-stack-block",
        start: "top 90%",
        once: true
      }
    });
  }

  /* ================================================================
     7. Stagger closing CTA cards
     ================================================================ */

  var ctaCards = document.querySelectorAll(".closing-cta-card");
  if (ctaCards.length) {
    gsap.from(ctaCards, {
      opacity: 0,
      y: 30,
      duration: 0.6,
      stagger: 0.12,
      ease: "power2.out",
      scrollTrigger: {
        trigger: ".closing-cta-grid",
        start: "top 85%",
        once: true
      }
    });
  }

  /* ================================================================
     8. Text reveals (uses revealText from text-reveal.js)
     ================================================================ */

  if (typeof window.revealText === "function") {
    var REVEALS = [
      [".exp-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
      [".exp-principle-title", { type: "chars", stagger: 0.02, blurAmount: 8, duration: 0.4 }],
      [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
      [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    ];

    REVEALS.forEach(function (entry) {
      window.revealText(entry[0], Object.assign({ scroll: true }, entry[1]));
    });
  }
})();
