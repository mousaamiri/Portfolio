import { useEffect, useRef, useState } from "react";
import "../styles/pages/about.css";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import { revealAll } from "../lib/textReveal";
import ClosingCta from "../components/ClosingCta";
import PageStatus from "../components/PageStatus";

const SKILL_ORDER = ["Backend", "Frontend", "Database", "Tools"];
const SKILL_META = {
  Backend: { key: "about.skills_backend", color: "amber" },
  Frontend: { key: "about.skills_frontend", color: "pink" },
  Database: { key: "about.skills_database", color: "blue" },
  Tools: { key: "about.skills_tools", color: "green" },
};

const HANDSON = [
  ["devicon-git-plain", "Git"],
  ["devicon-csharp-plain", "C#"],
  ["devicon-visualstudio-plain", "Visual Studio"],
  ["devicon-amazonwebservices-plain", "AWS"],
  ["devicon-azure-plain", "Azure"],
  ["devicon-googlecloud-plain", "Google Cloud"],
  ["devicon-dotnetcore-plain", "ASP.NET Core"],
  ["devicon-github-original", "GitHub"],
  ["devicon-python-plain", "Python"],
  ["devicon-react-original", "React"],
  ["devicon-linux-plain", "Linux"],
  ["devicon-docker-plain", "Docker"],
  ["devicon-nuget-original", "NuGet"],
  ["devicon-figma-plain", "Figma"],
];

function useDevicon() {
  useEffect(() => {
    const id = "devicon-css";
    if (document.getElementById(id)) return;
    const link = document.createElement("link");
    link.id = id;
    link.rel = "stylesheet";
    link.href = "https://cdn.jsdelivr.net/gh/devicons/devicon@latest/devicon.min.css";
    document.head.appendChild(link);
  }, []);
}

function LiveAge() {
  const ref = useRef(null);
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const BIRTHDATE = new Date("2000-01-01T00:00:00");
    const MS_PER_YEAR = 365.2425 * 24 * 60 * 60 * 1000;
    let lastUpdate = 0;
    let raf = 0;
    const tick = (now) => {
      if (now - lastUpdate >= 50) {
        const age = (Date.now() - BIRTHDATE.getTime()) / MS_PER_YEAR;
        el.textContent = `${age.toFixed(8)} years`;
        lastUpdate = now;
      }
      raf = requestAnimationFrame(tick);
    };
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          if (!raf) raf = requestAnimationFrame(tick);
        } else if (raf) {
          cancelAnimationFrame(raf);
          raf = 0;
        }
      },
      { threshold: 0 }
    );
    observer.observe(el);
    return () => {
      observer.disconnect();
      cancelAnimationFrame(raf);
    };
  }, []);
  return <span className="bio-stat-value bio-stat-value--mono" ref={ref}>--</span>;
}

function FootprintNumber({ target, suffix }) {
  const ref = useRef(null);
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const prefersReduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    const format = (v) => Math.round(v).toLocaleString("en-US") + (suffix || "");
    if (prefersReduced) {
      el.textContent = format(target);
      return;
    }
    let raf = 0;
    const observer = new IntersectionObserver(
      (entries, obs) => {
        if (!entries[0].isIntersecting) return;
        obs.unobserve(el);
        const duration = 1400;
        let start = null;
        const easeOutExpo = (x) => (x === 1 ? 1 : 1 - Math.pow(2, -10 * x));
        const step = (now) => {
          if (start === null) start = now;
          const progress = Math.min((now - start) / duration, 1);
          el.textContent = format(target * easeOutExpo(progress));
          if (progress < 1) raf = requestAnimationFrame(step);
        };
        raf = requestAnimationFrame(step);
      },
      { threshold: 0.4 }
    );
    observer.observe(el);
    return () => {
      observer.disconnect();
      cancelAnimationFrame(raf);
    };
  }, [target, suffix]);
  return (
    <div className="footprint-number" ref={ref}>
      0
    </div>
  );
}

function SkillBar({ value }) {
  const ref = useRef(null);
  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    const prefersReduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (prefersReduced) {
      el.style.width = `${value}%`;
      return;
    }
    const observer = new IntersectionObserver(
      (entries, obs) => {
        if (!entries[0].isIntersecting) return;
        obs.unobserve(el);
        el.style.width = `${value}%`;
      },
      { threshold: 0.2 }
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [value]);
  return <div className="skill-bar-fill" ref={ref} style={{ width: 0 }} />;
}

