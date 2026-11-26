// Port of the original text-reveal.js using the npm GSAP build.
// Exposes typewriterEffect() and revealText() with identical options.
import { gsap } from "gsap";
import { ScrollTrigger } from "gsap/ScrollTrigger";
import { SplitText } from "gsap/SplitText";

gsap.registerPlugin(ScrollTrigger, SplitText);

function prefersReducedMotion() {
  return window.matchMedia("(prefers-reduced-motion: reduce)").matches;
}

export function typewriterEffect(el, options) {
  const opts = Object.assign({ charDelay: 35, onComplete: null }, options || {});
  if (!el) return;

  // Cancel a previous run so two overlapping calls cannot interleave characters.
  if (el._typewriterTimer) {
    clearTimeout(el._typewriterTimer);
    el._typewriterTimer = null;
  }

  const fullText = el.textContent;
  // Identifies this run. A language switch re-renders the element's text while a
  // previous timer may still be pending; the token + prefix check below makes that
  // stale run abort instead of appending the old language's characters.
  const token = (el._typewriterToken || 0) + 1;
  el._typewriterToken = token;

  if (prefersReducedMotion()) {
    el.textContent = fullText;
    if (opts.onComplete) opts.onComplete();
    return;
  }

  el.textContent = "";
  let i = 0;

  function typeNext() {
    if (el._typewriterToken !== token) return;
    if (el.textContent !== fullText.slice(0, i)) return;
    if (i < fullText.length) {
      el.textContent += fullText.charAt(i);
      i += 1;
      el._typewriterTimer = setTimeout(typeNext, opts.charDelay);
    } else if (opts.onComplete) {
      opts.onComplete();
    }
  }

  typeNext();
}

function splitAndAnimate(el, opts) {
  const isRTL = document.documentElement.getAttribute("dir") === "rtl";
  const splitType = isRTL ? "words" : opts.type;
  const split = SplitText.create(el, { type: splitType });
  const units = splitType === "words" ? split.words : split.chars;

  gsap.set(units, { filter: `blur(${opts.blurAmount}px)`, opacity: 0, y: opts.yOffset });

  const tweenVars = {
    filter: "blur(0px)",
    opacity: 1,
    y: 0,
    duration: opts.duration,
    ease: opts.ease,
    stagger: opts.stagger,
    delay: opts.delay,
  };

  if (opts.scroll) {
    tweenVars.scrollTrigger = { trigger: el, start: "top 85%", once: true };
  }

  gsap.to(units, tweenVars);
}

function whenFontsReady(cb) {
  if (document.fonts && document.fonts.ready && typeof document.fonts.ready.then === "function") {
    document.fonts.ready.then(cb);
  } else {
    cb();
  }
}

export function revealText(target, options) {
  const opts = Object.assign(
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

  const els =
    typeof target === "string"
      ? Array.from(document.querySelectorAll(target))
      : [target];

  els.forEach((el) => {
    if (!el) return;
    if (el.dataset.revealApplied === "true") return;
    el.dataset.revealApplied = "true";
    el.style.visibility = "visible";

    if (prefersReducedMotion()) {
      el.style.filter = "blur(0px)";
      el.style.opacity = "1";
      el.style.transform = "none";
      return;
    }

    whenFontsReady(() => {
      try {
        splitAndAnimate(el, opts);
      } catch {
        el.style.filter = "blur(0px)";
        el.style.opacity = "1";
      }
    });
  });
}

// Reveals an array of [selector, options] pairs (mirrors about-reveal-init.js).
export function revealAll(reveals) {
  reveals.forEach(([selector, options]) => {
    revealText(selector, Object.assign({ scroll: true }, options));
  });
}
