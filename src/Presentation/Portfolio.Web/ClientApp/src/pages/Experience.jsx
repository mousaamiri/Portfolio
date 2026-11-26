import { useEffect } from "react";
import "../styles/pages/experience.css";
import { gsap } from "gsap";
import { ScrollTrigger } from "gsap/ScrollTrigger";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import { revealAll } from "../lib/textReveal";
import ClosingCta from "../components/ClosingCta";
import PageStatus from "../components/PageStatus";

gsap.registerPlugin(ScrollTrigger);

function fmtYear(date) {
  return date ? new Date(date).getFullYear() : "Present";
}

export default function Experience() {
  const { t, lang } = useI18n();
  usePageMeta(t("exp.title", "Experience"), "Professional engineering journey — Mousa Amiri Motlagh");

  const { data, loading, error } = useApi(
    () =>
      Promise.all([
        api.getProfile(lang),
        api.getExperiences(lang),
        api.getEducations(lang),
        api.getImpactMetrics(lang),
        api.getPrinciples(lang),
        api.getProficiencies(lang),
      ]).then(([profile, experiences, educations, metrics, principles, proficiency]) => ({
        profile: profile || {},
        experiences,
        educations,
        metrics,
        principles,
        proficiency,
      })),
    [lang]
  );

  useEffect(() => {
    if (!data) return;
    const prefersReduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (!prefersReduced) {
      const fadeTargets = [".exp-impact-row", ".exp-main-row", ".exp-proficiency-block", ".closing-cta-block"];
      fadeTargets.forEach((selector) => {
        const el = document.querySelector(selector);
        if (!el) return;
        gsap.from(el, {
          opacity: 0,
          y: 40,
          duration: 0.8,
          ease: "power2.out",
          scrollTrigger: { trigger: el, start: "top 85%", once: true },
        });
      });
      const stagger = (selector, trigger, vars) => {
        const els = document.querySelectorAll(selector);
        if (!els.length) return;
        gsap.from(els, {
          ...vars,
          scrollTrigger: { trigger, start: "top 85%", once: true },
        });
      };
      stagger(".exp-metric-card", ".exp-metrics-grid", { opacity: 0, y: 30, duration: 0.6, stagger: 0.1, ease: "power2.out" });
      stagger(".exp-proficiency-card", ".exp-proficiency-grid", { opacity: 0, y: 30, duration: 0.6, stagger: 0.12, ease: "power2.out" });
      stagger(".closing-cta-card", ".closing-cta-grid", { opacity: 0, y: 30, duration: 0.6, stagger: 0.12, ease: "power2.out" });
    }
    revealAll([
      [".exp-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
      [".exp-principle-title", { type: "chars", stagger: 0.02, blurAmount: 8, duration: 0.4 }],
      [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
      [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
    ]);
    return () => ScrollTrigger.getAll().forEach((st) => st.kill());
  }, [data]);

  if (!data) {
    return (
      <section className="experience-section">
        <div className="container">
          <PageStatus loading={loading} error={error} />
        </div>
      </section>
    );
  }

  const { profile, experiences, educations, metrics, principles, proficiency } = data;

  return (
    <section id="experience" className="experience-section">
      <div className="container">
        <div className="about-eyebrow-row">
          <span className="eyebrow">{t("exp.eyebrow", "EXPERIENCE")}</span>
          <span className="eyebrow-rule" aria-hidden="true"></span>
        </div>
        <h2 className="exp-heading">{t("exp.heading", "PROFESSIONAL ENGINEERING JOURNEY")}</h2>

        <div className="exp-impact-row">
          {metrics.length > 0 && (
            <div className="exp-metrics-grid">
              {metrics.map((m, i) => (
                <div className="exp-metric-card" key={m.id || i}>
                  <span className={`exp-metric-tag exp-metric-tag--${m.color}`}>{m.tag}</span>
                  <span className="exp-metric-value">{m.value}</span>
                  <p className="exp-metric-desc">{m.description}</p>
                </div>
              ))}
            </div>
          )}

          {principles.length > 0 && (
            <div className="exp-principles-card">
              <div className="about-eyebrow-row">
                <span className="eyebrow eyebrow--sm">{t("exp.principles", "CORE PRINCIPLES")}</span>
                <span className="eyebrow-rule" aria-hidden="true"></span>
              </div>
              <ul className="exp-principles-list">
                {principles.map((p, i) => (
                  <li className="exp-principle-item" key={p.id || i}>
                    <span className="exp-principle-bullet" aria-hidden="true"></span>
                    <div>
                      <span className="exp-principle-title">{p.title}</span>
                      <span className="exp-principle-desc">{p.description}</span>
                    </div>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>

        <div className="exp-main-row">
          <div className="exp-left-col">
            {profile.bio && (
              <div className="exp-summary-block">
                <div className="about-eyebrow-row">
                  <span className="eyebrow eyebrow--sm">{t("exp.summary", "SUMMARY")}</span>
                  <span className="eyebrow-rule" aria-hidden="true"></span>
                </div>
                <div className="exp-summary-name">
                  <span className="exp-name-bullet" aria-hidden="true"></span>
                  <span>{profile.fullName}</span>
                </div>
                <p className="exp-summary-text">{profile.bio}</p>
              </div>
            )}

            {educations.length > 0 && (
              <div className="exp-education-block">
                <div className="about-eyebrow-row">
                  <span className="eyebrow eyebrow--sm">{t("exp.education", "EDUCATION")}</span>
                  <span className="eyebrow-rule" aria-hidden="true"></span>
                </div>
                <div className="exp-edu-timeline">
                  {educations.map((ed, i) => {
                    const isLast = i === educations.length - 1;
                    const years = `${fmtYear(ed.startDate)} – ${fmtYear(ed.endDate)}`;
                    return (
                      <div className={`exp-edu-entry${isLast ? " exp-edu-entry--last" : ""}`} key={ed.id || i}>
                        <div className="exp-edu-node" aria-hidden="true">
                          <span className="exp-edu-circle"></span>
                          {!isLast && <span className="exp-edu-line"></span>}
                        </div>
                        <div className="exp-edu-content">
                          <h4 className="exp-edu-title">{ed.degree}</h4>
                          <span className="exp-edu-year">{years}</span>
                          <p className="exp-edu-institution">
                            {ed.institutionName}
                            {ed.fieldOfStudy ? ` · ${ed.fieldOfStudy}` : ""}
                          </p>
                          {ed.gpa > 0 && <p className="exp-edu-score">GPA {ed.gpa}</p>}
                          {ed.description && <p className="exp-edu-score">{ed.description}</p>}
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            )}
          </div>

          <div className="exp-right-col">
            <div className="about-eyebrow-row">
              <span className="eyebrow eyebrow--sm">{t("exp.professional", "PROFESSIONAL EXPERIENCE")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>

            {experiences.length > 0 ? (
              experiences.map((job, i) => (
                <div key={job.id || i}>
                  <div className="exp-job-header">
                    <div className="exp-job-title-row">
                      <span className="exp-name-bullet" aria-hidden="true"></span>
                      <h3 className="exp-job-title">{job.jobTitle}</h3>
                    </div>
                    <span className="exp-job-date-badge">
                      {fmtYear(job.startDate)} – {fmtYear(job.endDate)}
                    </span>
                  </div>
                  <p className="exp-job-company">
                    {job.companyName}
                    {job.location ? ` · ${job.location}` : ""}
                  </p>
                  {job.description && <p className="exp-job-desc">{job.description}</p>}
                </div>
              ))
            ) : (
              <p className="exp-job-empty">{t("exp.no_history", "Professional history will appear here soon.")}</p>
            )}
          </div>
        </div>

        {proficiency.length > 0 && (
          <div className="exp-proficiency-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("exp.proficiency", "TECHNICAL PROFICIENCY MATRIX")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>
            <div className="exp-proficiency-grid">
              {proficiency.map((pg, i) => (
                <div className="exp-proficiency-card" key={pg.id || i}>
                  <div className="exp-proficiency-header">
                    <span className={`exp-proficiency-bullet exp-proficiency-bullet--${pg.color}`} aria-hidden="true"></span>
                    <h4 className="exp-proficiency-title">{pg.title}</h4>
                  </div>
                  <ul className="exp-proficiency-list">
                    {(pg.items || "")
                      .split(",")
                      .map((s) => s.trim())
                      .filter(Boolean)
                      .map((item, j) => (
                        <li key={j}>{item}</li>
                      ))}
                  </ul>
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