export default function About() {
  const { t, lang } = useI18n();
  usePageMeta(t("about.title", "About"), "About Mousa Amiri Motlagh — Backend Engineer");
  useDevicon();

  const { data, loading, error } = useApi(
    () =>
      Promise.all([
        api.getProfile(lang),
        api.getSkills(lang),
        api.getEducations(lang),
        api.getTimeline(lang),
        api.getInterests(lang),
        api.getStats(lang),
        api.getTestimonials(lang),
      ]).then(([profile, skills, education, journey, interests, footprint, endorsements]) => ({
        profile: profile || {},
        skills,
        education,
        journey,
        interests,
        footprint,
        endorsements,
      })),
    [lang]
  );

  useEffect(() => {
    if (!data) return;
    revealAll([
      [".about-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
      [".bio-subheading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
      [".bio-quote", { type: "words", stagger: 0.05, blurAmount: 10, duration: 0.6 }],
      [".bio-paragraph", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
      [".skills-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
      [".endorsement-quote", { type: "words", stagger: 0.04, blurAmount: 8, duration: 0.5 }],
      [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
      [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    ]);
  }, [data]);

  if (!data) {
    return (
      <section className="about-section">
        <div className="container">
          <PageStatus loading={loading} error={error} />
        </div>
      </section>
    );
  }

  const { profile, skills, education, journey, interests, footprint, endorsements } = data;
    const portrait = profile.portraitUrl || "/images/about-portrait.jpg";

  return (
    <section id="about" className="about-section">
      <div className="container">
        <div className="about-eyebrow-row">
          <span className="eyebrow">{t("about.eyebrow", "ABOUT")}</span>
          <span className="eyebrow-rule" aria-hidden="true"></span>
        </div>
        <h2 className="about-heading">{t("about.heading", "THE ENGINEER BEHIND THE CODE")}</h2>

        <div className="bio-row">
          <div className="bio-photo-col">
            <div className="bio-photo-frame">
              <picture>
                <source type="image/avif" srcSet={portrait.replace(/\.(jpg|jpeg|png|webp)$/i, ".avif")} />
                <source type="image/webp" srcSet={portrait.replace(/\.(jpg|jpeg|png|avif)$/i, ".webp")} />
                <img
                  className="bio-photo"
                  src={portrait}
              alt={profile.portraitAlt || ""}
                  width="400"
                  height="500"
                  fetchPriority="high"
                  decoding="async"
                />
              </picture>
            </div>
          </div>
          <div className="bio-content-col">
            <h3 className="bio-subheading">
              <span>{t("about.bio_subheading", "Engineering with Purpose.")}</span>
              <span className="accent">{t("about.bio_accent", "Driven by Curiosity.")}</span>
            </h3>
            <blockquote className="bio-quote">
              {t("about.bio_quote", "I believe the best code is the code you can explain to someone who didn't write it.")}
            </blockquote>
            <p
              className="bio-paragraph"
              dangerouslySetInnerHTML={{
                __html: t(
                  "about.bio_p1",
                  'I\'m a self-taught <span class="highlight">C#/.NET backend developer</span> who fell for the gap between "it compiles" and "it actually works." Years of building side projects — not a classroom — taught me how to think like an engineer.'
                ),
              }}
            />
            <p
              className="bio-paragraph"
              dangerouslySetInnerHTML={{
                __html: t(
                  "about.bio_p2",
                  'Most of my learning has come from shipping real code: rebuilding tangled logic into <span class="highlight">maintainable services</span>, wiring up data access, and chasing down the difference between code that runs on my machine and code that survives production.'
                ),
              }}
            />
            <p
              className="bio-paragraph"
              dangerouslySetInnerHTML={{
                __html: t(
                  "about.bio_p3",
                  'Today I focus on <span class="highlight">Clean Architecture</span> and <span class="highlight">DDD</span> with ASP.NET Core — drawn to problems at the intersection of performance and simplicity, the kind of engineering that disappears when it\'s done right.'
                ),
              }}
            />

            <div className="bio-stats">
              <div className="bio-stat">
                <Icon name="briefcase" className="bio-stat-icon" size={18} />
                <span className="bio-stat-label">{t("about.stat_role", "ROLE")}</span>
              <span className="bio-stat-value">{t("about.stat_role_value", profile.roleBadge || "")}</span>
              </div>
              <div className="bio-stat">
                <Icon name="calendar-clock" className="bio-stat-icon" size={18} />
                <span className="bio-stat-label">{t("about.stat_experience", "EXPERIENCE")}</span>
              <span className="bio-stat-value">{t("about.stat_experience_value", profile.experienceBadge || "")}</span>
              </div>
              <div className="bio-stat">
                <Icon name="cake" className="bio-stat-icon" size={18} />
                <span className="bio-stat-label">{t("about.stat_age", "AGE")}</span>
                <LiveAge />
              </div>
              <div className="bio-stat">
                <Icon name="graduation-cap" className="bio-stat-icon" size={18} />
                <span className="bio-stat-label">{t("about.stat_degree", "DEGREE")}</span>
              <span className="bio-stat-value">{t("about.stat_degree_value", profile.degreeBadge || "")}</span>
              </div>
            </div>
          </div>
        </div>

        {journey.length > 0 && (
          <div className="journey-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.journey", "JOURNEY")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="timeline">
              <div className="timeline-line" aria-hidden="true"></div>
              {journey.map((e, i) => {
                const isRight = i % 2 === 0;
                const content = (
                  <div className={`timeline-content${isRight ? " timeline-content--right" : ""}`}>
                    <span className="timeline-year-mobile">{e.year}</span>
                    <h4 className="timeline-title">{e.title}</h4>
                    <p className="timeline-desc">{e.description}</p>
                  </div>
                );
                const node = (
                  <div className="timeline-node">
                    <Icon name={e.icon} size={18} />
                  </div>
                );
                const year = <div className="timeline-year">{e.year}</div>;
                return (
                  <div className="timeline-entry" key={e.id || i}>
                    {isRight ? (
                      <>
                        {content}
                        {node}
                        {year}
                      </>
                    ) : (
                      <>
                        {year}
                        {node}
                        {content}
                      </>
                    )}
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {footprint.length > 0 && (
          <div className="footprint-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.footprint", "TECHNICAL FOOTPRINT")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="footprint-grid">
              {footprint.map((s, i) => (
                <div className={`footprint-card${s.tipText ? " footprint-card--has-tip" : ""}`} key={s.id || i}>
                  {s.tipText && (
                    <button
                      className="footprint-tip-trigger"
                      type="button"
                      aria-label={s.tipAriaLabel || ""}
                      data-tooltip={s.tipText}
                    >
                      <Icon name="info" size={16} />
                    </button>
                  )}
                  <div className="footprint-icon"><Icon name={s.icon} size={22} /></div>
                  <FootprintNumber target={s.countTarget} suffix={s.suffix} />
                  <div className="footprint-label">{s.label}</div>
                </div>
              ))}
            </div>
          </div>
        )}

        {skills.length > 0 && (
          <div className="skills-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.skills", "SKILLS")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="skills-grid">
              {SKILL_ORDER.map((cat) => {
                const meta = SKILL_META[cat];
                const catSkills = skills.filter((x) => x.category === cat);
                return (
                  <div className="skills-card" key={cat}>
                    <div className="skills-card-title">
                      <span className={`skills-bullet skills-bullet--${meta.color}`} aria-hidden="true"></span>
                      {t(meta.key, cat.toUpperCase())}
                    </div>
                    {catSkills.map((sk, i) => (
                      <div className="skill-row" key={sk.id || i}>
                        <div className="skill-row-top">
                          <span>{sk.name}</span>
                          <span className="skill-pct">{sk.proficiency}%</span>
                        </div>
                        <div className="skill-bar">
                          <SkillBar value={sk.proficiency} />
                        </div>
                      </div>
                    ))}
                  </div>
                );
              })}
            </div>
          </div>
        )}

        <div className="handson-block">
          <div className="about-eyebrow-row">
            <span className="eyebrow">{t("about.handson", "HANDS-ON")}</span>
            <span className="eyebrow-rule" aria-hidden="true"></span>
          </div>
          <div className="pill-grid">
            {HANDSON.map(([icon, label]) => (
              <div className="tech-pill" key={label}>
                <i className={`${icon} colored`}></i>
                <span>{label}</span>
              </div>
            ))}
          </div>
        </div>

        {interests.length > 0 && (
          <div className="interests-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.interests", "INTERESTS")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="interests-grid">
              {interests.map((it, i) => (
                <div className="interest-card" key={it.id || i}>
                  <Icon name={it.icon} className="interest-icon" size={22} />
                  <span className="interest-label">{it.label}</span>
                </div>
              ))}
            </div>
          </div>
        )}

        {education.length > 0 && (
          <div className="education-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.education", "EDUCATION")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="education-grid">
              {education.map((ed, i) => {
                const startYear = new Date(ed.startDate).getFullYear();
                const endYear = ed.endDate ? new Date(ed.endDate).getFullYear() : "Present";
                return (
                  <div className="education-card" key={ed.id || i}>
                    <div className="education-icon"><Icon name="graduation-cap" size={22} /></div>
                    <div className="education-body">
                      <h4 className="education-degree">{ed.degree} — {ed.fieldOfStudy}</h4>
                      <span className="education-institution">{ed.institutionName}</span>
                      <div className="education-meta">
                        <span className="education-years">{startYear} — {endYear}</span>
                        {ed.gpa != null && <span className="education-score">{ed.gpa}</span>}
                      </div>
                      {ed.description && <p className="education-desc">{ed.description}</p>}
                    </div>
                  </div>
                );
              })}
            </div>
          </div>
        )}

        {endorsements.length > 0 && (
          <div className="endorsements-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("about.endorsements", "ENDORSEMENTS")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="endorsements-row">
              {endorsements.map((en, i) => (
                <div className="endorsement-card" key={en.id || i}>
                  <div className="endorsement-quote-mask">
                    <p className="endorsement-quote">{en.quote}</p>
                  </div>
                  <div className="endorsement-person">
                    <div className="endorsement-avatar" style={{ "--avatar-color": en.avatarColor }}>{en.initials}</div>
                    <div>
                      <div className="endorsement-name">{en.name}</div>
                      <div className="endorsement-role">{en.role}</div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>

      <ClosingCta />
    </section>
  );
}
