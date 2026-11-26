import { useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/home.css";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import { ROUTES } from "../lib/features";
import { revealText, typewriterEffect } from "../lib/textReveal";

export default function Home() {
  const { t, lang } = useI18n();
  const { data: profile } = useApi(() => api.getProfile(lang), [lang]);
  usePageMeta(t("home.title", "Home"), "Portfolio of Mousa Amiri Motlagh — Backend Engineer");

  const heroRef = useRef(null);
  const spotlightRef = useRef(null);
  const haloRef = useRef(null);
  const overlineRef = useRef(null);
  const headlineRef = useRef(null);
  const subRefs = useRef([]);
  const animatedLangRef = useRef(null);

  const p = profile || {};
  const resumeFile = `resume-${lang === "fa" ? "fa" : "en"}.pdf`;

  // Hero entrance sequence (port of home.js).
  useEffect(() => {
    const prefersReduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const overline = overlineRef.current;
    const headline = headlineRef.current;
    const subs = subRefs.current.filter(Boolean);
    const elements = [overline, headline, ...subs].filter(Boolean);

    // Never leave the hero permanently hidden: if the API is slow/failing or an
    // animation never fires, reveal the (fallback) content anyway.
    const revealFallback = () => {
      elements.forEach((el) => {
        el.style.visibility = "visible";
        el.style.filter = "blur(0px)";
        el.style.opacity = "1";
      });
    };

    if (prefersReduced) {
      revealFallback();
      return;
    }

    const safety = setTimeout(revealFallback, 1500);

    // Wait for the real profile before typing/revealing, and animate once per
    // language. Running the sequence twice (fallback text + API text) made the
    // typewriter interleave characters.
    if (!profile || animatedLangRef.current === lang) {
      return () => clearTimeout(safety);
    }
    animatedLangRef.current = lang;

    const CHAR_DELAY_MS = 35;
    const overlineText = overline ? overline.textContent : "";
    const typewriterDuration = overlineText.length * CHAR_DELAY_MS;
    const headlineStartDelay = Math.max(typewriterDuration - 200, 0);
    const firstSubDelay = headlineStartDelay + 350;

    if (overline) typewriterEffect(overline, { charDelay: CHAR_DELAY_MS });
    if (headline) {
      setTimeout(() => {
        revealText(headline, {
          stagger: 0.035,
          blurAmount: 16,
          duration: 0.7,
          yOffset: 8,
          ease: "power2.out",
        });
      }, headlineStartDelay);
    }
    subs.forEach((el, i) => {
      const isDescription = el.classList.contains("hero-description");
      setTimeout(() => {
        revealText(el, {
          type: isDescription ? "words" : "chars",
          stagger: isDescription ? 0.04 : 0.025,
          blurAmount: isDescription ? 8 : 16,
          duration: isDescription ? 0.4 : 0.6,
          yOffset: 8,
          ease: "power2.out",
        });
      }, firstSubDelay + i * 200);
    });

    return () => {
      clearTimeout(safety);
      // Stop any in-flight typewriter so a language switch cannot leave the old
      // language's characters appended to the new text.
      elements.forEach((el) => {
        if (el._typewriterTimer) {
          clearTimeout(el._typewriterTimer);
          el._typewriterTimer = null;
        }
      });
    };
  }, [profile, lang]);

  // Mouse-tracking spotlight + halo (port of home.js).
  useEffect(() => {
    const hero = heroRef.current;
    const spot = spotlightRef.current;
    const halo = haloRef.current;
    if (!hero || !spot || !halo) return;
    if (!window.matchMedia("(hover: hover) and (pointer: fine)").matches) return;

    let mouseX = -9999;
    let mouseY = -9999;
    let haloX = -9999;
    let haloY = -9999;
    let ticking = false;
    let raf = 0;

    const lerp = () => {
      if (!ticking) return;
      haloX += (mouseX - haloX) * 0.06;
      haloY += (mouseY - haloY) * 0.06;
      halo.style.setProperty("--hx", `${haloX}px`);
      halo.style.setProperty("--hy", `${haloY}px`);
      raf = requestAnimationFrame(lerp);
    };

    const onMove = (e) => {
      const rect = hero.getBoundingClientRect();
      mouseX = e.clientX - rect.left;
      mouseY = e.clientY - rect.top;
      spot.style.setProperty("--x", `${mouseX}px`);
      spot.style.setProperty("--y", `${mouseY}px`);
      if (!ticking) {
        ticking = true;
        raf = requestAnimationFrame(lerp);
      }
    };
    const onLeave = () => {
      ticking = false;
      cancelAnimationFrame(raf);
    };

    hero.addEventListener("mousemove", onMove);
    hero.addEventListener("mouseleave", onLeave);
    return () => {
      hero.removeEventListener("mousemove", onMove);
      hero.removeEventListener("mouseleave", onLeave);
      cancelAnimationFrame(raf);
    };
  }, []);

  return (
    <section id="hero" className="hero-section" ref={heroRef}>
      <div className="hero-spotlight" aria-hidden="true" ref={spotlightRef}></div>
      <div className="hero-halo" aria-hidden="true" ref={haloRef}></div>
      <div className="container hero-container">
        <p className="hero-overline">
          <span className="hero-overline-text" ref={overlineRef}>
            {t("home.cmd_hint", "PRESS CMD+K FOR COMMANDS")}
          </span>
          <span className="hero-cursor" aria-hidden="true">▮</span>
        </p>

        <h1 className="hero-headline" style={{ visibility: "hidden" }} ref={headlineRef}>
          {t("home.name", p.fullName || "Mousa Amiri Motlagh")}
        </h1>

        <p className="hero-subheadline" style={{ visibility: "hidden" }} ref={(el) => (subRefs.current[0] = el)}>
          {t("home.role", `> ${p.jobTitle || "Backend Engineer"}`)}
        </p>

        <p className="hero-description" style={{ visibility: "hidden" }} ref={(el) => (subRefs.current[1] = el)}>
          {t("home.description", p.bio || "")}
        </p>

        <div className="hero-actions" style={{ visibility: "hidden" }} ref={(el) => (subRefs.current[2] = el)}>
          <a href={`/resumes/${resumeFile}`} className="hero-btn hero-btn--primary" download={resumeFile}>
            <Icon name="download" size={18} />
            <span>{t("home.download_resume", "Download Resume")}</span>
          </a>
          <Link to={ROUTES.work} className="hero-btn hero-btn--ghost">
            <Icon name="layout-grid" size={18} />
            <span>{t("home.view_projects", "View Projects")}</span>
          </Link>
          <Link to={ROUTES.contact} className="hero-btn hero-btn--ghost">
            <Icon name="mail" size={18} />
            <span>{t("home.contact", "Contact")}</span>
          </Link>
        </div>

        <div className="hero-socials" style={{ visibility: "hidden" }} ref={(el) => (subRefs.current[3] = el)}>
          {p.githubUrl && p.githubUrl !== "#" && (
            <a href={p.githubUrl} target="_blank" rel="noopener noreferrer" className="hero-social-btn" aria-label="GitHub">
              <Icon name="github" size={18} />
            </a>
          )}
          {p.instagramUrl && p.instagramUrl !== "#" && (
            <a href={p.instagramUrl} target="_blank" rel="noopener noreferrer" className="hero-social-btn" aria-label="Instagram">
              <Icon name="instagram" size={18} />
            </a>
          )}
          {p.linkedinUrl && p.linkedinUrl !== "#" && (
            <a href={p.linkedinUrl} target="_blank" rel="noopener noreferrer" className="hero-social-btn" aria-label="LinkedIn">
              <Icon name="linkedin" size={18} />
            </a>
          )}
          {p.telegramUrl && p.telegramUrl !== "#" && (
            <a href={p.telegramUrl} target="_blank" rel="noopener noreferrer" className="hero-social-btn" aria-label="Telegram">
              <Icon name="send" size={18} />
            </a>
          )}
        </div>

        <div className="hero-status-wrapper">
          <div className="hero-availability-dot" aria-hidden="true"></div>
          {p.learningTitle && (
            <div className="hero-popover" aria-hidden="true">
              <div className="hero-popover-inner">
                <div className="hero-popover-header">
                  <span className="hero-popover-badge">{t("home.learning", "LEARNING")}</span>
                  <span className="hero-popover-label">{t("home.latest_note", "LATEST NOTE")}</span>
                </div>
                <p className="hero-popover-title">{t("home.learning_title", p.learningTitle)}</p>
                <p className="hero-popover-desc">{t("home.learning_desc", p.learningDesc || "")}</p>
                <span className="hero-popover-date">{t("home.learning_date", p.learningDate || "")}</span>
              </div>
              <div className="hero-popover-tail" aria-hidden="true"></div>
            </div>
          )}
        </div>
      </div>
    </section>
  );
}
