/* ==========================================================================
   home.js — Hero entrance sequence
   --------------------------------------------------------------------------
   Requires text-reveal.js (typewriterEffect + revealText) loaded first.
   ========================================================================== */

(function () {
  "use strict";

  var overlineEl = document.querySelector("[data-typewriter]");
  var headlineEl = document.querySelector("[data-reveal-headline]");
  var revealEls = document.querySelectorAll("[data-reveal-sub]");

  var prefersReducedMotion = window.matchMedia(
    "(prefers-reduced-motion: reduce)"
  ).matches;

  if (prefersReducedMotion) {
    if (overlineEl) overlineEl.textContent = overlineEl.textContent;
    revealEls.forEach(function (el) {
      el.style.visibility = "visible";
      el.style.filter = "blur(0px)";
      el.style.opacity = "1";
    });
    if (headlineEl) {
      headlineEl.style.visibility = "visible";
      headlineEl.style.filter = "blur(0px)";
      headlineEl.style.opacity = "1";
    }
    return;
  }

  var CHAR_DELAY_MS = 35;
  var HEADLINE_OVERLAP_MS = 200;
  var SUBHEADLINE_OVERLAP_MS = 350;
  var STAGGER_BETWEEN_SUBS = 200;

  var overlineText = overlineEl ? overlineEl.textContent : "";
  var typewriterDuration = overlineText.length * CHAR_DELAY_MS;
  var headlineStartDelay = Math.max(typewriterDuration - HEADLINE_OVERLAP_MS, 0);
  var firstSubDelay = headlineStartDelay + SUBHEADLINE_OVERLAP_MS;

  if (overlineEl) {
    window.typewriterEffect(overlineEl, { charDelay: CHAR_DELAY_MS });
  }

  if (headlineEl) {
    setTimeout(function () {
      window.revealText(headlineEl, {
        stagger: 0.035,
        blurAmount: 16,
        duration: 0.7,
        yOffset: 8,
        ease: "power2.out",
        scroll: false,
      });
    }, headlineStartDelay);
  }

  revealEls.forEach(function (el, i) {
    var isDescription = el.classList.contains("hero-description");
    setTimeout(function () {
      window.revealText(el, {
        type: isDescription ? "words" : "chars",
        stagger: isDescription ? 0.04 : 0.025,
        blurAmount: isDescription ? 8 : 16,
        duration: isDescription ? 0.4 : 0.6,
        yOffset: 8,
        ease: "power2.out",
        scroll: false,
      });
    }, firstSubDelay + i * STAGGER_BETWEEN_SUBS);
  });
})();

/* ==========================================================================
   Resume download — point at the correct PDF for the active language
   ========================================================================== */
(function () {
  "use strict";

  var link = document.getElementById("resume-download");
  if (!link) return;

  function update() {
    var lang = window.i18n && window.i18n.lang() === "fa" ? "fa" : "en";
    var file = "resume-" + lang + ".pdf";
    link.setAttribute("href", "../../resumes/" + file);
    link.setAttribute("download", file);
  }

  update();
  window.addEventListener("langchange", update);
})();

/* ==========================================================================
   Spotlight — mouse-tracking glow on hero dot grid
   ========================================================================== */
(function () {
  "use strict";

  var hero = document.getElementById("hero");
  var spot = hero && hero.querySelector(".hero-spotlight");
  var halo = hero && hero.querySelector(".hero-halo");
  if (!spot || !halo) return;

  var matchHover = window.matchMedia("(hover: hover) and (pointer: fine)");
  if (!matchHover.matches) return;

  var mouseX = -9999;
  var mouseY = -9999;
  var haloX = -9999;
  var haloY = -9999;
  var ticking = false;

  hero.addEventListener("mousemove", function (e) {
    var rect = hero.getBoundingClientRect();
    mouseX = e.clientX - rect.left;
    mouseY = e.clientY - rect.top;
    spot.style.setProperty("--x", mouseX + "px");
    spot.style.setProperty("--y", mouseY + "px");

    if (!ticking) {
      ticking = true;
      lerpHalo();
    }
  });

  hero.addEventListener("mouseleave", function () {
    ticking = false;
  });

  function lerpHalo() {
    if (!ticking) return;

    haloX += (mouseX - haloX) * 0.06;
    haloY += (mouseY - haloY) * 0.06;

    halo.style.setProperty("--hx", haloX + "px");
    halo.style.setProperty("--hy", haloY + "px");

    requestAnimationFrame(lerpHalo);
  }
})();

/* ==========================================================================
   Hero popover — touch/click toggle for mobile devices
   ========================================================================== */
(function () {
  "use strict";

  var wrapper = document.querySelector(".hero-status-wrapper");
  if (!wrapper) return;

  wrapper.addEventListener("click", function (e) {
    e.stopPropagation();
    wrapper.classList.toggle("popover-active");
  });

  document.addEventListener("click", function () {
    wrapper.classList.remove("popover-active");
  });
})();
