/* ==========================================================================
   about.js — About section behavior
   ========================================================================== */

(function () {
  "use strict";

  /* ---- Lucide icon render ------------------------------------------- */
  if (window.lucide && typeof window.lucide.createIcons === "function") {
    window.lucide.createIcons();
  }

  /* ---- Live decimal age counter ------------------------------------- */
  var BIRTHDATE = new Date("2000-01-01T00:00:00");

  var MS_PER_YEAR = 365.2425 * 24 * 60 * 60 * 1000;
  var ageEl = document.getElementById("live-age");

  if (ageEl) {
    var lastUpdate = 0;
    var UPDATE_INTERVAL_MS = 50;
    var ageRafId = 0;

    function tickAge(now) {
      if (now - lastUpdate >= UPDATE_INTERVAL_MS) {
        var ageYears = (Date.now() - BIRTHDATE.getTime()) / MS_PER_YEAR;
        ageEl.textContent = ageYears.toFixed(8) + " years";
        lastUpdate = now;
      }
      ageRafId = requestAnimationFrame(tickAge);
    }

    var ageObserver = new IntersectionObserver(function (entries) {
      if (entries[0].isIntersecting) {
        if (!ageRafId) ageRafId = requestAnimationFrame(tickAge);
      } else {
        if (ageRafId) { cancelAnimationFrame(ageRafId); ageRafId = 0; }
      }
    }, { threshold: 0 });

    ageObserver.observe(ageEl);
  }

  /* ---- Reduced motion check ----------------------------------------- */
  var prefersReducedMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
  ).matches;

  var hasGSAP = typeof window.gsap !== "undefined";
  if (hasGSAP && window.ScrollTrigger) {
    gsap.registerPlugin(ScrollTrigger);
  }

  /* ---- Section 3: Journey timeline ---------------------------------- */
  (function initTimeline() {
    var timelineEl = document.querySelector(".timeline");
    var lineEl = document.querySelector(".timeline-line");
    var entries = document.querySelectorAll(".timeline-entry");

    if (!timelineEl || !entries.length) return;

    if (prefersReducedMotion || !hasGSAP) {
      if (lineEl) lineEl.style.transform = "translateX(-50%) scaleY(1)";
      entries.forEach(function (entry) {
        entry.style.opacity = "1";
      });
      return;
    }

    if (lineEl) {
      gsap.fromTo(
        lineEl,
        { scaleY: 0 },
        {
          scaleY: 1,
          ease: "none",
          transformOrigin: "top center",
          scrollTrigger: {
            trigger: timelineEl,
            start: "top 75%",
            end: "bottom 75%",
            scrub: true,
          },
        }
      );
    }

    entries.forEach(function (entry, i) {
      gsap.from(entry, {
        opacity: 0,
        y: 40,
        duration: 0.7,
        ease: "power2.out",
        scrollTrigger: {
          trigger: entry,
          start: "top 85%",
        },
      });
    });
  })();

  /* ---- Section 4: Technical Footprint count-up ---------------------- */
  (function initCountUp() {
    var numberEls = document.querySelectorAll(".footprint-number[data-count-target]");
    if (!numberEls.length) return;

    function formatValue(value, suffix) {
      var rounded = Math.round(value);
      return rounded.toLocaleString("en-US") + (suffix || "");
    }

    function animateCount(el) {
      var target = parseFloat(el.getAttribute("data-count-target"), 10);
      var suffix = el.getAttribute("data-suffix") || "";

      if (prefersReducedMotion) {
        el.textContent = formatValue(target, suffix);
        return;
      }

      var duration = 1400;
      var startTime = null;

      function easeOutExpo(t) {
        return t === 1 ? 1 : 1 - Math.pow(2, -10 * t);
      }

      function step(now) {
        if (startTime === null) startTime = now;
        var elapsed = now - startTime;
        var progress = Math.min(elapsed / duration, 1);
        var eased = easeOutExpo(progress);
        el.textContent = formatValue(target * eased, suffix);
        if (progress < 1) {
          requestAnimationFrame(step);
        }
      }

      requestAnimationFrame(step);
    }

    var observer = new IntersectionObserver(
      function (entries, obs) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            animateCount(entry.target);
            obs.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.4 }
    );

    numberEls.forEach(function (el) {
      observer.observe(el);
    });
  })();

  /* ---- Section 5: Skills progress bars ------------------------------ */
  (function initSkillBars() {
    var cards = document.querySelectorAll(".skills-card");
    if (!cards.length) return;

    cards.forEach(function (card) {
      var bars = card.querySelectorAll(".skill-bar-fill");

      if (prefersReducedMotion || !hasGSAP) {
        bars.forEach(function (bar) {
          bar.style.width = bar.getAttribute("data-target-width") + "%";
        });
        return;
      }

      ScrollTrigger.create({
        trigger: card,
        start: "top 80%",
        once: true,
        onEnter: function () {
          bars.forEach(function (bar, i) {
            gsap.to(bar, {
              width: bar.getAttribute("data-target-width") + "%",
              duration: 1,
              delay: i * 0.08,
              ease: "power2.out",
            });
          });
        },
      });
    });
  })();
})();
