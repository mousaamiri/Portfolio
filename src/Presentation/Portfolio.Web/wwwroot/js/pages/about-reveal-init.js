/* ==========================================================================
   about-reveal-init.js — Wires revealText() onto About-section elements
   --------------------------------------------------------------------------
   Load AFTER text-reveal.js and about.js.
   ========================================================================== */

(function () {
  "use strict";

  var REVEALS = [
    [".about-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
    [".bio-subheading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
    [".bio-quote", { type: "words", stagger: 0.05, blurAmount: 10, duration: 0.6 }],
    [".bio-paragraph", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
    [".skills-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    [".endorsement-quote", { type: "words", stagger: 0.04, blurAmount: 8, duration: 0.5 }],
    [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
    [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
  ];

  REVEALS.forEach(function (entry) {
    var selector = entry[0];
    var options = entry[1];
    window.revealText(selector, Object.assign({ scroll: true }, options));
  });
})();
