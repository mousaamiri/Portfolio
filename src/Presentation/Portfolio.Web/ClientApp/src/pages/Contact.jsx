import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "../styles/pages/contact.css";
import { Icon } from "../lib/icons";
import { useI18n } from "../lib/i18n";
import { usePageMeta, useApi } from "../lib/hooks";
import { api } from "../lib/api";
import { FEATURES, ROUTES } from "../lib/features";
import { revealAll } from "../lib/textReveal";

const SOCIALS = [
  { key: "github", urlKey: "githubUrl", label: "GitHub" },
  { key: "linkedin", urlKey: "linkedinUrl", label: "LinkedIn" },
  { key: "instagram", urlKey: "instagramUrl", label: "Instagram" },
  { key: "send", urlKey: "telegramUrl", label: "Telegram" },
  { key: "twitter", urlKey: "twitterUrl", label: "X (Twitter)" },
];

const INTERESTS = [
  { value: "general", key: "contact.general", label: "General Inquiry" },
  { value: "job", key: "contact.job", label: "Job Opportunity" },
  { value: "freelance", key: "contact.freelance", label: "Freelance Project" },
];

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function useLocalTime() {
  const [time, setTime] = useState("--");
  useEffect(() => {
    const update = () => {
      const now = new Date();
      const formatted = now.toLocaleString("en-US", {
        timeZone: "Asia/Kolkata",
        weekday: "short",
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true,
      });
      setTime(`${formatted} (GMT+5:30)`);
    };
    update();
    const id = setInterval(update, 1000);
    return () => clearInterval(id);
  }, []);
  return time;
}

function CopyButton({ value, label }) {
  const [copied, setCopied] = useState(false);

  const copy = async () => {
    if (!value) return;
    try {
      await navigator.clipboard.writeText(value);
    } catch {
      const ta = document.createElement("textarea");
      ta.value = value;
      ta.style.position = "fixed";
      ta.style.opacity = "0";
      document.body.appendChild(ta);
      ta.select();
      try {
        document.execCommand("copy");
      } catch {
        /* silent */
      }
      document.body.removeChild(ta);
    }
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <button
      className={`contact-copy-btn${copied ? " copied" : ""}`}
      type="button"
      aria-label={label}
      onClick={copy}
    >
      {copied ? (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="16" height="16">
          <polyline points="20 6 9 17 4 12" />
        </svg>
      ) : (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="16" height="16">
          <rect width="14" height="14" x="8" y="8" rx="2" ry="2" />
          <path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2" />
        </svg>
      )}
    </button>
  );
}

export default function Contact() {
  const { t, lang } = useI18n();
  usePageMeta(t("contact.title", "Contact"), "Get in touch — Mousa Amiri Motlagh");

  const { data: profile } = useApi(() => api.getProfile(lang), [lang]);
  const { data: faqs } = useApi(() => api.getFaqs(lang), [lang]);
  const localTime = useLocalTime();

  const p = profile || {};
  const email = p.email || "";
  const phone = p.phone || "";
  const socials = SOCIALS.filter((s) => {
    const url = p[s.urlKey];
    return url && url !== "#";
  });

  const [interest, setInterest] = useState("general");
  const [openFaq, setOpenFaq] = useState(-1);
  const [submitting, setSubmitting] = useState(false);
  const [feedback, setFeedback] = useState({ type: "", message: "" });
  const [form, setForm] = useState({ name: "", email: "", phone: "", subject: "", message: "", website: "" });

  const onChange = (e) => setForm((f) => ({ ...f, [e.target.name]: e.target.value }));

  const onSubmit = async (e) => {
    e.preventDefault();
    if (submitting) return;

    const name = form.name.trim();
    const emailVal = form.email.trim();
    const message = form.message.trim();

    if (!name) return setFeedback({ type: "error", message: t("contact.error_name", "Please enter your name.") });
    if (!emailVal) return setFeedback({ type: "error", message: t("contact.error_email", "Please enter a valid email address.") });
    if (!EMAIL_RE.test(emailVal))
      return setFeedback({ type: "error", message: t("contact.error_email", "Please enter a valid email address.") });
    if (!message) return setFeedback({ type: "error", message: t("contact.error_message", "Please enter your message.") });

    setSubmitting(true);
    setFeedback({ type: "", message: "" });

    try {
      const res = await api.sendMessage({
        name,
        email: emailVal,
        phone: form.phone.trim(),
        subject: form.subject.trim(),
        message,
        interest,
      });

      if (res.status === 429) {
        setFeedback({
          type: "error",
          message: t("contact.error_ratelimit", "Too many attempts. Please wait a few minutes and try again."),
        });
      } else if (!res.ok) {
        setFeedback({
          type: "error",
          message: t("contact.error_generic", "Something went wrong. Please try again."),
        });
      } else {
        setFeedback({
          type: "success",
          message: t("contact.success", "Message sent successfully! I'll get back to you soon."),
        });
        setForm({ name: "", email: "", phone: "", subject: "", message: "", website: "" });
        setInterest("general");
        setTimeout(() => setFeedback({ type: "", message: "" }), 5000);
      }
    } catch {
      setFeedback({ type: "error", message: t("contact.error_generic", "Something went wrong. Please try again.") });
    } finally {
      setSubmitting(false);
    }
  };

  useEffect(() => {
    if (!profile) return;
    revealAll([
      [".contact-heading", { type: "chars", stagger: 0.025, blurAmount: 14, duration: 0.6 }],
      [".contact-info-title", { type: "words", stagger: 0.04, blurAmount: 10, duration: 0.5 }],
      [".contact-info-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.5 }],
      [".contact-detail-label", { type: "chars", stagger: 0.02, blurAmount: 8, duration: 0.4 }],
      [".contact-map-location", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
      [".closing-cta-heading", { type: "chars", stagger: 0.02, blurAmount: 14, duration: 0.6 }],
      [".closing-cta-card-title", { type: "chars", stagger: 0.02, blurAmount: 10, duration: 0.5 }],
      [".closing-cta-card-desc", { type: "words", stagger: 0.03, blurAmount: 8, duration: 0.45 }],
    ]);
  }, [profile]);

  return (
    <section id="contact" className="contact-section">
      <div className="container">
        <div className="about-eyebrow-row">
          <span className="eyebrow">{t("contact.eyebrow", "CONTACT")}</span>
          <span className="eyebrow-rule" aria-hidden="true"></span>
        </div>
        <h1 className="contact-heading">{t("contact.heading", "LET'S BUILD SOMETHING EXTRAORDINARY")}</h1>

        <div className="contact-details-grid">
          <div className="contact-detail-card">
            <div className="contact-detail-header">
              <span className="contact-detail-label">{t("contact.direct_email", "DIRECT EMAIL")}</span>
              <CopyButton value={email} label="Copy email address" />
            </div>
            <a href={`mailto:${email}`} className="contact-detail-value">{email}</a>
            <div className="contact-detail-decoration" aria-hidden="true">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round" width="48" height="48" opacity="0.06">
                <rect width="20" height="16" x="2" y="4" rx="2" />
                <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
              </svg>
            </div>
          </div>

          <div className="contact-detail-card">
            <div className="contact-detail-header">
              <span className="contact-detail-label">{t("contact.network", "NETWORK")}</span>
            </div>
            <div className="contact-social-icons">
              {socials.map((s) => (
                <a key={s.key} href={p[s.urlKey]} className="contact-social-btn" aria-label={s.label} target="_blank" rel="noopener noreferrer">
                  <Icon name={s.key} size={20} />
                </a>
              ))}
            </div>
          </div>

          <div className="contact-detail-card">
            <div className="contact-detail-header">
              <span className="contact-detail-label">{t("contact.phone", "PHONE")}</span>
              <CopyButton value={phone} label="Copy phone number" />
            </div>
            <a href={`tel:${phone}`} className="contact-detail-value" dir="ltr">{phone}</a>
            <div className="contact-detail-decoration" aria-hidden="true">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round" width="48" height="48" opacity="0.06">
                <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z" />
              </svg>
            </div>
          </div>
        </div>

        <div className="contact-form-faq">
          <div className="contact-form-block">
            <div className="about-eyebrow-row">
              <span className="eyebrow">{t("contact.send_message", "SEND A MESSAGE")}</span>
              <span className="eyebrow-rule" aria-hidden="true"></span>
            </div>

            <div className="contact-availability-badge contact-form-avail">
              <span className="contact-avail-dot"></span>
              <span>{t("contact.available", "Available for new opportunities")}</span>
            </div>

            <div className="contact-interest-pills">
              {INTERESTS.map((item) => (
                <button
                  key={item.value}
                  type="button"
                  className={`contact-pill${interest === item.value ? " active" : ""}`}
                  onClick={() => setInterest(item.value)}
                >
                  {t(item.key, item.label)}
                </button>
              ))}
            </div>

            <form className="contact-form" id="contactForm" noValidate onSubmit={onSubmit}>
              <div className="contact-form-row">
                <div className="contact-form-group">
                  <label className="contact-form-label" htmlFor="contactName">{t("contact.name_label", "NAME")}</label>
                  <input type="text" id="contactName" name="name" className="contact-input" placeholder={t("contact.name_placeholder", "Your name")} value={form.name} onChange={onChange} required />
                </div>
                <div className="contact-form-group">
                  <label className="contact-form-label" htmlFor="contactEmail">{t("contact.email_label", "EMAIL")}</label>
                  <input type="email" id="contactEmail" name="email" className="contact-input" placeholder={t("contact.email_placeholder", "you@email.com")} value={form.email} onChange={onChange} required />
                </div>
              </div>

              <div className="contact-form-row">
                <div className="contact-form-group">
                  <label className="contact-form-label" htmlFor="contactPhone">
                    {t("contact.phone_label", "PHONE")} <span className="contact-form-optional">{t("contact.optional", "(optional)")}</span>
                  </label>
                  <input type="tel" id="contactPhone" name="phone" className="contact-input" placeholder={t("contact.phone_placeholder", "+1 555 000 0000")} dir="ltr" autoComplete="tel" value={form.phone} onChange={onChange} />
                </div>
                <div className="contact-form-group">
                  <label className="contact-form-label" htmlFor="contactSubject">{t("contact.subject_label", "SUBJECT")}</label>
                  <input type="text" id="contactSubject" name="subject" className="contact-input" placeholder={t("contact.subject_placeholder", "What's this about?")} value={form.subject} onChange={onChange} />
                </div>
              </div>

              <div className="contact-hp" aria-hidden="true">
                <label htmlFor="contactWebsite">Website</label>
                <input type="text" id="contactWebsite" name="website" tabIndex={-1} autoComplete="off" value={form.website} onChange={onChange} />
              </div>

              <div className="contact-form-group">
                <label className="contact-form-label" htmlFor="contactMessage">{t("contact.message_label", "MESSAGE")}</label>
                <textarea id="contactMessage" name="message" className="contact-input contact-textarea" placeholder={t("contact.message_placeholder", "Tell me about your idea...")} rows="5" value={form.message} onChange={onChange} required></textarea>
              </div>

              <button type="submit" className="contact-submit-btn" disabled={submitting}>
                <span>{submitting ? t("contact.sending", "Sending...") : t("contact.submit", "Send Message")}</span>
                <span className="contact-submit-arrow">&rarr;</span>
              </button>

              <div className={`contact-form-feedback ${feedback.type}`} aria-live="polite">{feedback.message}</div>
            </form>
          </div>

          {faqs && faqs.length > 0 && (
            <div className="contact-faq-block">
              <div className="about-eyebrow-row">
                <span className="eyebrow">{t("contact.faqs", "FAQS")}</span>
                <span className="eyebrow-rule" aria-hidden="true"></span>
              </div>

              <div className="contact-faq-list">
                {faqs.map((faq, i) => {
                  const open = openFaq === i;
                  return (
                    <div className={`contact-faq-item${open ? " open" : ""}`} key={faq.id ?? i}>
                      <button
                        className="contact-faq-toggle"
                        type="button"
                        aria-expanded={open}
                        onClick={() => setOpenFaq(open ? -1 : i)}
                      >
                        <span>{faq.question}</span>
                        <svg className="contact-faq-chevron" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                          <polyline points="6 9 12 15 18 9" />
                        </svg>
                      </button>
                      <div className="contact-faq-answer">
                        <p>{faq.answer}</p>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}
        </div>

        <div className="contact-info-map">
          <div className="contact-info-card">
            <h2 className="contact-info-title">{t("contact.info_title", "Ready to bring your ideas to life?")}</h2>

            <div className="contact-availability-badge">
              <span className="contact-avail-dot"></span>
              <span>{t("contact.available", "Available for new opportunities")}</span>
            </div>

            <p className="contact-info-desc">
              {t(
                "contact.info_desc",
                "Whether you have a product idea, a full-time role, or just want to chat about technology, I'm always open to interesting conversations. I usually reply within 24 hours."
              )}
            </p>

            <div className="contact-local-time">
              <svg className="contact-clock-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="18" height="18">
                <circle cx="12" cy="12" r="10" />
                <polyline points="12 6 12 12 16 14" />
              </svg>
              <span className="contact-time-label">{t("contact.local_time", "My Local Time:")}</span>
              <span className="contact-time-value" id="localTime">{localTime}</span>
            </div>
          </div>

          <div className="contact-map-card">
            <div className="contact-map-visual">
              <div className="contact-map-placeholder">
                <svg className="contact-map-grid" viewBox="0 0 400 300" fill="none" xmlns="http://www.w3.org/2000/svg">
                  <line x1="0" y1="50" x2="400" y2="50" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="0" y1="100" x2="400" y2="100" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="0" y1="150" x2="400" y2="150" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="0" y1="200" x2="400" y2="200" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="0" y1="250" x2="400" y2="250" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="80" y1="0" x2="80" y2="300" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="160" y1="0" x2="160" y2="300" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="240" y1="0" x2="240" y2="300" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <line x1="320" y1="0" x2="320" y2="300" stroke="rgba(255,255,255,0.04)" strokeWidth="1" />
                  <path d="M180 60 C200 55, 260 50, 280 65 C300 80, 310 100, 300 130 C295 150, 285 160, 275 180 C265 200, 250 220, 240 240 C235 250, 225 258, 220 260 C215 255, 210 245, 205 235 C195 215, 185 195, 180 175 C175 155, 170 135, 172 115 C174 95, 176 75, 180 60Z" fill="rgba(255,255,255,0.05)" stroke="rgba(255,255,255,0.1)" strokeWidth="1" />
                  <circle cx="240" cy="200" r="6" fill="#22c55e" opacity="0.9" />
                  <circle cx="240" cy="200" r="12" fill="#22c55e" opacity="0.15" />
                  <circle cx="240" cy="200" r="20" fill="#22c55e" opacity="0.06" />
                </svg>
              </div>
            </div>
            <div className="contact-map-info">
              <div className="contact-map-label">
                <svg viewBox="0 0 24 24" fill="none" stroke="#ef4444" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" width="14" height="14">
                  <path d="M20 10c0 6-8 12-8 12s-8-6-8-12a8 8 0 0 1 16 0Z" />
                  <circle cx="12" cy="10" r="3" />
                </svg>
                <span>{t("contact.based_in", "BASED IN")}</span>
              </div>
              <div className="contact-map-location">{t("contact.location", p.location || "Tehran")}</div>
              <div className="contact-map-country">
                <span>{t("contact.country", p.country || "Iran")}</span> <span className="contact-map-code">{p.countryCode || "IR"}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="closing-cta-block">
        <div className="closing-cta-eyebrow-row">
          <span className="eyebrow">{t("cta.inner_mechanics", "INNER MECHANICS")}</span>
        </div>
        <h2
          className="closing-cta-heading"
          dangerouslySetInnerHTML={{
            __html: t("cta.heading", 'Where logic meets<br />\n            <span class="script-gradient">rhythm.</span>'),
          }}
        />

        <div className="closing-cta-grid">
          <div className="closing-cta-card">
            <div className="closing-cta-github-header">
              <Icon name="github" className="closing-cta-github-icon" size={22} />
              <span className="closing-cta-github-name">{t("cta.github_name", "Mousa's Github")}</span>
            </div>
            <div className="closing-cta-latest-push">
              <span className="closing-cta-latest-label">{t("cta.latest_push", "LATEST PUSH")}</span>
              <span className="closing-cta-latest-badge">{t("cta.ago", "17h ago")}</span>
            </div>
            <p className="closing-cta-commit-msg">{t("cta.pushed_updates", "Pushed repository updates")}</p>
            <div className="closing-cta-commit-files">
              <span className="closing-cta-file-badge">i-wiki</span>
              <span className="closing-cta-file-badge">doca2f5</span>
            </div>
            <div className="closing-cta-github-stats">
              <div>
                <span className="closing-cta-stat-number">13</span>
                <span className="closing-cta-stat-label">{t("cta.public_repos", "PUBLIC REPOS")}</span>
              </div>
              <div>
                <span className="closing-cta-stat-number">8</span>
                <span className="closing-cta-stat-label">{t("cta.followers", "FOLLOWERS")}</span>
              </div>
            </div>
            <div className="closing-cta-social-row">
              <a href={p.githubUrl || "#"} aria-label="GitHub" target="_blank" rel="noopener noreferrer">
                <Icon name="github" className="closing-cta-social-icon" size={20} />
              </a>
              <a href={p.websiteUrl || "#"} aria-label="Website" target="_blank" rel="noopener noreferrer">
                <Icon name="globe" className="closing-cta-social-icon" size={20} />
              </a>
              <a href={`mailto:${email}`} aria-label="Email">
                <Icon name="mail" className="closing-cta-social-icon" size={20} />
              </a>
            </div>
          </div>

          {FEATURES.blogEnabled && (
            <div className="closing-cta-card">
              <span className="eyebrow eyebrow--sm">{t("cta.writing", "WRITING")}</span>
              <h4 className="closing-cta-card-title">
                {t("cta.article_title", "Clean Architecture in ASP.NET Core: Boundaries that Scale")}
              </h4>
              <span className="closing-cta-meta">{t("cta.published_date", "Published on May 26, 2026")}</span>
              <Link to={ROUTES.blog} className="closing-cta-link">
                <span>{t("cta.read_article", "Read Article")}</span>
                <span className="closing-cta-link-arrow">&rarr;</span>
              </Link>
            </div>
          )}

          <div className="closing-cta-card">
            <span className="eyebrow eyebrow--sm">{t("cta.visitors", "VISITORS")}</span>
            <h4
              className="closing-cta-card-title--large"
              dangerouslySetInnerHTML={{
                __html: t("cta.guestbook_title", 'Leave your<br />\n                    <span class="script-gradient">signature</span>'),
              }}
            />
            <p className="closing-cta-card-desc">{t("cta.let_me_know", "Let me know you were here.")}</p>
            <div className="closing-cta-avatars">
              <span className="closing-cta-avatar">C</span>
              <span className="closing-cta-avatar" style={{ background: "var(--pink)" }}>C</span>
              <span className="closing-cta-avatar" style={{ background: "var(--blue)" }}>G</span>
              <span className="closing-cta-avatar-count">{t("cta.join_others", "Join 13 others")}</span>
            </div>
            <a href="#" className="closing-cta-link">
              <span>{t("cta.sign_guestbook", "Sign Guestbook")}</span>
              <span className="closing-cta-link-arrow">&rarr;</span>
            </a>
          </div>
        </div>
      </div>
    </section>
  );
}
